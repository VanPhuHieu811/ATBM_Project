-- CHẠY BẰNG TÀI KHOẢN ADMIN HOẶC TÀI KHOẢN CÓ QUYỀN DBA
-- Xóa role
BEGIN
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_BENHNHAN';
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_KYTHUATVIEN';
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_NHANVIEN';
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_DIEUPHOIVIEN';
    EXECUTE IMMEDIATE 'DROP ROLE ROLE_BACSI';
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

-- tao role ROLE_DIEUPHOIVIEN
CREATE ROLE ROLE_DIEUPHOIVIEN;

-- Cấp ROLE_DIEUPHOIVIEN cho từng nhân viên điều phối viên
DECLARE
    v_sql VARCHAR2(200);
BEGIN
    FOR r IN (SELECT MANV FROM admin.NHANVIEN WHERE VAITRO = N'Điều phối viên') LOOP
        BEGIN
            v_sql := 'GRANT ROLE_DIEUPHOIVIEN TO ' || r.MANV;
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

-- tao role ROLE_BACSI
CREATE ROLE ROLE_BACSI;

-- Cap ROLE_BACSI cho cac nhan vien bac si/y si demo NV005 - NV014
DECLARE
    v_sql VARCHAR2(200);
BEGIN
    FOR r IN (
        SELECT MANV
        FROM admin.NHANVIEN
        WHERE VAITRO = N'Bác sĩ/Y sĩ'
          AND MANV BETWEEN 'NV005' AND 'NV014'
    ) LOOP
        BEGIN
            v_sql := 'GRANT ROLE_BACSI TO ' || r.MANV;
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


-- HSBA - Bác sĩ chỉ thấy/cập nhật HSBA mình phụ trách
CREATE OR REPLACE FUNCTION FN_VPD_HSBA_BACSI (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
)
RETURN VARCHAR2
AS
BEGIN
    RETURN 'USER IN (
                SELECT MANV
                FROM ADMIN.NHANVIEN
                WHERE VAITRO = N''Điều phối viên''
            )
            OR MABS = USER';
END;
/


-- BENHNHAN - Bac si thay benh nhan minh phu trach; benh nhan thay chinh ho so cua minh
CREATE OR REPLACE FUNCTION FN_VPD_BENHNHAN_BACSI (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
)
RETURN VARCHAR2
AS
BEGIN
    RETURN 'USER IN (
                SELECT MANV
                FROM ADMIN.NHANVIEN
                WHERE VAITRO = N''Điều phối viên''
            )
            OR MABN IN (
            SELECT MABN
            FROM ADMIN.HSBA
            WHERE MABS = USER
         )';
END;
/


-- HSBA_DV - Bác sĩ chỉ xem/thêm/xóa dịch vụ mà mình phụ trách
CREATE OR REPLACE FUNCTION FN_VPD_HSBA_DV_BACSI (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
)
RETURN VARCHAR2
AS
BEGIN
    RETURN 'USER IN (
                SELECT MANV
                FROM ADMIN.NHANVIEN
                WHERE VAITRO = N''Điều phối viên''
            )
            OR MAHSBA IN (
        SELECT MAHSBA
        FROM ADMIN.HSBA
        WHERE MABS = USER
    )';
END;
/

-- DONTHUOC - Bác sĩ chỉ thao tác đơn thuốc thuộc HSBA mà mình phụ trách
CREATE OR REPLACE FUNCTION FN_VPD_DONTHUOC_BACSI (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
)
RETURN VARCHAR2
AS
BEGIN
    RETURN 'USER IN (
                SELECT MANV
                FROM ADMIN.NHANVIEN
                WHERE VAITRO = N''Điều phối viên''
            )
            OR MAHSBA IN (
        SELECT MAHSBA
        FROM ADMIN.HSBA
        WHERE MABS = USER
    )';
END;
/



--- POLICY
-- 1. Bác sĩ chỉ xem các hồ sơ bệnh án mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'HSBA',
        policy_name     => 'POL_HSBA_SELECT_BACSI',
        function_schema => 'ADMIN',
        policy_function => 'FN_VPD_HSBA_BACSI',
        statement_types => 'SELECT'
    );
END;
/


-- 2. Bác sĩ chỉ cập nhật CHANDOAN, DIEUTRI, KETLUAN trên HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'HSBA',
        policy_name     => 'POL_HSBA_UPDATE_BACSI',
        function_schema => 'ADMIN',
        policy_function => 'FN_VPD_HSBA_BACSI',
        statement_types => 'UPDATE',
        update_check    => TRUE
    );
END;
/

-- 3. Bác sĩ chỉ xem/thêm/xóa dịch vụ hỗ trợ chẩn đoán thuộc HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'HSBA_DV',
        policy_name     => 'POL_HSBA_DV_BACSI',
        function_schema => 'ADMIN',
        policy_function => 'FN_VPD_HSBA_DV_BACSI',
        statement_types => 'SELECT, INSERT, DELETE',
        update_check    => TRUE
    );
END;
/

-- 4. Bác sĩ chỉ xem bệnh nhân thuộc HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'BENHNHAN',
        policy_name     => 'POL_BENHNHAN_SELECT_BACSI',
        function_schema => 'ADMIN',
        policy_function => 'FN_VPD_BENHNHAN_BACSI',
        statement_types => 'SELECT'
    );
END;
/

-- 5. Bác sĩ chỉ cập nhật TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC của bệnh nhân mình điều trị
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'BENHNHAN',
        policy_name     => 'POL_BENHNHAN_UPDATE_BACSI',
        function_schema => 'ADMIN',
        policy_function => 'FN_VPD_BENHNHAN_BACSI',
        statement_types => 'UPDATE',
        update_check    => TRUE
    );
END;
/


-- 6. Bác sĩ chỉ thao tác đơn thuốc thuộc HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'DONTHUOC',
        policy_name     => 'POL_DONTHUOC_BACSI',
        function_schema => 'ADMIN',
        policy_function => 'FN_VPD_DONTHUOC_BACSI',
        statement_types => 'SELECT, INSERT, UPDATE, DELETE',
        update_check    => TRUE
    );
END;
/
