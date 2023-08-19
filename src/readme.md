- At src folder run:
  dotnet publish esp32-client/esp32-client.csproj -c Release -o ShareFiles/esp32-client/publish

- At ShareFiles run:

docker compose up -d --build esp32client.api

Or

- From src run:
  publish.bat

- Run offline: from src run once
  docker load -i aspnet6.tar

- Requirements:
  - [x] Chỉnh sửa hiển thị trang Monitoring
  - [ ] Link các đối tượng monitoring đến trang Setting
  - [x] Sửa thứ tự hiển thị các cột trong setting
  - [x] Thêm nhãn cho mỗi trang
  - [x] Load test cho web (500 request / 1s; 1000 request / 5s)
  - [ ] Thêm param IoT cho Machine
  - [ ] Thêm chức năng filter cho trang monitoring
  - [ ] Dùng MQTT để lấy dữ liệu sản xuất
  - [ ] Render riêng các số liệu sản xuất
