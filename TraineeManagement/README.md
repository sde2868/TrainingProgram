# TraineeManagement System

A comprehensive ASP.NET Core-based training program management platform designed for managing trainees, learning tasks, task submissions, and mentorship reviews with asynchronous submission processing capabilities.

## Table of Contents

- [Project Overview](#project-overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Core Components](#core-components)
- [Data Models](#data-models)
- [Services Layer](#services-layer)
- [API Endpoints](#api-endpoints)
- [Infrastructure & Configuration](#infrastructure--configuration)
- [Message Queue System](#message-queue-system)
- [Database Setup](#database-setup)
- [Development Setup](#development-setup)
- [Running the Application](#running-the-application)

## Project Overview

The TraineeManagement System is an enterprise-grade training platform that manages:
- **Trainees**: Individual participants in the training program
- **Users**: System users with authentication and authorization
- **Mentors**: Instructors who guide and review trainee work
- **Learning Tasks**: Training assignments with specific tech stack requirements
- **Task Assignments**: Assignment of learning tasks to trainees through mentors
- **Task Submissions**: Trainee submissions of completed tasks with file uploads
- **Reviews**: Mentor feedback and reviews on trainee submissions
- **Submission Files**: Files uploaded as part of task submissions

The system follows a **microservices-inspired architecture** with:
- **Main API** (`TraineeManagement`): RESTful API handling business logic and CRUD operations
- **Background Worker** (`SubmissionProcessor`): Asynchronous message consumer for processing submissions
- **Internal profile API** (`TrainingDirectory.Api`): Provides trainee profile data used during submission processing
- **Shared library** (`Shared`): Common models, DTOs, correlation middleware, and RabbitMQ topology
- **Internal profile API** (`TrainingDirectory.Api`): Provides trainee profile data used during submission processing
- **Shared library** (`Shared`): Common models, DTOs, message contracts, correlation middleware, and RabbitMQ topology

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Client Applications                      │
└──────────────────────┬──────────────────────────────────────┘
                       │ HTTP/REST
┌──────────────────────▼──────────────────────────────────────┐
│           TraineeManagement API (ASP.NET Core)              │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Controllers                                         │   │
│  │  - UserController, TraineeController                 │   │
│  │  - MentorController, LearningTaskController          │   │
│  │  - TaskAssignmentController, ReviewController        │   │
│  │  - TaskSubmissionController, SubmissionFileController|   |
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Services Layer (Business Logic)                     │   │
│  │  - TraineeServices, UserServices, MentorServices     │   │
│  │  - LearningTaskServices, TaskAssignmentServices      │   │
│  │  - TaskSubmissionServices, ReviewServices            │   │
│  │  - SubmissionFileServices, RabbitMqPublisher         │   │
│  │  - RedisCacheServices, LocalFileStorageServices      │   │
│  │  - Health Check Services                             │   │
│  └──────────────────────────────────────────────────────┘   │
│  ┌──────────────────────────────────────────────────────┐   │
│  │  Middleware & Exception Handling                     │   │
│  │  - ExceptionMiddleware (centralized error handling)  │   │
│  └──────────────────────────────────────────────────────┘   │
└──────────┬──────────────────┬────────────────┬──────────────┘
           │                  │                │
           ▼                  ▼                ▼
      ┌────────────┐   ┌──────────────┐  ┌─────────────┐
      │   MySQL    │   │    Redis     │  │   RabbitMQ  │
      │  Database  │   │    Cache     │  │   Message   │
      │            │   │              │  │    Queue    │
      └────────────┘   └──────────────┘  └──────┬──────┘
                                                  │
                                    ┌─────────────▼─────────┐
                                    │ SubmissionProcessor   │
                                    │  (Background Worker)  │
                                    │                       │
                                    │ - Consumes messages   │
                                    │ - Computes checksums  │
                                    │ - Calls TrainingDir.  │
                                    └───────────────────────┘
                                                  │
                                                  ▼
                                    ┌───────────────────────┐
                                    │ TrainingDirectory.Api  │
                                    │   (Internal Profile)   │
                                    │ - Trainee processing   │
                                    │   profile endpoint     │
                                    └───────────────────────┘
```

### Data Flow: Task Submission & Processing

1. **Submission Creation**: Trainee submits task files via TaskSubmissionController
2. **File Storage**: Files are stored in local file system (Uploads folder)
3. **Message Publishing**: RabbitMqPublisher publishes `SubmissionProcessingRequested` message
4. **Queue Persistence**: Message stored in RabbitMQ queue (submission-processing)
5. **Async Processing**: SubmissionProcessor Worker consumes and processes message
6. **Status Update**: Processing status updated in database

## Technology Stack

### Backend Framework
- **ASP.NET Core 10.0**: Web API framework
- **.NET 10.0**: Runtime environment

### Database & Persistence
- **MySQL 8.0.15**: Relational database
- **Entity Framework Core 9.0**: ORM for data access
- **EF Core Migrations**: Database schema versioning

### Caching & Session Management
- **Redis 7.4.5**: Distributed cache
- **StackExchange.Redis 3.0**: Redis client library

### Message Queue
- **RabbitMQ 3.0 (with Management)**: Asynchronous message broker
- **RabbitMQ.Client 7.2.1**: .NET client for RabbitMQ

### Internal Service Integration
- **IHttpClientFactory**: Typed HTTP client for internal API calls
- **Resilience policies**: Fallback and timeout behavior for TrainingDirectory API

### Security
- **JWT Bearer Authentication**: Token-based authentication
- **BCrypt.Net-Next 4.2**: Password hashing
- **Microsoft.AspNetCore.Authentication.JwtBearer 9.0**: JWT middleware

### API Documentation
- **Swagger/Swashbuckle.AspNetCore 6.6.2**: API documentation and testing UI

### File Storage
- **Local File System**: Files stored in `Storage/` and `Uploads/` directories
- **Maximum File Size**: 10MB (10,485,760 bytes)
- **Supported Extensions**: .pdf, .doc, .docx, .zip, .png

### Health Checks
- **Microsoft.AspNetCore.Diagnostics.HealthChecks**: Application health monitoring
- Checks: Database, Redis, External API

## Project Structure

```
TraineeManagement/
├── Controllers/                 # API endpoints
│   ├── UserController.cs
│   ├── TraineeController.cs
│   ├── MentorController.cs
│   ├── LearningTaskController.cs
│   ├── TaskAssignmentController.cs
│   ├── TaskSubmissionController.cs
│   ├── ReviewController.cs
│   ├── SubmissionFileController.cs
│   └── ProcessingJobController.cs
├── Models/                      # Data models and DTOs
│   ├── User.cs                 # User with role-based access
│   ├── Trainee.cs              # Training program participant
│   ├── Mentor.cs               # Training instructor
│   ├── LearningTask.cs         # Training assignment template
│   ├── TaskAssignment.cs       # Task assigned to trainee
│   ├── TaskSubmission.cs       # Trainee's task submission
│   ├── Review.cs               # Mentor's review on submission
│   ├── SubmissionFile.cs       # Uploaded files
│   ├── SubmissionProcessingRequested.cs  # Message contract
│   ├── FileUploadSettings.cs   # File upload configuration
│   ├── StorageSettings.cs      # Storage path configuration
│   ├── RabbitMqSettings.cs     # Message broker configuration
│   ├── RedisSettings.cs        # Cache configuration
│   └── DTOs/                   # Data Transfer Objects
├── Services/                    # Business logic layer
│   ├── UserServices.cs         # User CRUD and authentication
│   ├── TraineeServices.cs      # Trainee management
│   ├── MentorServices.cs       # Mentor management
│   ├── LearningTaskServices.cs # Task template management
│   ├── TaskAssignmentServices.cs # Task assignment logic
│   ├── TaskSubmissionServices.cs # Submission handling
│   ├── ReviewServices.cs       # Review management
│   ├── SubmissionFileServices.cs # File management
│   ├── RabbitMqPublisher.cs    # Message publishing
│   ├── RedisCacheServices.cs   # Cache operations
│   ├── LocalFileStorageServices.cs # File I/O operations
│   ├── Interfaces/             # Service contracts
│   └── HealthChecks/           # Health check implementations
├── Data/
│   └── AppDbContext.cs         # EF Core DbContext
├── Middlewares/
│   └── ExceptionMiddleware.cs  # Centralized exception handling
├── Migrations/                 # Database schema migrations
│   ├── 20260612120627_InitialCreate
│   ├── 20260619071025_AddSubmissionFiles
│   └── 20260619071646_SubmissionFiles
├── Constants/
│   ├── CacheKeys.cs            # Redis cache key constants
│   └── QueueNames.cs           # Message queue name constants
├── Properties/
│   └── launchSettings.json     # Development run configuration
├── Storage/                    # File storage root directory
├── Uploads/                    # File upload directory
├── docker-compose.yml          # Infrastructure orchestration
├── appsettings.json            # Configuration (connection strings, keys)
├── appsettings.Development.json# Development-specific settings
├── appsettings.example.json    # Configuration template
├── Program.cs                  # Application startup configuration
└── TraineeManagement.csproj    # Project file with dependencies

SubmissionProcessor/
├── Worker.cs                   # Background service for processing
├── Models/
│   └── SubmissionProcessingRequested.cs  # Message model
├── Program.cs                  # Worker host configuration
├── Properties/
│   └── launchSettings.json
├── appsettings.json            # RabbitMQ + processing configuration
├── appsettings.Development.json
├── appsettings.example.json
└── SubmissionProcessor.csproj  # Worker project dependencies

TrainingDirectory.Api/
├── Controllers/                # Internal profile API endpoints
│   └── TraineesController.cs
├── DTOs/                       # Response DTOs
│   └── TraineeProcessingProfileResponse.cs
├── appsettings.json            # Service configuration
├── appsettings.Development.json
├── appsettings.example.json
├── Dockerfile                  # Service container build definition
└── TrainingDirectory.Api.csproj # Project file

Shared/
├── Correlation/                 # Correlation middleware and accessors
├── Messaging/                   # RabbitMQ topology and shared contracts
├── Models/                      # Shared models and DTOs
└── Shared.csproj               # Shared library project
```

## Core Components

### 1. Controllers (API Layer)

Each controller exposes REST endpoints for CRUD operations:

| Controller | Endpoints | Purpose |
|-----------|-----------|---------|
| **UserController** | `/api/user/*` | User authentication, CRUD, role management |
| **TraineeController** | `/api/trainee/*` | Trainee registration and management |
| **MentorController** | `/api/mentor/*` | Mentor creation and management |
| **LearningTaskController** | `/api/learning-task/*` | Learning task CRUD and publishing |
| **TaskAssignmentController** | `/api/task-assignment/*` | Assign tasks to trainees |
| **TaskSubmissionController** | `/api/task-submission/*` | Trainee submissions and retrieval |
| **ProcessingJobsController** | `/api/processing-jobs/*` | Processing job status and details |
| **ReviewController** | `/api/review/*` | Mentor reviews on submissions |
| **SubmissionFileController** | `/api/submission-file/*` | File upload and download |

### 2. Models (Data Layer)

#### User
```csharp
- Id: int (Primary Key)
- UserName: string (Unique)
- Email: string (Unique)
- Password: string (BCrypt hashed)
- Role: UserRole (Admin, Mentor, Trainee)
- CreatedAt: DateTime
```

#### Trainee
```csharp
- Id: int (Primary Key, 1-1000)
- FirstName: string (3-50 chars, letters only)
- LastName: string (3-50 chars, letters only)
- Email: string (Unique)
- TechStack: string[] (Technical skills)
- Status: TraineeStatus (Active, Busy, Offline)
- CreatedAt, UpdatedAt: DateTime
```

#### Learning Task
```csharp
- Id: int (Primary Key)
- Title: string (3-50 chars)
- Description: string (3-5000 chars)
- ExpectedTechStack: string[]
- Status: LearningTaskStatus (Draft, Published, Closed)
- DueDate: DateTime
- CreatedAt, UpdatedAt: DateTime
```

#### Task Assignment
```csharp
- Id: int (Primary Key)
- TraineeId: int (Foreign Key → Trainee)
- MentorId: int (Foreign Key → Mentor)
- LearningTaskId: int (Foreign Key → LearningTask)
- AssignedDate: DateTime
- Deadline: DateTime
- Submissions: List<TaskSubmission> (Navigation)
```

#### Task Submission
```csharp
- Id: int (Primary Key)
- TaskAssignmentId: int (Foreign Key → TaskAssignment)
- SubmissionUrl: string
- Notes: string
- Status: TaskSubmissionStatus (Submitted, Resubmitted)
- SubmittedDate: DateTime
- TaskAssignment: TaskAssignment (Navigation)
- Files: List<SubmissionFile> (Navigation, Cascade Delete)
- Reviews: List<Review> (Navigation)
```

#### Submission File
```csharp
- Id: int (Primary Key)
- SubmissionId: int (Foreign Key → TaskSubmission, Cascade Delete)
- FileName: string
- FileSize: long
- FilePath: string
- UploadedDate: DateTime
- Submission: TaskSubmission (Navigation)
```

#### Review
```csharp
- Id: int (Primary Key)
- TaskSubmissionId: int (Foreign Key → TaskSubmission)
- MentorId: int (Foreign Key → Mentor)
- Rating: int (1-5 or similar)
- Comments: string
- ReviewedDate: DateTime
- Submission: TaskSubmission (Navigation)
- Mentor: Mentor (Navigation)
```

#### Processing Job
```csharp
- Id: int (Primary Key)
- MessageId: Guid (Unique message identifier)
- CorrelationId: string (Distributed tracing key)
- SubmissionId: int (Foreign Key → TaskSubmission)
- SubmissionFileId: int (Foreign Key → SubmissionFile)
- Status: ProcessingJobStatus (Queued, Processing, Completed, Failed)
- Attempts: int (Number of processing attempts)
- ErrorSummary: string? (Failure description)
- StartedAt: DateTime? (Processing start timestamp)
- CompletedAt: DateTime? (Processing completion timestamp)
- CreatedAt: DateTime
- Submission: TaskSubmission (Navigation)
- SubmissionFile: SubmissionFile (Navigation)
```

#### Message Contract
```csharp
SubmissionProcessingRequested
- MessageId: Guid (Unique identifier)
- CorrelationId: string (Track across systems)
- TaskSubmissionId: int (Reference to submission)
- SubmissionFileId: int (Reference to file)
- RequestedAt: DateTime
- ContractVersion: string (Version tracking, default "1.0")
```

### 3. Services Layer

#### User Service (IUser)
- User authentication and registration
- Role-based access control
- Password hashing and validation
- User CRUD operations

#### Trainee Service (ITrainee)
- Trainee CRUD operations
- Status management (Active, Busy, Offline)
- Tech stack management

#### Task Submission Service (ITaskSubmission)
- Submission CRUD
- Status tracking (Submitted, Resubmitted)
- Integration with RabbitMQ publisher for async processing
- File association management

#### File Storage Service (IFileStorageService)
- Local file system operations
- File size validation (max 10MB)
- Extension whitelist validation (.pdf, .doc, .docx, .zip, .png)
- File upload and download

#### RabbitMQ Publisher (IMessagePublisher)
- Publishes `SubmissionProcessingRequested` messages
- Queue: `submission-processing`
- Durable queue with message persistence
- Correlation ID tracking for distributed tracing

#### Training Directory Client (ITrainingDirectoryClient)
- Typed HTTP client using `IHttpClientFactory`
- Calls internal `TrainingDirectory.Api` for trainee processing profiles
- Propagates `X-Correlation-Id` for distributed tracing
- Handles service unavailability gracefully with fallback behavior

#### Redis Cache Service (ICacheService)
- Distributed caching layer
- Key-value operations with TTL (default 60 minutes)
- Cache invalidation support

#### Health Check Services
- **DatabaseHealthCheck**: Verifies MySQL connectivity
- **RedisHealthCheck**: Verifies Redis connectivity
- **ExternalServiceHealthCheck**: Monitors external API availability

## API Endpoints

### Authentication & Users
```
POST   /api/user/login              Login with credentials, receive JWT token
POST   /api/user/register           Create new user account
GET    /api/user/{id}               Get user by ID (requires JWT)
PUT    /api/user/{id}               Update user details
DELETE /api/user/{id}               Delete user account
```

### Trainees
```
POST   /api/trainee                 Create new trainee
GET    /api/trainee                 List all trainees
GET    /api/trainee/{id}            Get trainee details
PUT    /api/trainee/{id}            Update trainee information
DELETE /api/trainee/{id}            Delete trainee
GET    /api/trainee/{id}/assignments Get trainee's task assignments
GET    /api/trainee/{id}/submissions Get trainee's submissions
```

### Learning Tasks
```
POST   /api/learning-task           Create new learning task (Draft)
GET    /api/learning-task           List all tasks
GET    /api/learning-task/{id}      Get task details
PUT    /api/learning-task/{id}      Update task
DELETE /api/learning-task/{id}      Delete task
PATCH  /api/learning-task/{id}/publish Publish task for assignments
```

### Task Assignments
```
POST   /api/task-assignment         Assign task to trainee
GET    /api/task-assignment         List all assignments
GET    /api/task-assignment/{id}    Get assignment details
PUT    /api/task-assignment/{id}    Update assignment (deadline, etc.)
DELETE /api/task-assignment/{id}    Remove assignment
```

### Task Submissions
```
POST   /api/task-submission         Create/submit task with files
GET    /api/task-submission         List submissions (with filtering)
GET    /api/task-submission/{id}    Get submission details
PUT    /api/task-submission/{id}    Resubmit task with new files
GET    /api/task-submission/{id}/files Get submitted files
DELETE /api/task-submission/{id}    Delete submission
```

### Processing Jobs
```
GET    /api/processing-jobs/{id}    Get processing job details and status
```

### Reviews
```
POST   /api/review                  Create review on submission
GET    /api/review                  List all reviews
GET    /api/review/{id}             Get review details
PUT    /api/review/{id}             Update review
DELETE /api/review/{id}             Delete review
GET    /api/task-submission/{id}/reviews Get all reviews for submission
```

### File Management
```
GET    /api/submission-file/{id}    Download file
DELETE /api/submission-file/{id}    Delete file
GET    /api/submission-file/submission/{submissionId} List files in submission
```

### Health Checks
```
GET    /health                      General health status
GET    /health/ready                Readiness probe (dependencies)
GET    /health/live                 Liveness probe
```

## Infrastructure & Configuration

### Docker Compose Services

The `docker-compose.yml` orchestrates:

1. **Main API** (`trainee-management-api`)
   - Container: traineeManagement
   - Host port: 5237
   - Depends on MySQL, Redis, RabbitMQ
   - Uses environment variables for DB, Redis, RabbitMQ, storage, and TrainingDirectory URL

2. **Background Worker** (`submission-processor`)
   - Container: submissionprocessor
   - Depends on MySQL, Redis, RabbitMQ, and trainee-management-api
   - Uses the same shared RabbitMQ and storage configuration

3. **Internal profile API** (`trainingdirectory`)
   - Container: trainingdirectory
   - Host port: 5200
   - Serves trainee processing profiles to the worker

4. **MySQL 8.0.15** (Container: mysql)
   - Port: 3306
   - Database: `trainee_management_db`
   - Root Password: `Root@123`
   - Volume: `mysql_data:/var/lib/mysql`

5. **Redis 7.4.5** (Container: redis)
   - Port: 6379
   - No authentication by default
   - Health-checked with `redis-cli ping`

6. **RabbitMQ 3.0** (Container: rabbitmq)
   - Port: 5672 (AMQP)
   - Port: 15672 (Management UI)
   - Default User: `admin` / `admin123`
   - Volume: `rabbitmq_data:/var/lib/rabbitmq`

### Network
- **trainee_management_network**: All services connected on this custom bridge network

### Configuration Files

#### `appsettings.json` (Production/Default)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=trainee_management_db;user=root;password=Root@123;"
  },
  "Jwt": {
    "Key": "Jay_Hind_Jay_Hind_Jay_Hind_Jay_Hind_Jay_Hind",
    "Issuer": "TraineeManagementAPI",
    "Audience": "TraineeManagementAPI",
    "DurationInMinutes": 60
  },
  "Storage": {
    "RootPath": "Storage"
  },
  "FileStorage": {
    "StorageRoot": "Uploads",
    "MaxFileSizeBytes": 10485760,
    "AllowedExtensions": [".pdf", ".doc", ".docx", ".zip", ".png"]
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "DefaultTtlMinutes": 60
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "admin",
    "Password": "admin123",
    "VirtualHost": "/",
    "QueueName": "submission-processing",
    "DeadLetterExchange": "submission-processing-dlx",
    "DeadLetterQueue": "submission-processing-dlq"
  },
  "Processing": {
    "MaxAttempts": 3
  }
}
```
#### `appsettings.Development.json`
- Override with development-specific values
- Example: Different database connection string for development

#### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Set to "Development" or "Production"
- Connection strings and secrets can be overridden via environment

## Message Queue System

### RabbitMQ Architecture

**Queue Name**: `submission-processing`
**Dead Letter Exchange**: `submission-processing-dlx`
**Dead Letter Queue**: `submission-processing-dlq`
**Queue Type**: Durable, Persistent
**Consumer**: SubmissionProcessor Worker service

### Message Contract: SubmissionProcessingRequested

```csharp
{
  "MessageId": "12345678-1234-1234-1234-123456789012",
  "CorrelationId": "correlation-abc-123",
  "TaskSubmissionId": 42,
  "SubmissionFileId": 7,
  "RequestedAt": "2026-06-24T10:30:00Z",
  "ContractVersion": "1.0"
}
```

**Fields**:
- **MessageId**: Unique UUID per message for idempotency
- **CorrelationId**: String for tracing across services
- **TaskSubmissionId**: Primary key reference to submission
- **SubmissionFileId**: Primary key reference to uploaded file
- **RequestedAt**: ISO 8601 timestamp of request
- **ContractVersion**: Semantic versioning for message schema

### Publishing Flow

1. SubmissionFileServices receives a file upload and saves it to storage
2. A new `SubmissionFile` record is created in the database
3. A corresponding `ProcessingJob` is created with status `Queued`
4. A `SubmissionProcessingRequested` message is published to RabbitMQ
5. Message metadata includes:
   - Persistent delivery
   - MessageId and CorrelationId for tracing
   - Content-Type: application/json
6. Publisher logs message details and returns the uploaded file metadata

### Processing Flow (SubmissionProcessor)

1. Worker connects to RabbitMQ at startup
2. `RabbitMqTopologyConfigurator` declares the processing queue and DLQ topology
3. Sets QoS to 1 (process one message at a time)
4. `AsyncEventingBasicConsumer` receives messages
5. On receipt:
   - Deserialize JSON to `SubmissionProcessingRequested`
   - Log MessageId, SubmissionId, and FileId
   - Execute processing through `SubmissionProcessingService`
   - Retrieve trainee processing profile from `TrainingDirectory.Api` using `ITrainingDirectoryClient`
   - Propagate `X-Correlation-Id` for distributed tracing
   - Compute SHA-256 checksum for successful file processing
   - Update `ProcessingJob` status and timestamps
   - Acknowledge message (BasicAck) on success
   - On retryable failure, Nack with requeue=true
   - On permanent failure, Nack with requeue=false so message can be dead-lettered
6. Worker closes channels and connection gracefully on shutdown

### Error Handling

- **Duplicate deliveries**: ignored by idempotency checks on `MessageId` and already completed `SubmissionFileId`
- **Retry policy**: configured by `Processing.MaxAttempts` (default 3)
- **Retryable failures**: message is requeued for another attempt
- **Permanent failures**: message is Nack'ed without requeue and routed to DLQ
- **Processing job state**: status transitions through `Queued`, `Processing`, `Completed`, and `Failed`

- **Success**: BasicAck sent, message removed from queue
- **Failure**: BasicNack with requeue=true, message returned to queue for retry
- **Connection Loss**: Worker reconnects to RabbitMQ on restart
- **Logging**: All events logged to ILogger

## Database Setup

### Schema Overview

**Tables** (from migrations):
1. `Users` - User accounts with authentication
2. `Trainees` - Training program participants
3. `Mentors` - Instructors and reviewers
4. `LearningTasks` - Task templates and definitions
5. `TaskAssignments` - Trainee-Task mappings
6. `TaskSubmissions` - Submission records
7. `SubmissionFiles` - Uploaded files
8. `Reviews` - Mentor feedback

### Relationships

```
User (1) ──→ (N) Mentor
User (1) ──→ (N) Trainee

LearningTask (1) ──→ (N) TaskAssignment
Trainee (1) ──→ (N) TaskAssignment
Mentor (1) ──→ (N) TaskAssignment

TaskAssignment (1) ──→ (N) TaskSubmission
TaskSubmission (1) ──→ (N) SubmissionFile (Cascade Delete)
TaskSubmission (1) ──→ (N) Review

Mentor (1) ──→ (N) Review
```

### Key Constraints

- **Unique Indices**: UserName, User.Email, Mentor.Email
- **Cascade Delete**: SubmissionFile → TaskSubmission
- **Foreign Keys**: All navigation properties backed by FK constraints
- **Seed Data**: Admin user created during migration
  - Username: `Admin_Zeus_Learning`
  - Email: `admin@zeuslearning.com`
  - Password: `Admin@123` (BCrypt hashed)
  - Role: Admin

### Database Migrations

| Migration | Date | Purpose |
|-----------|------|---------|
| InitialCreate | 20260612120627 | Create core tables and schema |
| AddSubmissionFiles | 20260619071025 | Add SubmissionFile entity and relationships |
| SubmissionFiles | 20260619071646 | Final submission files configuration |

## Development Setup

### Prerequisites

- **.NET SDK 10.0** ([Install](https://dotnet.microsoft.com/download/dotnet/10.0))
- **MySQL 8.0+** ([Install](https://dev.mysql.com/downloads/mysql/))
- **Redis 7.0+** ([Install](https://redis.io/download))
- **RabbitMQ 3.0+** ([Install](https://www.rabbitmq.com/download.html))
- **Docker & Docker Compose** (Optional, for containerized setup)

### Local Setup (Without Docker)

#### 1. MySQL Database Setup

```bash
# Connect to MySQL
sudo mysql -u root -p

# Create database
CREATE DATABASE trainee_management_db;

# Verify
SHOW DATABASES;

# Exit
exit;
```

#### 2. Redis Setup

```bash
# Start Redis
redis-server

# Verify (in another terminal)
redis-cli ping
# Output: PONG
```

#### 3. RabbitMQ Setup

```bash
# RabbitMQ usually starts automatically after installation
# Management UI: http://localhost:15672
# Default credentials: admin / admin123
```

#### 4. Project Setup

```bash
# Clone/Navigate to project
cd /path/to/TraineeManagement

# Restore NuGet packages
dotnet restore

# Apply database migrations
cd TraineeManagement
dotnet ef database update

# Or create fresh database
dotnet ef database drop -f
dotnet ef database update
```

### Docker Setup

```bash
# Start all services (full stack)
docker compose up -d

# Verify services
docker compose ps

# Logs
docker compose logs -f

# Stop all services
docker compose down

# Clean up (remove volumes)
docker compose down -v
```

### Verify Setup

```bash
# MySQL
mysql -u root -p trainee_management_db
SELECT 1;
exit;

# Redis
redis-cli
ping
exit

# RabbitMQ Management UI
# Visit: http://localhost:15672

# TrainingDirectory API
curl http://localhost:5200/api/trainees/1/processing-profile
```
## Running the Application

### Main API (TraineeManagement)

```bash
cd TraineeManagement

# Debug/Development
dotnet run

# Production
dotnet publish -c Release
dotnet TraineeManagement.dll

# Alternatively with dotnet run
dotnet run --configuration Release
```

**Output**:
```
info: Microsoft.AspNetCore.Hosting.Diagnostics
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

**Access Points**:
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger/index.html
- **Health Check**: http://localhost:5000/health

### Background Worker (SubmissionProcessor)

```bash
cd SubmissionProcessor

# Run worker
dotnet run

# Output when connected
info: SubmissionProcessor.Worker
      ✅ Worker started listening to queue: submission-processing
```

**Behavior**:
- Automatically connects to RabbitMQ on startup
- Listens for `SubmissionProcessingRequested` messages
- Processes one message at a time (QoS = 1)
- Logs processing details with correlation IDs
- Continues running until stopped (Ctrl+C)

### Running Both Services

**Terminal 1: Start infrastructure and all services**
```bash
docker compose up -d
```

**Terminal 2: Watch logs**
```bash
docker compose logs -f
```

The compose setup starts:
- `trainee-management-api`
- `submission-processor`
- `trainingdirectory`
- `mysql`
- `redis`
- `rabbitmq`

### Testing Workflow

1. **Login**: Get JWT token
   ```
   POST /api/user/login
   {
     "username": "Admin_Zeus_Learning",
     "password": "Admin@123"
   }
   ```

2. **Create Trainee**: Add trainee using token
   ```
   POST /api/trainee
   Authorization: Bearer <token>
   ```

3. **Create Learning Task**: Create task template

4. **Assign Task**: Assign to trainee

5. **Submit Task**: Upload files
   ```
   POST /api/task-submission
   - Files uploaded
   - SubmissionProcessingRequested published to queue
   ```

6. **Monitor Worker**: Check worker logs for processing

7. **Create Review**: Mentor reviews submission

## Key Features

### Security
- JWT authentication with configurable token duration
- BCrypt password hashing
- Role-based access control (RBAC)
- Unique email/username constraints

### File Management
- File upload validation (size, extension)
- Virus scan ready (extensible architecture)
- Organized file storage with timestamps
- Cascade delete on submission removal

### Caching
- Redis-backed distributed cache
- Configurable TTL (default 60 minutes)
- Cache keys standardized in Constants/CacheKeys.cs

### Async Processing
- RabbitMQ-based message queue
- Background worker processing
- Correlation tracking across distributed system
- Durable message persistence

### Monitoring
- Health checks (database, cache, RabbitMQ, and internal HTTP API)
- Comprehensive structured logging with correlation IDs
- Request/Response tracing across API and worker

### API Documentation
- Swagger/OpenAPI support
- JWT security definition in Swagger UI
- Self-documenting endpoints

## Configuration Management

### Secrets (Development)
- User Secrets for sensitive data (JWT keys, passwords)
- Environment-specific configuration

### Production Readiness
- Environment variable overrides
- Health check endpoints for orchestrators
- Graceful shutdown support
- Distributed tracing support (correlation IDs)

## Deployment Considerations

1. **Database**: Use managed MySQL service (AWS RDS, Azure Database for MySQL)
2. **Cache**: Use managed Redis (AWS ElastiCache, Azure Cache for Redis)
3. **Message Queue**: Use managed RabbitMQ (CloudAMQP, managed Kubernetes)
4. **File Storage**: Consider cloud storage (S3, Azure Blob Storage) instead of local
5. **Secrets**: Use secure vault (AWS Secrets Manager, Azure Key Vault)
6. **Scaling**: Horizontal scaling of API instances; multiple workers for async processing
7. **Monitoring**: Integrate with application insights/logging services
8. **API Gateway**: Use API Management for rate limiting, authentication

## Constants Reference

### Cache Keys (`Constants/CacheKeys.cs`)
- Standardized keys for Redis operations
- Naming convention: `{Entity}:{Operation}:{Id}`
- Examples: `User:GetAll`, `Trainee:GetById:1`

### Queue Names (`Constants/QueueNames.cs`)
- `SubmissionProcessing`: Main queue for submission processing messages

## Extending the System

### Adding New Entity
1. Create Model in `Models/` with validation attributes
2. Add DbSet to `AppDbContext.cs`
3. Create Service in `Services/` implementing `IService`
4. Register in `Program.cs` as scoped/singleton
5. Create Controller in `Controllers/`
6. Create EF Core migration

### Adding New Message Type
1. Define message contract in `Models/`
2. Add queue name in `Constants/QueueNames.cs`
3. Implement publishing in `Services/RabbitMqPublisher.cs`
4. Add consumer in `SubmissionProcessor/Worker.cs`

### Adding New Service
1. Define interface in `Services/Interfaces/`
2. Implement in `Services/`
3. Register in `Program.cs`
4. Inject where needed

## Troubleshooting

### Connection Issues
- Verify services running: `docker compose ps`
- Check connection strings in `appsettings.json`
- Verify ports: MySQL (3306), Redis (6379), RabbitMQ (5672)

### Database Issues
- Drop and recreate: `dotnet ef database drop -f && dotnet ef database update`
- Check migrations applied: `dotnet ef migrations list`

### Worker Not Processing
- Verify RabbitMQ connection: Check logs for "Worker started listening"
- Check queue: RabbitMQ Management UI http://localhost:15672
- Verify correlation: Check both API and Worker logs

### Cache Issues
- Flush Redis: `redis-cli FLUSHALL`
- Verify connection: `redis-cli ping`

### Performance Issues
- Monitor cache hit rates
- Check slow queries in database
- Review application logs for bottlenecks
- Consider adding indices for frequently queried fields

---

**Last Updated**: June 2026  
**Maintained By**: Training Development Team  
**Status**: Active Development