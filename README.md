# PHRASE 1

## 1. Database
### Để dễ dàng trong việc sử dụng, không cần cài đặt nhiều:
- Chọn `MySql`
- Tương tác thông qua giao diện `PhpMyAdmin`
## 2. Tổ chức kiểu dữ liệu

### 2.1 Factory

|Id| FactoryName |FactoryNo  |
|--|-------------|-----------|
|1 |  Factory 1  |Factory 001|

### 2.2 Line

|Id|FactoryId   |LineNo             |LineName|ProductId |
|--|------------|-------------------|--------|----------|
|1 |      1     |Production line 1  |--------|1         |
|2 |      1     |Production line 2  |--------|1         |
|3 |      1     |Production line 3  |--------|1         |
|4 |      1     |Production line 4  |--------|1         |
|5 |      1     |Production line 5  |--------|1         |

### 2.3 Station

|Id|LineId      |StationNo   |StationName|
|--|------------|------------|-----------|
|2 |      1     |Station 2   |-----------|
|3 |      1     |Station 3   |-----------|
|4 |      1     |Station 4   |-----------|
|5 |      1     |Station 5   |-----------|

**StationNo**: Quy ước ở bảng Setting với dạng `Station {0}` (Trong đó {0} sẽ tăng dần trong 1 line và bắt đầu từ 1)

### 2.4 Product

|Id|FactoryId   |ProductName |ProductNo  |
|--|------------|------------|-----------|
|1 |      1     |Sản phẩm 1  |Product 001|
|2 |      1     |Sản phẩm 2  |Product 002|

**ProductNo**: Quy ước ở bảng Setting với dạng `Product {0}` (Trong đó {0} sẽ được thay bằng giá trị Id của Product)

### 2.5 Process

|Id|ProductId   |ProcessName|ProcessNo    |PatternNumber|PatternDirectory |OperationData|COAttachment|Description|
|--|------------|-----------|-------------|-------------|-----------------|-------------|------------|-----------|
|1 |      1     | Process 1 |Product 001.1|VD00001.VDT  |                 |             |            |           |
|2 |      1     | Process 2 |Product 001.2|VD00002.VDT  |                 |             |            |           |
|3 |      1     | Process 3 |Product 001.3|VD00003.VDT  |                 |             |            |           |
|4 |      1     | Process 4 |Product 001.4|VD00004.VDT  |                 |             |            |           |
|5 |      1     | Process 5 |Product 001.5|VD00005.VDT  |                 |             |            |           |


### 2.6 Machine

|Id|MachineName | IpAddress  |FactoryId   |LineId|StationId  |COPartNo|
|--|------------|------------|------------|------|-----------|--------|
|1 |Machine 1   |192.168.1.11|1           |1     |     1     |COP--   |
|2 |Machine 2   |192.168.1.12|1           |1     |     2     |COP--   |
|3 |Machine 3   |192.168.1.13|1           |1     |     3     |COP--   |
|4 |Machine 4   |192.168.1.14|1           |2     |     4     |COP--   |
|5 |Machine 5   |192.168.1.15|1           |2     |     5     |COP--   |

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
