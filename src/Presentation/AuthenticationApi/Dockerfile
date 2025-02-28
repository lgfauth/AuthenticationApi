# Etapa 1: Imagem base de runtime (aspnet)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
# Se a variável $APP_UID não estiver definida no ambiente,
# pode causar erro. Se não precisar rodar como usuário não-root,
# remova a linha abaixo ou defina um valor.
# USER $APP_UID

WORKDIR /app
# Exponha apenas a(s) porta(s) que sua aplicação vai escutar
ENV ASPNETCORE_URLS="http://0.0.0.0:8080"
EXPOSE 8080

# Etapa 2: Build (sdk)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia os arquivos .csproj para o contexto de build
COPY ["src/Presentation/AuthenticationApi/AuthenticationApi.csproj", "src/Presentation/AuthenticationApi/"]
COPY ["src/Application/ServicesApplication/ServicesApplication.csproj", "src/Application/ServicesApplication/"]
COPY ["src/Domain/Domain/Domain.csproj", "src/Domain/Domain/"]
COPY ["src/Repository/Repository/Repository.csproj", "src/Repository/Repository/"]

# Restaura o projeto principal (AuthenticationApi) - 
# isso também restaura as dependências dos demais, se referenciados
RUN dotnet restore "./src/Presentation/AuthenticationApi/AuthenticationApi.csproj"

# Copia todo o restante do código
COPY . .

# Compila o projeto principal
WORKDIR "/src/src/Presentation/AuthenticationApi"
RUN dotnet build "./AuthenticationApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Etapa 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthenticationApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa 4: Final (runtime)
FROM base AS final
WORKDIR /app

# Copia os artefatos publicados da etapa publish
COPY --from=publish /app/publish .

# Define o entrypoint para rodar a DLL principal
ENTRYPOINT ["dotnet", "AuthenticationApi.dll"]