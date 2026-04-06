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
    }
}