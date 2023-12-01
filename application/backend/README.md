# Backend

## Folders

```sh
.
├── docs                # documentation
│   └── diagrams        # some UML diagrams
├── manifests           # manifests for k8s deployment
├── src                 # main codebase
│   ├── Controllers     # controller classes
│   ├── Extensions      # entension methods
│   ├── HealthChecks    # health check classes
│   ├── Middleware      # middleware services
│   ├── Migrations      # database migrations
│   ├── Model           # data model
│   │   ├── Analysis
│   │   ├── Config
│   │   ├── Custom
│   │   ├── Enum
│   │   ├── Exceptions
│   │   ├── Rest
│   │   ├── Security
│   │   └── Users
│   └── Services        # collection of service classes and interfaces
│       ├── Database
│       ├── DevOps
│       ├── Expressions
│       ├── OAuth
│       ├── Security
│       └── Users
└── test                # tests
    └── backend.Test
        ├── Helpers
        ├── Model
        ├── Services
        └── TestResults
```

## Configuration

The file [appsettings.json](./src/appsettings.json) contains an example configuration for this application. OAUTH and service configurations should be made before deployment or active development.