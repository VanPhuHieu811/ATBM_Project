# 🚀 START HERE - BẮT ĐẦU TỪ ĐÂY!

> **Hệ thống Thông báo OLS (Oracle Label Security) - Hoàn thành 100%**

---

## 📌 Bạn đang ở đâu?

Bạn vừa nhận được một **hệ thống quản lý thông báo bệnh viện** sử dụng Oracle Label Security.

Hệ thống này cho phép:
- ✅ **Admin tạo thông báo** với các nhãn bảo mật khác nhau
- ✅ **Tự động lọc** thông báo dựa trên quyền của từng user
- ✅ **Phân cấp truy cập**: Nhân viên → Lãnh đạo → Ban giám đốc

---

## ⚡ 3 BƯỚC CHẠY NHANH

### **BƯỚC 1: Chạy Script Oracle (5-10 phút)**

```bash
File: Resources\3.OLS_Setup.sql
Chương trình: SQL Developer
Account: SYS (SYSDBA)
```

**Cách làm:**
1. Mở **SQL Developer**
2. **New Connection** → Username: `SYS`, Password: `[nhập pass]`, Service Name: `XEPDB1`, Role: `SYSDBA`
3. **File → Open** → `Resources\3.OLS_Setup.sql`
4. **Ctrl+A** (chọn tất cả) → **Ctrl+Enter** (chạy)
5. Chờ hoàn thành
6. Kiểm tra: `SELECT COUNT(*) FROM ADMIN.THONGBAO;` → Kết quả: `7` ✓

### **BƯỚC 2: Build Project C# (1 phút)**

```bash
File: ATBM_Project.sln
Chương trình: Visual Studio
```

**Cách làm:**
1. Mở **Visual Studio**
2. **File → Open Solution** → `ATBM_Project.sln`
3. **Build → Build Solution** (Ctrl+Shift+B)
4. Chờ: `Build succeeded` ✓

### **BƯỚC 3: Chạy & Test (5 phút)**

```bash
Visual Studio: F5
```

**Cách làm:**
1. Nhấn **F5** (hoặc Debug → Start Debugging)
2. **Login ADMIN / 1234**
3. Sidebar: **"Quản lý thông báo"** → Tạo thông báo
4. **Logout** → **Login U1_BGD / OLS123**
5. Xem danh sách thông báo → Thấy tất cả ✓
6. **Logout** → **Login U8_NV / OLS123**
7. Xem danh sách thông báo → Thấy ít hơn ✓

**→ Đó là OLS đang hoạt động!**

---

## 📚 Tài liệu - Nên đọc theo thứ tự này:

1. **📄 FINAL_SUMMARY.txt** (5 phút)
   - Tóm tắt toàn bộ dự án
   - Username/password
   - Kiến trúc OLS

2. **📄 IMPLEMENTATION_COMPLETE.md** (10 phút)
   - Chi tiết kỹ thuật
   - Danh sách file tạo/sửa

3. **📄 README_OLS_QUICK.md** (khi cần)
   - Quick reference
   - Tips & Tricks

4. **📄 README_OLS_Setup.md** (nếu có lỗi)
   - Hướng dẫn chi tiết 100%
   - Troubleshooting

---

## 🔑 Username & Password

### Admin (tạo thông báo):
```
Username: ADMIN
Password: 1234
```

### 8 User OLS Test (xem thông báo):
```
Username: U1_BGD | U2_LDK | U3_LDK | U4_NV | U5_NV | U6_LDK | U7_LDK | U8_NV
Password: OLS123 (tất cả)
```

---

## 📂 File cần biết

### 🔴 **CHẠY**
- `3.OLS_Setup.sql` → SQL Developer (lần đầu, 1 lần)
- `ATBM_Project.sln` → Visual Studio (Build)

### 🟡 **ĐỌC**
- `FINAL_SUMMARY.txt` → Tóm tắt
- `README_OLS_QUICK.md` → Quick ref
- `README_OLS_Setup.md` → Chi tiết

### 🟢 **CODE C#** (Đã viết xong)
- `Utilities/OracleHelper.cs` → Helper class
- `Views/FormThongBaoManagement.cs` → Form tạo TB
- `Views/FormThongBao.cs` → Form xem TB
- `Views/FormLogin.cs` (updated)
- `Views/FormMain.cs` (updated)

---

## ✅ Checklist trước khi bắt đầu

- [ ] Kiểm tra Oracle Database chạy tại `localhost:1521`
- [ ] Biết mật khẩu tài khoản `SYS`
- [ ] Đã cài SQL Developer
- [ ] Đã cài Visual Studio
- [ ] Đã clone project từ GitHub

---

## 🎯 Quy trình chi tiết

