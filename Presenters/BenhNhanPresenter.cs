using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class BenhNhanPresenter
    {
        public List<BenhNhan> GetBenhNhanList()
        {
            List<BenhNhan> list = new List<BenhNhan>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                // SQL nối chuỗi Số nhà, Tên đường, Quận/Huyện, Tỉnh/TP
                string sql = @"SELECT MABN, TENBN, PHAI, TO_CHAR(NGAYSINH, 'DD/MM/YYYY') AS NGAYSINH, CCCD, 
                               (SONHA || ' ' || TENDUONG || ', ' || QUANHUYEN || ', ' || TINHTP) AS DIACHI,
                               TIENSUBENH
                               FROM BENHNHAN";

                OracleCommand cmd = new OracleCommand(sql, conn);
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new BenhNhan
                        {
                            MaBN = reader["MABN"].ToString(),
                            TenBN = reader["TENBN"].ToString(),
                            Phai = reader["PHAI"].ToString(),
                            NgaySinh = reader["NGAYSINH"].ToString(),
                            CCCD = reader["CCCD"].ToString(),
                            DiaChi = reader["DIACHI"].ToString(),
                            TiensuBenh = reader["TIENSUBENH"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public BenhNhanModel GetProfile()
        {
            BenhNhanModel model = null;
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                // Truy vấn trực tiếp từ View bảo mật
                string sql = "SELECT * FROM admin.V_BENHNHAN_PROFILE";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new BenhNhanModel
                        {
                            MaBN = reader["MABN"].ToString(),
                            TenBN = reader["TENBN"].ToString(),
                            Phai = reader["PHAI"].ToString(),
                            NgaySinh = reader["NGAYSINH"].ToString(),
                            Cccd = reader["CCCD"].ToString(),
                            SoNha = reader["SONHA"]?.ToString(),
                            TenDuong = reader["TENDUONG"]?.ToString(),
                            QuanHuyen = reader["QUANHUYEN"]?.ToString(),
                            TinhTp = reader["TINHTP"]?.ToString(),
                            TienSuBenh = reader["TIENSUBENH"]?.ToString(),
                            TienSuBenhGd = reader["TIENSUBENHGD"]?.ToString(),
                            DiUngThuoc = reader["DIUNGTHUOC"]?.ToString()
                        };
                    }
                }
            }
            return model;
        }

        // 2. Hàm cập nhật thông tin cá nhân (Hệ thống đang báo thiếu hàm này)
        public bool UpdateProfile(BenhNhanModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                // Thực hiện lệnh UPDATE trực tiếp trên View bảo mật
                string sql = @"UPDATE admin.V_BENHNHAN_PROFILE 
                               SET SONHA = :sonha, TENDUONG = :tenduong, QUANHUYEN = :quanhuyen, TINHTP = :tinhtp, 
                                   TIENSUBENH = :tiensubenh, TIENSUBENHGD = :tiensubenhgd, DIUNGTHUOC = :diungthuoc";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    // Truyền đúng 7 tham số tương ứng với 7 trường cho phép sửa
                    cmd.Parameters.Add(new OracleParameter("sonha", model.SoNha));
                    cmd.Parameters.Add(new OracleParameter("tenduong", model.TenDuong));
                    cmd.Parameters.Add(new OracleParameter("quanhuyen", model.QuanHuyen));
                    cmd.Parameters.Add(new OracleParameter("tinhtp", model.TinhTp));
                    cmd.Parameters.Add(new OracleParameter("tiensubenh", model.TienSuBenh));
                    cmd.Parameters.Add(new OracleParameter("tiensubenhgd", model.TienSuBenhGd));
                    cmd.Parameters.Add(new OracleParameter("diungthuoc", model.DiUngThuoc));

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}