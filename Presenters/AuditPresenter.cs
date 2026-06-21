using ATBM_Project.Data;
using ATBM_Project.Utilities;
using System.Data;

namespace ATBM_Project.Presenters
{
    public class AuditPresenter
    {
        public DataTable GetTables()
        {
            string sql = "SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME";
            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);
        }

        public DataTable GetStandardAudit(string tableName)
        {
            string sql = "SELECT USERNAME, ACTION_NAME, OBJ_NAME, TIMESTAMP, RETURNCODE FROM DBA_AUDIT_TRAIL WHERE USERNAME NOT IN ('SYS', 'SYSTEM', 'DBSNMP', 'SYSMAN')";

            if (!string.IsNullOrEmpty(tableName))
            {
                sql += $" AND OBJ_NAME = '{tableName}'";
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
            sql += " ORDER BY TIMESTAMP DESC";

            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);
        }
    }
}