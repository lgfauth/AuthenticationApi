FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS build
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/AuthenticationApi/AuthenticationApi.csproj", "src/Presentation/AuthenticationApi/"]
RUN dotnet restore "./src/Presentation/AuthenticationApi/AuthenticationApi.csproj"
COPY . .
WORKDIR "/src/src/Presentation/AuthenticationApi"
RUN dotnet build "./AuthenticationApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthenticationApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN --mount=type=secret,id=MY_SECRETS \
    sh -c "echo '{\"RABBITMQCONFIGURATION__VIRTUALHOST\":\"value\",\"RABBITMQCONFIGURATION__USERNAME\":\"value\",\"RABBITMQCONFIGURATION__QUEUENAME\":\"value\",\"RABBITMQCONFIGURATION__PASSWORD\":\"value\",\"RABBITMQCONFIGURATION__HOSTNAME\":\"value\",\"MONGODBSETTINGS__DATABASENAME\":\"value\",\"MONGODBSETTINGS__CONNECTIONSTRING\":\"value\",\"MONGODBDATA__USER\":\"value\",\"MONGODBDATA__PASSWORD\":\"value\",\"MONGODBDATA__CLUSTER\":\"value\",\"Kestrel:Certificates:Development:Password\":\"value\",\"JWTSETTINGS__SECRETKEY\":\"value\",\"JWTSETTINGS__ISSUER\":\"value\",\"JWTSETTINGS__EXPIRATIONMINUTES\":\"value\",\"JWTSETTINGS__AUDIENCE\":\"value\"}' > /run/secrets/AUTHENTICATION_SECRETS"


FROM build AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationApi.dll"]