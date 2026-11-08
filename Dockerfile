# ---------- Compilación ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Primero solo los .csproj para aprovechar la caché de "restore".
COPY Bitacora.sln ./
COPY src/Bitacora.Domain/Bitacora.Domain.csproj src/Bitacora.Domain/
COPY src/Bitacora.Application/Bitacora.Application.csproj src/Bitacora.Application/
COPY src/Bitacora.Infrastructure/Bitacora.Infrastructure.csproj src/Bitacora.Infrastructure/
COPY src/Bitacora.Api/Bitacora.Api.csproj src/Bitacora.Api/
RUN dotnet restore src/Bitacora.Api/Bitacora.Api.csproj

COPY src/ src/
RUN dotnet publish src/Bitacora.Api/Bitacora.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ---------- Ejecución ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Bitacora.Api.dll"]
