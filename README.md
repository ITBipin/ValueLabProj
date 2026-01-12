# Clean Architecture Code Challenge

## Overview

This solution follows **Clean Architecture principles**, ensuring a clear separation of concerns across **Domain, Application, Infrastructure, and API layers**.
It demonstrates maintainable design, testable business logic, and proper layering.
Unit tests are included to validate core business rules.

---

## Solution Structure

### CleanArchitecture.Domain

* Contains core **domain entities** and **interfaces**
* Holds business rules and domain models
* Has **no dependency** on other layers

---

### CleanArchitecture.Application

* Contains **application services** and **business logic**
* Depends only on **domain interfaces**
* Independent of infrastructure and framework concerns

---

### CleanArchitecture.Infrastructure

* Contains **data access implementations** and external dependencies
* Includes an **in-memory repository** for persistence:

  * `InMemoryMessageRepository.cs`
* Can be replaced with a database implementation without impacting business logic

---

### ValueLabProj.Api

* ASP.NET Core Web API project
* Acts as the **application entry point**
* Exposes REST endpoints via controllers
* Communicates with the Application layer only

---

### CodeChallenge.Tests

* Unit test project
* Validates **service-level business logic**
* Uses **xUnit**, **Moq**, and **FluentAssertions**

---

## How to Run the Application

1. Open the solution in **Visual Studio**
2. Restore NuGet packages
3. Set **ValueLabProj.Api** as the startup project
4. Run the application using:

   * `Ctrl + F5` (without debugging)
   * or `F5` (with debugging)

The API will start and expose endpoints through the configured controllers.

---

## Unit Testing

* Unit tests are located in the **CodeChallenge.Tests** project
* All required test cases are implemented
* Tests validate:

  * Business rules
  * Service behavior
  * Success and failure scenarios

### Run Tests

#### Using Visual Studio

* Open **Test Explorer**
* Click **Run All Tests**

#### Using Command Line

```bash
dotnet test
```

---

## Key Architectural Highlights

* Strict separation of concerns
* Dependency inversion using interfaces
* Testable application services
* Infrastructure isolated from business logic
* Easy replacement of in-memory storage with a real database

---

