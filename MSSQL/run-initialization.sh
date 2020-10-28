# Wait to be sure that SQL Server came up
sleep 30s

# Run the setup script to create the DB and the schema in the DB
# Note: make sure that your password matches what is in the Dockerfile
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Pa&&word2020" -d master -i create-database-and-table.sql

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Pa&&word2020" -d master -i PersistAppointment_StoredProcedure.sql
