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
                    FROM DBA_SYS_PRIVS 
                    WHERE GRANTEE = :grantee AND NVL(COMMON, 'NO') = 'NO'
                    UNION ALL
                    SELECT PRIVILEGE AS PrivilegeName, 'OBJECT' AS Type, OWNER || '.' || TABLE_NAME AS TableName 
                    FROM DBA_TAB_PRIVS 
                    WHERE GRANTEE = :grantee AND NVL(COMMON, 'NO') = 'NO'
                    UNION ALL
                    SELECT GRANTED_ROLE AS PrivilegeName, 'ROLE' AS Type, NULL AS TableName 
                    FROM DBA_ROLE_PRIVS 
                    WHERE GRANTEE = :grantee AND NVL(COMMON, 'NO') = 'NO'
                    UNION ALL
                    SELECT PRIVILEGE AS PrivilegeName, 'COLUMN' AS Type, OWNER || '.' || TABLE_NAME || '(' || COLUMN_NAME || ')' AS TableName 
                    FROM DBA_COL_PRIVS 
                    WHERE GRANTEE = :grantee AND NVL(COMMON, 'NO') = 'NO'";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
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
            }
            return list;
        }

        public List<string> GetUsers()
        {
            List<string> users = new List<string>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT USERNAME FROM DBA_USERS WHERE ORACLE_MAINTAINED = 'N' ORDER BY USERNAME";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) users.Add(reader[0].ToString());
                }
            }
            return users;
        }

        public List<string> GetRoles()
        {
            List<string> roles = new List<string>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "SELECT ROLE FROM DBA_ROLES ORDER BY ROLE";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) roles.Add(reader[0].ToString());
                }
            }
            return roles;
        }

        public List<string> GetObjects(string objectType)
        {
            List<string> objects = new List<string>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = "";
                if (objectType.ToUpper() == "TABLE") sql = "SELECT TABLE_NAME FROM USER_TABLES";
                else if (objectType.ToUpper() == "VIEW") sql = "SELECT VIEW_NAME FROM USER_VIEWS";
                else if (objectType.ToUpper() == "PROCEDURE") sql = "SELECT OBJECT_NAME FROM USER_OBJECTS WHERE OBJECT_TYPE = 'PROCEDURE'";
                else if (objectType.ToUpper() == "FUNCTION") sql = "SELECT OBJECT_NAME FROM USER_OBJECTS WHERE OBJECT_TYPE = 'FUNCTION'";

                if (string.IsNullOrEmpty(sql)) return objects;
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) objects.Add(reader[0].ToString());
                }
            }
            return objects;
        }

        public List<string> GetColumns(string tableName)
        {
            List<string> columns = new List<string>();
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"SELECT COLUMN_NAME FROM USER_TAB_COLUMNS WHERE TABLE_NAME = '{tableName}'";
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read()) columns.Add(reader[0].ToString());
                }
            }
            return columns;
        }

        public void ExecuteGrant(PrivilegeModel priv)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                foreach (string p in priv.SelectedPrivileges)
                {
                    string sql = "";

                    if (p.ToUpper() == "SELECT" && priv.SelectedColumns != null && priv.SelectedColumns.Count > 0)
                    {
                        string cols = string.Join(", ", priv.SelectedColumns);

                        string prefix = priv.ObjectName.StartsWith("V_", StringComparison.OrdinalIgnoreCase) ? "" : "V_";
                        string viewName = $"{prefix}{priv.ObjectName}_{priv.Grantee}";

                        string createViewSql = $"CREATE OR REPLACE VIEW {viewName} AS SELECT {cols} FROM {priv.ObjectName}";
                        using (OracleCommand cmdView = new OracleCommand(createViewSql, conn))
                        {
                            cmdView.ExecuteNonQuery();
                        }
                        sql = $"GRANT SELECT ON {viewName} TO {priv.Grantee}";
                    }
                    else if (p.ToUpper() == "UPDATE" && priv.SelectedColumns != null && priv.SelectedColumns.Count > 0)
                    {
                        string cols = string.Join(", ", priv.SelectedColumns);
                        sql = $"GRANT UPDATE ({cols}) ON {priv.ObjectName} TO {priv.Grantee}";
                    }
                    else
                    {
                        sql = $"GRANT {p} ON {priv.ObjectName} TO {priv.Grantee}";
                    }

                    if (priv.WithGrantOption) sql += " WITH GRANT OPTION";

                    using (OracleCommand cmd = new OracleCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void ExecuteGrantRole(string grantee, string roleName, bool withAdminOption)
        {
            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                string sql = $"GRANT {roleName} TO {grantee}";
                if (withAdminOption) sql += " WITH ADMIN OPTION";

                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}