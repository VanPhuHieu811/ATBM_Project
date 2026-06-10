# README_HIEU - Phân hệ 2

File này mô tả phần việc của Hiệu: đăng nhập chung, điều hướng theo vai trò, giao diện Bác sĩ/Y sĩ và các điểm tích hợp với thành viên khác.

Trạng thái hiện tại: **chế độ demo giao diện**, chưa cần kết nối Oracle. Dữ liệu hiển thị trên UI là dữ liệu mẫu trong code để có thể bấm xem và thao tác trước.

## 1. Những gì đã làm

### Đăng nhập chung

Đã cập nhật `Views/FormLogin.cs`.

Luồng xử lý:

1. Người dùng nhập username/password bất kỳ.
2. Ứng dụng lưu username vào `DBConfig` để hiển thị trên UI.
3. Ứng dụng không gọi Oracle và không truy vấn database.
4. App điều hướng demo theo username:
   - `DBA`, `ADMIN`, `SYS`, `SYSTEM` -> `FormMain` của phân hệ quản trị.
   - `NV005`, `NV009`, `BS`, `BACSI` -> `FormDoctorMain`.
   - `NV001`, `DPV`, `DIEUPHOI` -> `FormCoordinatorMain`.
   - `NV015`, `KTV` -> `FormTechnicianMain`.
   - `BN001`, `BN...`, `BN` -> `FormPatientMain`.

Password demo mặc định có thể nhập `123`. Form chỉ kiểm tra có nhập password, không xác thực với Oracle.

Các form Điều phối viên, Kỹ thuật viên, Bệnh nhân hiện là form demo/placeholder để sau này gắn phần của thành viên khác.

### Xác định vai trò

File: `Presenters/SessionPresenter.cs`.

Ý nghĩa:

- Đây là lớp đã chuẩn bị cho hướng kết nối Oracle thật.
- Hiện tại `FormLogin` không gọi lớp này trong chế độ demo.
- Khi muốn quay lại logic database, có thể sửa `FormLogin.ResolveNextForm()` để dùng lại `SessionPresenter`.

### Giao diện Bác sĩ/Y sĩ

File mới: `Views/FormDoctorMain.cs`.

Ý nghĩa:

- Đây là màn hình chính cho bác sĩ/y sĩ.
- Hiển thị danh sách `HSBA` dạng rút gọn bằng dữ liệu mẫu: mã HSBA, mã/tên bệnh nhân, ngày, mã khoa và nút xem chi tiết.
- Chưa lọc bằng VPD vì chưa kết nối Oracle.
- Sau này khi có DB/VPD, chỉ cần thay logic trong `DoctorPresenter`.
- Có thanh tìm kiếm phía trên để lọc theo mã HSBA, mã/tên bệnh nhân, CCCD, chẩn đoán, điều trị, bác sĩ hoặc khoa.
- Bảng danh sách có cột `Xem chi tiết`; bấm vào sẽ chuyển sang trang chi tiết trong cùng cửa sổ.
- Trang chi tiết có nút `Quay lại` để trở về danh sách.

File mới: `Views/FormDoctorRecordDetail.cs`.

Form chi tiết có 4 tab:

- Tab `Hồ sơ`
  - Xem thông tin HSBA.
  - Chỉ cho sửa `CHANDOAN`, `DIEUTRI`, `KETLUAN`.
  - Các trường mã, ngày, bác sĩ, khoa là read-only.

- Tab `Dịch vụ`
  - Xem danh sách `HSBA_DV`.
  - Cho thêm dịch vụ.
  - Cho xóa dịch vụ.

- Tab `Đơn thuốc`
  - Xem danh sách `DONTHUOC`.
  - Cho thêm thuốc.
  - Cho sửa `LIEUDUNG`.
  - Cho xóa thuốc.

- Tab `Bệnh nhân`
  - Xem thông tin bệnh nhân liên quan đến HSBA.
  - Khóa thông tin hành chính như mã, tên, phái, ngày sinh, CCCD, địa chỉ.
  - Chỉ cho sửa `TIENSUBENH`, `TIENSUBENHGD`, `DIUNGTHUOC`.

### Logic dữ liệu cho Bác sĩ/Y sĩ

File: `Presenters/DoctorPresenter.cs`.

Ý nghĩa:

- Chứa toàn bộ dữ liệu mẫu và thao tác mẫu cho màn Bác sĩ/Y sĩ.
- Form không tự viết SQL trực tiếp.
- Các thao tác chính:
  - `GetMedicalRecords()`
  - `GetMedicalRecord()`
  - `UpdateMedicalRecord()`
  - `GetServices()`
  - `AddService()`
  - `DeleteService()`
  - `GetPrescriptions()`
  - `AddPrescription()`
  - `UpdatePrescriptionDose()`
  - `DeletePrescription()`
  - `GetPatientByMedicalRecord()`
  - `UpdatePatientHistory()`

Trong chế độ hiện tại, các thao tác này cập nhật dữ liệu mẫu trong bộ nhớ. Khi đóng app thì dữ liệu sẽ mất. Sau này muốn nối Oracle thì thay nội dung các method này bằng query thật.

