# 🔨 Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy everything (fix for project structure issues)
COPY . .

# change to project folder (АГАР НОМИ FOLDER ҲАСТ)
# WORKDIR /src/WebApp

RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

# 🚀 Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

# Render uses dynamic PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

ENTRYPOINT ["dotnet", "WebApp.dll"]