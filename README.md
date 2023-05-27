# Introduction

Just a simple little application to play around with Quartz.NET and SQL Server.

# Setup

> **Warning**
> This project will be full of shortcuts and bad practices. It is not intended to be used in production. Just simply a playground for me to learn and experiment with.

## Docker

### Install Docker Image
```plain
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=p@ssw0rd' -e MSSQL_PID=Developer -p 1433:1433 -v sql_server_volume:/var/opt/mssql --name sql_server_container -d mcr.microsoft.com/mssql/server:2019-latest
```

### Enable SQL Server Agent
```
/opt/mssql/bin/mssql-conf set sqlagent.enabled true
```

### Install Sample Database

[AdventureWorks 2019](https://learn.microsoft.com/en-us/sql/samples/adventureworks-install-configure?view=sql-server-ver16&tabs=ssms)
