
using Microsoft.EntityFrameworkCore;  // עבור Entity Framework Core
using TodoApi.Models; // הייבוא של המחלקות שלך (ApplicationDbContext ו-Item)

var builder = WebApplication.CreateBuilder(args);

// הוספת DbContext לשירותים
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseMySQL(builder.Configuration.GetConnectionString("ToDoDB")));
var connectionString = Environment.GetEnvironmentVariable("ToDoDB") 
                        ?? builder.Configuration.GetConnectionString("ToDoDB");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No connection string found! Check environment variables.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));


// הוספת Swagger לשירותים
builder.Services.AddEndpointsApiExplorer();  // צורך ב-API Explorer
builder.Services.AddSwaggerGen();  // צורך ב-Swagger UI

// הוספת CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("https://todoapi-noo0.onrender.com")  // הכתובת של הלקוח שלך
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // הוספת אפשרות זו אם נדרש
    });
});

var app = builder.Build();

// הפעלת CORS
app.UseCors("AllowSpecificOrigin");

// הפעלת Swagger רק אם האפליקציה בסביבה של פיתוח
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();  // הפעלת Swagger
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDo API V1");
        c.RoutePrefix = string.Empty;  // אם אתה רוצה ש-Swagger יהיה ב-root של האתר
    });
}

// 1. שליפת כל הפריטים
app.MapGet("/items", async (ApplicationDbContext db) =>
{
    var items = await db.Items.ToListAsync();  // קריאה ישירה ל-DbContext
    return Results.Ok(items);
});

// 2. שליפת פריט לפי ID
app.MapGet("/items/{id}", async (int id, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);  // קריאה ישירה ל-DbContext
    return item is not null ? Results.Ok(item) : Results.NotFound();
});

// 3. הוספת פריט חדש
app.MapPost("/items", async (Item item, ApplicationDbContext db) =>
{
    db.Items.Add(item);  // הוספת פריט ישירות דרך DbContext
    await db.SaveChangesAsync();  // שמירת השינויים
    return Results.Created($"/items/{item.Id}", item);
});

// 4. עדכון פריט לפי ID
app.MapPut("/items/{id}", async (int id, Item updatedItem, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);  // קריאה ישירה ל-DbContext
    // if (item is null) return Results.NotFound();

    item.Name = updatedItem.Name;
    item.IsComplete = updatedItem.IsComplete;

    await db.SaveChangesAsync();  // שמירת השינויים
    return Results.Ok(item);
});


// 5. מחיקת פריט לפי ID
app.MapDelete("/items/{id}", async (int id, ApplicationDbContext db) =>
{
    var item = await db.Items.FindAsync(id);  // קריאה ישירה ל-DbContext
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);  // מחיקת פריט
    await db.SaveChangesAsync();  // שמירת השינויים
    return Results.NoContent();
});

// הפעלת השרת
app.Run();
