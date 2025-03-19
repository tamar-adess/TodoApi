# Use the ASP.NET runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# Use .NET SDK 9.0 for building the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj file and restore dependencies
COPY "TodoApi.csproj" "./"
RUN dotnet restore "TodoApi.csproj"

# Copy the rest of the source code
COPY . . 

# Build the project
RUN dotnet build "TodoApi.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish

# Final image for running the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "TodoApi.dll"]
