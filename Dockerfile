# Etapa 1: Construção da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copie o arquivo .csproj e restaure as dependências
COPY ["AuthenticationAPI.csproj", "./"]
RUN dotnet restore "AuthenticationAPI.csproj"
# Copie o restante dos arquivos e faça o publish
COPY . .
RUN dotnet publish "AuthenticationAPI.csproj" -c Release -o /app/publish

# Etapa 2: Construção da imagem final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AuthenticationAPI.dll"]
