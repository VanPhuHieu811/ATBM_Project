-- ============================================================
-- PHÂN HỆ 1 - MÀN HÌNH 1: Quản lý User / Role
-- Các câu SQL dùng trong ứng dụng WinForm C#
-- ============================================================

-- ============================================================
-- 1. TẠO USER
-- ============================================================
-- Dùng String.Format hoặc parameterized trong C# (DDL không hỗ trợ bind param)
CREATE USER {0} IDENTIFIED BY {1}
    DEFAULT TABLESPACE {2}
    TEMPORARY TABLESPACE TEMP
    PROFILE {3};

-- Cấp quyền đăng nhập tối thiểu ngay sau khi tạo
GRANT CREATE SESSION TO {0};


-- ============================================================
-- 2. SỬA USER (đổi password / tablespace / profile)
-- ============================================================
ALTER USER {0} IDENTIFIED BY {1};
ALTER USER {0} DEFAULT TABLESPACE {1};
ALTER USER {0} PROFILE {1};
ALTER USER {0} ACCOUNT LOCK;     -- Khóa tài khoản
ALTER USER {0} ACCOUNT UNLOCK;   -- Mở khóa tài khoản


-- ============================================================
-- 3. XÓA USER
-- ============================================================
DROP USER {0} CASCADE;   -- CASCADE xóa luôn các object của user đó


-- ============================================================
-- 4. TẠO ROLE
-- ============================================================
CREATE ROLE {0};                            -- Không xác thực
CREATE ROLE {0} IDENTIFIED BY {1};         -- Xác thực bằng password
CREATE ROLE {0} IDENTIFIED EXTERNALLY;     -- Xác thực external


-- ============================================================
-- 5. XÓA ROLE
-- ============================================================
DROP ROLE {0};


-- ============================================================
-- 6. GÁN ROLE CHO USER
-- ============================================================
GRANT {0} TO {1};                          -- Gán role cho user
GRANT {0} TO {1} WITH ADMIN OPTION;       -- Cho phép user tiếp tục gán role này


-- ============================================================
-- 7. THU HỒI ROLE KHỎI USER
-- ============================================================
REVOKE {0} FROM {1};


-- ============================================================
-- 8. LẤY DANH SÁCH TẤT CẢ USER TRONG HỆ THỐNG
-- ============================================================
SELECT
    u.USERNAME,
    u.ACCOUNT_STATUS,
    u.DEFAULT_TABLESPACE,
    u.PROFILE,
    TO_CHAR(u.CREATED, 'DD/MM/YYYY') AS CREATED_DATE,
    u.EXPIRY_DATE,
    'USER' AS LOAI
FROM DBA_USERS u
WHERE u.USERNAME NOT IN (
    -- Loại bỏ các user hệ thống Oracle mặc định
    'SYS','SYSTEM','DBSNMP','SYSMAN','OUTLN','MDSYS','ORDSYS','EXFSYS',
    'DMSYS','WMSYS','CTXSYS','ANONYMOUS','XDB','ORDPLUGINS','OLAPSYS',
    'PUBLIC','LBACSYS','APEX_PUBLIC_USER','FLOWS_FILES','APEX_030200'
)
ORDER BY u.USERNAME;


-- ============================================================
-- 9. LẤY DANH SÁCH TẤT CẢ ROLE TRONG HỆ THỐNG
-- ============================================================
SELECT
    ROLE,
    PASSWORD_REQUIRED,
    TO_CHAR(SYSDATE, 'DD/MM/YYYY') AS CREATED_DATE,
    'ROLE' AS LOAI
FROM DBA_ROLES
WHERE ROLE NOT IN (
    -- Loại bỏ role hệ thống mặc định
    'CONNECT','RESOURCE','DBA','SELECT_CATALOG_ROLE','EXECUTE_CATALOG_ROLE',
    'DELETE_CATALOG_ROLE','EXP_FULL_DATABASE','IMP_FULL_DATABASE',
    'RECOVERY_CATALOG_OWNER','SCHEDULER_ADMIN','HS_ADMIN_ROLE',
    'GLOBAL_AQ_USER_ROLE','AQ_ADMINISTRATOR_ROLE','AQ_USER_ROLE',
    'DATAPUMP_EXP_FULL_DATABASE','DATAPUMP_IMP_FULL_DATABASE',
    'GATHER_SYSTEM_STATISTICS','LOGSTDBY_ADMINISTRATOR',
    'OEM_ADVISOR','OEM_MONITOR','XDBADMIN','AUTHENTICATEDUSER',
    'WM_ADMIN_ROLE','JAVAIDPRIV','JAVASYSPRIV','JAVAUSERPRIV',
    'JAVA_ADMIN','JAVA_DEPLOY','OLAP_DBA','OLAP_USER',
    'SPATIAL_CSW_ADMIN','SPATIAL_WFS_ADMIN','MGMT_USER','APEX_ADMINISTRATOR_ROLE'
)
ORDER BY ROLE;


-- ============================================================
-- 10. LẤY DANH SÁCH ROLE ĐANG GÁN CHO 1 USER
-- ============================================================
SELECT
    GRANTED_ROLE,
    ADMIN_OPTION,
    DEFAULT_ROLE
FROM DBA_ROLE_PRIVS
WHERE GRANTEE = :username   -- bind parameter
ORDER BY GRANTED_ROLE;


-- ============================================================
-- 11. LẤY DANH SÁCH USER ĐANG CÓ 1 ROLE CỤ THỂ
-- ============================================================
SELECT
    GRANTEE,
    ADMIN_OPTION,
    DEFAULT_ROLE
FROM DBA_ROLE_PRIVS
WHERE GRANTED_ROLE = :rolename
ORDER BY GRANTEE;


-- ============================================================
-- 12. LẤY DANH SÁCH TABLESPACE (cho combobox)
-- ============================================================
SELECT TABLESPACE_NAME
FROM DBA_TABLESPACES
WHERE CONTENTS IN ('PERMANENT', 'UNDO')
ORDER BY TABLESPACE_NAME;


-- ============================================================
-- 13. LẤY DANH SÁCH PROFILE (cho combobox)
-- ============================================================
SELECT DISTINCT PROFILE
FROM DBA_PROFILES
ORDER BY PROFILE;
