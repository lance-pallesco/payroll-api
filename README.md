# Payroll System Backend

This is the **.NET 8 Web API backend** for the Payroll System project.

## Features
- RESTful API to manage employees:
  - `GET /api/employees`
  - `POST /api/employees`
  - `PUT /api/employees/{id}`
  - `DELETE /api/employees/{id}`
  - `POST /api/employees/{id}/compute-pay`

## Payroll computation rules:
  - Employees work only on assigned days
  - Pay 2x daily rate if worked
  - Pay 100% daily rate on birthdays

  ## Setup

  1. Clone the repository:
  ```bash
  git clone https://github.com/lance-pallesco/payroll-backend.git
  cd payroll-backend
  ```

  2. Restore dependencies:
  ```bash
  dotnet restore
  ```

  3. Configure Database Connection
  Open appsettings.json and update:
  ```bash
  "ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=PayrollDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
  ```
  Replace Server=. if needed (e.g. localhost or .\SQLEXPRESS)
  
  4. Apply Database Migrations
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```

  5. Run the Backend API
  ```bash
   dotnet run
  ```
  API runs at http://localhost:5285.

  6. Access Swagger (API Documentation)
  Open your browser:
  ```bash
  https://localhost:5001/swagger
  ```
  
  You can:
 - Add employees
 - Update employees
 - Delete employees
 - Compute payroll

The frontend to interact with this API is here:
[Payroll System Frotne](https://github.com/lance-pallesco/payroll-ui)
