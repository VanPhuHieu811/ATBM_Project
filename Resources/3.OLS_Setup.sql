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

-- Cap quyen quan tri policy cho ADMIN ngay sau khi tao policy.
-- Neu de den cuoi file, ADMIN co the bi ORA-12446 khi apply/set labels.
BEGIN
    SA_USER_ADMIN.SET_USER_PRIVS(
        policy_name => 'HOSPITAL_TB_POL',
        user_name   => 'ADMIN',
        privileges  => 'FULL'
    );
END;
/

-- Enable policy
BEGIN
    SA_SYSDBA.ENABLE_POLICY('HOSPITAL_TB_POL');
END;
/

-- ============================================================
-- BƯỚC 3: Tạo Level (từ thấp đến cao)
-- ============================================================
BEGIN SA_COMPONENTS.CREATE_LEVEL('HOSPITAL_TB_POL', 10, 'NV',  'Nhan Vien');    EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_COMPONENTS.CREATE_LEVEL('HOSPITAL_TB_POL', 20, 'LDK', 'Lanh Dao Khoa'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_COMPONENTS.CREATE_LEVEL('HOSPITAL_TB_POL', 30, 'BGD', 'Ban Giam Doc');  EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- ============================================================
-- BƯỚC 4: Tạo Compartment (khoa)
-- ============================================================
BEGIN SA_COMPONENTS.CREATE_COMPARTMENT('HOSPITAL_TB_POL', 10, 'C_TH', 'Khoa Tieu Hoa');  EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_COMPONENTS.CREATE_COMPARTMENT('HOSPITAL_TB_POL', 20, 'C_TK', 'Khoa Than Kinh'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_COMPONENTS.CREATE_COMPARTMENT('HOSPITAL_TB_POL', 30, 'C_TM', 'Khoa Tim Mach');  EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- ============================================================
-- BƯỚC 5: Tạo Group (cơ sở địa phương)
-- ============================================================
BEGIN SA_COMPONENTS.CREATE_GROUP('HOSPITAL_TB_POL', 10, 'G_HCM', 'Ho Chi Minh'); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_COMPONENTS.CREATE_GROUP('HOSPITAL_TB_POL', 20, 'G_HP',  'Hai Phong');   EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_COMPONENTS.CREATE_GROUP('HOSPITAL_TB_POL', 30, 'G_HN',  'Ha Noi');      EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- ============================================================
-- BƯỚC 6: Tạo Data Labels
-- ============================================================

-- t1: Toàn thể nhân viên
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1001, 'NV',                    TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
-- t2: Ban giám đốc
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1002, 'BGD',                   TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
-- t3: Tất cả lãnh đạo khoa
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1003, 'LDK',                   TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
-- t4: LĐ Khoa Tiêu Hóa
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1004, 'LDK:C_TH',              TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
-- t5: NV Tiêu Hóa HCM
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1005, 'NV:C_TH:G_HCM',         TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
-- t6: NV Tiêu Hóa HN
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1006, 'NV:C_TH:G_HN',          TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
-- t7: LĐ TH+TK Hải Phòng
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1007, 'LDK:C_TH,C_TK:G_HP',   TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
-- label cho user session labels
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1008, 'LDK:C_TM:G_HCM',       TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1009, 'LDK:C_TK:G_HN',        TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1010, 'NV:C_TK:G_HCM',        TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1011, 'NV:C_TM:G_HCM',        TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1012, 'BGD:C_TH,C_TK,C_TM:G_HCM,G_HP,G_HN', TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/
BEGIN SA_LABEL_ADMIN.CREATE_LABEL('HOSPITAL_TB_POL', 1013, 'LDK:C_TH,C_TK,C_TM:G_HCM,G_HP,G_HN', TRUE); EXCEPTION WHEN OTHERS THEN NULL; END;
/

-- ============================================================
-- BƯỚC 7: Áp dụng policy lên bảng THONGBAO với NO_CONTROL trước
-- (để có thể UPDATE label thủ công ở bước sau)
-- ============================================================
BEGIN
    SA_POLICY_ADMIN.APPLY_TABLE_POLICY(
        policy_name   => 'HOSPITAL_TB_POL',
        schema_name   => 'ADMIN',
        table_name    => 'THONGBAO',
        table_options => 'NO_CONTROL'
    );
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -12987 THEN RAISE; END IF; -- -12987: Policy already applied
END;
/

-- ============================================================
-- BƯỚC 8: Tạo 8 Oracle user demo
-- ============================================================

GRANT SELECT ON ADMIN.THONGBAO TO NV021;
GRANT SELECT ON ADMIN.THONGBAO TO NV022;
GRANT SELECT ON ADMIN.THONGBAO TO NV023;
GRANT SELECT ON ADMIN.THONGBAO TO NV024;
GRANT SELECT ON ADMIN.THONGBAO TO NV025;
GRANT SELECT ON ADMIN.THONGBAO TO NV026;
GRANT SELECT ON ADMIN.THONGBAO TO NV027;
GRANT SELECT ON ADMIN.THONGBAO TO NV028;
/

-- ============================================================
-- BƯỚC 9: INSERT 7 thông báo mẫu, sau đó UPDATE label thủ công
-- Vì policy đang là NO_CONTROL, có thể UPDATE OLS_LABEL trực tiếp
-- ============================================================

-- Xóa dữ liệu cũ nếu có
DELETE FROM ADMIN.THONGBAO;
COMMIT;
/

-- INSERT 7 thông báo (không có OLS_LABEL - để NULL trước)
INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM)
VALUES(N'[t1] Họp toàn thể nhân viên 20/06 lúc 8h', CURRENT_TIMESTAMP, N'Hội trường chính');

INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM)
VALUES(N'[t2] Họp Ban Giám Đốc khẩn lúc 14h', CURRENT_TIMESTAMP, N'Văn phòng Ban Giám đốc');

INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM)
VALUES(N'[t3] Họp lãnh đạo khoa triển khai quy trình mới', CURRENT_TIMESTAMP, N'Phòng họp tầng 3');

INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM)
VALUES(N'[t4] LĐ Tiêu Hóa: cập nhật phác đồ điều trị', CURRENT_TIMESTAMP, N'Khoa Tiêu Hóa');

INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM)
VALUES(N'[t5] NV Tiêu Hóa HCM: kiểm tra thiết bị nội soi', CURRENT_TIMESTAMP, N'Khoa Tiêu Hóa TP HCM');

INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM)
VALUES(N'[t6] NV Tiêu Hóa HN: hội thảo chuyên môn 21/06', CURRENT_TIMESTAMP, N'Khoa Tiêu Hóa Hà Nội');

INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM)
VALUES(N'[t7] LĐ TH+TK Hải Phòng: họp triển khai mô hình mới', CURRENT_TIMESTAMP, N'Hải Phòng');

COMMIT;
/


-- UPDATE label thủ công cho từng thông báo
-- Dùng CHAR_TO_LABEL vì policy đang NO_CONTROL
UPDATE ADMIN.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('HOSPITAL_TB_POL', 'NV')
WHERE NOIDUNG LIKE '%t1%';

UPDATE ADMIN.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('HOSPITAL_TB_POL', 'BGD')
WHERE NOIDUNG LIKE '%t2%';

UPDATE ADMIN.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('HOSPITAL_TB_POL', 'LDK')
WHERE NOIDUNG LIKE '%t3%';

UPDATE ADMIN.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('HOSPITAL_TB_POL', 'LDK:C_TH')
WHERE NOIDUNG LIKE '%t4%';

UPDATE ADMIN.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('HOSPITAL_TB_POL', 'NV:C_TH:G_HCM')
WHERE NOIDUNG LIKE '%t5%';

