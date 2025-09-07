#!/usr/bin/env bash
set -euo pipefail

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait until SQL Server is accepting connections
echo "Waiting for SQL Server to accept connections..."
until /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "${SA_PASSWORD}" -Q "SELECT 1" >/dev/null 2>&1; do
  sleep 2
done

DB="${DB_NAME}"
echo "Ensuring database [${DB}] exists..."
/opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "${SA_PASSWORD}" \
  -Q "IF DB_ID('${DB}') IS NULL CREATE DATABASE [${DB}];"

# Keep the sqlservr process in the foreground
wait