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

- Go to: cd C:\Work\FastRide\fast-ride\FastRide.Server
- Run azure function: func start
- Run ngrok.exe and paste the command: ngrok http 7071
- Use the link into the client (FastRide:BaseUrl)

-
