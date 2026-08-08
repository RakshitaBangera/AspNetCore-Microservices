# ASP.NET Core Microservices

## 1. What is this project?
    What problem it demonstrates
    What a microservice is in THIS project
    Why the project is split this way

## 2. Big-picture architecture

    Client
       ↓
    API Gateway
       ↓
    ┌───────────────┬─────────────────┐
    ↓               ↓
    NameService     CalculatorService
       ↓
    ┌───────┴────────┐
    ↓                ↓
 PostgreSQL         Redis

    PLUS explanation of every arrow.

## 3. Complete request lifecycle

### Calculator request
    Postman
      ↓
    localhost:5132
      ↓
    YARP
      ↓
    calculatorservice:8080
      ↓
    CalculatorController
      ↓
    response

    Explain EXACTLY what happens at every step.

### Name request
    Postman
      ↓
    API Gateway
      ↓
    NameService
      ↓
    NameController
      ↓
    NameRequest DTO
      ↓
    NameDbContext
      ↓
    PostgreSQL

    Then explain Redis separately.

## 4. Repository structure

    AspNetCore-Microservices/
    │
    ├── ApiGateway/
    ├── Calculator/
    ├── NameService/
    ├── docker-compose.yml
    ├── .env
    ├── .env.example
    ├── .gitignore
    └── SolutionTemplate.slnx

    Then explain EVERY ONE.

## 5. ApiGateway/

    Program.cs
        What it does
        Why YARP is registered
        Why MapReverseProxy exists

    appsettings.json
        ReverseProxy
        Routes
        Clusters
        Destinations
        Path transformation

    Dockerfile
        Stage by stage explanation

## 6. NameService/

    DTO/
        NameRequest.cs
        Why DTO exists

    Models/
        Name.cs
        Product.cs
        Entity vs DTO

    Data/
        NameDbContext.cs
        DbSet
        EF Core

    Migrations/
        What a migration is
        Why there are multiple migrations
        Designer files
        ModelSnapshot
        Database.Migrate()

    NameController.cs
        Endpoint
        Request
        Database operation
        Redis operation
        Response

    Program.cs
        DI
        PostgreSQL
        Redis
        migrations
        middleware

## 7. Calculator/

    Controller
    Program
    csproj
    Dockerfile
    configuration

## 8. PostgreSQL

    Why PostgreSQL exists
    What tables exist
    How NameService connects
    Connection string
    Why "postgres" is used inside Docker
    Why localhost is different

## 9. Entity Framework Core

    Controller
       ↓
    DbContext
       ↓
    EF Core
       ↓
    Npgsql
       ↓
    PostgreSQL

    Explain migrations and Database.Migrate()

## 10. Redis

    Why cache is needed
    Cache miss
    Database lookup
    Cache population
    Cache hit
    TTL
    What happens when TTL expires

## 11. Docker

    What each Dockerfile does
    Image vs container
    Build vs run
    Why each service has its own container

## 12. Docker Compose

    Explain EVERY service in docker-compose.yml

    postgres
    redis
    nameservice
    calculatorservice
    apigateway

    Explain:
    build
    context
    ports
    environment
    depends_on
    volumes

## 13. Docker networking

    localhost vs service name

    Host:
    localhost:5132

    Docker:
    nameservice:8080
    calculatorservice:8080
    postgres:5432
    redis:6379

    Explain why.

## 14. Environment variables

    .env
    .env.example
    .gitignore
    why secrets aren't committed

## 15. Port mapping

    Host                 Docker
    localhost:5132  →    gateway:8080
    localhost:5219  →    nameservice:8080
    localhost:5278  →    calculator:8080
    localhost:5432  →    postgres:5432
    localhost:6379  →    redis:6379

    Explain what each side means.

## 16. API Gateway routing

    /api/names/*
          ↓
    NameService

    /api/calculator/*
          ↓
    CalculatorService

    Explain the path transformation
    and why the services themselves don't necessarily
    receive the exact same URL the client sends.

## 17. Running locally WITHOUT Docker

    dotnet restore
    dotnet build
    dotnet run / dotnet watch

    Explain why this is different from Docker.

## 18. Running WITH Docker

    docker compose up --build

    Explain:
    compose
    build
    image
    container
    network
    volume

## 19. Testing

    Exact Postman URLs
    Example request bodies
    Expected responses

## 20. Troubleshooting

    Redis localhost error
    PostgreSQL "relation does not exist"
    Port already allocated
    Docker container exited
    Gateway connection refused
    HTTPS redirection warning
    Wrong Calculator folder
    etc.

## 21. Technologies

    Not just a list —
    explain WHY each technology is being used.

## 22. What this project demonstrates

    Microservices
    API Gateway
    Docker
    PostgreSQL
    EF Core
    Redis
    networking
    configuration
    caching
