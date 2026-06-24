using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class CoordinatorPresenter
    {
        public DataTable GetPatients(string keyword)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MABN, TENBN, PHAI, TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS NGAYSINH,
                           CCCD, SONHA, TENDUONG, QUANHUYEN, TINHTP,
                           TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC
                    FROM admin.BENHNHAN
                    WHERE :kw IS NULL
                       OR UPPER(MABN) LIKE :like_kw
                       OR UPPER(TENBN) LIKE :like_kw
                       OR UPPER(CCCD) LIKE :like_kw
                    ORDER BY MABN";
                AddKeywordParameters(cmd, keyword);
                return Fill(cmd);
            }
        }

        public void AddPatient(CoordinatorPatientModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    INSERT INTO admin.BENHNHAN
                        (MABN, TENBN, PHAI, NGAYSINH, CCCD, SONHA, TENDUONG,
                         QUANHUYEN, TINHTP, TIENSUBENH, TIENSUBENHGD, DIUNGTHUOC)
                    VALUES
                        (:mabn, :tenbn, :phai, TO_DATE(:ngaysinh, 'DD/MM/YYYY'), :cccd,
                         :sonha, :tenduong, :quanhuyen, :tinhtp,
                         :tiensubenh, :tiensubenhgd, :diungthuoc)";
                AddPatientParameters(cmd, model, includeKey: true);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdatePatient(CoordinatorPatientModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    UPDATE admin.BENHNHAN
                    SET TENBN = :tenbn,
                        PHAI = :phai,
                        NGAYSINH = TO_DATE(:ngaysinh, 'DD/MM/YYYY'),
                        CCCD = :cccd,
                        SONHA = :sonha,
                        TENDUONG = :tenduong,
                        QUANHUYEN = :quanhuyen,
                        TINHTP = :tinhtp,
                        TIENSUBENH = :tiensubenh,
                        TIENSUBENHGD = :tiensubenhgd,
                        DIUNGTHUOC = :diungthuoc
                    WHERE MABN = :mabn";
                AddPatientParameters(cmd, model, includeKey: false);
                cmd.Parameters.Add("mabn", OracleDbType.Varchar2).Value = model.MaBN;
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetMedicalRecords(string keyword)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT h.MAHSBA, h.MABN, b.TENBN,
                           TO_CHAR(h.NGAY, 'DD/MM/YYYY') AS NGAY,
                           h.CHANDOAN, h.DIEUTRI, h.MABS, h.MAKHOA, h.KETLUAN
                    FROM admin.HSBA h
                    LEFT JOIN admin.BENHNHAN b ON b.MABN = h.MABN
                    WHERE :kw IS NULL
                       OR UPPER(h.MAHSBA) LIKE :like_kw
                       OR UPPER(h.MABN) LIKE :like_kw
                       OR UPPER(b.TENBN) LIKE :like_kw
                       OR UPPER(h.MABS) LIKE :like_kw
                       OR UPPER(h.MAKHOA) LIKE :like_kw
                    ORDER BY h.NGAY DESC, h.MAHSBA";
                AddKeywordParameters(cmd, keyword);
                return Fill(cmd);
            }
        }

        public void AddMedicalRecord(CoordinatorMedicalRecordModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    INSERT INTO admin.HSBA
                        (MAHSBA, MABN, NGAY, CHANDOAN, DIEUTRI, MABS, MAKHOA, KETLUAN)
                    VALUES
                        (:mahsba, :mabn, TO_DATE(:ngay, 'DD/MM/YYYY'), :chandoan,
                         :dieutri, :mabs, :makhoa, :ketluan)";
                AddMedicalRecordParameters(cmd, model, includeKey: true);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateMedicalRecordAssignment(CoordinatorMedicalRecordModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    UPDATE admin.HSBA
                    SET MAKHOA = :makhoa,
                        MABS = :mabs
                    WHERE MAHSBA = :mahsba";
                cmd.Parameters.Add("makhoa", OracleDbType.Varchar2).Value = ToDbValue(model.MaKhoa);
                cmd.Parameters.Add("mabs", OracleDbType.Varchar2).Value = ToDbValue(model.MaBS);
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = model.MaHSBA;
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetServices(string keyword)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT d.MAHSBA, d.LOAIDV,
                           TO_CHAR(d.NGAYDV, 'DD/MM/YYYY') AS NGAYDV,
                           d.MAKTV, d.KETQUA
                    FROM admin.HSBA_DV d
                    WHERE :kw IS NULL
                       OR UPPER(d.MAHSBA) LIKE :like_kw
                       OR UPPER(d.LOAIDV) LIKE :like_kw
                       OR UPPER(d.MAKTV) LIKE :like_kw
                    ORDER BY d.NGAYDV DESC, d.MAHSBA";
                AddKeywordParameters(cmd, keyword);
                return Fill(cmd);
            }
        }

        public void UpdateServiceTechnician(CoordinatorServiceModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    UPDATE admin.HSBA_DV
                    SET MAKTV = :maktv
                    WHERE MAHSBA = :mahsba
                      AND LOAIDV = :loaidv
                      AND NGAYDV = TO_DATE(:ngaydv, 'DD/MM/YYYY')";
                cmd.Parameters.Add("maktv", OracleDbType.Varchar2).Value = ToDbValue(model.MaKTV);
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = model.MaHSBA;
                cmd.Parameters.Add("loaidv", OracleDbType.NVarchar2).Value = model.LoaiDV;
                cmd.Parameters.Add("ngaydv", OracleDbType.Varchar2).Value = model.NgayDV;
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetDoctors()
        {
            return GetStaffByRole("Bác sĩ/Y sĩ");
        }

        public DataTable GetTechnicians()
        {
            return GetStaffByRole("Kỹ thuật viên");
        }

        private DataTable GetStaffByRole(string role)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            using (OracleCommand cmd = conn.CreateCommand())
            {
                conn.Open();
                cmd.BindByName = true;
                cmd.CommandText = @"
                    SELECT MANV, HOTEN, CHUYENKHOA
                    FROM admin.NHANVIEN
                    WHERE VAITRO = :vaitro
                    ORDER BY MANV";
                cmd.Parameters.Add("vaitro", OracleDbType.NVarchar2).Value = role;
                return Fill(cmd);
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

        private static void AddKeywordParameters(OracleCommand cmd, string keyword)
        {
            string normalized = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim().ToUpperInvariant();
            cmd.Parameters.Add("kw", OracleDbType.Varchar2).Value = (object)normalized ?? DBNull.Value;
            cmd.Parameters.Add("like_kw", OracleDbType.Varchar2).Value = normalized == null ? (object)DBNull.Value : "%" + normalized + "%";
        }

        private static void AddPatientParameters(OracleCommand cmd, CoordinatorPatientModel model, bool includeKey)
        {
            if (includeKey)
            {
                cmd.Parameters.Add("mabn", OracleDbType.Varchar2).Value = model.MaBN;
            }

            cmd.Parameters.Add("tenbn", OracleDbType.NVarchar2).Value = model.TenBN;
            cmd.Parameters.Add("phai", OracleDbType.NVarchar2).Value = model.Phai;
            cmd.Parameters.Add("ngaysinh", OracleDbType.Varchar2).Value = model.NgaySinh;
            cmd.Parameters.Add("cccd", OracleDbType.Varchar2).Value = model.Cccd;
            cmd.Parameters.Add("sonha", OracleDbType.NVarchar2).Value = ToDbValue(model.SoNha);
            cmd.Parameters.Add("tenduong", OracleDbType.NVarchar2).Value = ToDbValue(model.TenDuong);
            cmd.Parameters.Add("quanhuyen", OracleDbType.NVarchar2).Value = ToDbValue(model.QuanHuyen);
            cmd.Parameters.Add("tinhtp", OracleDbType.NVarchar2).Value = ToDbValue(model.TinhTp);
            cmd.Parameters.Add("tiensubenh", OracleDbType.NVarchar2).Value = ToDbValue(model.TienSuBenh);
            cmd.Parameters.Add("tiensubenhgd", OracleDbType.NVarchar2).Value = ToDbValue(model.TienSuBenhGd);
            cmd.Parameters.Add("diungthuoc", OracleDbType.NVarchar2).Value = ToDbValue(model.DiUngThuoc);
        }

        private static void AddMedicalRecordParameters(OracleCommand cmd, CoordinatorMedicalRecordModel model, bool includeKey)
        {
            if (includeKey)
            {
                cmd.Parameters.Add("mahsba", OracleDbType.Varchar2).Value = model.MaHSBA;
            }

            cmd.Parameters.Add("mabn", OracleDbType.Varchar2).Value = model.MaBN;
            cmd.Parameters.Add("ngay", OracleDbType.Varchar2).Value = model.Ngay;
            cmd.Parameters.Add("chandoan", OracleDbType.NVarchar2).Value = ToDbValue(model.ChanDoan);
            cmd.Parameters.Add("dieutri", OracleDbType.NVarchar2).Value = ToDbValue(model.DieuTri);
            cmd.Parameters.Add("mabs", OracleDbType.Varchar2).Value = ToDbValue(model.MaBS);
            cmd.Parameters.Add("makhoa", OracleDbType.Varchar2).Value = ToDbValue(model.MaKhoa);
            cmd.Parameters.Add("ketluan", OracleDbType.NVarchar2).Value = ToDbValue(model.KetLuan);
        }

        private static object ToDbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value.Trim();
        }
    }
}
