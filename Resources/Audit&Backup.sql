-- =========================================================================
-- SCRIPT CẤU HÌNH HẠ TẦNG AN TOÀN BẢO MẬT HỆ THỐNG
-- Mục đích: Khởi tạo môi trường Backup/Restore và thiết lập Audit
-- Yêu cầu: CHẠY SCRIPT NÀY BẰNG TÀI KHOẢN SYS (ROLE: SYSDBA)
-- LƯU Ý: Đảm bảo đã tạo thư mục C:\ATBM_Backup trên máy tính trước khi chạy
-- =========================================================================

-- 1. Trỏ phiên làm việc vào Pluggable Database của đồ án
ALTER SESSION SET CONTAINER = XEPDB1;

-- -------------------------------------------------------------------------
-- PHẦN I: THIẾT LẬP HẠ TẦNG SAO LƯU & CỨU HỘ (DATA PUMP & FLASHBACK)
-- -------------------------------------------------------------------------
CREATE OR REPLACE DIRECTORY ATBM_DIR AS 'C:\ATBM_Backup';

GRANT READ, WRITE ON DIRECTORY ATBM_DIR TO admin;
GRANT DATAPUMP_EXP_FULL_DATABASE TO admin;
GRANT DATAPUMP_IMP_FULL_DATABASE TO admin;
GRANT FLASHBACK ANY TABLE TO admin;

ALTER SESSION SET CURRENT_SCHEMA = admin;


-- -------------------------------------------------------------------------
-- PHẦN II: THIẾT LẬP KIỂM TOÁN TIÊU CHUẨN (STANDARD AUDIT)
-- -------------------------------------------------------------------------
NOAUDIT ALL;

AUDIT SESSION WHENEVER NOT SUCCESSFUL;                               
AUDIT SELECT ON admin.BENHNHAN BY ACCESS;                            
AUDIT TABLE BY ACCESS;                                               
AUDIT GRANT ANY PRIVILEGE, GRANT ANY ROLE BY ACCESS;                 
AUDIT INSERT, UPDATE, DELETE ON admin.HSBA WHENEVER NOT SUCCESSFUL;  


-- -------------------------------------------------------------------------
-- PHẦN III: THIẾT LẬP KIỂM TOÁN CHI TIẾT (FINE-GRAINED AUDIT - FGA)
-- -------------------------------------------------------------------------
BEGIN
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN', 'DONTHUOC', 'FGA_AUDIT_UPDATE_DONTHUOC'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN', 'HSBA', 'FGA_AUDIT_BS_UPDATE_HSBA'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN', 'HSBA', 'FGA_AUDIT_ILLEGAL_UPDATE_HSBA'); EXCEPTION WHEN OTHERS THEN NULL; END;
    BEGIN DBMS_FGA.DROP_POLICY('ADMIN', 'HSBA_DV', 'FGA_AUDIT_ILLEGAL_DML_HSBADV'); EXCEPTION WHEN OTHERS THEN NULL; END;

    -- a. Hành vi cập nhật trên thuộc tính MÃHSBA, NGÀYĐT, TÊNTHUỐC, LIỀUDÙNG của quan hệ ĐƠNTHUỐC
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'DONTHUOC', 
        policy_name     => 'FGA_AUDIT_UPDATE_DONTHUOC', 
        audit_column    => 'MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG', 
        audit_condition => '1=1', 
        statement_types => 'UPDATE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- b. Hành vi Y sĩ / Bác sĩ cập nhật thành công HSBA mà họ điều trị
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'HSBA', 
        policy_name     => 'FGA_AUDIT_BS_UPDATE_HSBA', 
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN', 
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') = MABS', 
        statement_types => 'UPDATE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- c. Hành vi cập nhật bất hợp pháp trên HSBA (Người sửa không phải bác sĩ phụ trách)
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'HSBA', 
        policy_name     => 'FGA_AUDIT_ILLEGAL_UPDATE_HSBA', 
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN', 
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') != MABS', 
        statement_types => 'UPDATE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- d. Hành vi thêm, xóa, sửa bất hợp pháp trên quan hệ HSBA_DV
    -- (Giả định nhân viên không có quyền (không phải chủ hồ sơ/kỹ thuật viên) cố tình thao tác)
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN', 
        object_name     => 'HSBA_DV', 
        policy_name     => 'FGA_AUDIT_ILLEGAL_DML_HSBADV', 
        audit_column    => NULL, 
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') != MABS', 
        statement_types => 'INSERT, UPDATE, DELETE', 
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );
END;
/

-- -------------------------------------------------------------------------
-- PHẦN IV: CẤP QUYỀN ĐỌC VIEW HỆ THỐNG CHO ỨNG DỤNG WINFORMS
-- -------------------------------------------------------------------------
GRANT SELECT ON DBA_AUDIT_TRAIL TO admin;
GRANT SELECT ON DBA_FGA_AUDIT_TRAIL TO admin;

COMMIT;