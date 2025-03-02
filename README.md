# AI Chatbot Starter for .NET

A beginner-friendly tutorial for building AI-powered agents in the .NET ecosystem. This project provides a simple console-based chatbot utilizing Microsoft Semantic Kernel, Ollama, and Qdrant for memory storage.

## Prerequisites

Before running this project, make sure you have the following tools installed:

| Tool | Version | Download Link |
|------|---------|--------------|
| **.NET SDK** | 8.0 | [Download .NET 8](https://dotnet.microsoft.com/en-us/download) |
| **Ollama** | 0.5.10 (latest) | [Download Ollama](https://ollama.com/download) |
| **Docker Desktop** | Latest | [Download Docker Desktop](https://www.docker.com/products/docker-desktop/) |
| **Qdrant** | 1.13.3 (latest) | [Qdrant Dashboard](http://localhost:6333/dashboard) |
| **Microsoft Visual Studio Community** | 2022 (64-bit) - Version 17.13.0 | [Download Visual Studio Community](https://visualstudio.microsoft.com/vs/community/) |

## Installation  

1. Clone this repository:

```sh
   git clone https://github.com/AsterixBG/my-first-ai-chatbot.git
```

Navigate to the project directory:
```sh
cd my-first-ai-chatbot/src
```

Restore dependencies:

```sh
dotnet restore
```

Run the application:

```sh
dotnet run
```

## How to Use the AI Chatbot

This AI-powered chatbot is designed to provide context-aware responses using **Microsoft Semantic Kernel**, **Ollama**, and **Qdrant**. Follow the steps below to run and interact with the agent.

### 1 Start the Required Services

Before running the chatbot, make sure you have the necessary services up and running:

- **Ollama** (local LLM model)  
  Start Ollama if it’s not running:  

```sh
ollama run mistral
```

- **Qdrant** (vector memory storage)

```sh
docker run -d --name qdrant -p 6333:6333 qdrant/qdrant
```

### 2 Run the Chatbot

Once the dependencies are running, navigate to the `src` folder and start the console application:

```sh
cd src
dotnet run
```

### 3 Interact with the AI Chatbot

Once running, the console will prompt you to enter a message:

```
AI Chatbot (with improved context) – Type 'exit' to quit.
Enter your question:
```

Simply type your message and press `Enter`. The chatbot will analyze previous conversations stored in **Qdrant** and generate a response using the **Mistral** model.

Example interaction:

```
Enter your question: What is the capital of France?
AI: The capital of France is Paris.
```

### 4 Exiting the Chatbot

To exit, simply type `exit`

