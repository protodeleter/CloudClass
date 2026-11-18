# Avalonia + Cosmos DB vNext: Cross-platform local Docker setup

This folder contains infrastructure files to place at the **root of your existing Avalonia solution**. The Avalonia UI is run natively from Visual Studio (Windows), Rider, or VS Code (macOS), while the Cosmos DB vNext Emulator runs in Docker locally on each machine.

## Prerequisites

- Docker Desktop, running **Linux containers**.
- A compatible .NET SDK (`dotnet --version`). The same .NET SDK version should be used on both machines (a `global.json` in your main solution can pin it).
- A terminal opened at the folder containing `docker-compose.yml`.
- On Windows, stop the standalone Windows Cosmos DB Emulator if it already uses port 8081.

## 1. Generate and trust a persistent localhost HTTPS certificate on EACH computer

We provide a persistent PFX to the emulator (`CERT_PATH`), rather than trusting a new emulator-generated certificate after restarts. The certificate, PFX password and Docker volume stay local; **never commit or share the PFX or .env**.

### Windows PowerShell

From the directory with `docker-compose.yml`:

```powershell
New-Item -ItemType Directory -Force -Path docker/cosmos/certs | Out-Null
Copy-Item .env.example .env
# Edit .env, replacing COSMOS_CERT_PASSWORD with YOUR OWN locally chosen password.
# Use the SAME password in the following command.
dotnet dev-certs https --export-path ./docker/cosmos/certs/localhost.pfx --password "YOUR_LOCAL_PASSWORD" --trust
```

### macOS Terminal

```bash
mkdir -p docker/cosmos/certs
cp .env.example .env
# Edit .env, replacing COSMOS_CERT_PASSWORD with YOUR OWN locally chosen password.
# Use the SAME password in the following command.
dotnet dev-certs https --export-path ./docker/cosmos/certs/localhost.pfx --password "YOUR_LOCAL_PASSWORD" --trust
```

If your local .NET installation has a stale or invalid development cert, see `dotnet dev-certs https --help`. Do not run `--clean` casually: it can remove development certificates used by other applications.

The PFX is used ONLY for localhost development. The `CERT_SECRET` setting in Docker Compose must match the PFX password in your `.env`. The .NET SDK trusts the local development certificate through the OS when `dotnet dev-certs https --trust` is used (including with `--export-path`). This custom-certificate setup uses emulator features `CERT_PATH` and `CERT_SECRET`.

## 2. Start the emulator

```bash
docker compose up -d
# The first start downloads the emulator image and initializes the database.
docker compose logs -f cosmos
```

When ready, open:

- API endpoint (HTTPS): https://localhost:8081/
- Data Explorer (HTTP): http://localhost:1234/
- Readiness: http://localhost:8080/ready

The image is pinned to `vnext-EN20261008` to keep Windows and macOS on the same release. Change the tag deliberately on both computers after testing.

## 3. Verify database initialization

```bash
docker exec avalonia-cosmos-emulator cosmoshell.sh -c 'ls; query "SELECT VALUE COUNT(1) FROM c" --database=AvaloniaDemo --container=Customer'
```

Initialization files run in ascending filename order on the first run of a fresh data volume:

- `docker/cosmos/init/01-create-schema.csh` creates `AvaloniaDemo`, `Customer` and `Products`.
- `docker/cosmos/init/02-seed-data.csh` inserts 2 customers and 1 product.
- `docker/cosmos/init/03-verify.csh` queries the data.

Both containers use partition key path `/category`. Your C# `Customer` model must serialize a `category` field, and the partition key parameter must match its value.

## 4. Point your existing Avalonia project at the local emulator

Your existing `CosmosDbClient` and MVVM design can stay in place.

- Endpoint: `https://localhost:8081/`
- Key: the **public default Azure Cosmos DB Emulator account key**, NOT a production key.
- Database name to supply to your existing constructor: `AvaloniaDemo`.
- Containers: `Customer`, `Products`.
- SDK option: `ConnectionMode = ConnectionMode.Gateway`.

Use `new CosmosDbClient("AvaloniaDemo")` once at startup, and call your existing `InsertAsync("Customer", customer, customer.Category)` from a ViewModel command. The application remains native and uses `localhost` on each computer independently.

Do not commit production keys. For non-emulator environments, use proper credential management and authentication.

## 5. Everyday commands

```bash
docker compose up -d          # Start the local emulator
docker compose ps             # See service status
docker compose logs -f cosmos # View startup, init scripts and errors
docker compose down           # Stop without deleting database
docker compose down -v        # DANGER: deletes your local emulator data volume
```

**Initialization runs only for a new data volume.** If you edit `*.csh` after initial setup, scripts will NOT automatically rerun on an existing volume. To reset, use `docker compose down -v` then `docker compose up -d`, but this permanently deletes all local emulator documents.

## Troubleshooting

- **Port 8081 already in use (Windows):** Exit/stop the native Windows Cosmos DB Emulator or change port mappings AND the application endpoint.
- **TLS handshake / untrusted certificate:** Check you ran `dotnet dev-certs https --trust` on that machine, exported the same certificate into `docker/cosmos/certs/localhost.pfx`, and matched the PFX password in `.env`. Restart the Avalonia app. Verify the certificate is for `localhost`. Do not globally disable certificate validation.
- **Container doesn't start:** `docker compose logs cosmos`; verify PFX file exists, CERT_SECRET matches, Docker Desktop runs Linux containers.
- **HTTP vs HTTPS:** Use **HTTPS on 8081** for the .NET Cosmos SDK. The Data Explorer deliberately uses HTTP on 1234.
- **Missing databases:** Check that the data volume was initially empty and inspect startup logs for initialization errors. If reset is acceptable, delete the volume with `docker compose down -v` and restart.
- **Partition key mismatch (400/1001):** Ensure the document contains lowercase `category`, and the `PartitionKey` matches its value. Containers already created with another partition key require a different container.
- **Different documents on Windows/macOS:** This is expected. Each machine has its own named Docker volume; Git shares scripts, not live database data.

## Resources

- Microsoft vNext emulator: https://learn.microsoft.com/en-us/azure/cosmos-db/emulator-linux
- Official vNext image releases: https://github.com/Azure/azure-cosmos-db-emulator-docker/releases
- Microsoft `dotnet dev-certs` command: https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-dev-certs

Note: The Compose file and scripts follow the documented CLI/Compose syntax but have not been run against an emulator Docker daemon in this environment. If you encounter a startup failure, check `docker compose logs cosmos` and confirm the emulator image's certificate compatibility.
