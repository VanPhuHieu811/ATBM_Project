using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;
using ATBM_Project.Models;

namespace ATBM_Project.Presenters
{
    public class PrivilegePresenter
    {
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