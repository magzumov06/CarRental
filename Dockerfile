# 🔨 Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj
COPY *.csproj .
RUN dotnet restore

# copy all
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 🚀 Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

# муҳим барои Render (PORT)
ENV ASPNETCORE_URLS=http://+:$PORT

EXPOSE 10000

ENTRYPOINT ["dotnet", "WebApp.dll"]