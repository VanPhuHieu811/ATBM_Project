using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models; // Thêm thư viện Models

namespace ATBM_Project.Presenters
{
    public class KTVPresenter
    {
        // Hàm lấy danh sách dịch vụ được điều phối cho KTV
        public List<KTVServiceModel> GetAssignedServices()
        {
            List<KTVServiceModel> list = new List<KTVServiceModel>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT MAHSBA, LOAIDV, TO_CHAR(NGAYDV, 'DD/MM/YYYY') as NGAYDV, MAKTV, KETQUA FROM admin.V_KTV_HSBA_DV ORDER BY NGAYDV DESC";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new KTVServiceModel
                            {
                                MaHSBA = reader["MAHSBA"].ToString(),
                                LoaiDV = reader["LOAIDV"].ToString(),
                                NgayDV = reader["NGAYDV"].ToString(),
                                MaKTV = reader["MAKTV"].ToString(),
                                KetQua = reader["KETQUA"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        // Truyền thẳng Model vào thay vì 4 biến string rời rạc
        public bool UpdateResult(KTVServiceModel model)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "UPDATE admin.V_KTV_HSBA_DV SET KETQUA = :ketqua WHERE MAHSBA = :mahsba AND LOAIDV = :loaidv AND NGAYDV = TO_DATE(:ngaydv, 'DD/MM/YYYY')";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("ketqua", model.KetQua));
                    cmd.Parameters.Add(new OracleParameter("mahsba", model.MaHSBA));
                    cmd.Parameters.Add(new OracleParameter("loaidv", model.LoaiDV));
                    cmd.Parameters.Add(new OracleParameter("ngaydv", model.NgayDV));

                    int rows = cmd.ExecuteNonQuery();
                    return rows > 0;
                }
            }
        }
    }
}