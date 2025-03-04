FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR emulator

RUN dotnet tool install -g Microsoft.Azure.SignalR.Emulator

COPY . .

ENV PATH="/root/.dotnet/tools:${PATH}"

RUN asrs-emulator upstream init

ENTRYPOINT asrs-emulator start