FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/SkyTower.WebApi/SkyTower.WebApi.csproj", "src/SkyTower.WebApi/"]
RUN dotnet restore "src/SkyTower.WebApi/SkyTower.WebApi.csproj"
COPY . .
WORKDIR "/src/src/SkyTower.WebApi"
RUN dotnet build "./SkyTower.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SkyTower.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SkyTower.WebApi.dll"]
