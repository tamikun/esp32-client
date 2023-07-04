- At src folder run:
dotnet publish esp32-client/esp32-client.csproj -c Release -o ShareFiles/esp32-client/publish

- At ShareFiles run:

docker compose up -d --build esp32client.api

Or

- From src run:
publish.bat
