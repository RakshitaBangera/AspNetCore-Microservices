# ASP.NET Core Microservices

A containerized microservices application built with **ASP.NET Core, C#, PostgreSQL, Redis, Entity Framework Core, YARP API Gateway, and Docker Compose**.

This project demonstrates how independent backend services can work together through an API Gateway while using PostgreSQL for persistent storage and Redis for caching.

---

## Overview

The application consists of two independent microservices:

- **NameService** – handles name and product-related operations
- **CalculatorService** – handles calculator operations

Both services are exposed through a central **YARP API Gateway**.

NameService additionally communicates with:

- **PostgreSQL** for persistent data storage
- **Redis** for caching

Docker Compose is used to run the entire system as a group of containers.

---

# Architecture

```text
                              Client
                           (Postman / UI)
                                |
                                |
                                v
                     +---------------------+
                     |     API Gateway     |
                     |        YARP         |
                     |      :5132          |
                     +----------+----------+
                                |
                    +-----------+-----------+
                    |                       |
                    v                       v
          +------------------+     +-------------------+
          |   NameService    |     | CalculatorService |
          |   ASP.NET Core   |     |   ASP.NET Core    |
          |      :8080       |     |       :8080       |
          +--------+---------+     +-------------------+
                   |
             +-----+------+
             |            |
             v            v
      +------------+   +-------+
      | PostgreSQL |   | Redis |
      |    :5432   |   | :6379 |
      +------------+   +-------+
```

### Request flow

A client communicates with the API Gateway:

```text
Client
  |
  v
API Gateway
  |
  +----> NameService
  |
  +----> CalculatorService
```

NameService then communicates with the data infrastructure:

```text
NameService
    |
    +----> PostgreSQL
    |
    +----> Redis
```

---

# Repository Structure

```text
AspNetCore-Microservices/
│
├── ApiGateway/
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Dockerfile
│   ├── .dockerignore
│   └── ApiGateway.csproj
│
├── Calculator/
│   ├── CalculatorController.cs
│   ├── Program.cs
│   ├── Dockerfile
│   ├── .dockerignore
│   └── Calculator.csproj
│
├── NameService/
│   ├── DTO/
│   ├── Data/
│   ├── Models/
│   ├── Migrations/
│   ├── NameController.cs
│   ├── Program.cs
│   ├── Dockerfile
│   └── NameService.csproj
│
├── docker-compose.yml
├── .env.example
├── .gitignore
├── SolutionTemplate.slnx
└── README.md
```

---

# Services

## API Gateway

**Location:** `ApiGateway/`

The API Gateway provides a single entry point into the application.

It uses **YARP (Yet Another Reverse Proxy)** to forward incoming requests to the appropriate microservice.

Instead of exposing the internal architecture to the client:

```text
Client
  |
  +----> NameService
  |
  +----> CalculatorService
```

the client only communicates with:

```text
Client
  |
  v
API Gateway
```

The Gateway then handles routing internally.

### Gateway responsibilities

- Receive client requests
- Match requests against configured routes
- Forward requests to the correct service
- Hide internal Docker service addresses
- Provide a single public entry point

### Gateway routes

```text
/api/names/*
        |
        v
   NameService

/api/calculator/*
        |
        v
 CalculatorService
```

Inside the Docker network, the Gateway communicates with:

```text
http://nameservice:8080
http://calculatorservice:8080
```

---

# NameService

**Location:** `NameService/`

NameService is the data-oriented microservice in the application.

It is responsible for:

- Processing names
- Persisting names in PostgreSQL
- Working with product data
- Checking Redis for cached products
- Loading products from PostgreSQL when they are not cached
- Returning the final API response

The service uses:

```text
ASP.NET Core
     |
     +---- Entity Framework Core
     |
     +---- PostgreSQL
     |
     +---- Redis
```

---

## NameService Structure

```text
NameService/
│
├── DTO/
│   └── NameRequest.cs
│
├── Data/
│   └── NameDbContext.cs
│
├── Models/
│   ├── Name.cs
│   └── Product.cs
│
├── Migrations/
│   └── Entity Framework migrations
│
├── NameController.cs
├── Program.cs
├── Dockerfile
├── .dockerignore
├── appsettings.json
└── NameService.csproj
```

### DTO

```text
NameService/DTO/
```

Contains request models used by the API.

`NameRequest.cs` represents incoming name information.

Example:

```json
{
  "firstName": "John",
  "lastName": "Doe"
}
```

DTOs keep API request models separate from database entities.

---

### Data

```text
NameService/Data/
```

Contains the Entity Framework Core database context.

`NameDbContext.cs` is responsible for defining the application's database access layer.

It contains the database sets for:

```text
Names
Products
```

The overall data flow is:

```text
NameController
      |
      v
NameDbContext
      |
      v
Entity Framework Core
      |
      v
PostgreSQL
```

---

### Models

```text
NameService/Models/
```

Contains database entities.

```text
Name.cs
Product.cs
```

`Name.cs` represents name records stored in PostgreSQL.

