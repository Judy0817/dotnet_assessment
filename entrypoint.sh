#!/bin/bash

# Start SQL Server
/opt/mssql/bin/sqlservr &

# Wait for SQL Server to start (max 60 seconds)
for i in {1..60}; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -Q "SELECT 1" &> /dev/null
    if [ $? -eq 0 ]; then
        echo "SQL Server is up"
        break
    else
        echo "Waiting for SQL Server to start"
        sleep 1
    fi
done

# Run the initialization script
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -d master -i /usr/src/app/setup.sql

# Prevent the container from exiting
tail -f /dev/null