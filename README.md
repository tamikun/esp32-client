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

|Id| DepartmentName |
|--|----------------|
|1 |  Department 1  |

### 2.2 Line

- Định nghĩa các line theo Department (thứ tự hiển thị)

|Id|DepartmentId|LineName|Order|ProductId |
|--|------------|--------|-----|----------|
|1 |      1     |Line 1  |  1  |1         |
|2 |      1     |Line 2  |  2  |1         |
|3 |      1     |Line 3  |  3  |1         |
|4 |      1     |Line 4  |  4  |1         |
|5 |      1     |Line 5  |  5  |1         |

### 2.3 Product

|Id|ProductName |
|--|------------|
|1 |Product1    |
|2 |Product2    |


### 2.4 Pattern

|Id|PatternNumber|FileName|FileData|Description   |
|--|-------------|--------|--------|--------------|
|1 |00001        |VDT001  |base64  |Diễn giải thêm|


### 2.5 Process

|Id|ProductId   |ProcessName|PatternId|Order|
|--|------------|-----------|---------|-----|
|1 |      1     | P1        |1        |0    |
|2 |      1     | P2        |1        |1    |
|3 |      1     | P3        |1        |2    |
|4 |      1     | P4        |1        |3    |
|5 |      1     | P5        |1        |4    |


### 2.6 Machine

|Id|MachineName | IpAddress  |DepartmentId|LineId|ProcessId  |
|--|------------|------------|------------|------|-----------|
|1 |Machine 1   |192.168.1.11|1           |1     |     1     |
|2 |Machine 2   |192.168.1.12|1           |1     |     2     |
|3 |Machine 3   |192.168.1.13|1           |1     |     3     |
|4 |Machine 4   |192.168.1.14|1           |2     |     4     |
|5 |Machine 5   |192.168.1.15|1           |2     |     5     |

### 2.7 UserAccount

|Id|LoginName   |Password|SalfKey|UserName|
|--|------------|--------|-------|--------|
|1 |quantm      |12qaewfa|abc    |Quan    |

### 2.8 UserRole

|Id|RoleName     |
|--|-------------|
|1 |Administrator|
|2 |Operator     |
|3 |View         |

### 2.9 RoleOfUser

|Id|UserId       |RoleId      |
|--|-------------|------------|
|1 |1            |1           |
|2 |1            |2           |
|3 |2            |2           |

### 2.10 UserRight

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

