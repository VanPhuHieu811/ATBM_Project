# ATBM Project - Oracle Security Admin

Đây là đồ án môn **CSC12001 - An toàn và Bảo mật dữ liệu trong HTTT**. Source hiện tại là ứng dụng WinForms phục vụ phân hệ quản trị Oracle và script dữ liệu mẫu cho bài toán quản lý y tế.

## Trạng thái hiện tại

Ứng dụng đang dùng một luồng giao diện chính duy nhất:

`Program` -> `FormLogin` -> `FormMain` -> các form chức năng trong sidebar.

Các form cũ `frmUserRole` và `frmEditUser` đã được loại khỏi project để tránh tồn tại hai hướng UI song song. Khi phát triển tiếp, chỉ mở rộng các màn hình đang được `FormMain` gọi.

## Cấu trúc thư mục

```text
ATBM_Project/
├── App.config
├── ATBM_Project.csproj
├── ATBM_Project.sln
├── Config/
│   └── DBConfig.cs
├── Models/
│   └── AccountModels.cs
├── Presenters/
│   ├── PrivilegePresenter.cs
│   ├── RevokePresenter.cs
│   ├── RolePresenter.cs
│   └── UserPresenter.cs
├── Resources/
│   └── Database.sql
├── Utilities/
│   └── Prompt.cs
└── Views/
    ├── FormLogin.cs
    ├── FormMain.cs
    ├── FormUser.cs
    ├── FormRole.cs
    ├── FormGrantPrivileges.cs
    ├── FormGrantRoles.cs
    ├── FormRevoke.cs
    ├── FormRevokePrivilege.cs
    └── FormViewPrivileges.cs
```

## Vai trò từng lớp

- `Config/DBConfig.cs`: lưu cấu hình kết nối Oracle theo thông tin nhập ở màn login. Đây là nơi duy nhất tạo `OracleConnection`.
- `Models/AccountModels.cs`: chứa model đơn giản cho user, role và privilege.
- `Presenters/`: chứa logic truy vấn Oracle và thực thi lệnh quản trị như `CREATE USER`, `CREATE ROLE`, `GRANT`, `REVOKE`.
- `Views/`: chứa WinForms. Form chỉ nên gọi presenter, không nên tự viết chuỗi kết nối riêng.
- `Resources/Database.sql`: script tạo user `admin`, schema y tế, dữ liệu mẫu, procedure và function test.

## Cách chạy project

1. Mở `ATBM_Project.sln` bằng Visual Studio.
2. Chọn cấu hình `Debug`, không chạy bằng `Release` khi cần debug.
3. Khởi động Oracle XE và bảo đảm service PDB `xepdb1` đang chạy.
4. Chạy script `Resources/Database.sql` bằng tài khoản có quyền DBA/SYSDBA.
5. Chạy ứng dụng và đăng nhập:
   - Host: `localhost`
   - Port: `1521`
   - Service/PDB: `xepdb1`
   - Username: `admin`
   - Password: `1234`

Nếu đăng nhập bằng `SYS`, `DBConfig` có hỗ trợ thêm `DBA Privilege=SYSDBA`, nhưng màn login đang khuyến nghị dùng service PDB như `xepdb1`, không dùng root service `xe`.

## Chức năng đã có

- Xem, tìm kiếm, tạo, đổi mật khẩu và xóa Oracle user.
- Xem, tìm kiếm, tạo và xóa Oracle role.
- Cấp quyền object cho user/role trên table, view, procedure, function.
- Hỗ trợ `WITH GRANT OPTION` khi cấp quyền object cho user.
- Hỗ trợ cấp role với `WITH ADMIN OPTION`.
- Hỗ trợ quyền `SELECT` và `UPDATE` theo cột.
- Thu hồi quyền từ user/role.
- Xem quyền object và quyền theo cột của user/role.

## Đối chiếu với đề bài

Phân hệ 1 đã có nền chính cho quản trị user, role, grant, revoke và xem quyền. Các phần cần hoàn thiện thêm:

- Bổ sung validate tên user, role, object chặt hơn cho các câu DDL.
- Tách rõ quyền hệ thống và quyền object trong màn cấp quyền nếu muốn demo đầy đủ hơn.
- Bổ sung sửa role nâng cao nếu giảng viên yêu cầu.

Phân hệ 2 hiện mới có script dữ liệu y tế mẫu. Các phần lớn còn cần làm:

- RBAC cho `Điều phối viên`, `Bác sĩ/Y sĩ`, `Kỹ thuật viên`, `Bệnh nhân`.
- VPD bằng `DBMS_RLS` cho chính sách truy cập theo dòng.
- OLS cho bảng `THONGBAO`.
- Standard Audit, Fine-grained Audit hoặc Unified Audit.
- Script sao lưu và phục hồi dữ liệu.
- Giao diện minh họa cho người dùng y tế theo từng vai trò.

## Quy ước phát triển tiếp

- Không tạo thêm form quản trị user/role mới nếu chức năng có thể đặt vào `FormUser`, `FormRole`, `FormGrantPrivileges`, `FormGrantRoles`, `FormRevoke` hoặc `FormViewPrivileges`.
- Không viết connection string trực tiếp trong form. Luôn dùng `DBConfig.GetConnection()`.
- Logic SQL đặt trong `Presenters/`; form chỉ lấy input, hiển thị kết quả và thông báo lỗi.
- Với DDL như `CREATE USER`, `CREATE ROLE`, `GRANT`, `REVOKE`, Oracle không bind được tên object, nên phải validate identifier trước khi ghép chuỗi.
- Script CSDL nên để trong `Resources/` và tách thêm file mới nếu triển khai RBAC, VPD, OLS, Audit hoặc Backup/Restore.

## Lỗi thường gặp

- `ORA-01017`: sai username/password hoặc dùng sai service. Thử kiểm tra lại password và service `xepdb1`.
- Không debug được breakpoint: đang chạy cấu hình `Release`; chuyển sang `Debug`.
- Không thấy user/role mới sau khi tạo: bấm lại nút view/search hoặc mở lại màn hình tương ứng.
- Lỗi quyền khi xem `DBA_USERS`, `DBA_ROLES`, `DBA_TAB_PRIVS`: tài khoản đăng nhập cần quyền DBA hoặc các quyền catalog tương ứng.