```
1. [SQL Developer] Chạy 3.OLS_Setup.sql
   ├─ GRANT INHERIT PRIVILEGES
   ├─ GRANT OLS permissions
   ├─ CREATE POLICY HOSPITAL_TB_POL
   ├─ CREATE LEVELS, COMPARTMENTS, GROUPS
   ├─ CREATE DATA LABELS
   ├─ CREATE 8 USERS (U1_BGD → U8_NV)
   ├─ SET USER LABELS
   ├─ CREATE FUNCTION FN_BUILD_LABEL
   ├─ CREATE PROCEDURE SP_INSERT_THONGBAO
   ├─ CREATE PROCEDURE SP_GET_THONGBAO
   └─ INSERT 7 test data

2. [Visual Studio] Build ATBM_Project.sln
   ├─ Compile OracleHelper.cs
   ├─ Compile FormThongBaoManagement.cs
   ├─ Compile FormThongBao.cs
   ├─ Update FormLogin.cs
   ├─ Update FormMain.cs
   └─ Build succeeded ✓

3. [WinForm] F5 (Run)
   ├─ FormLogin hiển thị
   ├─ Input ADMIN / 1234
   ├─ FormMain mở (Sidebar với "Quản lý thông báo")
   └─ Click button

4. [FormThongBaoManagement] Tạo thông báo
   ├─ Điền nội dung
   ├─ Chọn cấp bậc/khoa/cơ sở
   ├─ Xem nhãn OLS preview
   ├─ Click "Gửi"
   └─ Success message

5. [FormLogin] Logout & Login U1_BGD
   └─ FormThongBao mở

6. [FormThongBao] Xem thông báo
   ├─ Hiển thị: "Xin chào U1_BGD — 8 thông báo"
   ├─ DataGridView: Toàn bộ 7 TB + 1 vừa tạo = 8
   └─ ✓ OLS lọc đúng

7. [FormLogin] Logout & Login U8_NV
   └─ FormThongBao mở

8. [FormThongBao] Xem thông báo
   ├─ Hiển thị: "Xin chào U8_NV — X thông báo"
   ├─ DataGridView: Chỉ TB phù hợp với "NV:C_TH:G_HN"
   └─ ✓ OLS lọc đúng (ít hơn U1_BGD)
```

---

## 🆘 Nếu gặp vấn đề

### ❌ Build lỗi C#
→ Xem **Output** tab trong Visual Studio

### ❌ Không kết nối Oracle
→ Kiểm tra: `localhost:1521/XEPDB1` có chạy?

### ❌ SP không tìm thấy
→ Chạy lại phần **BƯỚC 11** trong `3.OLS_Setup.sql`

### ❌ User không thấy thông báo
→ Xem **README_OLS_Setup.md** → Troubleshooting

---

## 💡 Mẹo

1. **Muốn xem nhãn của thông báo?**
   ```sql
   SELECT MATB, LBACSYS.LABEL_TO_CHAR('HOSPITAL_TB_POL', OLS_LABEL) AS Label
   FROM ADMIN.THONGBAO;
   ```

2. **Muốn reset toàn bộ?**
   ```sql
   DELETE FROM ADMIN.THONGBAO;
   COMMIT;
   ```

3. **Muốn thêm user khác?**
   - Sửa `FormLogin.cs` → Thêm điều kiện `ResolveNextForm()`

---

## 🎓 Tìm hiểu thêm

**OLS là gì?**
- Cơ chế bảo mật ở mức database
- Mỗi dòng có nhãn, mỗi user có quyền nhãn
- User chỉ thấy dòng có nhãn ≤ quyền của họ
- Tự động, không cần logic phức tạp

**Trong dự án này:**
- Policy: `HOSPITAL_TB_POL`
- Levels: NV (10) < LDK (20) < BGD (30)
- Compartments: C_TH, C_TK, C_TM
- Groups: G_HCM, G_HP, G_HN
- Label ví dụ: `NV:C_TH:G_HCM` (Nhân viên Tiêu Hóa TP HCM)

---

## 🚀 Bắt đầu ngay!

```bash
1. Mở SQL Developer → Chạy 3.OLS_Setup.sql
   ⏱ Mất 5-10 phút

2. Mở Visual Studio → Build project
   ⏱ Mất 1 phút

3. Nhấn F5 → Test hệ thống
   ⏱ Mất 5 phút

⏱ Tổng cộng: ~15-20 phút
```

---

## 📞 Liên hệ / Cần giúp?

- **SQL lỗi** → Xem `README_OLS_Setup.md`
- **C# lỗi** → Xem Output window
- **Quên password** → ADMIN=1234, U*=OLS123
- **Quên username** → Xem `FINAL_SUMMARY.txt`

---

## ✨ Đặc biệt cảm ơn

Dự án này được xây dựng với:
- ✅ Oracle 12c+ OLS
- ✅ C# .NET Framework 4.7.2
- ✅ Windows Forms
- ✅ PL/SQL

---

**🎉 Chúc bạn thành công!**

Hãy bắt đầu với **BƯỚC 1** phía trên. 

→ Nếu có câu hỏi, đọc **FINAL_SUMMARY.txt** hoặc **README_OLS_Setup.md**

---

**Made with ❤️ for Hospital Notification System**

Last updated: 2025
