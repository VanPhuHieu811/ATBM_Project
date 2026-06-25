ALTER SESSION SET CONTAINER = XEPDB1;

-- xoa cac policy cu truoc do
BEGIN
    FOR rec IN (SELECT object_schema, object_name, policy_name FROM dba_audit_policies WHERE object_schema = 'ADMIN') LOOP
        DBMS_FGA.DROP_POLICY(rec.object_schema, rec.object_name, rec.policy_name);
    END LOOP;
END;
/

-- standard audit
NOAUDIT ALL; -- tat cac audit mac dinh

-- giam sat dang nhap khong thanh cong (chong brute-force)
AUDIT SESSION WHENEVER NOT SUCCESSFUL;

-- giam sat lenh select tren bang benhnhan (lay thong tin)
AUDIT ALL ON admin.BENHNHAN BY ACCESS;

-- giam sat toan ven cau truc du lieu (them, xoa bang, them, xoa cot)
AUDIT TABLE BY ACCESS;
AUDIT ALTER TABLE BY ACCESS;

-- giam sat leo thang dac quyen
AUDIT SYSTEM GRANT BY ACCESS;

-- giam sat thao tac loi tren hsba 
AUDIT INSERT, UPDATE, DELETE ON admin.HSBA WHENEVER NOT SUCCESSFUL;

-- fine-grained audit
BEGIN
    -- Hành vi cập nhật trên thuộc tính MÃHSBA, NGÀYĐT, TÊNTHUỐC, LIỀUDÙNG của quan hệ ĐƠNTHUỐC của y sĩ/ bác sĩ điều trị sau khi đơn thuốc đã được chỉ định (được tạo xong).
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'DONTHUOC',
        policy_name     => 'FGA_A_BS_UPDATE_DONTHUOC',
        audit_column    => 'MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG',
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') = (SELECT h.MABS FROM ADMIN.HSBA h WHERE h.MAHSBA = MAHSBA)',
        statement_types => 'UPDATE',
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- Hành vi của người dùng thuộc vai trò “Y sĩ / Bác sĩ” đã cập nhật thành công trên các trường CHẨNĐOÁN, ĐIỀUTRỊ, KẾTLUẬN của hồ sơ bệnh án (HSBA) mà y sĩ/ bác sĩ đó điều trị.
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'HSBA',
        policy_name     => 'FGA_B_BS_UPDATE_HSBA',
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN',
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') = MABS',
        statement_types => 'UPDATE',
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- Hành vi của người dùng cập nhật bất hợp pháp trên các trường CHẨNĐOÁN, ĐIỀUTRỊ, KẾTLUẬN.
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'HSBA',
        policy_name     => 'FGA_C_ILLEGAL_UPDATE_HSBA',
        audit_column    => 'CHANDOAN, DIEUTRI, KETLUAN',
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') != MABS OR MABS IS NULL',
        statement_types => 'UPDATE',
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );

    -- Hành vi thêm, xóa, sửa bất hợp pháp trên quan hệ HSBA_DV.
    DBMS_FGA.ADD_POLICY(
        object_schema   => 'ADMIN',
        object_name     => 'HSBA_DV',
        policy_name     => 'FGA_D_ILLEGAL_DML_HSBADV',
        audit_column    => 'KETQUA',
        audit_condition => 'SYS_CONTEXT(''USERENV'', ''SESSION_USER'') != MAKTV OR MAKTV IS NULL',
        statement_types => 'INSERT, UPDATE, DELETE',
        audit_trail     => DBMS_FGA.DB + DBMS_FGA.EXTENDED
    );
END;
/

-- phan backup
-- tao directory object tro ve thu muc chua file backup
CREATE OR REPLACE DIRECTORY ATBM_DIR AS 'C:\ATBM_Backup';

-- cap quyen audit va backup cho admin
GRANT READ, WRITE ON DIRECTORY ATBM_DIR TO admin;
GRANT SELECT ON DBA_AUDIT_TRAIL TO admin;
GRANT SELECT ON DBA_FGA_AUDIT_TRAIL TO admin;

COMMIT;