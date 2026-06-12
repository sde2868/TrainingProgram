# TraineeManagement
A simple ASP.NET Core Web API project for managing trainees using CRUD operations with Entity Framework Core InMemory Database.

## Technology Used
- ASP.NET Core Web API
- Entity Framework Core
- EF Core InMemory Database
- Swagger UI

## MySql Setup
1. Update
```bash
sudo apt update -y
```
2. Upgrade
```bash
sudo apt upgrade -y
```
3. Install MySql Server in the cli
```bash
sudo apt install mysql-server
```
4. Start the server
```bash
sudo service mysql start
```
5. Verify the running status
```bash
sudo service mysql status
# expected: active (running)
```
6. Open MySQL as sudo User
```bash
sudo mysql
```
7. Change Root Authentication to Password-Based Login
```sql
ALTER USER 'root'@'localhost'
IDENTIFIED WITH mysql_native_password
BY 'password_to_be_used_everytime_for_using_mysql_server_cli';
```
8. Apply Changes
```sql
FLUSH PRIVILEGES;
```
9. Verify Authentication Plugin
```sql
SELECT user, host, plugin FROM mysql.user;
-- expected: root | localhost | mysql_native_password
```
10. Exit MySQL
```sql
exit;
```
11. Restart MySQL
```bash
sudo service mysql restart
```
12. Login Using Root Password
```bash
mysql -u root -p
```
13. Create Database
```sql
CREATE DATABASE trainee_management_db;
```
14. Verify
```sql
SHOW DATABASES;
-- expected: trainee_management_db
```

---

## Backend Setup
1. Extract the ZIP from github repo (https://github.com/sde2868/TrainingProgram/tree/main/TraineeManagement) and open in a code editor.
2. Install dotnet sdk and aspnet runtime (version 10.0) in the cli using the following commands:
```bash
sudo apt update
sudo apt upgrade
sudo systemctl enable --now snapd
sudo ln -s /var/lib/snapd/snap /snap
sudo apt-get install -y dotnet-sdk-10.0
sudo apt-get install -y aspnetcore-runtime-10.0
```
3. Open the project folder in cli and install the required packages by executing "dotnet restore" command in the cli.
4. After all the required packages are installed, execute "dotnet run" command in the cli and click "Open in Browser" when prompted.

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
4. Migration
```bash
dotnet ef migrations add InitialCreate
dotnet ef update database
```

5. Verify
```bash
mysql -u root -p
# Add the password that you set
```
```mysql
<!-- Inside mysql server cli -->
show databases;
use trainee_management_db;
show tables;
select * from Trainees;
```

---

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

---

## User Model Creation and Login API using BCrypt password hashing & JWT bearer header
1. Add required packages
```bash
dotnet add package BCrypt.Net-Next
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

2. Create User Model, and Login API in User Controller (POST /api/users/login)
- Sample request json
```json
// UserDTO
{
  "UserName": "Admin_Zeus_Learning",
  "Password": "Admin@123"
}
// Use the same credentials for testing purposes
```
- Sample Response json
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW5fWmV1c19MZWFybmluZyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvbmFtZWlkZW50aWZpZXIiOiIxIiwiZXhwIjoxNzgxMjUyOTMyLCJpc3MiOiJUcmFpbmVlTWFuYWdlbWVudEFQSSIsImF1ZCI6IlRyYWluZWVNYW5hZ2VtZW50QVBJIn0.8aMGKgYLTCY2t_hH4cSSuWDBQpttYwCF4Y-ijeMRoa8",
  "expiresIn": 3600,
  "user": {
    "id": 1,
    "username": "Admin_Zeus_Learning",
    "role": "Admin"
  }
}
```

3. Migrate
```bash
dotnet ef migrations add UserMigration
dotnet ef update database
```

4. Use Swagger UI to Authorize and use Protected APIs
- Copy the content of the "token" field received in response body
- Scroll to top and click "Authorize" on the right.
- Under the "Available Authorizations", paste your token into the text field after "Bearer  (http, Bearer) | Enter JWT | Token Value:".
- Click Authorize.

---

## Known Limitations
1. Role-based authorization yet to be implemented.
2. 

## Security Checklist
| Security Area | Expected Practice | Applied |
|---------------|-------------------|---------|
| Authentication | JWT validation enabled | ✅ |
| Authorization | Protected APIs require token | ✅ |
| Password storage | Passwords stored as hash only | ✅ |
| Excessive data exposure | DTOs used; password hash not returned | ✅ |
| Injection | EF Core used; no unsafe raw SQL | ✅ |
| Security misconfiguration | CORS restricted to expected origin | ✅ |
| Sensitive data exposure | Secrets not hardcoded in controllers | ✅ |
| Error handling | Stack traces not returned | ✅ |
| Logging | Passwords and tokens not logged | ✅ |

## Next Improvement Areas
1. 