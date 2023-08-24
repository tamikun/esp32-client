@echo off
cd /d D:\MiQ\MiQ_NT\ESP32Client\esp32-client\src



dotnet publish esp32-client\esp32-client.csproj -c Release -o ShareFiles\esp32-client\publish

REM Check if the publish command was successful
IF %errorlevel% NEQ 0 (
  echo Failed to publish the project. Exiting...
  exit /b 1
)

cd ShareFiles
docker compose up -d --build esp32client.api
REM pause