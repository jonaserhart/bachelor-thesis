# SCRUM Analysis Application

This application helps analyze SCRUM processes logged digitally for example by Azure DevOps or Jira Boards.

## Outline

[front-end](./frontend/README.md): a React application that defines the front-end of the application.

[back-end](./backend/README.md): an ASP.NET web API to model the back-end of the application.

## Develop and Contribute

### Prerequisites

- [Docker](https://www.docker.com)
- [ASP.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [NodeJS 19+](https://nodejs.org/en)

### Run it locally

`docker compose --file compose.dev.yml up` will run three containers: front-end, back-end and nginx for routing.

The application should be reachable at `localhost:3050`.

Make sure you set all required env variables and configurations in [appsettings.Development.json](./backend/src/appsettings.Development.json) before you start.