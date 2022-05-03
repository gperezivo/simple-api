# simple-api
Simple API on .NET 6 to read MyVar and MyVar2 from environment variable.



>For demo purpose, the API reads an SQL connection data from config file and environment variables and execute a query.
## /sql-connection
Reads the SQL from the appsettings.json file and environment variables.
```json
 "Database":{
    "User": "sa",
    "Password": "insecurepassword",
    "Name": "TestDB",
    "Server": "localhost"
  }
```
To override the SQL connection, you can set the following environment variables:
```
  DATABASE__USER=sa
  DATABASE__PASSWORD=insecurepassword
  DATABASE__NAME=TestDB
  DATABASE__SERVER=localhost
```
### Response
```json
{
    "sqlConnection":"Data Source=localhost;Initial Catalog=DockerDatabase;User ID=sa;Password=insecurepassword;Multiple Active Result Sets=True;Connect Timeout=30",
    "version":"v2"
}
```

## /sql
Executes the SQL Query on configured Database (see [/sql-connection](#sql-connection) section) and return results.

```SQL
select * from sys.tables
```

## /external-api

Calls to external API and return results.
The Api Url is read from the appsettings.json file and environment variable `APIURL`

## /ip 

Returns the request IP address.

