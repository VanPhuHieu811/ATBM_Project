ALTER SESSION SET CONTAINER = XEPDB1;

-- -------------------------------------------------------------------------
-- 1. VŨ KHÍ HẠT NHÂN: DỌN SẠCH 100% CÁC POLICY LỖI CŨ
-- -------------------------------------------------------------------------
BEGIN
    FOR rec IN (SELECT object_schema, object_name, policy_name FROM dba_audit_policies WHERE object_schema = 'ADMIN') LOOP
        DBMS_FGA.DROP_POLICY(rec.object_schema, rec.object_name, rec.policy_name);
    END LOOP;
END;
/

-- -------------------------------------------------------------------------
-- 2. THIẾT LẬP KIỂM TOÁN TIÊU CHUẨN (STANDARD AUDIT)
-- -------------------------------------------------------------------------
NOAUDIT ALL;
AUDIT SESSION WHENEVER NOT SUCCESSFUL;                               
AUDIT SELECT ON admin.BENHNHAN BY ACCESS;                            
AUDIT TABLE BY ACCESS;                                               
AUDIT GRANT ANY PRIVILEGE, GRANT ANY ROLE BY ACCESS;                 
AUDIT INSERT, UPDATE, DELETE ON admin.HSBA WHENEVER NOT SUCCESSFUL;  

-- -------------------------------------------------------------------------
-- 3. THIẾT LẬP KIỂM TOÁN CHI TIẾT (FINE-GRAINED AUDIT - FGA)
-- -------------------------------------------------------------------------
BEGIN
    -- a. FGA ĐƠN THUỐC (Bắt hành vi sửa Liều dùng)
    DBMS_FGA.ADD_POLICY(
        object_schema => 'ADMIN', object_name => 'DONTHUOC', policy_name => 'FGA_A_UPDATE_DONTHUOC', 
        audit_column => 'LIEUDUNG', audit_condition => NULL, statement_types => 'UPDATE', audit_trail => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- b. FGA HSBA HỢP PHÁP (Giám sát cập nhật vào cột Chẩn đoán)
    DBMS_FGA.ADD_POLICY(
        object_schema => 'ADMIN', object_name => 'HSBA', policy_name => 'FGA_B_BS_UPDATE_HSBA', 
        audit_column => 'CHANDOAN', audit_condition => NULL, statement_types => 'UPDATE', audit_trail => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- c. FGA HSBA BẤT HỢP PHÁP (Giám sát kẻ gian cố tình sửa Kết luận)
    DBMS_FGA.ADD_POLICY(
        object_schema => 'ADMIN', object_name => 'HSBA', policy_name => 'FGA_C_ILLEGAL_UPDATE_HSBA', 
        audit_column => 'KETLUAN', audit_condition => NULL, statement_types => 'UPDATE', audit_trail => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- d. FGA HSBA_DV BẤT HỢP PHÁP (Giám sát thao tác trái phép trên Kết quả Dịch vụ)
    DBMS_FGA.ADD_POLICY(
        object_schema => 'ADMIN', object_name => 'HSBA_DV', policy_name => 'FGA_D_ILLEGAL_DML_HSBADV', 
        audit_column => 'KETQUA', audit_condition => NULL, statement_types => 'INSERT, UPDATE, DELETE', audit_trail => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );
END;
/

GRANT SELECT ON DBA_AUDIT_TRAIL TO admin;
GRANT SELECT ON DBA_FGA_AUDIT_TRAIL TO admin;
COMMIT;