### Placeholder cho vai trò khác

File mới: `Views/FormRolePlaceholders.cs`.

Trong file này có:

- `FormCoordinatorMain`
- `FormTechnicianMain`
- `FormPatientMain`
- `FormRolePlaceholder`

Ý nghĩa:

- Giữ sẵn điểm điều hướng đúng vai trò.
- Khi Huy/Cường hoàn thành form thật, chỉ cần thay nội dung các class này hoặc đổi route trong `FormLogin`.

### Project file

Đã cập nhật `ATBM_Project.csproj` để compile các file mới:

- `Presenters/SessionPresenter.cs`
- `Presenters/DoctorPresenter.cs`
- `Views/FormDoctorMain.cs`
- `Views/FormDoctorRecordDetail.cs`
- `Views/FormRolePlaceholders.cs`

## 2. Điều kiện Oracle cần có

Để xem demo UI hiện tại, **không cần Oracle user thật**.

Tài khoản demo đề xuất:

- Bác sĩ/Y sĩ: `NV005` / `123`
- Bác sĩ/Y sĩ khác: `NV009` / `123`
- Điều phối viên: `NV001` / `123`
- Kỹ thuật viên: `NV015` / `123`
- Bệnh nhân: `BN001` / `123`
- Quản trị: `ADMIN` / `123`

Khi chuyển sang logic Oracle thật, mỗi user đăng nhập phải là Oracle user thật.

Quy ước nên dùng:

- Username nhân viên = `MANV`, ví dụ `NV009`.
- Username bệnh nhân = `MABN`, ví dụ `BN001`.

`Resources/Database.sql` đã được bổ sung block sinh dữ liệu theo quy mô đề bài:

- Tổng cộng 20 điều phối viên.
- Tổng cộng 100 bác sĩ/y sĩ.
- Tổng cộng 50 kỹ thuật viên.
- Tổng cộng 100000 bệnh nhân.
- Tạo Oracle account với username là `MANV`/`MABN`.
- Password mặc định của các account sinh tự động là `"123"`.

Trong block seed có biến:

```sql
c_create_patient_accounts CONSTANT BOOLEAN := TRUE;
```

Nếu máy chạy chậm hoặc chỉ cần demo nhanh, có thể đổi thành `FALSE` để không tạo đủ 100000 Oracle account bệnh nhân. Dữ liệu bệnh nhân vẫn được sinh đủ; chỉ phần account bệnh nhân sẽ được bỏ qua.

Khi nối lại database, user bác sĩ/y sĩ cần có quyền tối thiểu:

- `CREATE SESSION`
- `SELECT` trên `HSBA`
- `UPDATE (CHANDOAN, DIEUTRI, KETLUAN)` trên `HSBA`
- `SELECT`, `INSERT`, `DELETE` trên `HSBA_DV`
- `SELECT`, `INSERT`, `UPDATE`, `DELETE` trên `DONTHUOC`
- `SELECT` trên `BENHNHAN`
- `UPDATE (TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC)` trên `BENHNHAN`

Phần giới hạn dòng dữ liệu phải do VPD xử lý ở Oracle. Code C# demo hiện tại không thay thế VPD.

## 3. Cách demo phần của Hiệu

1. Build và chạy app.
2. Ở màn đăng nhập, nhập `NV005` và password `123`.
3. App sẽ mở màn hình `Danh sách hồ sơ bệnh án`.
4. Có thể nhập từ khóa ở ô tìm kiếm để lọc theo bệnh nhân/hồ sơ.
5. Bấm nút `Xem chi tiết` ở dòng cần xem.
6. App chuyển sang trang chi tiết trong cùng cửa sổ để demo 4 tab:
   - sửa chẩn đoán/điều trị/kết luận
   - thêm/xóa dịch vụ
   - thêm/sửa/xóa đơn thuốc
   - sửa tiền sử bệnh/dị ứng của bệnh nhân
7. Bấm `Quay lại` để trở về danh sách.
8. Quay lại màn login và thử `NV001`, `NV015`, `BN001` để xem luồng điều hướng vai trò khác.

## 4. Việc còn cần phối hợp

- Cường cần hoàn thành VPD để danh sách HSBA tự lọc theo `MABS = USER` khi nối DB thật.
- Huy cần hoàn thành RBAC/view cho Kỹ thuật viên và Bệnh nhân.
- Khi Cường/Huy có form thật, thay placeholder trong `Views/FormRolePlaceholders.cs`.
- Kiệt cần hoàn thành audit để các thao tác update của bác sĩ được ghi vết.

## 5. Ghi chú kỹ thuật

- `FormLogin` vẫn hiển thị host/port/service nhưng đang để read-only vì chưa dùng DB.
- Nếu đăng nhập bằng `ADMIN`, app đi vào phân hệ quản trị cũ.
- Nếu username không khớp vai trò demo, app mặc định mở màn Bác sĩ/Y sĩ để bạn vẫn xem được UI.
- Khi nối Oracle thật, cần sửa lại `FormLogin.ResolveNextForm()` để dùng `SessionPresenter`.