`Product.cs` represents product records stored in PostgreSQL.

---

### Migrations

```text
NameService/Migrations/
```

Contains Entity Framework Core migrations.

Migrations allow the database schema to be version-controlled alongside the application.

The database can be created or updated using the migration history instead of manually creating tables.

At application startup, pending migrations are applied through:

```csharp
db.Database.Migrate();
```

The flow is:

```text
NameService starts
       |
       v
Database.Migrate()
       |
       v
PostgreSQL
       |
       v
Apply pending migrations
```

---

### NameController

```text
NameService/NameController.cs
```

Contains the NameService API endpoints.

The controller handles incoming requests and coordinates the application's database and cache operations.

---

### Program.cs

```text
NameService/Program.cs
```

Configures the NameService application.

It registers:

- ASP.NET Core controllers
- Entity Framework Core
- PostgreSQL
- Redis
- Database migration execution

---

# CalculatorService

**Location:** `Calculator/`

CalculatorService is an independent ASP.NET Core microservice.

Its responsibility is intentionally simple: perform calculator operations.

This demonstrates that functionality can be isolated into separate services rather than being implemented inside one large application.

### Structure

```text
Calculator/
│
├── CalculatorController.cs
├── Program.cs
├── Calculator.csproj
├── Dockerfile
├── .dockerignore
├── appsettings.json
└── Calculator.http
```

### Example

```http
GET /api/add?a=10&b=5
```

Response:

```text
15
```

CalculatorService does not depend on NameService, PostgreSQL, or Redis.

---

# PostgreSQL

PostgreSQL is the **persistent database** used by NameService.

It stores application data such as:

```text
Names
Products
```

The PostgreSQL container is defined in:

```text
docker-compose.yml
```

A Docker volume is attached to PostgreSQL:

```text
postgres_data
```

This allows database data to persist even when the PostgreSQL container is recreated.

---

# Redis

Redis is used as a **cache** for product data.

PostgreSQL remains the persistent source of the data, while Redis provides faster access to frequently requested product information.

The caching flow is:

```text
                 Request
                    |
                    v
               NameService
                    |
                    v
               Check Redis
                    |
             +------+------+
             |             |
           HIT            MISS
             |             |
             v             v
       Return Cache    PostgreSQL
                           |
                           v
                      Product Data
                           |
                           v
                      Store in Redis
                           |
                           v
                       Response
```

On a cache hit:

```text
NameService → Redis → Response
```

On a cache miss:

```text
NameService → Redis
                  |
                 MISS
                  |
                  v
             PostgreSQL
                  |
                  v
                Redis
                  |
                  v
               Response
```

The cached data uses a TTL, so Redis automatically expires the cached entry after the configured period.

---

# Docker

Each application service has its own Dockerfile.

```text
ApiGateway/
    └── Dockerfile

NameService/
    └── Dockerfile

Calculator/
    └── Dockerfile
```

This allows each service to be packaged as an independent Docker image.

The resulting containers are:

```text
apigateway
nameservice
calculatorservice
microservices-postgres
microservices-redis
```

---

# Docker Compose

The root-level:

```text
docker-compose.yml
```

orchestrates the entire application.

It defines:

```text
postgres
redis
nameservice
calculatorservice
apigateway
```

The overall startup flow is:

```text
docker compose up --build
          |
          v
    Build application
       images
          |
          v
    Create containers
          |
          v
    Create Docker network
          |
          v
      Start services
```

---

# Docker Networking

Docker Compose creates a shared network for the services.

Containers communicate using their **service names**.

For example:

```text
API Gateway
    |
    +----> nameservice:8080
    |
    +----> calculatorservice:8080
```

NameService communicates with:

```text
NameService
    |
    +----> postgres:5432
    |
    +----> redis:6379
```

### Why not `localhost`?

Inside a container, `localhost` refers to **that same container**.

Therefore:

```text
localhost:6379
```

from inside NameService would mean:

> "Look for Redis inside the NameService container."

That is incorrect.

Instead, Docker's service discovery allows:

```text
redis:6379
```

to reach the Redis container.

The same applies to PostgreSQL:

```text
postgres:5432
```

and the other application services.

---

# Host Ports vs Container Ports

The application uses different ports depending on whether the request comes from the host machine or from another Docker container.

| Service | Host | Container |
|---|---:|---:|
| API Gateway | `5132` | `8080` |
| NameService | `5219` | `8080` |
| CalculatorService | `5278` | `8080` |
| PostgreSQL | `5432` | `5432` |
| Redis | `6379` | `6379` |

From Postman on the host machine:

```text
http://localhost:5132
```

Inside Docker:

```text
http://apigateway:8080
```

Similarly:

```text
Host:
http://localhost:5219

Docker:
http://nameservice:8080
```

and:

```text
Host:
http://localhost:5278

Docker:
http://calculatorservice:8080
```

---

# Complete NameService Flow

A typical NameService request looks like this:

