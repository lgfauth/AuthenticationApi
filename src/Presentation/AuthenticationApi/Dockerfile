FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AuthenticationApi.csproj", "AuthenticationApi/"]
RUN dotnet restore "AuthenticationApi.csproj"
COPY . .
WORKDIR "./AuthenticationApi"
RUN dotnet build "./AuthenticationApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthenticationApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM build AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationApi.dll"]