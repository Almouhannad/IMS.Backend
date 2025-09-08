# IMS.SharedKernel

This library contains cross-cutting building blocks shared across the application. It is organized into the following modules:

- **ResultPattern** – Implements the `Result` and `Error` types plus related helpers for representing operation outcomes.
- **CQRS** – Defines command and query interfaces along with their respective handlers to support the Command-Query Responsibility Segregation pattern.
- **Decorators** – Contains logging and validation decorators that wrap handlers with cross-cutting behaviors.
- **API** – Provides primitives for API layers, including a global exception handler, a base endpoint contract, and utilities for mapping results to HTTP responses.
- **Config** – Supplies helpers for working with environment variables and application configuration.