```text
                         Client
                           |
                           v
                    localhost:5132
                           |
                           v
                     API Gateway
                           |
                           | /api/names/*
                           v
                      NameService
                           |
                    +------+------+
                    |             |
                    v             v
              PostgreSQL       Redis
                    |             |
                    |        Cache lookup
                    |             |
                    |        +----+----+
                    |        |         |
                    |       HIT       MISS
                    |        |         |
                    |        |         v
                    |        |    PostgreSQL
                    |        |         |
                    |        |         v
                    |        |      Redis
                    |        |         |
                    +--------+---------+
                             |
                             v
                          Response
```

---

# Complete Calculator Flow

```text
Client
  |
  v
localhost:5132
  |
  v
API Gateway
  |
  | /api/calculator/*
  v
CalculatorService
  |
  v
CalculatorController
  |
  v
Result
```

---

# Environment Variables

The project uses a root `.env` file for local configuration.

Example:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
POSTGRES_DB=nameservice_db
```

The actual `.env` file should not be committed to Git.

Instead, the repository contains:

```text
.env.example
```

which provides the expected configuration structure.

---

# `.gitignore`

The root `.gitignore` prevents sensitive and generated files from being committed.

Important entries include:

```text
.env
bin/
obj/
.vs/
.vscode/
*.user
```

This keeps:

- Database credentials
- Build output
- IDE-specific files
- Local development files

out of the repository.

---

# API Gateway Routes

The Gateway exposes clean client-facing routes.

## NameService

```text
/api/names/*
```

Example:

```http
POST http://localhost:5132/api/names/concatenate
```

The Gateway forwards the request to the NameService container.

---

## CalculatorService

```text
/api/calculator/*
```

Example:

```http
GET http://localhost:5132/api/calculator/add?a=10&b=5
```

The Gateway forwards the request to CalculatorService.

---

# API Examples

## Calculator

### Request

```http
GET http://localhost:5132/api/calculator/add?a=10&b=5
```

### Response

```text
15
```

---

## NameService

### Request

```http
POST http://localhost:5132/api/names/concatenate
```

### Example Body

```json
[
  {
    "firstName": "John",
    "lastName": "Doe"
  },
  {
    "firstName": "Jane",
    "lastName": "Smith"
  }
]
```

---

# Running the Project

## Prerequisites

Install:

- .NET 10 SDK
- Docker Desktop
- Git

---

## Clone the repository

```bash
git clone https://github.com/RakshitaBangera/AspNetCore-Microservices.git
```

```bash
cd AspNetCore-Microservices
```

---

## Configure environment variables

Create a `.env` file in the repository root.

Use `.env.example` as a template.

Example:

```env
POSTGRES_USER=postgres
POSTGRES_PASSWORD=your_password
POSTGRES_DB=nameservice_db
```

---

## Start all services

Run this command from the **repository root**, where `docker-compose.yml` is located:

```bash
docker compose up --build
```

To run in detached mode:

```bash
docker compose up --build -d
```

---

## Check running containers

```bash
docker ps
```

Expected containers:

```text
apigateway
nameservice
calculatorservice
microservices-postgres
microservices-redis
```

---

## Stop the application

```bash
docker compose down
```

---

# Local Development vs Docker

The services can also be run individually during development.

For example:

```bash
cd NameService
dotnet watch run
```

This runs NameService directly using the local .NET environment.

However, when the complete application is containerized, the recommended approach is:

```bash
docker compose up --build
```

This starts:

```text
API Gateway
NameService
CalculatorService
PostgreSQL
Redis
```

together.

---

# Project Technologies

| Technology | Purpose |
|---|---|
| C# | Application development |
| ASP.NET Core | REST API framework |
| .NET 10 | Runtime |
| YARP | API Gateway / Reverse Proxy |
| Entity Framework Core | Database access |
| PostgreSQL | Persistent relational storage |
| Npgsql | PostgreSQL provider for .NET |
| Redis | Distributed caching |
| StackExchange.Redis | Redis client |
| Docker | Containerization |
| Docker Compose | Multi-container orchestration |
| Git | Version control |
| GitHub | Source control and collaboration |

---

# Key Concepts Demonstrated

This project demonstrates practical implementation of:

- Microservice architecture
- Independent ASP.NET Core services
- API Gateway pattern
- Reverse proxy routing
- REST APIs
- Entity Framework Core
- PostgreSQL persistence
- EF Core migrations
- Redis distributed caching
- Cache-aside pattern
- TTL-based caching
- Docker containerization
- Docker Compose
- Docker networking
- Container-to-container communication
- Environment variables
- Docker volumes
- Service separation

---

# Future Improvements

Possible future extensions include:

- JWT authentication
- Role-based authorization
- Service-to-service authentication
- Health checks
- Centralized logging
- OpenTelemetry
- Distributed tracing
- Swagger/OpenAPI
- Automated integration tests
- GitHub Actions CI/CD
- Kubernetes deployment
- Load balancing
- AWS deployment
- Azure deployment

---

# Author

**Rakshita Bangera**

GitHub:  
https://github.com/RakshitaBangera
