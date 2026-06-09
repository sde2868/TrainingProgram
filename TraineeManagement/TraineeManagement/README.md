# TraineeManagement
A simple ASP.NET Core Web API project for managing trainees using CRUD operations with Entity Framework Core InMemory Database.

## Technology Used
- ASP.NET Core Web API
- Entity Framework Core
- EF Core InMemory Database
- Swagger UI

## How To Run
1. Extract the ZIP and open in a code editor.
2. Open the main folder in cli and run install the required packages by executing "dotnet restore" in the cli.
3. After all the required packages are installed, execute "dotnet run" in the cli.

## API List
### 1. Get All Trainees by Search (GET /api/trainee?search=\<string\>)
- Sample Request JSON:
```http
GET /api/trainee
```
- Sample Response JSON:

```json
[
  {
    "id": 1,
    "firstName": "Zeus",
    "lastName": "Learning",
    "email": "zeuslearning@email.com",
    "techStack": ["C#", "Dotnet"],
    "status": "Active",
    "createdAt": "2026-06-08T10:30:00Z",
    "updatedAt": "2026-06-08T10:30:00Z"
  }
]
```

### 2. Get Trainee By Id (GET /api/trainee/\<id\>)
- Sample Request JSON: 
```http
GET /api/trainee/1
```
- Sample Reponse JSON:
```json
[
  {
    "id": 1,
    "firstName": "Zeus",
    "lastName": "Learning",
    "email": "zeuslearning@email.com",
    "techStack": ["C#", "Dotnet"],
    "status": "Active",
    "createdAt": "2026-06-08T10:30:00Z",
    "updatedAt": "2026-06-08T10:30:00Z"
  }
]
```

### 3. Create Trainee (POST /api/trainee)
- Sample Request JSON:
```http
POST /api/trainee
Content-Type: application/json
```
```json
{
  "firstName": "Amit",
  "lastName": "Sharma",
  "email": "amit@email.com",
  "techStack": ["React", "NodeJS"],
  "status": "Busy"
}
```
- Sample Response JSON:
```json
{
  "id": 2,
  "firstName": "Amit",
  "lastName": "Sharma",
  "email": "amit@email.com",
  "techStack": ["React", "NodeJS"],
  "status": "Busy",
  "createdAt": "2026-06-08T11:00:00Z",
  "updatedAt": "2026-06-08T11:00:00Z"
}
```

### 4. Update Trainee (PUT /api/trainee/\<id\>)
- Sample Request JSON:
```http
PUT /api/trainee/2
Content-Type: application/json
```

```json
{
  "firstName": "Amit",
  "lastName": "Patel",
  "email": "amitpatel@email.com",
  "techStack": ["Angular", ".NET"],
  "status": "Active"
}
```
- Sample Response JSON:
```json
{
  "firstName": "Amit",
  "lastName": "Patel",
  "email": "amitpatel@email.com",
  "techStack": ["Angular", ".NET"],
  "status": "Active"
}
```

### 5. Delete Trainee (DELETE /api/trainee/\<id\>)
- Sample Request JSON:
```http
DELETE /api/trainee/2
```
- Sample Response JSON:
```http
204 No Content
```

# Known Limitations
- No Auth (Both Authentication and Authorization)
- Temporary (In-memory) Storage, data reset on refresh / reset

---
### Read (from review)

2. HealthCheck controller: Checks health of service + health of dependent service
In our case we are using in-memory database but we generally check health of SQL, Redis and any other dependent items too as apis are dependent on SQL too.

8. Ideally we have authorization and authentication whenever user hits api : Read about it that how it happens in dotnet via middlewares.
Authentication (who you are)
Authorization (what you can access)

9.Read about Dependency Injection & Lifetimes
Understand basic lifetimes:
Scoped → per request (most used)
Singleton → one instance always
Used for services, DbContext

11. HTTP Status Codes
Use correct responses:
200 → success
201 → created
204 → no content
400 → bad request
404 → not found

12. Read about how Logging should be done.
Use built-in logging:
ILogger<T>
Helps debug issues in production
---

## MySql Migration
1. Install mysql-server on WSL-cli
```bash
sudo apt update && sudo apt install mysql-server -y
sudo service mysql start
sudo service mysql status
sudo mysql
mysql -u root -p
```

2. Update Connection String in appsettings.json
```json
"ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=trainee_management_db;user=root;password=your_password;"
},
```

3. Remove unimportant packages and Install required packages
```bash
dotnet clean
dotnet remove package Microsoft.EntityFrameworkCore
dotnet remove package Microsoft.EntityFrameworkCore.InMemory
dotnet remove package Microsoft.EntityFrameworkCore.Tools
dotnet remove package Microsoft.EntityFrameworkCore.Design
dotnet remove package Microsoft.EntityFrameworkCore.Relational

dotnet clean
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 9.0.0
dotnet add package Microsoft.EntityFrameworkCore.Design --version 9.0.0
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 9.0.0
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
```