UPDATE ADMIN.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('HOSPITAL_TB_POL', 'NV:C_TH:G_HN')
WHERE NOIDUNG LIKE '%t6%';

UPDATE ADMIN.THONGBAO SET OLS_LABEL = CHAR_TO_LABEL('HOSPITAL_TB_POL', 'LDK:C_TH,C_TK:G_HP')
WHERE NOIDUNG LIKE '%t7%';

COMMIT;
/

-- Kiểm tra dữ liệu và label
SELECT MATB, NOIDUNG, OLS_LABEL FROM ADMIN.THONGBAO ORDER BY MATB;

-- ============================================================
-- BƯỚC 10: Đổi policy sang READ_CONTROL, WRITE_CONTROL
-- Xóa NO_CONTROL policy rồi apply lại với option đầy đủ
-- ============================================================
BEGIN
    SA_POLICY_ADMIN.REMOVE_TABLE_POLICY('HOSPITAL_TB_POL', 'ADMIN', 'THONGBAO');
EXCEPTION WHEN OTHERS THEN NULL;
END;
/

BEGIN
    SA_POLICY_ADMIN.APPLY_TABLE_POLICY(
        policy_name   => 'HOSPITAL_TB_POL',
        schema_name   => 'ADMIN',
        table_name    => 'THONGBAO',
        table_options => 'READ_CONTROL'
    );
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -12987 THEN RAISE; END IF;
END;
/

-- ============================================================
-- BƯỚC 11: UPDATE trigger để OLS nhận lại label sau khi đổi option
-- ============================================================
UPDATE ADMIN.THONGBAO SET NOIDUNG = NOIDUNG;
COMMIT;
/

-- ============================================================
-- BƯỚC 12: Gán nhãn session cho 8 user demo
-- Theo pattern giáo viên: SET_USER_LABELS chỉ cần 1 label string (max_read)
-- ============================================================
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV021', 'BGD:C_TH,C_TK,C_TM:G_HCM,G_HP,G_HN'); END;
/
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV022', 'LDK:C_TM:G_HCM'); END;
/
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV023', 'LDK:C_TK:G_HN'); END;
/
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV024', 'NV:C_TK:G_HCM'); END;
/
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV025', 'NV:C_TM:G_HCM'); END;
/
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV026', 'LDK:C_TM:G_HCM'); END;
/
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV027', 'LDK:C_TH,C_TK,C_TM:G_HCM,G_HP,G_HN'); END;
/
BEGIN SA_USER_ADMIN.SET_USER_LABELS('HOSPITAL_TB_POL', 'NV028', 'NV:C_TH:G_HN'); END;
/

-- ============================================================
-- BƯỚC 13: Tạo hàm FN_BUILD_LABEL
-- ============================================================

CREATE OR REPLACE FUNCTION ADMIN.FN_BUILD_LABEL(
    p_level VARCHAR2,
    p_comp  VARCHAR2,
    p_group VARCHAR2
) RETURN VARCHAR2
IS
    v_label  VARCHAR2(200);
    v_comp   VARCHAR2(200);
    v_group  VARCHAR2(200);
BEGIN
    v_label := p_level;
    v_comp  := TRIM(p_comp);
    v_group := TRIM(p_group);

    -- Oracle: '' = NULL nên dùng IS NOT NULL thay vì != ''
    IF v_comp IS NOT NULL THEN
        v_label := v_label || ':' || v_comp;

        IF v_group IS NOT NULL THEN
            v_label := v_label || ':' || v_group;
        END IF;
    END IF;

    RETURN v_label;
END FN_BUILD_LABEL;
/
-- ============================================================
-- BƯỚC 14: Tạo SP_INSERT_THONGBAO
-- ============================================================
-- 1. Gán session label cho ADMIN
BEGIN
    SA_USER_ADMIN.SET_USER_LABELS(
        'HOSPITAL_TB_POL',
        'ADMIN',
        'BGD:C_TH,C_TK,C_TM:G_HCM,G_HP,G_HN'
    );
