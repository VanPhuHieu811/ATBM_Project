using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;

namespace ATBM_Project.Presenters
{
    public class DoctorPresenter
    {
        public DataTable GetCurrentDoctorProfile()
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MANV, HOTEN, PHAI,
                           TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS NGAYSINH,
                           CMND, QUEQUAN, SODT, VAITRO, CHUYENKHOA
                    FROM admin.NHANVIEN
                    WHERE MANV = USER";
                return Fill(cmd);
            }
        }

        public DataTable GetMedicalRecords()
        {
            return GetMedicalRecords(string.Empty);
        }

        public DataTable GetMedicalRecords(string keyword)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT h.MAHSBA, h.MABN, CAST(NULL AS NVARCHAR2(100)) AS TENBN, h.NGAY,
                           h.CHANDOAN, h.DIEUTRI, h.MABS, h.MAKHOA, h.KETLUAN
                    FROM admin.HSBA h
                    WHERE :kw IS NULL
                       OR UPPER(h.MAHSBA) LIKE :like_kw
                       OR UPPER(h.MABN) LIKE :like_kw
                       OR UPPER(h.CHANDOAN) LIKE :like_kw
                       OR UPPER(h.DIEUTRI) LIKE :like_kw
                       OR UPPER(h.MAKHOA) LIKE :like_kw
                       OR UPPER(h.KETLUAN) LIKE :like_kw
                    ORDER BY h.NGAY DESC, h.MAHSBA";
                AddKeywordParameters(cmd, keyword);
                return Fill(cmd);
            }
        }

        public DataTable GetMedicalRecord(string maHsba)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MAHSBA, MABN, NGAY, CHANDOAN, DIEUTRI, MABS, MAKHOA, KETLUAN
                    FROM admin.HSBA
                    WHERE MAHSBA = :mahsba";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                return Fill(cmd);
            }
        }

        public void UpdateMedicalRecord(string maHsba, string chanDoan, string dieuTri, string ketLuan)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    UPDATE admin.HSBA
                    SET CHANDOAN = :chandoan,
                        DIEUTRI = :dieutri,
                        KETLUAN = :ketluan
                    WHERE MAHSBA = :mahsba";
                cmd.Parameters.Add("chandoan", OracleDbType.NVarchar2).Value = ToDbValue(chanDoan);
                cmd.Parameters.Add("dieutri", OracleDbType.NVarchar2).Value = ToDbValue(dieuTri);
                cmd.Parameters.Add("ketluan", OracleDbType.NVarchar2).Value = ToDbValue(ketLuan);
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                ExecuteExpectingRow(cmd);
            }
        }

        public DataTable GetServices(string maHsba)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA
                    FROM admin.HSBA_DV
                    WHERE MAHSBA = :mahsba
                    ORDER BY NGAYDV DESC, LOAIDV";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                return Fill(cmd);
            }
        }

        public void AddService(string maHsba, string loaiDv, DateTime ngayDv, string maKtv, string ketQua)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    INSERT INTO admin.HSBA_DV (MAHSBA, LOAIDV, NGAYDV, MAKTV, KETQUA)
                    VALUES (:mahsba, :loaidv, :ngaydv, :maktv, :ketqua)";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                cmd.Parameters.Add("loaidv", OracleDbType.NVarchar2).Value = ToDbValue(loaiDv);
                cmd.Parameters.Add("ngaydv", OracleDbType.Date).Value = ngayDv.Date;
                cmd.Parameters.Add("maktv", OracleDbType.Varchar2).Value = ToDbValue(maKtv);
                cmd.Parameters.Add("ketqua", OracleDbType.NVarchar2).Value = ToDbValue(ketQua);
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteService(string maHsba, string loaiDv, DateTime ngayDv)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    DELETE FROM admin.HSBA_DV
                    WHERE MAHSBA = :mahsba
                      AND LOAIDV = :loaidv
                      AND NGAYDV = :ngaydv";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                cmd.Parameters.Add("loaidv", OracleDbType.NVarchar2).Value = loaiDv;
                cmd.Parameters.Add("ngaydv", OracleDbType.Date).Value = ngayDv.Date;
                ExecuteExpectingRow(cmd);
            }
        }

        public DataTable GetPrescriptions(string maHsba)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG
                    FROM admin.DONTHUOC
                    WHERE MAHSBA = :mahsba
                    ORDER BY NGAYDT DESC, TENTHUOC";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                return Fill(cmd);
            }
        }

        public void AddPrescription(string maHsba, DateTime ngayDt, string tenThuoc, string lieuDung)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    INSERT INTO admin.DONTHUOC (MAHSBA, NGAYDT, TENTHUOC, LIEUDUNG)
                    VALUES (:mahsba, :ngaydt, :tenthuoc, :lieudung)";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                cmd.Parameters.Add("ngaydt", OracleDbType.Date).Value = ngayDt.Date;
                cmd.Parameters.Add("tenthuoc", OracleDbType.NVarchar2).Value = ToDbValue(tenThuoc);
                cmd.Parameters.Add("lieudung", OracleDbType.NVarchar2).Value = ToDbValue(lieuDung);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePrescriptionDose(string maHsba, DateTime ngayDt, string tenThuoc, string lieuDung)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    UPDATE admin.DONTHUOC
                    SET LIEUDUNG = :lieudung
                    WHERE MAHSBA = :mahsba
                      AND NGAYDT = :ngaydt
                      AND TENTHUOC = :tenthuoc";
                cmd.Parameters.Add("lieudung", OracleDbType.NVarchar2).Value = ToDbValue(lieuDung);
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                cmd.Parameters.Add("ngaydt", OracleDbType.Date).Value = ngayDt.Date;
                cmd.Parameters.Add("tenthuoc", OracleDbType.NVarchar2).Value = tenThuoc;
                ExecuteExpectingRow(cmd);
            }
        }

        public void DeletePrescription(string maHsba, DateTime ngayDt, string tenThuoc)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    DELETE FROM admin.DONTHUOC
                    WHERE MAHSBA = :mahsba
                      AND NGAYDT = :ngaydt
                      AND TENTHUOC = :tenthuoc";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                cmd.Parameters.Add("ngaydt", OracleDbType.Date).Value = ngayDt.Date;
                cmd.Parameters.Add("tenthuoc", OracleDbType.NVarchar2).Value = tenThuoc;
                ExecuteExpectingRow(cmd);
            }
        }

        public DataTable GetPatientByMedicalRecord(string maHsba)
        {
            string maBn = GetPatientIdByMedicalRecord(maHsba);
            if (string.IsNullOrWhiteSpace(maBn))
            {
                return CreatePatientTable();
            }

            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MABN, TENBN, PHAI, NGAYSINH, CCCD,
                           SONHA, TENDUONG, QUANHUYEN, TINHTP,
                           TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC
                    FROM admin.BENHNHAN
                    WHERE MABN = :mabn";
                cmd.Parameters.Add("mabn", OracleDbType.Varchar2).Value = maBn;
                return Fill(cmd);
            }
        }

        public void UpdatePatientHistory(string maBn, string tienSuBenh, string tienSuBenhGd, string diUngThuoc)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    UPDATE admin.BENHNHAN
                    SET TIENSUBENH = :tiensubenh,
                        TIENSUBENHGD = :tiensubenhgd,
                        DIUNGTHUOC = :diungthuoc
                    WHERE MABN = :mabn";
                cmd.Parameters.Add("tiensubenh", OracleDbType.NVarchar2).Value = ToDbValue(tienSuBenh);
                cmd.Parameters.Add("tiensubenhgd", OracleDbType.NVarchar2).Value = ToDbValue(tienSuBenhGd);
                cmd.Parameters.Add("diungthuoc", OracleDbType.NVarchar2).Value = ToDbValue(diUngThuoc);
                cmd.Parameters.Add("mabn", OracleDbType.Varchar2).Value = NormalizeKey(maBn);
                ExecuteExpectingRow(cmd);
            }
        }

        private static DataTable Fill(OracleCommand cmd)
        {
            using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                return table;
            }
        }

        private static string GetPatientIdByMedicalRecord(string maHsba)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MABN
                    FROM admin.HSBA
                    WHERE MAHSBA = :mahsba";
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = NormalizeKey(maHsba);
                object value = cmd.ExecuteScalar();
                return value == null || value == DBNull.Value ? string.Empty : value.ToString();
            }
        }

        private static DataTable CreatePatientTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("MABN", typeof(string));
            table.Columns.Add("TENBN", typeof(string));
            table.Columns.Add("PHAI", typeof(string));
            table.Columns.Add("NGAYSINH", typeof(DateTime));
            table.Columns.Add("CCCD", typeof(string));
            table.Columns.Add("SONHA", typeof(string));
            table.Columns.Add("TENDUONG", typeof(string));
            table.Columns.Add("QUANHUYEN", typeof(string));
            table.Columns.Add("TINHTP", typeof(string));
            table.Columns.Add("TIENSUBENH", typeof(string));
            table.Columns.Add("TIENSUBENHGD", typeof(string));
            table.Columns.Add("DIUNGTHUOC", typeof(string));
            return table;
        }

        private static void AddKeywordParameters(OracleCommand cmd, string keyword)
        {
            string normalized = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim().ToUpperInvariant();
            cmd.Parameters.Add("kw", OracleDbType.Varchar2).Value = (object)normalized ?? DBNull.Value;
            cmd.Parameters.Add("like_kw", OracleDbType.Varchar2).Value = normalized == null ? (object)DBNull.Value : "%" + normalized + "%";
        }

        private static void ExecuteExpectingRow(OracleCommand cmd)
        {
            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception("Không có dòng nào được cập nhật. Có thể dữ liệu không tồn tại hoặc tài khoản không có quyền thao tác.");
            }
        }

        private static string NormalizeKey(string value)
        {
            return (value ?? string.Empty).Trim().ToUpperInvariant();
        }

        private static object ToDbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }
    }
}
