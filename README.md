# ATBM Project — Hệ thống quản lý bệnh viện & Bảo mật Oracle

Đồ án môn **CSC12001 — An toàn và Bảo mật dữ liệu trong HTTT**.

Ứng dụng WinForms kết nối Oracle XE, gồm phân hệ quản trị CSDL (user/role/grant/revoke), RBAC theo vai trò y tế, và Oracle Label Security (OLS) cho bảng thông báo.

---

## Mục lục

1. [Yêu cầu hệ thống](#1-yêu-cầu-hệ-thống)
2. [Cài đặt database](#2-cài-đặt-database)
3. [Chạy ứng dụng](#3-chạy-ứng-dụng)
4. [Tài khoản đăng nhập](#4-tài-khoản-đăng-nhập)
5. [Chức năng theo vai trò](#5-chức-năng-theo-vai-trò)
6. [Oracle Label Security (OLS)](#6-oracle-label-security-ols)
7. [Kiểm thử từng phân hệ](#7-kiểm-thử-từng-phân-hệ)
8. [Cấu trúc mã nguồn](#8-cấu-trúc-mã-nguồn)
9. [Lỗi thường gặp](#9-lỗi-thường-gặp)
10. [Quy ước phát triển](#10-quy-ước-phát-triển)

---

## 1. Yêu cầu hệ thống

- Oracle Database XE (PDB: `xepdb1`, port `1521`)
- SQL Developer (hoặc công cụ SQL tương đương)
- Visual Studio với .NET Framework 4.7.2
- Quyền `SYS as SYSDBA` để chạy script khởi tạo

Kết nối mặc định trong app (`Config/DBConfig.cs`):

| Tham số | Giá trị |
|---------|---------|
| Host | `localhost` |
| Port | `1521` |
| Service | `xepdb1` |

Màn hình đăng nhập chỉ nhập **Username** và **Password**; thông tin kết nối cố định trong code.

---

## 2. Cài đặt database

Chạy các script trong `Resources/` **theo đúng thứ tự**:

| Bước | File | Tài khoản | Mô tả |
|------|------|-----------|-------|
| 1 | `1.Database.sql` | **SYS as SYSDBA** | Tạo schema `ADMIN`, bảng, dữ liệu mẫu, tài khoản NV/BN |
| 2 | `2.RBAC&VPD.sql` | **ADMIN** | RBAC: role, view bảo mật cho BN/KTV/NV/Điều phối viên |
| 3a | `3.OLS_Setup.sql` — **PHẦN A** | **SYS as SYSDBA** | Tạo policy OLS, cấp FULL cho ADMIN, tạo user U1–U8 |
| 3b | `3.OLS_Setup.sql` — **PHẦN B** | **ADMIN** | Level, label, dữ liệu mẫu, stored procedure OLS |

### 2.1. Script `1.Database.sql`

```text
SYS as SYSDBA → chạy toàn bộ file
```

Tạo user `ADMIN` (mật khẩu `1234`), các bảng y tế, dữ liệu nhân viên/bệnh nhân, và Oracle user tương ứng (mật khẩu `123`).

### 2.2. Script `2.RBAC&VPD.sql`

```text
ADMIN → chạy toàn bộ file
```

Thiết lập `ROLE_BENHNHAN`, `ROLE_KYTHUATVIEN`, `ROLE_NHANVIEN`, `ROLE_DIEUPHOIVIEN` và các view RBAC.

### 2.3. Script `3.OLS_Setup.sql` (quan trọng — tách 2 phần)

**Không chạy** 4 dòng đầu (`CONFIGURE_OLS`, `SHUTDOWN`, `STARTUP`) trừ khi cài Oracle lần đầu và OLS chưa bật.

#### PHẦN A — SYS as SYSDBA

Chạy từ `ALTER SESSION SET CONTAINER = XEPDB1;` đến hết tạo user `U8_NV`, **dừng** trước dòng `PHAN B`.

PHẦN A thực hiện:

- Reset policy cũ `HOSPITAL_TB_POL`
- Cấp quyền OLS cho `ADMIN`
- `CREATE_POLICY`, `ENABLE_POLICY`
- `SET_USER_PRIVS('ADMIN', 'FULL')`
- Tạo user demo `U1_BGD` … `U8_NV`

Kiểm tra sau PHẦN A:

```sql
SELECT POLICY_NAME, STATUS
FROM   DBA_SA_POLICIES
WHERE  POLICY_NAME = 'HOSPITAL_TB_POL';

SELECT USER_NAME, USER_PRIVILEGES
FROM   DBA_SA_USER_PRIVS
WHERE  POLICY_NAME = 'HOSPITAL_TB_POL'
  AND  USER_NAME   = 'ADMIN';
```

Kỳ vọng: policy `ENABLED`, ADMIN có `USER_PRIVILEGES = FULL`.

> Cột đúng là `USER_PRIVILEGES`, không phải `PRIVILEGES` hay `POLICY_PRIVILEGES`.

#### PHẦN B — ADMIN

1. **Đăng xuất** SYS
2. **Đăng nhập** ADMIN / `1234`
3. Chạy từ **Bước 3** (tạo Level) đến hết file

Kiểm tra sau PHẦN B:

```sql
SELECT COUNT(*) FROM THONGBAO;   -- 7

SELECT OBJECT_NAME FROM USER_PROCEDURES
WHERE OBJECT_NAME IN ('SP_INSERT_THONGBAO', 'SP_GET_THONGBAO');
```

### 2.4. Phân quyền Điều phối viên (tùy chọn)

Thực hiện qua giao diện ADMIN sau khi đăng nhập:

1. Tạo role `RL_DIEUPHOIVIEN`
2. Cấp quyền:
   - `BENHNHAN`: SELECT, INSERT, UPDATE (full bảng)
   - `HSBA`: SELECT, INSERT (full); UPDATE (`MABS`, `MAKHOA`)
   - `HSBA_DV`: SELECT (full); UPDATE (`MAKTV`)
3. Gán role cho nhân viên có vai trò Điều phối viên (ví dụ `NV001`)
4. Đăng nhập lại bằng `NV001` để dùng phân hệ điều phối

---

## 3. Chạy ứng dụng

1. Mở `ATBM_Project.sln` trong Visual Studio
2. Chọn cấu hình **Debug**
3. **Build → Build Solution**
4. Bảo đảm Oracle XE đang chạy tại `localhost:1521/xepdb1`
5. Nhấn **F5**, đăng nhập bằng tài khoản phù hợp (xem mục 4)

Luồng giao diện:

```text
Program → FormLogin → (theo vai trò) → Form tương ứng
```

- `ADMIN` / `SYS` → `FormMain` (quản trị CSDL + quản lý thông báo OLS)
- Điều phối viên → `FormCoordinatorMain`
- Bác sĩ → `FormDoctorMain`
- Kỹ thuật viên → `FormKTVMain`
- Bệnh nhân → `FormBenhNhanMain`

---

## 4. Tài khoản đăng nhập

### Quản trị & OLS

| Username | Password | Ghi chú |
|----------|----------|---------|
| `ADMIN` | `1234` | DBA, quản lý thông báo OLS |
| `SYS` | *(mật khẩu SYS của bạn)* | Quản trị CSDL (tùy chọn) |

### Nhân viên / Bệnh nhân (RBAC)

| Username | Password | Vai trò ví dụ |
|----------|----------|---------------|
| `NV001`–`NV004` | `123` | Điều phối viên |
| `NV005`–`NV014` | `123` | Bác sĩ/Y sĩ |
| `NV015`–`NV020` | `123` | Kỹ thuật viên |
| `BN001`–`BN020` | `123` | Bệnh nhân |

### User demo OLS (kiểm tra bằng SQL)

| Username | Password | Nhãn session (tóm tắt) |
|----------|----------|------------------------|
| `NV021` | `123` | Ban giám đốc — toàn quyền |
| `NV022`,`NV023`,`NV026`,`NV027` | `123` | Lãnh đạo khoa (theo khoa/cơ sở) |
| `NV024`, `NV025`, `NV028` | `123` | Nhân viên (theo khoa/cơ sở) |

> User `NV021`–`NV028` dùng để **test OLS trực tiếp trên Oracle** (mục 6.4). App WinForms hiện điều hướng theo bảng `NHANVIEN`/`BENHNHAN`, chưa mở form thông báo cho các user U*.

---

## 5. Chức năng theo vai trò

### ADMIN / DBA (`FormMain`)

- Quản lý Oracle user: xem, tạo, đổi mật khẩu, xóa
- Quản lý role: xem, tạo, xóa
- Cấp/thu hồi quyền object và role
- Xem quyền user/role (kể cả quyền theo cột)
- **Quản lý thông báo OLS** (`FormThongBaoManagement`)

### Điều phối viên

- Quản lý bệnh nhân, hồ sơ bệnh án, dịch vụ (sau khi gán role `RL_DIEUPHOIVIEN` hoặc `ROLE_DIEUPHOIVIEN` từ script)

### Bác sĩ/Y sĩ

- Xem hồ sơ bệnh án (một phần dùng dữ liệu demo trong `DoctorPresenter`)

### Kỹ thuật viên

- Xem dịch vụ được chỉ định, cập nhật kết quả xét nghiệm/chẩn đoán

### Bệnh nhân

- Xem và chỉnh sửa hồ sơ cá nhân (các cột được phép qua view RBAC)

---

## 6. Oracle Label Security (OLS)

### 6.1. Tổng quan

- **Policy:** `HOSPITAL_TB_POL`
- **Bảng:** `ADMIN.THONGBAO` (cột `OLS_LABEL`)
- **Level:** `NV` (10) < `LDK` (20) < `BGD` (30)
- **Compartment (khoa):** `C_TH`, `C_TK`, `C_TM`
- **Group (cơ sở):** `G_HCM`, `G_HP`, `G_HN`
- **Ví dụ nhãn:** `NV:C_TH:G_HCM` — Nhân viên, Khoa Tiêu Hóa, TP.HCM

### 6.2. Ai chạy lệnh gì

## 6.2. Ai chạy lệnh gì

Toàn bộ script chia thành **Phần A** (chạy bằng `SYS AS SYSDBA`) và **Phần B** (chạy bằng `ADMIN`).

---

### Phần A — Kết nối `SYS AS SYSDBA`

| Bước | Thao tác | Lệnh / Package |
|------|----------|----------------|
| Tiền đề | Bật OLS (nếu chưa), shutdown & restart | `LBACSYS.CONFIGURE_OLS`, `LBACSYS.OLS_ENFORCEMENT.ENABLE_OLS` |
| Reset | Xóa policy cũ nếu có | `SA_POLICY_ADMIN.REMOVE_TABLE_POLICY`, `SA_SYSDBA.DROP_POLICY` |
| Bước 0 | Cấp `INHERIT PRIVILEGES` cho ADMIN | `GRANT INHERIT PRIVILEGES ON USER ADMIN TO LBACSYS` |
| Bước 1 | Cấp quyền OLS cho ADMIN (CONNECT, RESOURCE, LBAC_DBA, EXECUTE các package OLS) | `GRANT ... TO ADMIN` |

> `SYS` **không** tạo user demo. Việc tạo user NV021–NV028 do `ADMIN` thực hiện ở Bước 8.

---

### Phần B — Kết nối `ADMIN`

| Bước | Thao tác | Lệnh / Package |
|------|----------|----------------|
| Bước 2 | Tạo chính sách `HOSPITAL_TB_POL` (NO_CONTROL) | `SA_SYSDBA.CREATE_POLICY` |
| Bước 2 | Cấp quyền `FULL` cho ADMIN trên policy | `SA_USER_ADMIN.SET_USER_PRIVS(..., 'FULL')` |
| Bước 2 | Enable policy | `SA_SYSDBA.ENABLE_POLICY` |
| Bước 3 | Tạo Level: NV / LDK / BGD | `SA_COMPONENTS.CREATE_LEVEL` |
| Bước 4 | Tạo Compartment: C_TH / C_TK / C_TM | `SA_COMPONENTS.CREATE_COMPARTMENT` |
| Bước 5 | Tạo Group: G_HCM / G_HP / G_HN | `SA_COMPONENTS.CREATE_GROUP` |
| Bước 6 | Tạo Data Labels (1001–1013) | `SA_LABEL_ADMIN.CREATE_LABEL` |
| Bước 7 | Apply policy lên bảng `THONGBAO` với `NO_CONTROL` | `SA_POLICY_ADMIN.APPLY_TABLE_POLICY` |
| Bước 8 | Tạo 8 Oracle user demo (NV021–NV028), cấp SELECT / EXECUTE | `CREATE USER`, `GRANT SELECT`, `GRANT EXECUTE` |
| Bước 9 | INSERT 7 thông báo mẫu, UPDATE `OLS_LABEL` thủ công | `INSERT INTO THONGBAO`, `UPDATE ... CHAR_TO_LABEL` |
| Bước 10 | Đổi policy sang `READ_CONTROL` (remove rồi apply lại) | `SA_POLICY_ADMIN.REMOVE_TABLE_POLICY`, `SA_POLICY_ADMIN.APPLY_TABLE_POLICY` |
| Bước 11 | Touch toàn bộ hàng để OLS nhận lại label | `UPDATE THONGBAO SET NOIDUNG = NOIDUNG` |
| Bước 12 | Gán session label cho 8 user demo | `SA_USER_ADMIN.SET_USER_LABELS` |
| Bước 13 | Tạo hàm `FN_BUILD_LABEL` | `CREATE OR REPLACE FUNCTION` |
| Bước 14 | Gán label BGD toàn cục cho ADMIN, tạo `SP_INSERT_THONGBAO` | `SA_USER_ADMIN.SET_USER_LABELS` + `CREATE OR REPLACE PROCEDURE` |
| Bước 15 | Tạo `SP_GET_THONGBAO`, cấp EXECUTE cho 8 user demo | `CREATE OR REPLACE PROCEDURE` + `GRANT EXECUTE` |
| Bổ sung | Gán nhãn `NV` cho toàn bộ nhân viên còn lại trong bảng `NHANVIEN` | Loop `SA_USER_ADMIN.SET_USER_LABELS(..., 'NV')` |

---

### Tóm tắt phân quyền theo thao tác

| Thao tác | SYS | ADMIN |
|----------|:---:|:-----:|
| Bật OLS, SHUTDOWN / STARTUP | ✓ | ✗ |
| `CREATE_POLICY`, `DROP_POLICY`, `ENABLE_POLICY` | ✗ | ✓ |
| `SET_USER_PRIVS` – cấp FULL cho ADMIN | ✗ | ✓ |
| `GRANT` quyền hệ thống cho ADMIN | ✓ | ✗ |
| `CREATE USER` NV021–NV028 | ✗ | ✓ |
| `CREATE_LEVEL`, `CREATE_COMPARTMENT`, `CREATE_GROUP` | ✗ | ✓ |
| `CREATE_LABEL` | ✗ | ✓ |
| `APPLY_TABLE_POLICY` / `REMOVE_TABLE_POLICY` | ✗ | ✓ |
| `SET_USER_LABELS` (8 user demo + ADMIN + bổ sung) | ✗ | ✓ |
| Tạo `SP_INSERT_THONGBAO`, `SP_GET_THONGBAO` | ✗ | ✓ |

> **Lưu ý quan trọng:** `SYS` **không** trực tiếp gọi `SET_USER_PRIVS`.
> Lệnh này do `ADMIN` tự chạy ngay sau `CREATE_POLICY` (Bước 2),
> nhờ được cấp `EXECUTE ON LBACSYS.SA_USER_ADMIN WITH GRANT OPTION` từ Bước 1.
> `SYS` chỉ có vai trò cấp quyền hệ thống ban đầu và bật OLS.

### 6.3. Test OLS trên app

1. Đăng nhập `ADMIN` / `1234`
2. Sidebar → **Quản lý thông báo**
3. Tạo thông báo mới (theo các nhãn thông báo từ t1-t7)
4. Kiểm tra dữ liệu trong DB

### 6.4. Test OLS bằng SQL (user U*)

```sql
-- Đăng nhập NV021 / 123
SELECT COUNT(*) FROM ADMIN.THONGBAO;

-- Đăng nhập NV028 / 123
SELECT COUNT(*) FROM ADMIN.THONGBAO;
```

Số dòng khác nhau → OLS lọc đúng.

Xem nhãn dạng chữ:

```sql
SELECT MATB, NOIDUNG,
       LBACSYS.LABEL_TO_CHAR('HOSPITAL_TB_POL', OLS_LABEL) AS LABEL
FROM   ADMIN.THONGBAO
ORDER  BY MATB;
```

### 6.5. Lỗi OLS khi chạy script

| Mã lỗi | Nguyên nhân | Cách xử lý |
|--------|-------------|------------|
| ORA-12440 | Gọi `SA_SYSDBA` bằng ADMIN | Chạy bằng SYS as SYSDBA |
| ORA-12446 | ADMIN chưa có FULL khi `APPLY_TABLE_POLICY` | SYS chạy `SET_USER_PRIVS`, reconnect ADMIN |
| ORA-12407 | ADMIN chưa có FULL khi `SET_USER_LABELS` | Tương tự ORA-12446 |
| ORA-12416 | Policy chưa tồn tại | Chạy lại PHẦN A bằng SYS |
| ORA-00904 | Sai tên cột khi query | Dùng `USER_PRIVILEGES` trong `DBA_SA_USER_PRIVS` |

### 6.6. Chạy lại OLS từ đầu

1. SYS → chạy lại PHẦN A (có block reset đầu file)
2. Đăng nhập lại ADMIN
3. ADMIN → chạy lại PHẦN B

---

## 7. Kiểm thử từng phân hệ

### 7.1. Kỹ thuật viên (NV015)

1. Chạy `2.RBAC&VPD.sql` bằng ADMIN
2. Đăng nhập `NV015` / `123`
3. Kiểm tra danh sách dịch vụ — chỉ thấy dịch vụ được chỉ định cho NV015
4. Chọn dịch vụ → **Cập nhật kết quả** → Lưu

### 7.2. Bệnh nhân (BN001)

1. Đăng nhập `BN001` / `123`
2. Xem hồ sơ cá nhân
3. **Chỉnh sửa** địa chỉ hoặc tiền sử bệnh → Lưu
4. CCCD, Họ tên không sửa được (đúng RBAC)

### 7.3. Điều phối viên (NV001)

1. Gán role điều phối (mục 2.4)
2. Đăng nhập `NV001` / `123`
3. Kiểm tra quản lý bệnh nhân, hồ sơ, dịch vụ

### 7.4. Thông báo OLS

1. `ADMIN` → tạo thông báo trong app
2. So sánh số lượng thông báo khi query bằng `U1_BGD` vs `U8_NV` trong SQL Developer

---

## 8. Cấu trúc mã nguồn

```text
ATBM_Project/
├── Config/DBConfig.cs          # Kết nối Oracle
├── Models/                       # Model dữ liệu
├── Presenters/                   # Logic SQL & nghiệp vụ
├── Utilities/
│   ├── OracleHelper.cs           # Helper OLS / stored procedure
│   └── Prompt.cs
├── Views/
│   ├── FormLogin.cs              # Đăng nhập & điều hướng vai trò
│   ├── FormMain.cs               # Shell ADMIN/DBA
│   ├── FormThongBao*.cs          # OLS thông báo
│   ├── BN/                       # Bệnh nhân
│   ├── KTV/                      # Kỹ thuật viên
│   ├── DPV/                      # Điều phối viên
│   └── NV/                       # Nhân viên
└── Resources/
    ├── 1.Database.sql
    ├── 2.RBAC&VPD.sql
    └── 3.OLS_Setup.sql
```

**Quy ước lớp:**

- `Views/` — giao diện, gọi presenter
- `Presenters/` — truy vấn Oracle, DDL/DML
- `DBConfig.GetConnection()` — nơi duy nhất tạo kết nối

---
