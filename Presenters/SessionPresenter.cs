using System;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;

namespace ATBM_Project.Presenters
{
    public class SessionPresenter
    {
        public string GetCurrentRole()
        {
            string currentUser = (DBConfig.User ?? string.Empty).Trim().ToUpperInvariant();
            if (currentUser == "ADMIN" || currentUser == "SYS" || currentUser == "SYSTEM")
            {
                return "DBA";
            }

            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();

                string role = TryGetScalar(conn, "SELECT VAITRO FROM ADMIN.NHANVIEN WHERE UPPER(MANV) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(role))
                {
                    return role.Trim();
                }

                role = TryGetScalar(conn, "SELECT VAITRO FROM NHANVIEN WHERE UPPER(MANV) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(role))
                {
                    return role.Trim();
                }

                role = TryGetScalar(conn, "SELECT 'Bệnh nhân' FROM ADMIN.BENHNHAN WHERE UPPER(MABN) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(role))
                {
                    return role.Trim();
                }

                role = TryGetScalar(conn, "SELECT 'Bệnh nhân' FROM BENHNHAN WHERE UPPER(MABN) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(role))
                {
                    return role.Trim();
                }
            }

            return string.Empty;
        }

        public string GetCurrentDisplayName()
        {
            string currentUser = (DBConfig.User ?? string.Empty).Trim().ToUpperInvariant();

            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();

                string name = TryGetScalar(conn, "SELECT HOTEN FROM ADMIN.NHANVIEN WHERE UPPER(MANV) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.Trim();
                }

                name = TryGetScalar(conn, "SELECT HOTEN FROM NHANVIEN WHERE UPPER(MANV) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.Trim();
                }

                name = TryGetScalar(conn, "SELECT TENBN FROM ADMIN.BENHNHAN WHERE UPPER(MABN) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.Trim();
                }

                name = TryGetScalar(conn, "SELECT TENBN FROM BENHNHAN WHERE UPPER(MABN) = :username", currentUser);
                if (!string.IsNullOrWhiteSpace(name))
                {
                    return name.Trim();
                }
            }

            return currentUser;
        }

        private string TryGetScalar(OracleConnection conn, string sql, string username)
        {
            try
            {
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add("username", OracleDbType.Varchar2).Value = username;
                    object value = cmd.ExecuteScalar();
                    return value == null || value == DBNull.Value ? string.Empty : value.ToString();
                }
            }
            catch (OracleException)
            {
                return string.Empty;
            }
        }
    }
}
