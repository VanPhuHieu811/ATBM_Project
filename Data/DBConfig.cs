using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_Project.Data
{
    public class DBConfig
    {
        // 1. Khai báo các thông số kết nối
        private static string host = "localhost";
        private static string port = "1521";
        private static string sid = "xe";
        private static string user = "ADMIN_YTE";
        private static string pass = "12345678";

        // 2. Chuỗi kết nối (Connection String) chuẩn Oracle
        public static string ConnectionString = $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={sid})));User Id={user};Password={pass};";

        // 3. Hàm lấy đối tượng kết nối
        public static OracleConnection GetConnection()
        {
            return new OracleConnection(ConnectionString);
        }

        // 4. Hàm kiểm tra kết nối (Dùng để test lúc đầu)
        public static bool TestConnection()
        {
            using (OracleConnection conn = GetConnection())
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}