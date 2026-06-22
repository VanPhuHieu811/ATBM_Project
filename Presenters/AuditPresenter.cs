using ATBM_Project.Data;
using ATBM_Project.Utilities;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace ATBM_Project.Presenters
{
    public class AuditPresenter
    {
        private string GetActiveTableFilter()
        {
            return @"SELECT TABLE_NAME
                     FROM USER_TABLES t
                     WHERE t.DROPPED = 'NO' 
                       AND t.TEMPORARY = 'N' 
                       AND t.SECONDARY = 'N'
                       AND EXISTS (
                           SELECT 1 FROM USER_CONSTRAINTS c 
                           WHERE c.TABLE_NAME = t.TABLE_NAME 
                             AND c.CONSTRAINT_TYPE = 'P'
                       )";
        }

        public DataTable GetTables()
        {
            string sql = $"SELECT TABLE_NAME FROM ({GetActiveTableFilter()}) ORDER BY TABLE_NAME";
            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);
        }

        public DataTable GetStandardAudit(string tableName)
        {
            string sql = "SELECT USERNAME, ACTION_NAME, OBJ_NAME, TIMESTAMP, RETURNCODE FROM DBA_AUDIT_TRAIL WHERE USERNAME NOT IN ('SYS', 'SYSTEM', 'DBSNMP', 'SYSMAN')";

            if (!string.IsNullOrEmpty(tableName))
            {
                sql += $" AND OBJ_NAME = '{tableName}'";
            }
            else
            {
                sql += @" AND (OBJ_NAME IS NULL 
                            OR (OBJ_NAME NOT LIKE 'BIN$%' 
                                AND OBJ_NAME NOT LIKE 'ET$%' 
                                AND OBJ_NAME NOT LIKE 'SYS_%' 
                                AND OBJ_NAME NOT IN ('SPD_SCRATCH_TAB', 'IMPDP_STATS')))";
            }

            sql += " ORDER BY TIMESTAMP DESC";

            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);
        }

        public DataTable GetFGAAudit(string tableName)
        {
            string sql = "SELECT DB_USER, OBJECT_NAME, POLICY_NAME, SQL_TEXT, TIMESTAMP FROM DBA_FGA_AUDIT_TRAIL WHERE OBJECT_SCHEMA = 'ADMIN'";

            if (!string.IsNullOrEmpty(tableName))
            {
                sql += $" AND OBJECT_NAME = '{tableName}'";
            }
            else
            {
                sql += @" AND (OBJECT_NAME IS NULL 
                            OR (OBJECT_NAME NOT LIKE 'BIN$%' 
                                AND OBJECT_NAME NOT LIKE 'ET$%' 
                                AND OBJECT_NAME NOT LIKE 'SYS_%' 
                                AND OBJECT_NAME NOT IN ('SPD_SCRATCH_TAB', 'IMPDP_STATS')))";
            }

            sql += " ORDER BY TIMESTAMP DESC";

            DataTable dt = OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);

            foreach (DataRow row in dt.Rows)
            {
                string sqlText = row["SQL_TEXT"]?.ToString();

                if (!string.IsNullOrEmpty(sqlText))
                {
                    try
                    {
                        string decodedText = Regex.Replace(sqlText, @"\\([0-9a-fA-F]{4})",
                            m => ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());

                        decodedText = decodedText.Replace("= u'", "= '");

                        row["SQL_TEXT"] = decodedText;
                    }
                    catch
                    {
                    }
                }
            }

            return dt;
        }
    }
}