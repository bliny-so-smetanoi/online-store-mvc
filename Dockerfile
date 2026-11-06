# syntax=docker/dockerfile:1

# ---------- 1. Frontend (webpack) ----------
FROM node:20-alpine AS frontend
WORKDIR /src/OnlineStore/Scripts
COPY OnlineStore/Scripts/package.json OnlineStore/Scripts/package-lock.json ./
RUN npm ci
COPY OnlineStore/Scripts/ ./
# Builds the bundles into /src/OnlineStore/wwwroot/js
RUN npm run build

# ---------- 2. Backend (dotnet publish) ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# csproj files first, so restore is cached separately from the sources
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

# Directory for Data Protection keys (mounted as a volume so sign-in cookies survive container re-creation)
RUN mkdir -p /home/app/.aspnet/DataProtection-Keys && chown -R app:app /home/app/.aspnet

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
USER app
ENTRYPOINT ["dotnet", "OnlineStore.dll"]
