using System;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_Project.Data
{
    public class DBConfig
    {
        private const string DefaultHost = "localhost";
        private const string DefaultPort = "1521";
        private const string DefaultServiceName = "xe";

        private static string host = DefaultHost;
        private static string port = DefaultPort;
        private static string sid = DefaultServiceName;
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
                return (sid ?? string.Empty).Trim().ToUpperInvariant();
            }
        }

        public static void UpdateConfig(string pHost, string pPort, string pSid, string pUser, string pPass)
        {
            host = NormalizeOrDefault(pHost, DefaultHost);
            port = NormalizeOrDefault(pPort, DefaultPort);
            sid = NormalizeOrDefault(pSid, DefaultServiceName);
            user = (pUser ?? string.Empty).Trim();
            pass = pPass ?? string.Empty;
        }

        private static string NormalizeOrDefault(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
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