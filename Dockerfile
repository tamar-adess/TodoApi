# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# WORKDIR /app
# EXPOSE 80
# FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# # FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# WORKDIR /src
# # COPY ["TodoApi/TodoApi.csproj", "TodoApi/"]
# COPY ["TodoApi.csproj", "./"]

# # RUN dotnet restore "TodoApi/TodoApi.csproj"
# RUN dotnet restore "TodoApi.csproj"

# COPY . .
# WORKDIR "/src/TodoApi"
# RUN dotnet build "TodoApi.csproj" -c Release -o /app/build

# FROM build AS publish
# RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish

# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT ["dotnet", "TodoApi.dll"]



# Use the ASP.NET runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use .NET SDK 6.0 for building the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj file and restore dependencies
COPY "TodoApi.csproj", "./"
RUN dotnet restore "TodoApi.csproj"

# Copy the rest of the source code
COPY . . 
WORKDIR "/src/TodoApi"

# Build the project
RUN dotnet build "TodoApi.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish

# Final image for running the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApi.dll"]
