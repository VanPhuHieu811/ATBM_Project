using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class UserPresenter
    {
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

        public void CreateUser(string username, string password)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"CREATE USER {username} IDENTIFIED BY \"{password}\"";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                string grantSql = $"GRANT CREATE SESSION TO {username}";
                using (OracleCommand cmd = new OracleCommand(grantSql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DropUser(string username)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"DROP USER {username} CASCADE";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void ChangePassword(string username, string newPassword)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"ALTER USER {username} IDENTIFIED BY \"{newPassword}\"";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
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
    }
} 