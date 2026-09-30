# OnlineStore application
## Project information
MVC application built with ASP.NET

## Tech stack
.NET 8 \
SQL Server 16 \
Node.js 20.11.0 (npm 10.2.4) \
Keycloak 26.1.4 \
Seq

## Migrations
dotnet ef migrations add [MIGRATION_NAME] --project .\Persistance\Persistance.csproj --startup-project .\OnlineStore\OnlineStore.csproj

docker run -d --name keycloak -p 8080:8080 -e KC_BOOTSTRAP_ADMIN_USERNAME=admin   -e KC_BOOTSTRAP_ADMIN_PASSWORD=admin123  -e KC_HTTP_ENABLED=true -e KC_HOSTNAME=localhost -e KC_DB=mssql -e KC_DB_USERNAME=sa -e KC_DB_PASSWORD=localhost -e KC_DB_URL="jdbc:sqlserver://host.docker.internal:1433;databaseName=MvcTemplateIdentity;encrypt=true;trustServerCertificate=true" quay.io/keycloak/keycloak:26.1.4 start
