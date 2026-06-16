# FastRide

A **ride-sharing application** built on **Azure Functions** (Durable Functions + SignalR) with a Blazor client. Drivers and passengers can connect in real time for ride sharing.

## Overview

FastRide is a cloud-native ride-sharing platform. The server uses Azure Durable Functions to orchestrate ride matching workflows, Azure SignalR Service for real-time communication between drivers and passengers, and Docker for local infrastructure (Azurite storage emulator). The client is a Blazor web application.

## Features

- Real-time ride matching via Azure SignalR
- - Durable Functions orchestration for ride lifecycle
  - - HTTP API triggers for ride requests and management
    - - SignalR triggers for real-time events
      - - JWT authentication
        - - Docker-based local development setup (Azurite + SignalR emulator)
          - - ngrok support for local-to-mobile testing
            - - Blazor web client
             
              - ## Project Structure
             
              - ```
                FastRide.Server/src/
                  FastRide.Server/
                    HttpTriggers/        Azure Function HTTP endpoints
                    SignalRTriggers/     Azure SignalR trigger functions
                    Orchestrations/      Durable Function orchestrators
                    Activities/          Durable Function activities
                    Authentication/      JWT auth handling
                    Models/              Domain models
                  FastRide.Server.Services/   Business logic
                  FastRide.Server.Contracts/  Interfaces and contracts
                  FastRide.Server.Sdk/        Shared SDK
                  FastRide.Server.sln

                FastRide.Client/             Blazor web client

                Run/Docker/                  Docker Compose setup (Azurite, SignalR emulator)
                Documentation/               Project documentation
                ```

                ## Technologies

                - C# / .NET
                - - Azure Functions (HTTP + SignalR + Durable Functions)
                  - - Azure SignalR Service
                    - - Azurite (local Azure Storage emulator)
                      - - Microsoft.Azure.SignalR.Emulator
                        - - Docker / Docker Compose
                          - - Blazor (client)
                            - - ngrok (local tunneling)
                             
                              - ## Setup
                             
                              - ### Prerequisites
                             
                              - - .NET SDK
                                - - Docker Desktop
                                  - - Azure Functions Core Tools (`npm install -g azure-functions-core-tools@4`)
                                    - - ngrok (optional, for mobile testing)
                                     
                                      - ### 1. Start infrastructure (Docker)
                                     
                                      - ```bash
                                        cd Run/Docker
                                        docker-compose up -d
                                        dotnet tool install -g Microsoft.Azure.SignalR.Emulator
                                        asrs-emulator start
                                        ```

                                        ### 2. Start the server

                                        ```bash
                                        cd FastRide.Server/src/FastRide.Server
                                        func start
                                        ```

                                        Or run from your IDE (Visual Studio / Rider).

                                        The server runs on `http://localhost:7102`.

                                        ### 3. Start the client

                                        ```bash
                                        dotnet run --urls "http://localhost:7028" --project FastRide.Client
                                        ```

                                        Or run from your IDE.

                                        ### 4. (Optional) ngrok for external access

                                        ```bash
                                        # Expose server
                                        ngrok http http://localhost:7102

                                        # Paste the ngrok URL into the client appsettings as FastRide:BaseUrl

                                        # Expose client
                                        ngrok http https://localhost:7028
                                        ```
