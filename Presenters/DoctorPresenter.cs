using System;
using System.Data;

namespace ATBM_Project.Presenters
{
    public class DoctorPresenter
    {
        private static readonly DataTable MedicalRecords = CreateMedicalRecords();
        private static readonly DataTable Services = CreateServices();
        private static readonly DataTable Prescriptions = CreatePrescriptions();
        private static readonly DataTable Patients = CreatePatients();

        public DataTable GetMedicalRecords()
        {
            return GetMedicalRecords(string.Empty);
        }

        public DataTable GetMedicalRecords(string keyword)
        {
            DataTable result = CreateMedicalRecordListTable();
            string normalizedKeyword = (keyword ?? string.Empty).Trim().ToUpperInvariant();

            foreach (DataRow record in MedicalRecords.Rows)
            {
                DataRow patient = FindPatient(record["MABN"].ToString());
                string tenBn = patient == null ? string.Empty : patient["TENBN"].ToString();
                string cccd = patient == null ? string.Empty : patient["CCCD"].ToString();

                if (!MatchesKeyword(record, tenBn, cccd, normalizedKeyword))
                {
                    continue;
                }

                result.Rows.Add(
                    record["MAHSBA"],
                    record["MABN"],
                    tenBn,
                    record["NGAY"],
                    record["CHANDOAN"],
                    record["DIEUTRI"],
                    record["MABS"],
                    record["MAKHOA"],
                    record["KETLUAN"]
                );
            }

            return result;
        }

        public DataTable GetMedicalRecord(string maHsba)
        {
            return SelectRows(MedicalRecords, $"MAHSBA = '{Escape(maHsba)}'");
        }

        public void UpdateMedicalRecord(string maHsba, string chanDoan, string dieuTri, string ketLuan)
        {
            foreach (DataRow row in MedicalRecords.Select($"MAHSBA = '{Escape(maHsba)}'"))
            {
                row["CHANDOAN"] = ToDbValue(chanDoan);
                row["DIEUTRI"] = ToDbValue(dieuTri);
                row["KETLUAN"] = ToDbValue(ketLuan);
            }
        }

        public DataTable GetServices(string maHsba)
        {
            return SelectRows(Services, $"MAHSBA = '{Escape(maHsba)}'");
        }

        public void AddService(string maHsba, string loaiDv, DateTime ngayDv, string maKtv, string ketQua)
        {
            Services.Rows.Add(maHsba, loaiDv, ngayDv.Date, maKtv, ketQua);
        }

        public void DeleteService(string maHsba, string loaiDv, DateTime ngayDv)
        {
            foreach (DataRow row in Services.Select($"MAHSBA = '{Escape(maHsba)}' AND LOAIDV = '{Escape(loaiDv)}'"))
            {
                if (Convert.ToDateTime(row["NGAYDV"]).Date == ngayDv.Date)
                {
                    Services.Rows.Remove(row);
                    break;
                }
            }
        }

        public DataTable GetPrescriptions(string maHsba)
        {
            return SelectRows(Prescriptions, $"MAHSBA = '{Escape(maHsba)}'");
        }

        public void AddPrescription(string maHsba, DateTime ngayDt, string tenThuoc, string lieuDung)
        {
            Prescriptions.Rows.Add(maHsba, ngayDt.Date, tenThuoc, lieuDung);
        }

        public void UpdatePrescriptionDose(string maHsba, DateTime ngayDt, string tenThuoc, string lieuDung)
        {
            foreach (DataRow row in Prescriptions.Select($"MAHSBA = '{Escape(maHsba)}' AND TENTHUOC = '{Escape(tenThuoc)}'"))
            {
                if (Convert.ToDateTime(row["NGAYDT"]).Date == ngayDt.Date)
                {
                    row["LIEUDUNG"] = ToDbValue(lieuDung);
                    break;
                }
            }
        }

        public void DeletePrescription(string maHsba, DateTime ngayDt, string tenThuoc)
        {
            foreach (DataRow row in Prescriptions.Select($"MAHSBA = '{Escape(maHsba)}' AND TENTHUOC = '{Escape(tenThuoc)}'"))
            {
                if (Convert.ToDateTime(row["NGAYDT"]).Date == ngayDt.Date)
                {
                    Prescriptions.Rows.Remove(row);
                    break;
                }
            }
        }

        public DataTable GetPatientByMedicalRecord(string maHsba)
        {
            DataRow[] hsbaRows = MedicalRecords.Select($"MAHSBA = '{Escape(maHsba)}'");
            if (hsbaRows.Length == 0)
            {
                return Patients.Clone();
            }

            string maBn = hsbaRows[0]["MABN"].ToString();
            return SelectRows(Patients, $"MABN = '{Escape(maBn)}'");
        }

        public void UpdatePatientHistory(string maBn, string tienSuBenh, string tienSuBenhGd, string diUngThuoc)
        {
            foreach (DataRow row in Patients.Select($"MABN = '{Escape(maBn)}'"))
            {
                row["TIENSUBENH"] = ToDbValue(tienSuBenh);
                row["TIENSUBENHGD"] = ToDbValue(tienSuBenhGd);
                row["DIUNGTHUOC"] = ToDbValue(diUngThuoc);
            }
        }