END;
/

-- 2. Cập nhật SP_INSERT_THONGBAO dùng INSERT trực tiếp OLS_LABEL
CREATE OR REPLACE PROCEDURE ADMIN.SP_INSERT_THONGBAO(
    p_noidung NVARCHAR2,
    p_ngaygio TIMESTAMP,
    p_diadiem NVARCHAR2,
    p_level   VARCHAR2,
    p_comp    VARCHAR2,
    p_group   VARCHAR2
)
IS
    v_label_str VARCHAR2(200);
    v_label_num NUMBER;
BEGIN
    v_label_str := ADMIN.FN_BUILD_LABEL(p_level, p_comp, p_group);
    v_label_num := CHAR_TO_LABEL('HOSPITAL_TB_POL', v_label_str);
    INSERT INTO ADMIN.THONGBAO(NOIDUNG, NGAYGIO, DIADIEM, OLS_LABEL)
    VALUES(p_noidung, p_ngaygio, p_diadiem, v_label_num);
    COMMIT;
EXCEPTION
    WHEN OTHERS THEN
        ROLLBACK;
        RAISE;
END SP_INSERT_THONGBAO;
/

-- ============================================================
-- BƯỚC 15: Tạo SP_GET_THONGBAO
-- ============================================================
CREATE OR REPLACE PROCEDURE ADMIN.SP_GET_THONGBAO(
    p_cursor OUT SYS_REFCURSOR
)
IS
BEGIN
    OPEN p_cursor FOR
    SELECT MATB, NOIDUNG, NGAYGIO, DIADIEM
    FROM   ADMIN.THONGBAO
    ORDER  BY NGAYGIO DESC;
END SP_GET_THONGBAO;
/
BEGIN
    SA_USER_ADMIN.SET_USER_PRIVS(
        policy_name => 'HOSPITAL_TB_POL',
        user_name   => 'ADMIN',
        privileges  => 'FULL'
    );
END;
/

GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV021;
GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV022;
GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV023;
GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV024;
GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV025;
GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV026;
GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV027;
GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO NV028;

-- Coi bảng label đã tạo
SELECT label_tag, label
FROM dba_sa_labels
WHERE policy_name = 'HOSPITAL_TB_POL'
ORDER BY label_tag;
/
-- ============================================================
-- BƯỚC BỔ SUNG: Gán nhãn mặc định (NV) và cấp quyền cho TẤT CẢ các user còn lại
-- ============================================================
BEGIN
    FOR r IN (
        SELECT MANV FROM ADMIN.NHANVIEN 
        WHERE MANV NOT IN ('NV021', 'NV022', 'NV023', 'NV024', 'NV025', 'NV026', 'NV027', 'NV028')
    ) 
    LOOP
        -- 1. Cấp quyền SELECT trên bảng thông báo
        BEGIN
            EXECUTE IMMEDIATE 'GRANT SELECT ON ADMIN.THONGBAO TO ' || r.MANV;
        EXCEPTION 
            WHEN OTHERS THEN NULL;
        END;

        -- 2. Cấp quyền EXECUTE cho Stored Procedure lấy thông báo
        BEGIN
            EXECUTE IMMEDIATE 'GRANT EXECUTE ON ADMIN.SP_GET_THONGBAO TO ' || r.MANV;
        EXCEPTION 
            WHEN OTHERS THEN NULL;
        END;

        -- 3. Gán nhãn mặc định thấp nhất ('NV') cho tài khoản Oracle
        BEGIN
            SA_USER_ADMIN.SET_USER_LABELS(
                policy_name    => 'HOSPITAL_TB_POL',
                user_name      => r.MANV,
                max_read_label => 'NV'
            );
        EXCEPTION 
            WHEN OTHERS THEN NULL;
        END;
    END LOOP;
END;
/
