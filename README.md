# ASP.NET Core Microservices

A containerized microservices project built with ASP.NET Core, C#, PostgreSQL, Redis, Entity Framework Core, YARP API Gateway, and Docker Compose.

The project demonstrates how independent microservices communicate through an API Gateway while using PostgreSQL for persistent storage and Redis for caching.

---

# Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Complete Request Flow](#complete-request-flow)
- [Repository Structure](#repository-structure)
- [API Gateway](#1-api-gateway)
- [NameService](#2-nameservice)
- [Calculator Service](#3-calculator-service)
- [PostgreSQL](#4-postgresql)
- [Redis](#5-redis)
- [Docker Compose](#6-docker-compose)
- [Docker Networking](#7-docker-networking)
- [Environment Configuration](#8-environment-configuration)
- [Port Configuration](#9-port-configuration)
- [API Endpoints](#10-api-endpoints)
- [Database and EF Core Migrations](#11-database-and-ef-core-migrations)
- [Redis Caching Flow](#12-redis-caching-flow)
- [Running the Project](#13-running-the-project)
- [Technology Stack](#14-technology-stack)
- [Future Improvements](#15-future-improvements)

---

# Project Overview

The application is divided into independent services instead of implementing all functionality inside one ASP.NET Core application.

The main components are:

1. **API Gateway**
2. **NameService**
3. **CalculatorService**
4. **PostgreSQL**
5. **Redis**

The API Gateway provides a single entry point for clients.

NameService handles names and products and communicates with PostgreSQL and Redis.

CalculatorService provides calculator functionality as an independent microservice.

PostgreSQL provides persistent storage.

Redis provides fast, temporary cached access to product data.

Docker Compose is used to run all components together.

---

# Architecture

```text
                         CLIENT / POSTMAN
                                |
                                |
                                v
                     +---------------------+
                     |     API GATEWAY     |
                     |        YARP         |
                     |      Port 5132      |
                     +----------+----------+
                                |
                 +--------------+--------------+
                 |                             |
                 |                             |
                 v                             v
        +------------------+          +--------------------+
        |   NameService    |          | CalculatorService |
        |   ASP.NET Core   |          |   ASP.NET Core     |
        |      :8080       |          |       :8080        |
        +--------+---------+          +--------------------+
                 |
           +-----+------+
           |            |
           v            v
    +------------+   +-------+
    | PostgreSQL |   | Redis |
    |    :5432   |   | :6379 |
    +------------+   +-------+
