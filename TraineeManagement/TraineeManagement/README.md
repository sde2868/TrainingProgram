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
- No exception handling