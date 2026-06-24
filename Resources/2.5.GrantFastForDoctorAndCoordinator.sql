-- CHAY BANG ADMIN HOAC TAI KHOAN CO QUYEN DBA
-- Script cap quyen nhanh cho ROLE_BACSI va ROLE_DIEUPHOIVIEN
-- Muc dich: bo qua phan he cap quyen trong app khi can test nhanh.

-- Tao ROLE_BACSI neu chua co
BEGIN
    EXECUTE IMMEDIATE 'CREATE ROLE ROLE_BACSI';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1921 THEN
            RAISE;
        END IF;
END;
/

-- Tao ROLE_DIEUPHOIVIEN neu chua co
BEGIN
    EXECUTE IMMEDIATE 'CREATE ROLE ROLE_DIEUPHOIVIEN';
EXCEPTION
    WHEN OTHERS THEN
        IF SQLCODE != -1921 THEN
            RAISE;
        END IF;
END;
/

-- ============================================================================
-- ROLE_BACSI
-- HSBA: SELECT; UPDATE (CHANDOAN, DIEUTRI, KETLUAN)
GRANT SELECT ON admin.HSBA TO ROLE_BACSI;
GRANT UPDATE (CHANDOAN, DIEUTRI, KETLUAN) ON admin.HSBA TO ROLE_BACSI;

-- HSBA_DV: SELECT, INSERT, DELETE
GRANT SELECT, INSERT, DELETE ON admin.HSBA_DV TO ROLE_BACSI;

-- BENHNHAN: SELECT; UPDATE (TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC)
GRANT SELECT ON admin.BENHNHAN TO ROLE_BACSI;
GRANT UPDATE (TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC) ON admin.BENHNHAN TO ROLE_BACSI;

-- DONTHUOC: SELECT, INSERT, UPDATE, DELETE
GRANT SELECT, INSERT, UPDATE, DELETE ON admin.DONTHUOC TO ROLE_BACSI;

-- NHANVIEN: cho UI bac si doc thong tin ca nhan hien tai
GRANT SELECT ON admin.NHANVIEN TO ROLE_BACSI;

-- Gan ROLE_BACSI cho cac nhan vien co vai tro Bac si/Y si
DECLARE
    v_sql VARCHAR2(200);
BEGIN
    FOR r IN (SELECT MANV FROM admin.NHANVIEN WHERE VAITRO = N'Bác sĩ/Y sĩ') LOOP
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

-- ============================================================================
-- ROLE_DIEUPHOIVIEN
-- BENHNHAN: SELECT, INSERT, UPDATE full bang
GRANT SELECT, INSERT, UPDATE ON admin.BENHNHAN TO ROLE_DIEUPHOIVIEN;

-- HSBA: SELECT, INSERT full bang; UPDATE (MABS, MAKHOA)
GRANT SELECT, INSERT ON admin.HSBA TO ROLE_DIEUPHOIVIEN;
GRANT UPDATE (MABS, MAKHOA) ON admin.HSBA TO ROLE_DIEUPHOIVIEN;

-- HSBA_DV: SELECT full bang; UPDATE (MAKTV)
GRANT SELECT ON admin.HSBA_DV TO ROLE_DIEUPHOIVIEN;
GRANT UPDATE (MAKTV) ON admin.HSBA_DV TO ROLE_DIEUPHOIVIEN;

-- NHANVIEN: cho dieu phoi vien doc danh sach bac si/ky thuat vien de gan phu trach
GRANT SELECT ON admin.NHANVIEN TO ROLE_DIEUPHOIVIEN;

-- Gan ROLE_DIEUPHOIVIEN cho cac nhan vien co vai tro Dieu phoi vien
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
