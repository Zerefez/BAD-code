# Shared Experiences Docker Setup

This repository contains Docker configurations for running the Shared Experiences API with SQL Server and MongoDB databases.

## Prerequisites

- [Docker](https://www.docker.com/products/docker-desktop/) installed on your machine
- [Docker Compose](https://docs.docker.com/compose/install/) (included with Docker Desktop for Windows and Mac)

## Getting Started

1. Navigate to the SharedExperiences-MSSQL directory
2. Run the following command to build and start all containers:

```bash
docker-compose up -d
```

This will:
- Build the API container
- Pull and start SQL Server and MongoDB containers
- Set up networking between containers
- Mount volumes for database persistence
- Configure the API to connect to both databases

## Accessing the Application

- API: http://localhost:8080
- Swagger UI: http://localhost:8080 (Swagger is enabled for production)
- SQL Server: localhost:1433
- MongoDB: localhost:27017

## Environment Configuration

The docker-compose.yml file contains environment variables that configure:
- Database connection strings
- Logging configuration
- JWT authentication settings

## Data Persistence

Data is persisted using Docker volumes:
- SQL Server data: sqlserver-data volume
- MongoDB data: mongodb-data volume

## Stopping the Application

To stop all containers:

```bash
docker-compose down
```

To stop and remove volumes (this will delete all data):

```bash
docker-compose down -v
```

## Logs

API logs are stored in the ./logs directory, which is mounted to the API container. 