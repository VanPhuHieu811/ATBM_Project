-- HSBA - Bác sĩ chỉ thấy/cập nhật HSBA mình phụ trách
CREATE OR REPLACE FUNCTION FN_VPD_HSBA_BACSI (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
)
RETURN VARCHAR2
AS
BEGIN
    RETURN 'MABS = SYS_CONTEXT(''USERENV'', ''SESSION_USER'')';
END;
/


-- BENHNHAN - Bác sĩ chỉ thấy/cập nhật bệnh nhân mà mình phụ trách
CREATE OR REPLACE FUNCTION FN_VPD_BENHNHAN_BACSI (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
)
RETURN VARCHAR2
AS
BEGIN
    RETURN 'MABN IN (
        SELECT MABN
        FROM AD_QLBV.HSBA
        WHERE MABS = SYS_CONTEXT(''USERENV'', ''SESSION_USER'')
    )';
END;
/

   
-- HSBA_DV - Bác sĩ chỉ thêm/xóa dịch vụ mà mình phụ trách
CREATE OR REPLACE FUNCTION FN_VPD_HSBA_DV_BACSI (
    p_schema IN VARCHAR2,
    p_object IN VARCHAR2
)
RETURN VARCHAR2
AS
BEGIN
    RETURN 'MAHSBA IN (
        SELECT MAHSBA
        FROM AD_QLBV.HSBA
        WHERE MABS = SYS_CONTEXT(''USERENV'', ''SESSION_USER'')
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
    RETURN 'MAHSBA IN (
        SELECT MAHSBA
        FROM AD_QLBV.HSBA
        WHERE MABS = SYS_CONTEXT(''USERENV'', ''SESSION_USER'')
    )';
END;
/



--- POLICY
-- 1. Bác sĩ chỉ xem các hồ sơ bệnh án mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'AD_QLBV',
        object_name     => 'HSBA',
        policy_name     => 'POL_HSBA_SELECT_BACSI',
        function_schema => 'AD_QLBV',
        policy_function => 'FN_VPD_HSBA_BACSI',
        statement_types => 'SELECT'
    );
END;
/


-- 2. Bác sĩ chỉ cập nhật CHANDOAN, DIEUTRI, KETLUAN trên HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'AD_QLBV',
        object_name     => 'HSBA',
        policy_name     => 'POL_HSBA_UPDATE_BACSI',
        function_schema => 'AD_QLBV',
        policy_function => 'FN_VPD_HSBA_BACSI',
        statement_types => 'UPDATE',
        update_check    => TRUE
    );
END;
/

-- 3. Bác sĩ chỉ thêm/xóa dịch vụ hỗ trợ chẩn đoán thuộc HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'AD_QLBV',
        object_name     => 'HSBA_DV',
        policy_name     => 'POL_HSBA_DV_BACSI',
        function_schema => 'AD_QLBV',
        policy_function => 'FN_VPD_HSBA_DV_BACSI',
        statement_types => 'INSERT, DELETE',
        update_check    => TRUE
    );
END;
/

-- 4. Bác sĩ chỉ xem bệnh nhân thuộc HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'AD_QLBV',
        object_name     => 'BENHNHAN',
        policy_name     => 'POL_BENHNHAN_SELECT_BACSI',
        function_schema => 'AD_QLBV',
        policy_function => 'FN_VPD_BENHNHAN_BACSI',
        statement_types => 'SELECT'
    );
END;
/

-- 5. Bác sĩ chỉ cập nhật TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC của bệnh nhân mình điều trị
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'AD_QLBV',
        object_name     => 'BENHNHAN',
        policy_name     => 'POL_BENHNHAN_UPDATE_BACSI',
        function_schema => 'AD_QLBV',
        policy_function => 'FN_VPD_BENHNHAN_BACSI',
        statement_types => 'UPDATE',
        update_check    => TRUE
    );
END;
/


-- 6. Bác sĩ chỉ thao tác đơn thuốc thuộc HSBA mà mình phụ trách
BEGIN
    DBMS_RLS.ADD_POLICY(
        object_schema   => 'AD_QLBV',
        object_name     => 'DONTHUOC',
        policy_name     => 'POL_DONTHUOC_BACSI',
        function_schema => 'AD_QLBV',
        policy_function => 'FN_VPD_DONTHUOC_BACSI',
        statement_types => 'SELECT, INSERT, UPDATE, DELETE',
        update_check    => TRUE
    );
END;
/