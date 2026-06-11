using System;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class NhanVienPresenter
    {
        // Lấy thông tin cá nhân của nhân viên đang session login
        public NhanVienModel GetProfile()
        {
            NhanVienModel model = null;
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT * FROM admin.V_NHANVIEN_PROFILE";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new NhanVienModel
                        {
                            ManV = reader["MANV"].ToString(),
                            HoTen = reader["HOTEN"].ToString(),
                            Phai = reader["PHAI"].ToString(),
                            NgaySinh = reader["NGAYSINH"].ToString(),
                            Cmnd = reader["CMND"].ToString(),
                            QueQuan = reader["QUEQUAN"]?.ToString(),
                            SoDt = reader["SODT"]?.ToString(),
                            VaiTro = reader["VAITRO"].ToString(),
                            ChuyenKhoa = reader["CHUYENKHOA"]?.ToString()
                        };
                    }
                }
            }
            return model;
        }

        // Cập nhật thông tin liên lạc (Chỉ truyền đúng 2 tham số cần update để tránh ORA-01006)
        public bool UpdateProfile(NhanVienModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = @"UPDATE admin.V_NHANVIEN_PROFILE 
                               SET QUEQUAN = :quequan, SODT = :sodt";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("quequan", model.QueQuan));
                    cmd.Parameters.Add(new OracleParameter("sodt", model.SoDt));

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}