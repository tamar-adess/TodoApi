using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 🔹 שליפת מחרוזת חיבור ממשתני סביבה או מהקובץ
var connectionString = Environment.GetEnvironmentVariable("ToDoDB") 
                        ?? builder.Configuration.GetConnectionString("ToDoDB");

// 🔍 הדפסת המחרוזת כדי לוודא שהיא נטענת
Console.WriteLine($"🔍 ConnectionString: {connectionString}");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("❌ No connection string found! Check environment variables.");
}


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 🔹 הוספת שירותי Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 הוספת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://todoapi-noo0.onrender.com")
  
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// 🔹 הפעלת CORS
app.UseCors("AllowSpecificOrigin");

// 🔹 טיפול בשגיאות גלובלי
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("❌ שגיאת שרת פנימית. בדוק את ה-Logs.");
    });
});

// 🔹 הפעלת Swagger בסביבת פיתוח בלבד
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
        c.RoutePrefix = string.Empty;
    });
}

// 🔹 בדיקת חיבור למסד הנתונים לפני הפעלת השרת
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        Console.WriteLine("🔄 מנסה להתחבר למסד הנתונים...");
        if (dbContext.Database.CanConnect())
        {
            Console.WriteLine("✅ חיבור למסד הנתונים הצליח!");
            dbContext.Database.Migrate(); // מריץ מיגרציות אוטומטית
        }
        else
        {
            Console.WriteLine("❌ חיבור למסד הנתונים נכשל.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ שגיאה בחיבור ל-DB: {ex.Message}");
    }
}
app.MapGet("/", () => Results.Ok("Server is running!"));

// 🔹 1. שליפת כל הפריטים
app.MapGet("/items", async (ApplicationDbContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});

// 🔹 2. שליפת פריט לפי ID
app.MapGet("/items/{id}", async (int id, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

// 🔹 3. הוספת פריט חדש
app.MapPost("/items", async (Item item, ApplicationDbContext db) =>
{
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

// 🔹 4. עדכון פריט לפי ID
app.MapPut("/items/{id}", async (int id, Item updatedItem, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await db.SaveChangesAsync();
    return Results.Ok(item);
});

// 🔹 5. מחיקת פריט לפי ID
app.MapDelete("/items/{id}", async (int id, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
