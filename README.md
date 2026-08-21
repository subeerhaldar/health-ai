# 🏥 Healthcare AI Copilot & Microservices Ecosystem

Welcome to the **Healthcare AI** project! This is a state-of-the-art, event-driven microservices architecture designed to empower medical professionals with an autonomous AI Copilot. The system integrates advanced LLM routing, Retrieval-Augmented Generation (RAG) for medical literature, direct hospital system integration (MCP), and automated workflow orchestration.

---

## 🏗️ Architecture & Flowchart

The system consists of a glassmorphic Angular frontend that communicates in real-time with an intelligent backend via an API Gateway. The central "brain" is the **Orchestrator API**, which uses Microsoft Semantic Kernel to autonomously route user intents to specialized microservices.

```mermaid
graph TD
    UI[🖥️ Angular 17 UI]
    GW[🌐 YARP API Gateway]
    ORCH[🧠 Orchestrator.Api<br/><i>Semantic Kernel</i>]
    RAG[📚 RagService.Api<br/><i>pgvector</i>]
    MCP[🏥 McpClient.Api]
    WF[⚙️ WorkflowEngine.Api<br/><i>MassTransit</i>]
    RMQ[🐇 RabbitMQ]
    DB[(🐘 PostgreSQL)]
    LLM[☁️ OpenRouter / LLM]

    UI -- SignalR / HTTP --> GW
    GW -- /orchestrator/chatHub --> ORCH
    
    ORCH -- Semantic Kernel Plugins --> RAG
    ORCH -- Semantic Kernel Plugins --> MCP
    ORCH -- Semantic Kernel Plugins --> WF
    
    ORCH -. AutoInvoke .-> LLM
    RAG -. Embeddings & Queries .-> LLM

    RAG --> DB
    WF --> RMQ
```

---

## 🛠️ Technologies Used

### Frontend
- **Angular 17** (Standalone Components)
- **RxJS & SignalR** (Real-time WebSockets)
- **CSS3** (Custom Glassmorphism, CSS Variables, Flexbox/Grid)
- **Nginx** (Lightweight containerized web server)

### Backend (Microservices)
- **.NET 8 Web API** (Minimal APIs)
- **YARP** (Reverse Proxy API Gateway)
- **Microsoft Semantic Kernel** (Autonomous AI Function Calling)
- **MassTransit** (Saga State Machines for Workflow automation)
- **SignalR** (Real-time bidirectional streaming)
- **Serilog** (Structured logging)

### Infrastructure & Data
- **Docker & Docker Compose** (Container orchestration)
- **PostgreSQL + pgvector** (Relational & Vector database for RAG & Workflows)
- **RabbitMQ** (Message broker for MassTransit Sagas)
- **OpenRouter** (Unified LLM access with model `nvidia/nemotron-3.5-lightning:free`)

---

## 🔐 Environment & Secret Configuration

To prevent sensitive credentials and API keys from leaking to Git, secrets are stored in a local `.env` file (which is git-ignored) and dynamically injected into Docker containers at runtime. 

Non-sensitive configuration defaults (such as model choices and endpoints) are maintained in `appsettings.json` files for each microservice:
- **Orchestrator**: Configured in [src/Orchestrator.Api/appsettings.json](file:///d:/Code/health-ai/src/Orchestrator.Api/appsettings.json) (`CompletionModel`, `BaseUrl`).
- **RAG Service**: Configured in [src/RagService.Api/appsettings.json](file:///d:/Code/health-ai/src/RagService.Api/appsettings.json) (`EmbeddingModel`, `CompletionModel`).
- **Workflow Engine**: Configured in [src/WorkflowEngine.Api/appsettings.json](file:///d:/Code/health-ai/src/WorkflowEngine.Api/appsettings.json).

---

## 🚀 How to Execute the App

### Prerequisites
- Docker & Docker Compose installed
- An OpenRouter API Key (Free tier works!)

### 1. Create your Environment File (`.env`)
Create a `.env` file in the project root directory (`/health-ai`) with your OpenRouter key and database credentials:

```env
OPENROUTER_API_KEY=sk-or-v1-YOUR-ACTUAL-KEY-HERE
POSTGRES_USER=healthai
POSTGRES_PASSWORD=password
POSTGRES_DB=healthai_db
```

> ⚠️ **Note**: Never commit `.env` to source control. It is already included in `.gitignore`.

### 2. Build and Spin Up the Containers
From the root directory (`/health-ai`), run the following command to build the entire ecosystem and start the containers in the background:
```bash
docker-compose up --build -d
```

### 3. Verify Services are Running
Ensure all containers booted successfully by checking:
```bash
docker-compose ps
```
You should see `postgres`, `rabbitmq`, `apigateway`, `orchestrator-api`, `ragservice-api`, `mcpclient-api`, `workflowengine-api`, and `frontend` all marked as **Up**.

### 4. Access the Application
- **Frontend UI**: Open [http://localhost:4200](http://localhost:4200) in your web browser.
- **API Gateway**: Running on [http://localhost:5000](http://localhost:5000).

### 5. Test the AI Copilot!
Navigate to the **AI Copilot** tab in the UI. 
- **Test MCP**: Ask *"What are John Doe's latest lab results?"*
- **Test Workflows**: Ask *"Please schedule a cardiology appointment for John Doe next Monday."*
- **Test RAG**: Ask *"What are the latest clinical guidelines for treating hypertension?"*

The Semantic Kernel will automatically deduce your intent, invoke the correct backend C# plugin, fetch the data, and stream the formatted response back to your screen with a color-coded status badge!

---

## 🗄️ Database Inspection (PostgreSQL / DBeaver)

### Connect via DBeaver / GUI Client:
- **Host**: `localhost` (or `127.0.0.1`)
- **Port**: `5432`
- **Database**: `healthai_db`
- **Username**: `healthai`
- **Password**: `password`

### Query directly via Docker CLI:
```bash
# Interactive psql shell
docker exec -it postgres psql -U healthai -d healthai_db

# Query RAG document chunks
docker exec -it postgres psql -U healthai -d healthai_db -c "SELECT \"Id\", \"Content\" FROM \"Documents\";"

# Query Appointment Sagas
docker exec -it postgres psql -U healthai -d healthai_db -c "SELECT * FROM \"AppointmentSagaState\";"
```

---

## 🛑 Shutting Down
To tear down the ecosystem and remove the containers, run:
```bash
docker-compose down
```
*(To wipe the PostgreSQL and RabbitMQ data volumes completely, append `-v`)*
