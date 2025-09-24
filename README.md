# HMCTS .NET API

<!-- badges -->
[![GitHub latest commit](https://img.shields.io/github/last-commit/zmahmood98/hmcts-backend.svg)](https://github.com/zmahmood98/hmcts-backend/commit/)
[![GitHub forks](https://img.shields.io/github/forks/zmahmood98/hmcts-backend.svg)](https://github.com/zmahmood98/hmcts-backend)

Backend repo for the dts-developer-challenge. Frontend project repo can be found [here](https://github.com/zmahmood98/hmcts-frontend).

A simple Task Manager application built with **.NET** and **PostgreSQL**.  
It provides a RESTful API for managing tasks (create, read, update, delete) and supports Docker for local development.  

---
## 🚀 Getting Started

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- If you do not have it already, install EF Core CLI tools using `dotnet tool install --global dotnet-ef`

## Database Setup
Run PostgreSQL in Docker:

```
docker run --name hmcts-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=hmctsdb -p 5432:5432 -d postgres:15
```

## Usage
1. Clone the repo `git clone git@github.com:zmahmood98/hmcts-backend.git`
2. Enter the directory `cd hmcts-backend`
3. Run `dotnet ef database update` to apply database migrations
4. To start the server in development mode, run `dotnet build` and then `dotnet run` 

---

## 📖 API Documentation

### Base URL
```
http://localhost:5025/api
```
### Endpoints

| **URL**                 |**Description**                  | **HTTP Verb** | **Action** | **Request Body**                                                                   | **Response Example** |
|-------------------------|---------------------------------|---------------|------------|------------------------------------------------------------------------------------|----------------------|
| `/tasks`                |Returns all tasks                | GET           | index      | –                                                                                  | <pre><code>[{ "id": 1, "title": "Finish project", "description": null, "status": "To do", "dueDate": "2025-09-24T18:13:32.254982+01:00" }]</code></pre> |
| `/tasks`                |Creates a new task               | POST          | create     | <pre><code>{ "title": "New Task", "dueDate": "2025-09-23T11:00:00Z" }</code></pre> | - |
| `/tasks/{id}`           |Returns specific task            | GET           | show       | –                                                                                  | <pre><code>{ "id": 1, "title": "Finish project", "description": null, "status": "To do", "dueDate": "2025-09-24T18:13:32.254982+01:00" }</code></pre>   |
| `/tasks/{id}`           |Returns specific task            | DELETE        | delete     | –                                                                                  | - |
| `/tasks/status/{status}`|Returns task with specific status| GET           | show       | –                                                                                  | <pre><code>[{ "id": 1, "title": "Finish project", "description": null, "status": "Done", "dueDate": "2025-09-24T18:13:32.254982+01:00" }]</code></pre> |
| `/tasks/{id}/status`    |Updates status of a specific task| PUT           | update     | <pre><code>"Doing"</code></pre>                                                    | - |

---

## 🧪 Running Tests
In the root directory run `dotnet test` to start the test suite.

---

## 🛠 Tech Stack
- **.NET 8**
- **PostgreSQL**
- **Docker**

---

## 🛣 Roadmap
- [ ] Refactor code to use a .env file for sensitive data like passwords 
- [ ] Deploy API and database, along with frontend
- [ ] Add authentication, user, and login routes

---

## 📌 Notes
- Make sure Docker is running before starting the app.
