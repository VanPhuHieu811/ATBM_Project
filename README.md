# 🏥 Dự án CSC12001 - An toàn và Bảo mật dữ liệu trong HTTT

Dự án tập trung vào Phân hệ 1: Ứng dụng Quản trị CSDL Oracle.

### 📁 Cấu trúc thư mục (Project Architecture)

| Thư mục | Mục đích sử dụng |
| :--- | :--- |
| **Config/** | Chứa cấu hình kết nối CSDL (`DBConfig.cs`). Đây là nơi duy nhất tương tác với Driver Oracle. |
| **Models/** | Định nghĩa các thực thể dữ liệu như `User`, `Role`, `NhanVien`, `BenhNhan`. |
| **Views/** | Chứa giao diện người dùng (Forms, UserControls). Chỉ tập trung vào hiển thị. |
| **Presenters/** | Cầu nối điều khiển: Lấy dữ liệu từ DB để hiển thị lên Form và ngược lại. |
| **Utilities/** | Các công cụ hỗ trợ: Định dạng chuỗi, xuất báo cáo, xử lý lỗi. |
| **Resources/** |  Lưu trữ tài liệu đồ án, icon và các script SQL (`.sql`). |

### 🛠 Quy trình và Quy định làm việc

1. **Kết nối DB**: Sử dụng duy nhất chuỗi kết nối trong `Data/DBConfig.cs` với tài khoản `admin`.
2. **Thêm chức năng**: 
   - Thiết kế UI tại **Views**.
   - Viết logic xử lý SQL tại **Presenters**.
   - Tuyệt đối không viết chuỗi kết nối trực tiếp trong Form.
3. **Quy định viết mã**:
   - **Không viết Comment**: Hạn chế tối đa việc viết comment vào mã nguồn theo chỉ dẫn.
   - **Xử lý ngoại lệ**: Mọi thao tác với Oracle phải nằm trong khối `try-catch`.
4. **Bảo mật**: Cấp quyền phải tuân thủ chính sách đóng. Quyền `SELECT` và `UPDATE` phải hỗ trợ phân quyền mức cột.

### 🚀 Danh sách chức năng Phân hệ 1
*  Tạo, xóa, hiệu chỉnh User/Role.
*  Xem danh sách tài khoản và Role.
*  Cấp quyền (User, Role, đối tượng CSDL) kèm tùy chọn `WITH GRANT OPTION`.
*  Thu hồi quyền.
*  Xem thông tin quyền trên đối tượng dữ liệu.