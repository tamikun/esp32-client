# PHRASE 1

## 1. Database
### Để dễ dàng trong việc sử dụng, không cần cài đặt nhiều:
- Chọn `MySql`
- Tương tác thông qua giao diện `PhpMyAdmin`
## 2. Tổ chức kiểu dữ liệu

- Đầu tiên cần có dữ liệu về `Partern`
- Sau đó định nghĩa về `Product` (mấy process, tên process, pattern)
- Thêm Department
- Thêm `Line` tương ứng với `Department`. Trong mỗi Line cần chọn Product của line đó (Nếu chưa có product thì phải khai báo product trước). Ta sẽ được các `Process` tương ứng với `Product`, tiếp tục định nghĩa `Machine` cho từng `Process` (Các Machine khả dụng)


### 2.1 Department

- Lưu tên định danh cho Department
- Cấu hình ẩn hiện trên giao diện

|Id| DepartmentName |Visible|
|--|----------------|-------|
|1 |  Department 1  |  true |

### 2.2 Line

- Định nghĩa các line theo Department (thứ tự hiển thị)

|Id|DepartmentId|LineName|Order|Visible|Product |
|--|------------|--------|-----|-------|--------|
|1 |      1     |Line 1  |  1  |  true |Product1|
|2 |      1     |Line 2  |  2  |  true |Product1|
|3 |      1     |Line 3  |  3  |  true |Product1|
|4 |      1     |Line 4  |  4  |  true |Product1|
|5 |      1     |Line 5  |  5  |  true |Product1|

### 2.3 Machine

|Id|MachineName | IpAddress  |DepartmentId|LineId|ProcessName|
|--|------------|------------|------------|------|-----------|
|1 |Machine 1   |192.168.1.11|1           |1     |           |
|2 |Machine 2   |192.168.1.12|1           |1     |           |
|3 |Machine 3   |192.168.1.13|1           |1     |           |
|4 |Machine 4   |192.168.1.14|1           |2     |           |
|5 |Machine 5   |192.168.1.15|1           |2     |           |

### 2.4 Partern

|Id|PatternNumber|FileName|FileData|Description   |
|--|-------------|--------|--------|--------------|
|1 |00001        |VDT001  |base64  |Diễn giải thêm|

### 2.5 Product

- Định nghĩa process theo các line
- Chọn Pattern cho Process

|Id|ProductName |ProcessName|Order|PatternNumber|
|--|------------|-----------|-----|-------------|
|1 |Product1    |Process 1  |1    |00001        |
|2 |Product1    |Process 2  |2    |00001        |
|3 |Product1    |Process 3  |3    |00001        |
|4 |Product1    |Process 4  |4    |00001        |
|5 |Product1    |Process 5  |5    |00001        |

### 2.6 Process

- Định nghĩa process theo các line
- Chọn Pattern cho Process

|Id|DepartmentId|LineId|Order|MachineId|
|--|------------|------|-----|---------|
|1 |      1     |  1   |1    |   1     |
|2 |      1     |  1   |2    |   2     |
|3 |      1     |  1   |3    |   3     |
|4 |      1     |  1   |4    |   4     |
|5 |      1     |  1   |5    |   5     |

### 2.6 UserAccount

|Id|LoginName   |Password|SalfKey|UserName|
|--|------------|--------|-------|--------|
|1 |quantm      |12qaewfa|abc    |Quan    |

### 2.7 UserRole

|Id|RoleName     |
|--|-------------|
|1 |Administrator|
|2 |Operator     |

### 2.7 RoleOfUser

|Id|UserId       |RoleId      |
|--|-------------|------------|
|1 |1            |1           |
|2 |1            |2           |
|3 |2            |2           |

### 2.7 UserRight

|Id|RoleId       |ControllerName|ActionName|
|--|-------------|--------------|----------|
|1 |1            |Home          |Index     |
|2 |1            |Patern        |Add       |
|3 |2            |Home          |Index     |

## 3. Các trang cần tạo

### Account

- Tạo tài khoản
- Thông tin tài khoản
- Đổi mật khẩu
- Phân quyền tài khoản

### Thêm, bớt, xóa, sửa

- Department
- Line
- Process
- Machine
- Pattern

### Giao diện Overview (theo phân quyền từng user)

