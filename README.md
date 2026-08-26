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

## 🧩 System Class Diagram & Code Execution Flow

### 1. Class Diagram & Component Structure
The class diagram below details the underlying C# and Angular classes, interfaces, plugins, EF Core contexts, and how they connect across microservices:

```mermaid
classDiagram
    namespace Frontend {
        class ChatComponent {
            +string userId
            +string role
            +sendMessage(string text)
            +ngOnInit()
        }
        class ChatService {
            +startConnection()
            +sendMessage(string userId, string role, string message)
            +onReceiveMessage(callback)
        }
    }

    namespace Orchestrator_Api {
        class ChatHub {
            +SendMessage(string userId, string role, string message)
        }
        class PluginTrackingFilter {
            +OnFunctionInvocationAsync(context, next)
        }
        class PluginTracker {
            +string LastInvokedPlugin
        }
        class PatientDataPlugin {
            +GetPatientRecordAsync(string patientId) string
            +GetLabResultsAsync(string patientId) string
        }
        class ClinicalGuidelinesPlugin {
            +QueryGuidelinesAsync(string query) string
        }
        class WorkflowPlugin {
            +StartAppointmentWorkflowAsync(string patientId, string providerId, DateTime date) string
            +StartPrescriptionRefillAsync(string patientId, string medicationId) string
        }
    }

    namespace RagService_Api {
        class RagQueryEndpoint {
            +MapPost_RagQuery(request)
        }
        class RagSearchService {
            +RankDocumentsAsync(query, documents, topK)
        }
        class OpenRouterEmbeddingService {
            +GenerateEmbeddingAsync(string text)
        }
        class OpenRouterCompletionService {
            +GenerateAnswerAsync(string query, context)
        }
        class RagDbContext {
            +DbSet_DocumentChunk Documents
        }
        class DocumentChunk {
            +Guid Id
            +string Content
            +Vector Embedding
        }
    }

    namespace McpClient_Api {
        class McpEndpoints {
            +GetPatientRecordsEndpoint(string patientId)
            +GetLabResultsEndpoint(string patientId)
        }
        class HospitalSystemConnectorFactory {
            +GetConnector(string systemName) IHospitalSystemConnector
        }
        class IHospitalSystemConnector {
            <<interface>>
            +GetPatientRecordAsync(string patientId)
            +GetLabResultsAsync(string patientId)
        }
        class EmrConnector {
            +GetPatientRecordAsync(string patientId)
        }
        class LisConnector {
            +GetLabResultsAsync(string patientId)
        }
    }

    namespace WorkflowEngine_Api {
        class WorkflowEndpoints {
            +StartAppointmentEndpoint(request)
        }
        class AppointmentSchedulingSaga {
            +DefineState()
        }
        class AppointmentSagaState {
            +Guid CorrelationId
            +string CurrentState
            +string PatientId
        }
        class WorkflowDbContext {
            +DbSet_AppointmentSagaState AppointmentSagas
        }
    }

    ChatComponent --> ChatService : uses
    ChatService --> ChatHub : SignalR WebSockets
    ChatHub --> PluginTracker : resets tracker
    ChatHub ..> PluginTrackingFilter : intercepts invocation
    PluginTrackingFilter --> PluginTracker : records plugin

    ChatHub --> PatientDataPlugin : Semantic Kernel AutoInvoke
    ChatHub --> ClinicalGuidelinesPlugin : Semantic Kernel AutoInvoke
    ChatHub --> WorkflowPlugin : Semantic Kernel AutoInvoke

    PatientDataPlugin --> McpEndpoints : HTTP GET /mcp
    McpEndpoints --> HospitalSystemConnectorFactory : resolves connector
    HospitalSystemConnectorFactory --> IHospitalSystemConnector : returns stub
    IHospitalSystemConnector <|.. EmrConnector : implements
    IHospitalSystemConnector <|.. LisConnector : implements

    ClinicalGuidelinesPlugin --> RagQueryEndpoint : HTTP POST /rag/query
    RagQueryEndpoint --> RagSearchService : ranks docs
    RagSearchService --> OpenRouterEmbeddingService : generates embeddings
    RagQueryEndpoint --> RagDbContext : fetches all docs
    RagQueryEndpoint --> OpenRouterCompletionService : generates answer
    RagDbContext --> DocumentChunk : contains pgvector

    WorkflowPlugin --> WorkflowEndpoints : HTTP POST /workflows
    WorkflowEndpoints --> AppointmentSchedulingSaga : MassTransit Event
    AppointmentSchedulingSaga --> WorkflowDbContext : persists saga state
```

---

### 2. Contextual Execution Flows (By Prompt Scenario)

Depending on the specific prompt entered by the user, Semantic Kernel dynamically deduces the intent and routes the execution flow through different microservice pathways:

