# ASP.NET Core Microservices

A Dockerized microservices project built with **ASP.NET Core, C#, PostgreSQL, Redis, Entity Framework Core, YARP, and Docker Compose**.

The project demonstrates a simple microservices architecture with an API Gateway, independent services, persistent storage, and distributed caching.

## Architecture

```text
                    Client
                      |
                      v
               +--------------+
               | API Gateway  |
               |    YARP      |
               +------+-------+
                      |
              +-------+-------+
              |               |
              v               v
        NameService     Calculator
              |
        +-----+-----+
        |           |
        v           v
   PostgreSQL     Redis
```

### Services

- **API Gateway** – Routes client requests to the appropriate microservice using YARP.
- **NameService** – Handles name and product operations, PostgreSQL persistence, and Redis caching.
- **CalculatorService** – Provides calculator operations as an independent service.
- **PostgreSQL** – Persistent database used by NameService.
- **Redis** – Caches product data to reduce repeated database queries.

## Project Structure

```text
AspNetCore-Microservices/
│
├── ApiGateway/          # YARP API Gateway
├── Calculator/          # Calculator microservice
├── NameService/         # Name + product microservice
│   ├── DTO/             # Request DTOs
│   ├── Data/            # EF Core DbContext
│   ├── Models/          # Database entities
│   └── Migrations/      # EF Core migrations
│
├── docker-compose.yml   # Runs all containers
├── .env.example         # Environment variable template
├── .gitignore
└── README.md
```

## Request Flow

### Calculator

```text
Client
  ↓
API Gateway
  ↓
CalculatorService
  ↓
Response
```

### NameService

```text
Client
  ↓
API Gateway
  ↓
NameService
  ├──→ PostgreSQL
  └──→ Redis
```

Redis is used as a cache for product data. On a cache miss, NameService retrieves the data from PostgreSQL and stores it in Redis.

## API Endpoints

### Through API Gateway

```http
GET http://localhost:5132/api/calculator/add?a=10&b=5
```

```http
POST http://localhost:5132/api/names/concatenate
```

Example body:

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

### Direct service endpoints

```text
CalculatorService → http://localhost:5278
NameService        → http://localhost:5219
```

The API Gateway is the recommended entry point.

## Running the Project

Create a `.env` file in the root using `.env.example` as a template.

Then run from the repository root:

```bash
docker compose up --build
```

The application starts:

```text
API Gateway
NameService
CalculatorService
PostgreSQL
Redis
```

## Tech Stack

- C#
- ASP.NET Core
- Entity Framework Core
- PostgreSQL
- Redis
- YARP
- Docker
- Docker Compose
