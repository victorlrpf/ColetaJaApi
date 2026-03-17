# Estágio de construção
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto e restaurar dependências
COPY ["ColetaJaApi.csproj", "./"]
RUN dotnet restore "ColetaJaApi.csproj"

# Copiar o restante do código e compilar
COPY . .
RUN dotnet build "ColetaJaApi.csproj" -c Release -o /app/build

# Estágio de publicação
FROM build AS publish
RUN dotnet publish "ColetaJaApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final (imagem de execução)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ColetaJaApi.dll"]
