using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class RolePresenter
    {
        public List<RoleAccount> GetRoles()
        {
            List<RoleAccount> list = new List<RoleAccount>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ROLE, ROLE_ID FROM DBA_ROLES";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new RoleAccount
                        {
                            RoleName = reader["ROLE"].ToString(),
                            RoleId = reader["ROLE_ID"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public void CreateRole(string roleName)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"CREATE ROLE {roleName}";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DropRole(string roleName)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"DROP ROLE {roleName}";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<RoleAccount> SearchRoles(string keyword)
        {
            List<RoleAccount> list = new List<RoleAccount>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ROLE, ROLE_ID FROM DBA_ROLES WHERE UPPER(ROLE) LIKE :kw";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.Parameters.Add(new OracleParameter("kw", "%" + keyword.ToUpper() + "%"));
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RoleAccount
                            {
                                RoleName = reader["ROLE"].ToString(),
                                RoleId = reader["ROLE_ID"].ToString()
                            });
                        }
                    }
                }
            }
            return list;
        }
    }
}