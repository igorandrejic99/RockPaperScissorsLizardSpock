
# RockPaperScissorsLizardSpock API

This project is a RESTful API that simulates the game **Rock-Paper-Scissors-Lizard-Spock**. The API provides endpoints for retrieving available choices, generating a random choice, and playing a game round against the computer.

This project was designed as a monolithic application rather than a microservice architecture. Given its focus on simulating the Rock-Paper-Scissors-Lizard-Spock game, all functionality—such as retrieving choices, playing game rounds, and generating computer choices, is tightly interdependent and centered on the core game logic. A monolithic structure provides clear organization and straightforward maintainability for these features without the added complexity that a microservices approach would introduce. Since there are no distinct subdomains requiring separate scalability or independent deployment, the monolithic architecture is seen as an optimal and efficient choice for this application.

## Table of Contents
- [Project Structure](#project-structure)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Setup and Installation](#setup-and-installation)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Dockerization](#dockerization)

## Project Structure

The primary components of this project include:
- **Controllers**: Handles incoming HTTP requests and manages API routes.
- **Services**: Implements the game logic and random number retrieval.
- **Validators**: Validates incoming commands and queries.
- **Configuration**: Defines configuration options, Swagger options, and application settings.
- **Infrastructure**: Contains classes interacting with external services.
- **Models**: Defines data models used throughout the API.
- **Docker files**: Configures Docker for containerization.

## Features

- **API Versioning**: Supports versioning using URL segments.
- **Swagger Documentation**: Comprehensive Swagger UI available for testing API endpoints.
- **Polly Integration**: Adds resilience to HTTP requests using retry policies.
- **Integration and Unit Tests**: Complete suite of tests covering controllers, services, and infrastructure.

## Technology Stack

- **.NET 8.0** - The framework for the application.
- **Docker** - Containerizes the application.
- **Swagger** - Documents API endpoints.
- **xUnit, Moq, and Polly** - Used for testing and retry logic.
- **CI Pipeline** - Automates build and test when doing push on main branch.

## Setup and Installation

### Prerequisites
- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://docs.docker.com/get-docker/)
- Recommended: [Visual Studio Code](https://code.visualstudio.com/) or any preferred IDE.

### Installation
1. **Clone the repository**:
    ```bash
    git clone https://github.com/igorandrejic99/RockPaperScissorsLizardSpock.git
    cd RockPaperScissors
    ```

2. **Set up environment variables**:
    Ensure environment-specific configurations are in `appsettings.json` or `appsettings.Development.json` for local development.

3. **Restore dependencies**:
    ```bash
    dotnet restore
    ```

## Running the Application

### Running with Docker Compose

The application is configured to run using Docker Compose, which simplifies multi-container setups that could be needed in future. 

1. **Start the application**:
    ```bash
    docker-compose up -d
    ```

2. **Access the API**:
   - Swagger UI: [http://localhost:5081/swagger/index.html](http://localhost:5081/swagger/index.html)

3. **Stop the application**:
   ```bash
   docker-compose down
   ```

## API Endpoints

### Base URL
- `http://localhost:5081/game` or `http://localhost:5081/api/v1/game`

### Available Endpoints

1. **Retrieve Choices**
   - **Endpoint**: `/choices`
   - **Method**: `GET`
   - **Description**: Returns a list of game choices.
   
2. **Retrieve Random Choice**
   - **Endpoint**: `/choice`
   - **Method**: `GET`
   - **Description**: Returns a randomly generated choice.

3. **Play Game**
   - **Endpoint**: `/play`
   - **Method**: `POST`
   - **Body**: `{ "Player": <int> }` (where `<int>` is a number between 1 and 5)
   - **Description**: Plays a game round with the player’s choice and returns the result.

### Response Example

**Play Game** (POST `/play`):
```json
{
  "player": 1,
  "computer": 3,
  "results": "win"
}
```

## Testing

### Running Unit Tests
1. **Navigate to the unit test directory**:
    ```bash
    cd test/RockPaperScissors.UnitTests
    ```

2. **Run unit tests**:
    ```bash
    dotnet test
    ```

### Running Integration Tests
1. **Navigate to the integration test directory**:
    ```bash
    cd test/RockPaperScissors.IntegrationTests
    ```

2. **Run integration tests**:
    ```bash
    dotnet test
    ```
    
## Dockerization

This project includes both a `Dockerfile` and `docker-compose.yml` file for containerization:

### Dockerfile
Defines the setup for the application, splitting into build and runtime stages.

### Docker Compose
The `docker-compose.yml` simplifies running multi-container setups if needed in the future, such as adding a database container.

### CI Pipeline (GitHub Actions)
1. **Build** - Builds the application.
2. **Test** - Executes unit and integration tests within the pipeline.
The CI pipeline in GitHub Actions is an automated workflow that ensures code quality by building the application and running all unit and integration tests each time code is pushed on main branch, helping catch errors early and maintain a stable codebase.
