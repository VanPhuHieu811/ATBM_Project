Hướng dẫn cài đặt & Chạy thử (Testing)

Bước 1: Khởi tạo View và Phân quyền (Run on SQL Developer)
Chạy script `Resources/huy_pj.sql`  bằng tài khoản `admin` để thiết lập môi trường cho phân hệ.

Bước 2: Test Luồng Kỹ thuật viên (NV015)

	1.Đăng nhập ứng dụng với Username: NV015, Password: [Mật khẩu đã tạo].
	2. Hệ thống nhận diện role và điều hướng đến FormKTVMain.
	3.Kiểm tra danh sách dịch vụ: Chỉ thấy các dịch vụ do bác sĩ chỉ định cho NV015.
	4.Chọn một dịch vụ và nhấn Cập nhật kết quả. Nhập kết quả và nhấn Lưu.

Bước 3: Test Luồng Bệnh nhân (BN001)

	1. Đăng nhập ứng dụng với Username: BN001.
	2. Hệ thống điều hướng đến FormBenhNhanMain.
	3. Giao diện hiển thị hồ sơ cá nhân của chính bệnh nhân BN001.
	4. Nhấn Chỉnh sửa thông tin: Thử thay đổi địa chỉ hoặc tiền sử bệnh và nhấn Lưu. (Các ô CCCD, Họ tên không thể click/sửa).