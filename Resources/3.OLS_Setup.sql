SET DEFINE OFF;

-- ============================================================
-- OLS_Setup.sql - Thiết lập Oracle Label Security cho hệ thống thông báo bệnh viện
-- Container: XEPDB1, Schema: ADMIN
--
-- HƯỚNG DẪN CHẠY (theo đúng thứ tự kết nối):
--
--   PHẦN A: Kết nối SYS as SYSDBA
--     - Bước 0: Cấp quyền INHERIT PRIVILEGES
--     - Bước 1: Tạo user ADMIN (nếu chưa có) và cấp quyền OLS
--
--   PHẦN B: Kết nối ADMIN (schema chính của hệ thống)
--     - Bước 2:  Tạo chính sách OLS
--     - Bước 3:  Tạo Level
--     - Bước 4:  Tạo Compartment
--     - Bước 5:  Tạo Group
--     - Bước 6:  Tạo Data Labels
--     - Bước 7:  Áp dụng policy lên bảng THONGBAO (NO_CONTROL trước)
--     - Bước 8:  Tạo 8 Oracle user demo
--     - Bước 9:  INSERT 7 thông báo mẫu (dùng UPDATE gán label thủ công)
--     - Bước 10: Đổi policy sang READ_CONTROL,WRITE_CONTROL
--     - Bước 11: UPDATE trigger để OLS nhận label
--     - Bước 12: Gán nhãn session cho 8 user demo (SET_USER_LABELS)
--     - Bước 13: Tạo hàm FN_BUILD_LABEL
--     - Bước 14: Tạo SP_INSERT_THONGBAO
--     - Bước 15: Tạo SP_GET_THONGBAO
--
-- ============================================================
---> BẬT OLS NẾU CHƯA BẬT/ CHƯA ĐĂNG KÝ THÌ ĐĂNG KÝ OLS VÀ BẬT OLS
EXEC LBACSYS.CONFIGURE_OLS;
EXEC LBACSYS.OLS_ENFORCEMENT.ENABLE_OLS;
---> KHỞI ĐỘNG LẠI
SHUTDOWN IMMEDIATE;
STARTUP
-- ============================================================
-- PHẦN A: Chạy bằng SYS as SYSDBA
-- ============================================================

ALTER SESSION SET CONTAINER = XEPDB1;

-- ============================================================
-- RESET POLICY CU CUA PROJECT TRUOC KHI TAO MOI
-- Chay tot nhat bang SYS AS SYSDBA. Neu dang chay bang ADMIN ma khong du quyen,
-- cac lenh reset se duoc bo qua va can don sach bang SYS truoc.
-- ============================================================
BEGIN
    SA_POLICY_ADMIN.REMOVE_TABLE_POLICY('HOSPITAL_TB_POL', 'ADMIN', 'THONGBAO');
EXCEPTION
    WHEN OTHERS THEN NULL;
END;
/

BEGIN
    SA_SYSDBA.DROP_POLICY('HOSPITAL_TB_POL', TRUE);
EXCEPTION
    WHEN OTHERS THEN NULL;
END;
/

BEGIN
    EXECUTE IMMEDIATE 'DROP ROLE HOSPITAL_TB_POL_DBA';
EXCEPTION
    WHEN OTHERS THEN NULL;
END;
/

-- ============================================================
-- BƯỚC 0: Cấp quyền INHERIT PRIVILEGES
-- ============================================================
GRANT INHERIT PRIVILEGES ON USER ADMIN TO LBACSYS;
/

-- ============================================================
-- BƯỚC 1: Cấp quyền OLS cho ADMIN
-- ============================================================
GRANT CONNECT, RESOURCE TO ADMIN;
GRANT UNLIMITED TABLESPACE TO ADMIN;
GRANT SELECT ANY DICTIONARY TO ADMIN;
GRANT EXECUTE ON SA_SESSION TO ADMIN;
GRANT EXECUTE ON LBACSYS.SA_COMPONENTS   TO ADMIN WITH GRANT OPTION;
GRANT EXECUTE ON LBACSYS.SA_USER_ADMIN   TO ADMIN WITH GRANT OPTION;
GRANT EXECUTE ON LBACSYS.SA_LABEL_ADMIN  TO ADMIN WITH GRANT OPTION;
GRANT EXECUTE ON SA_POLICY_ADMIN         TO ADMIN WITH GRANT OPTION;
GRANT EXECUTE ON LBACSYS.SA_SYSDBA       TO ADMIN WITH GRANT OPTION;
GRANT EXECUTE ON LBACSYS.TO_LBAC_DATA_LABEL TO ADMIN WITH GRANT OPTION;
GRANT EXECUTE ON CHAR_TO_LABEL           TO ADMIN WITH GRANT OPTION;
GRANT LBAC_DBA TO ADMIN;
/

-- ============================================================
-- PHẦN B: Chạy bằng ADMIN
-- ============================================================

-- ============================================================
-- BƯỚC 2: Tạo chính sách OLS (HOSPITAL_TB_POL)
-- ============================================================

BEGIN
    SA_SYSDBA.CREATE_POLICY(
        policy_name     => 'HOSPITAL_TB_POL',
        column_name     => 'OLS_LABEL',
        default_options => 'NO_CONTROL'
    );
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -12912 THEN RAISE; END IF; -- -12912: Policy already exists
END;
/
