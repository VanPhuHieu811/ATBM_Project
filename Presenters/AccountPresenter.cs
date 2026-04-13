using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class AccountPresenter
    {
        public List<UserAccount> GetUsers()
        {
            List<UserAccount> list = new List<UserAccount>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT USERNAME, TO_CHAR(CREATED, 'DD/MM/YYYY') as CREATED, ACCOUNT_STATUS FROM DBA_USERS";
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

        public List<RoleAccount> GetRoles()
        {
            List<RoleAccount> list = new List<RoleAccount>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ROLE, ROLE_ID FROM DBA_ROLES";
                OracleCommand cmd = new OracleCommand(sql, conn);
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

        public List<PrivilegeInfo> GetPrivileges(string grantee)
        {
            List<PrivilegeInfo> list = new List<PrivilegeInfo>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = @"
                    SELECT PRIVILEGE AS PrivilegeName, 'SYSTEM' AS Type, NULL AS TableName 
                    FROM DBA_SYS_PRIVS WHERE GRANTEE = :grantee
                    UNION ALL
                    SELECT PRIVILEGE AS PrivilegeName, 'OBJECT' AS Type, TABLE_NAME AS TableName 
                    FROM DBA_TAB_PRIVS WHERE GRANTEE = :grantee
                    UNION ALL
                    SELECT GRANTED_ROLE AS PrivilegeName, 'ROLE' AS Type, NULL AS TableName 
                    FROM DBA_ROLE_PRIVS WHERE GRANTEE = :grantee";

                OracleCommand cmd = new OracleCommand(sql, conn);
                cmd.Parameters.Add(new OracleParameter("grantee", grantee.ToUpper()));
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PrivilegeInfo
                        {
                            PrivilegeName = reader["PrivilegeName"].ToString(),
                            Type = reader["Type"].ToString(),
                            TableName = reader["TableName"]?.ToString()
                        });
                    }
                }
            }
            return list;
        }

        public bool RevokePrivilege(string privilege, string userOrRole)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"REVOKE {privilege} FROM {userOrRole}";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
        }
    }
}