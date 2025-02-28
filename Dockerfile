FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Presentation/AuthApi/AuthApi.csproj", "src/Presentation/AuthApi/"]

RUN dotnet restore "./src/Presentation/AuthApi/AuthApi.csproj"

COPY . .

WORKDIR "/src/src/Presentation/AuthApi"
RUN dotnet build "./AuthApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app

ENV ASPNETCORE_URLS="http://0.0.0.0:${PORT:-8080}"

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AuthApi.dll"]