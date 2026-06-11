-- CHẠY BẰNG TÀI KHOẢN ADMIN HOẶC TÀI KHOẢN CÓ QUYỀN DBA
--BỆNH NHÂN

-- 1. Tạo Vai trò (Role) cho Bệnh nhân
CREATE ROLE ROLE_BENHNHAN;

-- 2. Tạo View bảo mật lấy thông tin của CHÍNH BỆNH NHÂN đang đăng nhập
CREATE OR REPLACE VIEW admin.V_BENHNHAN_PROFILE AS
SELECT 
    MABN, TENBN, PHAI, NGAYSINH, CCCD, 
    SONHA, TENDUONG, QUANHUYEN, TINHTP, 
    TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC
FROM admin.BENHNHAN
WHERE MABN = SYS_CONTEXT('USERENV', 'SESSION_USER');

-- 3. Cấp quyền xem (SELECT) trên View cho Role
GRANT SELECT ON admin.V_BENHNHAN_PROFILE TO ROLE_BENHNHAN;

-- 4. Cấp quyền sửa (UPDATE) chi tiết trên các cột được phép
GRANT UPDATE (SONHA, TENDUONG, QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC) 
ON admin.V_BENHNHAN_PROFILE TO ROLE_BENHNHAN;

-- 5. Tạo user Oracle cho bệnh nhân BN001
 CREATE USER BN001 IDENTIFIED BY "1234";
 GRANT CREATE SESSION TO BN001;

-- Bước 6: Gán vai trò ROLE_BENHNHAN cho tài khoản BN001
GRANT ROLE_BENHNHAN TO BN001;

--===========================================================================
--NHÂN VIÊN

-- 1. Tạo View bảo mật lấy thông tin của CHÍNH NHÂN VIÊN đang đăng nhập
CREATE OR REPLACE VIEW admin.V_NHANVIEN_PROFILE AS
SELECT 
    MANV, HOTEN, PHAI, TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS NGAYSINH, 
    CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA
FROM admin.NHANVIEN
WHERE MANV = SYS_CONTEXT('USERENV', 'SESSION_USER');

-- 2. Cấp quyền cho Role Kỹ thuật viên
GRANT SELECT ON admin.V_NHANVIEN_PROFILE TO ROLE_KYTHUATVIEN;
GRANT UPDATE (QUEQUAN, SODT) ON admin.V_NHANVIEN_PROFILE TO ROLE_KYTHUATVIEN;
--===========================================================================
--KĨ THUẬT VIÊN

-- 1. Tạo Role Kỹ thuật viên 
CREATE ROLE ROLE_KYTHUATVIEN;

-- 2. Tạo View lọc dòng theo tài khoản đang đăng nhập
CREATE OR REPLACE VIEW admin.V_KTV_HSBA_DV AS
SELECT 
    MAHSBA, 
    LOAIDV, 
    NGAYDV, 
    MAKTV, 
    KETQUA
FROM admin.HSBA_DV
WHERE MAKTV = SYS_CONTEXT('USERENV', 'SESSION_USER');

-- 3. Cấp quyền SELECT trên View để KTV có thể đọc danh sách dịch vụ của mình
GRANT SELECT ON admin.V_KTV_HSBA_DV TO ROLE_KYTHUATVIEN;

-- 4. Cấp quyền UPDATE (chỉ định duy nhất cột KETQUA) trên View
GRANT UPDATE (KETQUA) ON admin.V_KTV_HSBA_DV TO ROLE_KYTHUATVIEN;

GRANT ROLE_KYTHUATVIEN TO NV015;