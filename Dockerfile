FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
# COPY ["TodoApi/TodoApi.csproj", "TodoApi/"]
COPY ["TodoApi.csproj", "./"]

# RUN dotnet restore "TodoApi/TodoApi.csproj"
RUN dotnet restore "TodoApi.csproj"

COPY . .
WORKDIR "/src/TodoApi"
RUN dotnet build "TodoApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoApi.dll"]
