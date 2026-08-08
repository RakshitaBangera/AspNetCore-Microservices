# ASP.NET Core Microservices

A Dockerized microservices application built with ASP.NET Core, PostgreSQL, Redis, Entity Framework Core, YARP API Gateway, and Docker Compose.

The project demonstrates microservice separation, REST API development, database persistence, Redis caching, API Gateway routing, containerization, and Docker-based service orchestration.

---

## Architecture

```text
                         Client / Postman
                               |
                               v
                    +---------------------+
                    |     API Gateway     |
                    |        YARP         |
                    +----------+----------+
                               |
                 +-------------+-------------+
                 |                           |
                 v                           v
        +----------------+          +----------------+
        |  NameService   |          | Calculator     |
        | ASP.NET Core   |          |   Service      |
        +-------+--------+          +----------------+
                |
          +-----+-----+
          |           |
          v           v
    PostgreSQL      Redis
