# fast-ride

Application for ride sharing

**Setup:**

- Open terminal
- Go to: cd C:\Work\FastRide\fast-ride\Run\Docker
- Run: docker-compose up -d
- Run: dotnet tool install -g Microsoft.Azure.SignalR.Emulator

**Run:**

- Open terminal
- Go to: cd C:\Work\FastRide\fast-ride\Run\Docker
- Run Docker and start the azurite
- Run: asrs-emulator start

**For ngrok**
- Go to: cd C:\Work\FastRide\fast-ride\FastRide.Server
- Run azure function: func start (or run from IDE)
- Run ngrok.exe and paste the command: ngrok http http://localhost:7102
- Use the link into the client (FastRide:BaseUrl)

- Run client: dotnet run --urls "http://localhost:7028" (or run from IDE)
- Run ngrok.exe and paste the command: ngrok http https://localhost:7028
