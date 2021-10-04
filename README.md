# BodyShop Booster - Test Project - Vinicius Silva
## Description
This repository contains an implementation for the test project for a .NET Back-End Developer position at [BodyShop Booster](https://www.bodyshopbooster.com/).

## Used libraries and frameworks

The main Web API application has been developed using [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-5.0), runing on [.NET 5.0](https://dotnet.microsoft.com/). The application's database is expected to be a [Microsoft SQL Server (v2019 or later)](https://www.microsoft.com/en-us/sql-server/sql-server-2019) instance, and data access was implemented by using [Entity Framework Core v5.0.10](https://docs.microsoft.com/en-us/ef/core/).

[OpenAPI support](https://swagger.io/specification/) has been added to the project through the [Swashbuckle OpenAPI Documents generator](https://www.nuget.org/packages/Swashbuckle.AspNetCore.Swagger/). There is also an included browsable user interface which allows humans to read the documentation on the back-end server's endpoints, as illustrated below.

![Swashbuckle's OpenAPI UI Viewer](docs/images/swashbuckle-ui.png "Title")

Unit tests were implemented by using the [latest Microsoft's Test SDK templates](https://www.nuget.org/packages/Microsoft.NET.Test.Sdk/) (as of the time of writing this document), and the implementation was done in an [XUnit-based project (v2.4.1)](https://xunit.net/). Support for mocking and stubbing of object and service instances is provided by the popular [Moq Library (v4.16)](https://github.com/moq/moq4).

Finally, the project also features some Dockerfiles for executing both the back-end server and its corresponding unit tests. These files were written and tested using [Docker v20.10.8](https://www.docker.com/) on a Windows platform, running over a [Windows Subsystem for Linux v2 (WSL2)](https://docs.microsoft.com/en-us/windows/wsl/install)-based engine.

# Running the project
## Prerequisites

For running the project on your local machine, it is necessary to have the [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0) installed. Also, for executing the Entity Framework Core's database migrations, it is necessary to have a recent [EF Core Tooling](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) installed.

This project also provides some alternative Docker-based ways to run, which will be described bellow. These are:

* By using Docker to build the project's Dockerfiles and run the back-end server and/or the unit tests.
* By using Docker in conjunction with [Visual Studio Code's Development Containers feature](https://code.visualstudio.com/docs/remote/containers), which allows you to run Visual Studio Code within an isolated and pre-configured container that has all of the necessary dependencies installed.

**IMPORTANT:** The application's database should be running and initialized before the back-end server is run. Each of the following sections will describe the necessary procedures for doing so.

The following sections will assume you have a local copy of the project's code, and that you are working within the project's root folder. They will also assume you have the [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0), the [EF Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet), and [Docker](https://www.docker.com/) installed - depending in what's necessary for each specific case.

If using any of the Docker-based solutions (including a Docker-based Microsoft SQL Server database), it is recommended to create a Docker network calleds "**bsb-test-project-network**" to avoid having to do extra configurations in the project.

```
docker network create bsb-test-project-network
```

### Running the database: the recommended way

For development purposes, it might be interesting to run the database using Docker. The following are the default configurations which are expected for the database:

* **Default/expected port:** 1433 (default Microsoft SQL Server port).
* **Default/expected host:**
	* **If running locally:** localhost
	* **If running using Docker:** bsb-test-project-database
		* It should be present in a Docker network called **bsb-test-project-network**. This allows for all of the development solutions to run seamlessly, without the need of any extra configurations.
* **Default/expected credentials:**
	* **User:** sa
	* **Password:** my-database-default-P4SSWORD

The following command can be used to start the database, by using the default configurations mentioned above:

```
docker run -it \
	-e "ACCEPT_EULA=Y" \
	-e "SA_PASSWORD=my-database-default-P4SSWORD" \
	-p 1433:1433 \
	--name=bsb-test-project-database \
	--network=bsb-test-project-network \
	mcr.microsoft.com/mssql/server:2019-CU12-ubuntu-20.04
```

### Initializing the database

This project used Entity Framework Core Migrations in order to keep the database's structure synchronized between users. Thus, it is recommended to have [EF Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) installed in order to initialize the database's structure.

After installing the tools and spinning up the database, you can enter the back-end server project's folder and execute the migrations in order to initialize the database and its structure.

```
cd src/webapi/
dotnet ef database update
```

Assuming all configurations are correct, this will:
* Build the project.
* Connect to the database's server.
* Create the database (if it still does not exist).
* Create the database tables.

## Running locally

You can run the back-end server locally on your machine by entering the `src/webapi` folder, and running the `dotnet run` command. That folder contains the back-end server's project.

```
cd src/webapi/
dotnet run
```

This will automatically restore the project's dependencies, build it, and running, according to necessary. Notice that the back-end server will try to bind to the address **http://localhost:5000** by default. Please, refer to the end of this section in order to learn how to configure this and other options for the back-end server.

In order to run the Unit Tests, you should enter the `src/webapi-unit` project's folder and run the `dotnet test` command.

```
cd src/webapi-unit/
dotnet test
```

### Configuring

You can change the default port where the back-end server runs by passing in a configuration via the `--urls` command-line parameter to the `dotnet run` command. For example, the following command would run the server on port 5008:

```
dotnet run --urls=http://*:5008
```

If necessary, you can also change the connection string used to connect to the database by passing in the `ConnectionStrings:DatabaseConnectionString="<connection-string>"` options. Via the connection string, it is possible to alter the target database's name, user, password, and other properties.

```
dotnet run ConnectionStrings:DatabaseConnectionString="Server=200.100.123.50;Database=my_database_name;User Id=my_user;Password=S0m3-OtHeR-PASSw0rd;"
```

## Running with Docker

The second easiest way to run the project is by using Docker. This repository contains 2 Dockerfiles: one for running the back-end server, and other one for running the Unit Tests for the server.

Assuming you want to run both the back-end server and the database using Docker, here are the required steps:

```
# Ensure you have created a network to contain both the back-end
# and the database of the application
docker network create bsb-test-project-network


# Spin-up the database with the default settings expected by the back-end
docker run -it \
	-e "ACCEPT_EULA=Y" \
	-e "SA_PASSWORD=my-database-default-P4SSWORD" \
	-p 1433:1433 \
	--name=bsb-test-project-database \
	--network=bsb-test-project-network \
	mcr.microsoft.com/mssql/server:2019-CU12-ubuntu-20.04

# Set the working directory to the "src/" folder, which contains
# the Dockerfiles of the project.
cd src/


# Build and run an ephemeral container for the back-end server:
docker build -t bsb-test-project -f Dockerfile-webapi .
docker run --rm -it \
	-p 5000:5000 \
	--name bsb-test-project-container \
	--network=bsb-test-project-network \
	bsb-test-project


# (Optional) Build and run an ephemeral container for
# executing the Unit Tests:
docker build -t bsb-test-project-unit -f Dockerfile-webapi-unit .
docker run --rm -it bsb-test-project-unit
```


## Running locally with Visual Studio Code Devcontainers

If you use Visual Studio Code, you can leverage the Development Containers feature it has. With this feature, Vscode will be able to detect the `.devcontainer` folder of the project, parse it, and create a development container which is fully configured for you to start developing right away.

The development container for this project is configured to include:

* The [.NET 5 SDK](https://dotnet.microsoft.com/download/dotnet/5.0).
* The [EF Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet), which allow you to execute database operations such as Migrations.
* The `ms-dotnettools.csharp` extension, which allows you to develop C#-based projects in Visual Studio Code.

The container comes pre-configured to joint the default "**bsb-test-project-network**". Nevertheless, an extra configuration step is required to allow it to connect to the database (assuming it is running with the recommended Docker-based way): you should edit the `appsettings.Development.json` file and update the database's Connection String to point to the "**bsb-test-project-database**" host.

```
{
  "Logging": {
    ...
  },
  "ConnectionStrings": {
    "DatabaseConnectionString": "Server=bsb-test-project-database;Database=bsb_testproject_database;User Id=sa;Password=my-database-default-P4SSWORD;"
  }
}
```

It is also possible to modify the port for the back-end server by editing the `.devcontainer/devcontainer.json` file and specifying the target port (in Docker format) using the `appPort` field. For example, to run the server in port 5008, use the following configuration:

```
{
	...
	"appPort": "5000:5000"
}
```


# Deployment plans
## Deploying to a Kubernetes cluster
## CI/CD