ALTER SESSION SET CONTAINER = XEPDB1;

-- =========================================================================
-- 1. VŨ KHÍ HẠT NHÂN: DỌN SẠCH CHÍNH SÁCH FGA CŨ (NẾU CÓ)
-- =========================================================================
BEGIN
    FOR rec IN (SELECT object_schema, object_name, policy_name FROM dba_audit_policies WHERE object_schema = 'ADMIN') LOOP
        DBMS_FGA.DROP_POLICY(rec.object_schema, rec.object_name, rec.policy_name);
    END LOOP;
END;
/

-- =========================================================================
-- 2. THIẾT LẬP 5 CHÍNH SÁCH STANDARD AUDIT (GIÁM SÁT DIỆN RỘNG)
-- =========================================================================
NOAUDIT ALL; -- Tắt các audit mặc định rác của Oracle để tiết kiệm I/O

-- [1] Giám sát nỗ lực đăng nhập bất thường
AUDIT SESSION WHENEVER NOT SUCCESSFUL;                               

-- [2] Giám sát quyền riêng tư dữ liệu hành chính
AUDIT SELECT ON admin.BENHNHAN BY ACCESS;                            

-- [3] Giám sát toàn vẹn cấu trúc (CREATE, DROP, TRUNCATE, ALTER)
AUDIT TABLE BY ACCESS;  
AUDIT ALTER TABLE BY ACCESS;

-- [4] Giám sát hành vi leo thang đặc quyền
AUDIT SYSTEM GRANT BY ACCESS;                 

-- [5] Giám sát các nỗ lực thao tác trái phép/lỗi trên Hồ sơ bệnh án
AUDIT INSERT, UPDATE, DELETE ON admin.HSBA WHENEVER NOT SUCCESSFUL;  

-- =========================================================================
-- 3. THIẾT LẬP 4 CHÍNH SÁCH FINE-GRAINED AUDIT (GIÁM SÁT CHI TIẾT)
-- =========================================================================
BEGIN
    -- a. Cập nhật Đơn thuốc (Theo dõi cột Liều dùng)
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'DONTHUOC', 
        policy_name     => 'FGA_A_UPDATE_DONTHUOC', 
        audit_column    => 'LIEUDUNG', 
        audit_condition => NULL, 
        statement_types => 'UPDATE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- b. Bác sĩ cập nhật hợp pháp trên HSBA
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'HSBA', 
        policy_name     => 'FGA_B_BS_UPDATE_HSBA', 
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN', 
        audit_condition => 'USER = MABS', 
        statement_types => 'UPDATE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- c. Cập nhật bất hợp pháp trên HSBA (Kẻ gian / Không phải bác sĩ phụ trách)
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'HSBA', 
        policy_name     => 'FGA_C_ILLEGAL_UPDATE_HSBA', 
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN', 
        audit_condition => 'USER != MABS OR MABS IS NULL', 
        statement_types => 'UPDATE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- d. Thao tác bất hợp pháp trên Dịch vụ HSBA_DV
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'HSBA_DV', 
        policy_name     => 'FGA_D_ILLEGAL_DML_HSBADV', 
        audit_column    => 'KETQUA', 
        audit_condition => 'USER != MAKTV OR MAKTV IS NULL', 
        statement_types => 'INSERT, UPDATE, DELETE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );
END;
/

-- =========================================================================
-- 4. CẤU HÌNH HẠ TẦNG CHO PHÂN HỆ BACKUP & RECOVERY (DATA PUMP)
-- =========================================================================
-- Tạo Directory Object trỏ về thư mục chứa file Backup trên ổ C
CREATE OR REPLACE DIRECTORY ATBM_DIR AS 'C:\ATBM_Backup';

-- =========================================================================
-- 5. CẤP QUYỀN TRUY CẬP CHO TÀI KHOẢN ADMIN (DBA APP)
-- =========================================================================
GRANT READ, WRITE ON DIRECTORY ATBM_DIR TO admin;
GRANT SELECT ON DBA_AUDIT_TRAIL TO admin;
GRANT SELECT ON DBA_FGA_AUDIT_TRAIL TO admin;

COMMIT;