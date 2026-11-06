# syntax=docker/dockerfile:1

# ---------- 1. Frontend (webpack) ----------
FROM node:20-alpine AS frontend
WORKDIR /src/OnlineStore/Scripts
COPY OnlineStore/Scripts/package.json OnlineStore/Scripts/package-lock.json ./
RUN npm ci
COPY OnlineStore/Scripts/ ./
# Собирает бандлы в /src/OnlineStore/wwwroot/js
RUN npm run build

# ---------- 2. Backend (dotnet publish) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Сначала только csproj — чтобы restore кешировался отдельно от исходников
COPY OnlineStore.sln ./
COPY OnlineStore/OnlineStore.csproj OnlineStore/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY KeycloakExtension/KeycloakExtension.csproj KeycloakExtension/
COPY Persistence/Persistence.csproj Persistence/
COPY Resource/Resource.csproj Resource/
RUN dotnet restore OnlineStore/OnlineStore.csproj

COPY . .
COPY --from=frontend /src/OnlineStore/wwwroot/js OnlineStore/wwwroot/js
RUN dotnet publish OnlineStore/OnlineStore.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- 3. Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Каталог для ключей Data Protection (монтируется volume, чтобы куки входа переживали пересоздание контейнера)
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && chown -R app:app /home/app/.aspnet

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "OnlineStore.dll"]
