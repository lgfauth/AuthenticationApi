# Etapa 1: Construção da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copie o arquivo .csproj e restaure as dependências
COPY ["./AuthenticationAPI/AuthenticationAPI.csproj", "./AuthenticationAPI"]
RUN dotnet restore "./AuthenticationAPI/AuthenticationAPI.csproj"
# Copie o restante dos arquivos e faça o publish
COPY . .
WORKDIR "/src/AuthenticationAPI"
RUN dotnet publish "./AuthenticationAPI.csproj" -c Release -o /app/publish

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthenticationAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa 2: Construção da imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationAPI.dll"]