        private static DataTable CreateMedicalRecords()
        {
            DataTable table = new DataTable();
            table.Columns.Add("MAHSBA", typeof(string));
            table.Columns.Add("MABN", typeof(string));
            table.Columns.Add("NGAY", typeof(DateTime));
            table.Columns.Add("CHANDOAN", typeof(string));
            table.Columns.Add("DIEUTRI", typeof(string));
            table.Columns.Add("MABS", typeof(string));
            table.Columns.Add("MAKHOA", typeof(string));
            table.Columns.Add("KETLUAN", typeof(string));

            table.Rows.Add("BA001", "BN001", new DateTime(2024, 1, 1), "Đau bao tử", "Uống thuốc tiêu hóa", "NV009", "K01", "Ổn định");
            table.Rows.Add("BA002", "BN002", new DateTime(2024, 1, 2), "Viêm phổi", "Kháng sinh", "NV005", "K02", "Theo dõi thêm");
            table.Rows.Add("BA003", "BN003", new DateTime(2024, 1, 3), "Rối loạn nhịp tim", "Đặt máy tạo nhịp", "NV007", "K03", "Nhập viện");
            table.Rows.Add("BA004", "BN004", new DateTime(2024, 1, 4), "Đau đầu kéo dài", "Chụp CT", "NV011", "K04", "Chờ kết quả");
            table.Rows.Add("BA005", "BN005", new DateTime(2024, 1, 5), "Sốt xuất huyết", "Truyền dịch", "NV006", "K02", "Nguy hiểm");

            return table;
        }

        private static DataTable CreateMedicalRecordListTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("MAHSBA", typeof(string));
            table.Columns.Add("MABN", typeof(string));
            table.Columns.Add("TENBN", typeof(string));
            table.Columns.Add("NGAY", typeof(DateTime));
            table.Columns.Add("CHANDOAN", typeof(string));
            table.Columns.Add("DIEUTRI", typeof(string));
            table.Columns.Add("MABS", typeof(string));
            table.Columns.Add("MAKHOA", typeof(string));
            table.Columns.Add("KETLUAN", typeof(string));
            return table;
        }

        private static DataTable CreateServices()
        {
            DataTable table = new DataTable();
            table.Columns.Add("MAHSBA", typeof(string));
            table.Columns.Add("LOAIDV", typeof(string));
            table.Columns.Add("NGAYDV", typeof(DateTime));
            table.Columns.Add("MAKTV", typeof(string));
            table.Columns.Add("KETQUA", typeof(string));

            table.Rows.Add("BA001", "Siêu âm bụng", new DateTime(2024, 1, 1), "NV015", "Dạ dày có vết loét");
            table.Rows.Add("BA002", "Chụp X-Quang phổi", new DateTime(2024, 1, 2), "NV020", "Phổi mờ");
            table.Rows.Add("BA003", "Đo điện tâm đồ", new DateTime(2024, 1, 3), "NV016", "Nhịp tim chậm");
            table.Rows.Add("BA004", "Chụp CT đầu", new DateTime(2024, 1, 4), "NV017", "Không u");

            return table;
        }

        private static DataTable CreatePrescriptions()
        {
            DataTable table = new DataTable();
            table.Columns.Add("MAHSBA", typeof(string));
            table.Columns.Add("NGAYDT", typeof(DateTime));
            table.Columns.Add("TENTHUOC", typeof(string));
            table.Columns.Add("LIEUDUNG", typeof(string));

            table.Rows.Add("BA001", new DateTime(2024, 1, 1), "Phosphalugel", "3 gói/ngày");
            table.Rows.Add("BA002", new DateTime(2024, 1, 2), "Amoxicillin", "2 viên/ngày");
            table.Rows.Add("BA003", new DateTime(2024, 1, 3), "Digoxin", "1 viên/sáng");
            table.Rows.Add("BA004", new DateTime(2024, 1, 4), "Paracetamol", "3 viên/ngày");

            return table;
        }

        private static DataTable CreatePatients()
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

            table.Rows.Add("BN001", "Nguyễn Văn Khách", "Nam", new DateTime(1990, 1, 1), "001090123451", "10", "Lý Thường Kiệt", "Quận 10", "TP HCM", "Đau dạ dày", "Không", "");
            table.Rows.Add("BN002", "Trần Thị Hoa", "Nữ", new DateTime(1985, 5, 15), "001085123452", "20", "Cách Mạng Tháng 8", "Quận 3", "TP HCM", "Viêm xoang", "Tiểu đường", "Aspirin");
            table.Rows.Add("BN003", "Lê Công Vinh", "Nam", new DateTime(1985, 12, 10), "001085123453", "15", "Trần Hưng Đạo", "Quận 1", "TP HCM", "", "", "");
            table.Rows.Add("BN004", "Phạm Huỳnh Đông", "Nam", new DateTime(1983, 4, 5), "001083123454", "05", "Lê Lợi", "Hà Đông", "Hà Nội", "Cao huyết áp", "Tim mạch", "");
            table.Rows.Add("BN005", "Đỗ Hải Yến", "Nữ", new DateTime(1982, 10, 20), "001082123455", "101", "Nguyễn Huệ", "Quận 1", "TP HCM", "", "", "Penicillin");

            return table;
        }

        private static DataTable SelectRows(DataTable source, string filter)
        {
            DataTable result = source.Clone();
            foreach (DataRow row in source.Select(filter))
            {
                result.ImportRow(row);
            }

            return result;
        }

        private static DataRow FindPatient(string maBn)
        {
            DataRow[] rows = Patients.Select($"MABN = '{Escape(maBn)}'");
            return rows.Length == 0 ? null : rows[0];
        }

        private static bool MatchesKeyword(DataRow record, string tenBn, string cccd, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return true;
            }

            string combined = string.Join(" ",
                record["MAHSBA"],
                record["MABN"],
                tenBn,
                cccd,
                record["CHANDOAN"],
                record["DIEUTRI"],
                record["MABS"],
                record["MAKHOA"],
                record["KETLUAN"]
            ).ToUpperInvariant();

            return combined.Contains(keyword);
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }

        private static object ToDbValue(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? (object)string.Empty : value.Trim();
        }
    }
}
