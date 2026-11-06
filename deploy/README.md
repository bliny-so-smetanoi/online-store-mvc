# Развёртывание в Docker (Ubuntu Server)

Один `docker-compose.yml` в корне репозитория поднимает:

| Сервис | Что это | Наружу |
|---|---|---|
| `caddy` | reverse proxy + автоматический HTTPS (Let's Encrypt) | 80, 443 |
| `app` | OnlineStore (образ собирается из `Dockerfile`: webpack + dotnet publish) | через Caddy: `https://DOMAIN/` |
| `keycloak` | Keycloak 26.1.4, realm `web-net` импортируется из `deploy/keycloak/web-net-realm.json` | через Caddy: `https://DOMAIN/auth` |
| `sqlserver` | SQL Server 2022 (БД приложения и БД Keycloak) | `MSSQL_PORT` (1433), логин `sa` |
| `sqlserver-init` | одноразово создаёт БД для Keycloak | — |
| `seq` | логи Serilog | только `127.0.0.1:5341` на сервере |

## Требования

- Ubuntu 22.04/24.04 x64, минимум ~4 ГБ RAM (SQL Server требует 2 ГБ).
- Docker Engine + Compose plugin: `curl -fsSL https://get.docker.com | sudo sh`
- Домен с A-записью на IP сервера, открытые порты 80 и 443.
  Без своего домена подойдёт `<ip-через-дефисы>.sslip.io`, например `203-0-113-10.sslip.io`.

HTTPS обязателен: без него браузер отбрасывает cookie входа OIDC, а Keycloak не пускает в консоль с внешних адресов.

## Первый запуск

```bash
git clone <repo> onlinestore && cd onlinestore
cp .env.example .env
nano .env        # DOMAIN, пароли, секреты (openssl rand -hex 24)
docker compose up -d --build
docker compose ps
docker compose logs -f app
```

Что происходит при старте:
1. `sqlserver` → `sqlserver-init` создаёт БД Keycloak.
2. `keycloak` при пустой БД импортирует realm `web-net` с клиентами `mvc-template` и `backend-admin`,
   ролями `root`/`employee`/`visitor`; секреты и redirect URI берутся из `.env`.
3. `app` применяет миграции EF Core (`Database__MigrateOnStartup=true`) и стартует.

## После первого запуска

Пользователей realm не содержит. Чтобы войти в админку магазина:
1. Откройте `https://DOMAIN/auth/admin`, войдите `KEYCLOAK_ADMIN` / `KEYCLOAK_ADMIN_PASSWORD`.
2. Realm `web-net` → Users → Create user, задайте пароль (Credentials) и назначьте realm-роль `root`.

## Обслуживание

```bash
docker compose pull && docker compose up -d --build   # обновить образы и пересобрать приложение
git pull && docker compose up -d --build app          # выкатить новую версию приложения
docker compose logs -f keycloak
ssh -L 5341:localhost:5341 user@server                # затем http://localhost:5341 — Seq
```

Бэкап SQL Server:

```bash
docker compose exec sqlserver bash -c '/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "BACKUP DATABASE [OnlineStore] TO DISK = N'"'"'/var/opt/mssql/data/OnlineStore.bak'"'"' WITH INIT"'
docker compose cp sqlserver:/var/opt/mssql/data/OnlineStore.bak ./OnlineStore.bak
```

## Подключение к SQL Server снаружи

SSMS / Rider / Azure Data Studio: сервер `DOMAIN,1433` (или IP сервера), SQL-аутентификация `sa` / `MSSQL_SA_PASSWORD`,
включите «Trust server certificate» (сертификат SQL Server самоподписанный).

Порт открыт для всего интернета, а Docker публикует порты в обход `ufw`. Поэтому:
- задайте сильный `MSSQL_SA_PASSWORD`;
- лучше ограничьте 1433 по IP в firewall'е провайдера/облака, либо
- поставьте `MSSQL_BIND=127.0.0.1` и подключайтесь через туннель: `ssh -L 1433:localhost:1433 user@server`, сервер `localhost,1433`.

## Важно

- Realm импортируется **только если его ещё нет** в БД Keycloak. Изменения `.env` (секреты, `DOMAIN`) после этого
  нужно вносить в консоли Keycloak вручную (Clients → mvc-template: Credentials, Valid redirect URIs).
- Данные живут в docker volumes (`sqlserver-data`, `seq-data`, `caddy-data`, `app-dataprotection`).
  `docker compose down -v` удалит их безвозвратно.
- Если домена нет и нужен только локальный тест, `DOMAIN=localhost` тоже работает: Caddy выдаст
  самоподписанный сертификат (браузер покажет предупреждение).
