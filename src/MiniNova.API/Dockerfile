FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/MiniNova.API/MiniNova.API.csproj", "src/MiniNova.API/"]
COPY ["src/MiniNova.BLL/MiniNova.BLL.csproj", "src/MiniNova.BLL/"]
COPY ["src/MiniNova.DAL/MiniNova.DAL.csproj", "src/MiniNova.DAL/"]
RUN dotnet restore "src/MiniNova.API/MiniNova.API.csproj"
COPY . .
WORKDIR "/src/src/MiniNova.API"
RUN dotnet build "./MiniNova.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MiniNova.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/scripts ./scripts
ENTRYPOINT ["dotnet", "MiniNova.API.dll"]
