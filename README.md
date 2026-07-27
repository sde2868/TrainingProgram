# **TrainingProgram**

## **Task folders**

### **DevTools/**
- **Purpose:** Provides development utilities, analysis reports, and tooling references used during training.
- **Highlights:**
  - Browser performance and accessibility reports.
  - Tool documentation and environment setup support.
 
### **TaskTracker/**
- **Purpose:** Front-end implementation for tracking tasks and progress in a training environment.
- **Highlights:**
  - HTML, CSS, and JavaScript files for the task tracker UI.
  - A simple interactive interface for task management.

### **TraineeManagement/**
- **Purpose:** Full-stack ASP.NET project for managing trainees, tasks, reviews, and mentors.
- **Highlights:**
  - ASP.NET Core backend with controllers, services, data models, and Entity Framework migrations.
  - Configuration files for app settings and launch profiles.
  - API endpoints for trainee management workflows.

### **TraineeManagement.client/**
- **Purpose:** Angular + TypeScript frontend for the TraineeManagement. It consumes the ASP.NET Core backend APIs from `TraineeManagement/` and provides protected UI screens for managing trainees, mentors, learning tasks, assignments, submissions, reviews, and related workflow operations.
- **Highlights:**
  - Modern Angular project using standalone configuration with `app.config.ts` and `app.routes.ts`.
  - Feature-based folder structure with `core/`, `shared/`, `features/`, and `models/`.
  - Planned protected routing for Login, Dashboard, Trainees, Mentors, Learning Tasks, Assignments, Submissions, and Reviews as required by the Angular training task.
  - Reusable shared components planned for buttons, inputs, select controls, data tables, loaders, error messages, pagination, and status badges.
  - API integration planned with typed Angular services for backend routes such as `/api/auth/login`, `/api/trainees`, `/api/mentors`, `/api/learning-tasks`, `/api/task-assignments`, `/api/submissions`, and `/api/reviews`.
  - Authentication flow planned with JWT login, route guard, HTTP interceptor, logout, and protected API access.
  - Focus areas include strict TypeScript usage, reactive forms, loading/error/empty states, accessibility, basic tests, screenshots, and README documentation.
 
### **FootballMatchSimulator/**
- **Purpose:** OOP and SOLID principles practice task in typescript simulating events and match commentary with score.
- **Highlights:**
  - Classes / types / interface in typescript including player, team, event, score, match, commentary.
  - EnglishCommentary implements Commentary interface and Striker, GoalKeeper, Midfielder, Defender extends Player.
  - Methods following OOP and SOLID principles for simple object creation, event recording, score updating and live commentary.
 
### **GridSimulator/**
- **Purpose:** Simulation of Microsoft Excel Grid using typescript.
- **Highlights:**
  - Grid rendering using row, column and cell models on canvas using virtualization.
  - Scrolling, cell selection, row/column resizing, data-storage, cell editing, range selection.
  - Status bar, formulas, range undo/redo, range copy/paste.
