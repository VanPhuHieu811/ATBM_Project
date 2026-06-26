using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class UserPresenter
    {
        private static readonly Regex SafeIdentifierRegex = new Regex(@"^[A-Z][A-Z0-9_]{0,29}$", RegexOptions.Compiled);

        public List<UserAccount> GetUsers()
        {
            List<UserAccount> list = new List<UserAccount>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT USERNAME, TO_CHAR(CREATED, 'DD/MM/YYYY') as CREATED, ACCOUNT_STATUS FROM DBA_USERS WHERE USERNAME NOT IN ('SYS', 'SYSTEM', 'XDB', 'ANONYMOUS', 'WMSYS', 'OJVMSYS', 'CTXSYS', 'ORDSYS', 'ORDDATA', 'MDSYS', 'OLAPSYS', 'MDDATA', 'SYSMAN', 'MGMT_VIEW', 'SI_INFORMTN_SCHEMA', 'ORDPLUGINS', 'OWBSYS', 'DBSNMP', 'OUTLN', 'APPQOSSYS', 'DVSYS', 'DVF', 'AUDSYS') ORDER BY USERNAME";
                OracleCommand cmd = new OracleCommand(sql, conn);
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UserAccount
                        {
                            Username = reader["USERNAME"].ToString(),
                            CreatedDate = reader["CREATED"].ToString(),
                            Status = reader["ACCOUNT_STATUS"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<PendingAccountItem> GetEmployeesWithoutAccount()
        {
            const string sql = @"
                SELECT n.MANV,
                       n.HOTEN,
                       n.VAITRO,
                       n.PHAI,
                       TO_CHAR(n.NGAYSINH, 'DD/MM/YYYY') AS NGAYSINH
                FROM ADMIN.NHANVIEN n
                WHERE NOT EXISTS (
                    SELECT 1 FROM DBA_USERS u WHERE u.USERNAME = n.MANV
                )
                ORDER BY n.MANV";

            return ReadPendingItems(sql, isEmployee: true);
        }

        public List<PendingAccountItem> GetPatientsWithoutAccount()
        {
            const string sql = @"
                SELECT b.MABN,
                       b.TENBN,
                       N'Bệnh nhân' AS VAITRO,
                       b.PHAI,
                       TO_CHAR(b.NGAYSINH, 'DD/MM/YYYY') AS NGAYSINH
                FROM ADMIN.BENHNHAN b
                WHERE NOT EXISTS (
                    SELECT 1 FROM DBA_USERS u WHERE u.USERNAME = b.MABN
                )
                ORDER BY b.MABN";

            return ReadPendingItems(sql, isEmployee: false, idColumn: "MABN", nameColumn: "TENBN");
        }

        public void CreateEmployeeAccount(string manv, string password)
        {
            string username = NormalizeIdentifier(manv);
            string vaiTro = GetEmployeeRole(username);

            CreateOracleUser(username, password);
            GrantMinimalLoginPrivileges(username);
            GrantEmployeeRole(username, vaiTro);
        }

        public void CreatePatientAccount(string mabn, string password)
        {
            string username = NormalizeIdentifier(mabn);

            CreateOracleUser(username, password);
            GrantMinimalLoginPrivileges(username);
            ExecuteNonQuery($"GRANT ROLE_BENHNHAN TO {username}");
        }

        public void CreateUser(string username, string password)
        {
            CreateOracleUser(NormalizeIdentifier(username), password);
            ExecuteNonQuery($"GRANT CREATE SESSION TO {NormalizeIdentifier(username)}");
        }

        public void DropUser(string username)
        {
            string safeUsername = NormalizeIdentifier(username);
            ExecuteNonQuery($"DROP USER {safeUsername} CASCADE");
        }

        public void ChangePassword(string username, string newPassword)
        {
            string safeUsername = NormalizeIdentifier(username);
            string escapedPassword = EscapePassword(newPassword);
            ExecuteNonQuery($"ALTER USER {safeUsername} IDENTIFIED BY \"{escapedPassword}\"");
        }

        public List<UserAccount> SearchUsers(string keyword)
        {
            List<UserAccount> list = new List<UserAccount>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT USERNAME, TO_CHAR(CREATED, 'DD/MM/YYYY') as CREATED, ACCOUNT_STATUS " +
                             "FROM DBA_USERS WHERE UPPER(USERNAME) LIKE :kw";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("kw", "%" + keyword.ToUpper() + "%"));
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new UserAccount
                            {
                                Username = reader["USERNAME"].ToString(),
                                CreatedDate = reader["CREATED"].ToString(),
                                Status = reader["ACCOUNT_STATUS"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }

        private List<PendingAccountItem> ReadPendingItems(string sql, bool isEmployee, string idColumn = "MANV", string nameColumn = "HOTEN")
        {
            List<PendingAccountItem> list = new List<PendingAccountItem>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PendingAccountItem
                        {
                            AccountId = reader[idColumn].ToString(),
                            DisplayName = reader[nameColumn].ToString(),
                            RoleOrType = reader["VAITRO"].ToString(),
                            Gender = reader["PHAI"].ToString(),
                            BirthDate = reader["NGAYSINH"].ToString(),
                            IsEmployee = isEmployee
                        });
                    }
                }
            }
            return list;
        }

        private string GetEmployeeRole(string manv)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(
                    "SELECT VAITRO FROM ADMIN.NHANVIEN WHERE MANV = :manv", conn))
                {
                    cmd.Parameters.Add("manv", OracleDbType.Varchar2).Value = manv;
                    object value = cmd.ExecuteScalar();
                    if (value == null || value == DBNull.Value)
                    {
                        throw new Exception($"Không tìm thấy nhân viên {manv} trong bảng NHANVIEN.");
                    }
                    return value.ToString();
                }
            }
        }

        private void GrantEmployeeRole(string username, string vaiTro)
        {
            string roleName;
            switch (vaiTro)
            {
                case "Điều phối viên":
                    roleName = "ROLE_DIEUPHOIVIEN";
                    break;
                case "Bác sĩ/Y sĩ":
                    roleName = "ROLE_BACSI";
                    break;
                case "Kỹ thuật viên":
                    roleName = "ROLE_KYTHUATVIEN";
                    break;
                default:
                    throw new Exception($"Vai trò nhân viên không được hỗ trợ tạo tài khoản: {vaiTro}");
            }

            ExecuteNonQuery($"GRANT {roleName} TO {username}");
        }

        private void GrantMinimalLoginPrivileges(string username)
        {
            ExecuteNonQuery($"GRANT CREATE SESSION TO {username}");
            TryExecute($"GRANT SELECT ON ADMIN.NHANVIEN TO {username}");
            TryExecute($"GRANT SELECT ON ADMIN.BENHNHAN TO {username}");
        }

        private void CreateOracleUser(string username, string password)
        {
            string escapedPassword = EscapePassword(password);
            ExecuteNonQuery(
                $"CREATE USER {username} IDENTIFIED BY \"{escapedPassword}\" DEFAULT TABLESPACE users TEMPORARY TABLESPACE temp QUOTA 10M ON users");
        }

        private void ExecuteNonQuery(string sql)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void TryExecute(string sql)
        {
            try
            {
                ExecuteNonQuery(sql);
            }
            catch (OracleException)
            {
            }
        }

        private static string NormalizeIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new Exception("Mã tài khoản không hợp lệ.");
            }

            string normalized = value.Trim().ToUpperInvariant();
            if (!SafeIdentifierRegex.IsMatch(normalized))
            {
                throw new Exception("Mã tài khoản chỉ được chứa chữ, số và dấu gạch dưới.");
            }

            return normalized;
        }

        private static string EscapePassword(string password)
        {
            return (password ?? string.Empty).Replace("\"", "\"\"");
        }
    }
}
