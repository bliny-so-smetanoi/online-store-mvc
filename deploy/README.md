# Docker deployment (Ubuntu Server)

A single `docker-compose.yml` in the repository root brings up:

| Service | What it is | Exposed |
|---|---|---|
| `caddy` | reverse proxy + automatic HTTPS (Let's Encrypt) | 80, 443 |
| `app` | OnlineStore (image built from the `Dockerfile`: webpack + dotnet publish) | via Caddy: `https://DOMAIN/` |
| `keycloak` | Keycloak 26.1.4, realm `web-net` imported from `deploy/keycloak/web-net-realm.json` | via Caddy: `https://DOMAIN/auth` |
| `sqlserver` | SQL Server 2022 (application DB and Keycloak DB) | `MSSQL_PORT` (1433), login `sa` |
| `sqlserver-init` | one-off job that creates the Keycloak DB | — |
| `seq` | Serilog logs | only `127.0.0.1:5341` on the server |

## Requirements

- Ubuntu 22.04/24.04 x64, at least ~4 GB RAM (SQL Server needs 2 GB).
- Docker Engine + Compose plugin: `curl -fsSL https://get.docker.com | sudo sh`
- A domain with an A record pointing to the server IP, ports 80 and 443 open.
  Without your own domain you can use `<ip-with-dashes>.sslip.io`, e.g. `203-0-113-10.sslip.io`.

HTTPS is required: without it the browser drops the OIDC sign-in cookie, and Keycloak refuses console access from external addresses.

## First run

```bash
git clone <repo> onlinestore && cd onlinestore
cp .env.example .env
nano .env        # DOMAIN, passwords, secrets (openssl rand -hex 24)
docker compose up -d --build
docker compose ps
docker compose logs -f app
```

What happens on startup:
1. `sqlserver` → `sqlserver-init` creates the Keycloak DB.
2. `keycloak`, if its DB is empty, imports realm `web-net` with clients `mvc-template` and `backend-admin`
   and roles `root`/`employee`/`visitor`; secrets and redirect URIs are taken from `.env`.
3. `app` applies EF Core migrations (`Database__MigrateOnStartup=true`) and starts.

## After the first run

The realm contains no users. To sign in to the store admin area:
1. Open `https://DOMAIN/auth/admin` and sign in with `KEYCLOAK_ADMIN` / `KEYCLOAK_ADMIN_PASSWORD`.
2. Realm `web-net` → Users → Create user, set a password (Credentials) and assign the realm role `root`.

## Maintenance

```bash
docker compose pull && docker compose up -d --build   # update images and rebuild the app
git pull && docker compose up -d --build app          # roll out a new app version
docker compose logs -f keycloak
ssh -L 5341:localhost:5341 user@server                # then http://localhost:5341 — Seq
```

SQL Server backup:

```bash
docker compose exec sqlserver bash -c '/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "BACKUP DATABASE [OnlineStore] TO DISK = N'"'"'/var/opt/mssql/data/OnlineStore.bak'"'"' WITH INIT"'
docker compose cp sqlserver:/var/opt/mssql/data/OnlineStore.bak ./OnlineStore.bak
```

## Connecting to SQL Server from outside

SSMS / Rider / Azure Data Studio: server `DOMAIN,1433` (or the server IP), SQL authentication `sa` / `MSSQL_SA_PASSWORD`,
enable "Trust server certificate" (the SQL Server certificate is self-signed).

The port is open to the whole internet, and Docker publishes ports bypassing `ufw`. Therefore:
- set a strong `MSSQL_SA_PASSWORD`;
- preferably restrict port 1433 by IP in your hosting/cloud provider's firewall, or
- set `MSSQL_BIND=127.0.0.1` and connect through a tunnel: `ssh -L 1433:localhost:1433 user@server`, server `localhost,1433`.

## Important

- The realm is imported **only if it does not exist yet** in the Keycloak DB. After that, changes to `.env` (secrets, `DOMAIN`)
  must be applied manually in the Keycloak console (Clients → mvc-template: Credentials, Valid redirect URIs).
- Data lives in docker volumes (`sqlserver-data`, `seq-data`, `caddy-data`, `app-dataprotection`).
  `docker compose down -v` deletes them permanently.

## Local network / VirtualBox (no internet access)

Let's Encrypt cannot verify the domain, so switch to a self-signed certificate:

```
CADDY_TLS=internal
DOMAIN=onlinestore.example.com   # or localhost / the VM's IP
```

- For a domain, add the line `<VM IP> onlinestore.example.com` to `hosts` on every client
  (Windows: `C:\Windows\System32\drivers\etc\hosts`). The site only works via `DOMAIN`, not via the IP —
  Keycloak redirects point to it.
- VirtualBox NAT: forward ports 80/443 (and 1433) from the host and use `DOMAIN=localhost`.
- To stop browser warnings, install Caddy's root certificate into "Trusted Root Certification Authorities":
  `docker compose cp caddy:/data/caddy/pki/authorities/local/root.crt ./caddy-root.crt`.
- Switching to `CADDY_TLS=acme` later: change `.env` and run `docker compose up -d caddy`.
