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
  - [x] Sửa thứ tự hiển thị các cột trong setting
  - [x] Thêm nhãn cho mỗi trang
  - [x] Load test cho web (500 request / 1s; 1000 request / 5s)
  - [x] Thêm param Cnc cho Machine
  - [x] Thêm chức năng filter cho trang monitoring
  - [x] Render riêng các số liệu product cho trang monitoring
  - [x] Thêm setting screen
  - [x] Tạo Jwt
  - [x] Thêm chức năng quản lí User Session
  - [x] Thêm màn hình quản lí session
  - [x] Thêm chức năng delete all token
  - [x] Thêm chức năng copy token, dùng để authorize khi gọi api từ các server khác
  - [x] Display full param
  - [x] Link các đối tượng monitoring đến trang Setting với options (Setting station, Setting Machine)
  - [ ] Lưu dữ liệu trong bao lâu?
  - [ ] Reset product number of machine
- Caching ???
- Phân trang cho các trang Setting và Assigning ???
- Tách bảng dữ liệu thành 2 bảng chính và phụ (tăng performance)
