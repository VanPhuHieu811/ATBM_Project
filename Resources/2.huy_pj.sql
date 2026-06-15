-- CHẠY BẰNG TÀI KHOẢN ADMIN HOẶC TÀI KHOẢN CÓ QUYỀN DBA
-- Xóa role
BEGIN
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_BENHNHAN';
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_KYTHUATVIEN';
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_NHANVIEN';
    EXCEPTION WHEN OTHERS THEN NULL;
END;
/
--===========================================================================   
-- BỆNH NHÂN
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

--===========================================================================
-- KĨ THUẬT VIÊN

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

--===========================================================================
--NHÂN VIÊN
-- 1. Tạo Base Role cho mọi nhân viên
CREATE ROLE ROLE_NHANVIEN;

-- 2. Tạo View bảo mật lấy thông tin của CHÍNH NHÂN VIÊN đang đăng nhập
CREATE OR REPLACE VIEW admin.V_NHANVIEN_PROFILE AS
SELECT 
    MANV, HOTEN, PHAI, TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS NGAYSINH, 
    CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA
FROM admin.NHANVIEN
WHERE MANV = SYS_CONTEXT('USERENV', 'SESSION_USER');

-- 3. Cấp quyền cho Role nhân viên
GRANT SELECT ON admin.V_NHANVIEN_PROFILE TO ROLE_NHANVIEN;
GRANT UPDATE (QUEQUAN, SODT) ON admin.V_NHANVIEN_PROFILE TO ROLE_NHANVIEN;

-- 4.Cấp Role nhân viên cho Role kỹ thuật viên
GRANT ROLE_NHANVIEN TO ROLE_KYTHUATVIEN;

--============================================================================
-- Cấp ROLE_KYTHUATVIEN cho từng kỹ thuật viên và cấp ROLE_BENHNHAN cho từng bệnh nhân
DECLARE
    v_sql VARCHAR2(200);
BEGIN
    -- Cấp ROLE_KYTHUATVIEN cho từng kỹ thuật viên
    FOR r IN (SELECT MANV FROM admin.NHANVIEN WHERE VAITRO = N'Kỹ thuật viên') LOOP
        BEGIN
            v_sql := 'GRANT ROLE_KYTHUATVIEN TO ' || r.MANV;
            EXECUTE IMMEDIATE v_sql;
        EXCEPTION
            WHEN OTHERS THEN
                IF SQLCODE != -1917 THEN 
                    RAISE; 
                END IF;
        END;
    END LOOP;

    -- Cấp ROLE_BENHNHAN cho từng bệnh nhân
    FOR r IN (SELECT MABN FROM admin.BENHNHAN) LOOP
        BEGIN
            v_sql := 'GRANT ROLE_BENHNHAN TO ' || r.MABN;
            EXECUTE IMMEDIATE v_sql;
        EXCEPTION
            WHEN OTHERS THEN
                IF SQLCODE != -1917 THEN 
                    RAISE; 
                END IF;
        END;
    END LOOP;
END;
/