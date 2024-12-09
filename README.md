# fast-ride

Application for ride sharing
<br/>
**Setup:**
<br/>
_For Azurite-emulator instalation:_
<br/>
1. docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
<br/>
_For signalR-emulator instalation:_
<br/>
1. cd ...\fast-ride\Docker
<br/>
2. docker build -t signalr-emulator -f signalr-emulator .
<br/>
3. docker-compose -f signalr-docker-compose.yml up -d