#### 🧪 Scenario A: Patient Records & Hospital Integration (MCP)
**User Prompt:** *"What are John Doe's lab results?"*

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant UI as Angular Chat UI
    participant GW as YARP Gateway
    participant HUB as Orchestrator ChatHub
    participant SK as Semantic Kernel & Filter
    participant MCP as PatientDataPlugin
    participant SVC as McpClient.Api
    participant LIS as LisConnector Stub

    User->>UI: Types "What are John Doe's lab results?" & clicks Send
    UI->>GW: SignalR WebSocket SendMessage(userId, role, message)
    GW->>HUB: Forwards to /orchestrator/chatHub
    HUB->>SK: InvokePromptAsync(prompt)
    SK->>SK: LLM infers tool: PatientDataPlugin.GetLabResultsAsync("p-123")
    SK->>SK: PluginTrackingFilter records "MCP"
    SK->>MCP: Executes GetLabResultsAsync("p-123")
    MCP->>SVC: HTTP GET http://mcpclient-api:8080/mcp/lab-results/p-123
    SVC->>LIS: Aggregates lab records from LisConnector stub
    LIS-->>SVC: Returns JsonObject (HbA1c: 6.8%, Cholesterol: 180)
    SVC-->>MCP: Returns Aggregated JSON response
    MCP-->>SK: Returns data to Semantic Kernel
    SK-->>HUB: LLM formats response into natural language table
    HUB->>UI: SignalR ReceiveMessage(answer, "MCP")
    UI-->>User: Renders formatted table & "MCP" badge
```

---

#### 📚 Scenario B: Medical Literature & Guidelines Search (RAG)
**User Prompt:** *"What are the clinical guidelines for managing Type 2 Diabetes?"*

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant UI as Angular Chat UI
    participant GW as YARP Gateway
    participant HUB as Orchestrator ChatHub
    participant SK as Semantic Kernel & Filter
    participant RAG_P as ClinicalGuidelinesPlugin
    participant RAG_API as RagService.Api
    participant DB as pgvector Database
    participant LLM as OpenRouter LLM

    User->>UI: Types "What are the clinical guidelines for Type 2 Diabetes?"
    UI->>GW: SignalR WebSocket SendMessage(userId, role, message)
    GW->>HUB: Forwards to /orchestrator/chatHub
    HUB->>SK: InvokePromptAsync(prompt)
    SK->>SK: LLM infers tool: ClinicalGuidelinesPlugin.QueryGuidelinesAsync
    SK->>SK: PluginTrackingFilter records "RAG"
    SK->>RAG_P: Executes QueryGuidelinesAsync("Type 2 Diabetes guidelines")
    RAG_P->>RAG_API: HTTP POST http://ragservice-api:8080/rag/query
    RAG_API->>LLM: Generates vector embedding for prompt query
    LLM-->>RAG_API: Returns 384-float vector array
    RAG_API->>DB: Cosine Distance Search on DocumentChunk vector embeddings
    DB-->>RAG_API: Returns top K relevant medical context chunks
    RAG_API->>LLM: Generates grounded response using retrieved context
    LLM-->>RAG_API: Returns answer with citations
    RAG_API-->>RAG_P: Returns RagQueryResponse payload
    RAG_P-->>SK: Returns guideline context to Semantic Kernel
    SK-->>HUB: Finalizes formatted answer
    HUB->>UI: SignalR ReceiveMessage(answer, "RAG")
    UI-->>User: Renders clinical recommendations & "RAG" badge
```

---

#### ⚙️ Scenario C: Workflow Automation & Appointment Scheduling (MassTransit Sagas)
**User Prompt:** *"Please schedule a cardiology appointment for patient p-123 next Monday."*

```mermaid
sequenceDiagram
    autonumber
    actor User
    participant UI as Angular Chat UI
    participant GW as YARP Gateway
    participant HUB as Orchestrator ChatHub
    participant SK as Semantic Kernel & Filter
    participant WF_P as WorkflowPlugin
    participant WF_API as WorkflowEngine.Api
    participant RMQ as RabbitMQ Broker
    participant SAGA as AppointmentSchedulingSaga
    participant DB as PostgreSQL (WorkflowDb)

    User->>UI: Types "Schedule a cardiology appointment for p-123 next Monday"
    UI->>GW: SignalR WebSocket SendMessage(userId, role, message)
    GW->>HUB: Forwards to /orchestrator/chatHub
    HUB->>SK: InvokePromptAsync(prompt)
    SK->>SK: LLM infers tool: WorkflowPlugin.StartAppointmentWorkflowAsync
    SK->>SK: PluginTrackingFilter records "Workflow"
    SK->>WF_P: Executes StartAppointmentWorkflowAsync("p-123", "cardiology", "2026-08-31")
    WF_P->>WF_API: HTTP POST http://workflowengine-api:8080/workflows/appointment/start
    WF_API->>RMQ: Publishes StartAppointmentScheduling Event (CorrelationId)
    RMQ->>SAGA: MassTransit triggers State Machine Saga
    SAGA->>DB: Persists initial saga state to AppointmentSagaState table
    WF_API-->>WF_P: Returns HTTP 202 Accepted { WorkflowId }
    WF_P-->>SK: Returns Workflow confirmation details
    SK-->>HUB: Finalizes response message
    HUB->>UI: SignalR ReceiveMessage(answer, "Workflow")
    UI-->>User: Renders workflow confirmation & "Workflow" badge
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
