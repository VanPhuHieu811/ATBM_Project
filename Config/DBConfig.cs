using System;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_Project.Data
{
    public class DBConfig
    {
        private static string host = "localhost";
        private static string port = "1521";
        private static string sid = "xepdb1";
        private static string user = string.Empty;
        private static string pass = string.Empty;

        public static string User => user;

        public static string ConnectionString
        {
            get
            {
                string connStr =
                    $"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT={port}))(CONNECT_DATA=(SERVICE_NAME={sid})));User Id={user};Password={pass};";
                if (!string.IsNullOrEmpty(user) && user.Equals("sys", StringComparison.OrdinalIgnoreCase))
                {
                    connStr += "DBA Privilege=SYSDBA;";
                }
                return connStr;
            }
        }

        public static string ExpectedContainerName
        {
            get
            {
                return sid.ToUpperInvariant();
            }
        }

        public static void UpdateConfig(string pHost, string pPort, string pSid, string pUser, string pPass)
        {
            host = string.IsNullOrWhiteSpace(pHost) ? "localhost" : pHost.Trim();
            port = string.IsNullOrWhiteSpace(pPort) ? "1521" : pPort.Trim();
            sid = string.IsNullOrWhiteSpace(pSid) ? "xepdb1" : pSid.Trim();
            user = (pUser ?? string.Empty).Trim();
            pass = pPass ?? string.Empty;
        }

        public static OracleConnection GetConnection()
        {
            return new OracleConnection(ConnectionString);
        }

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