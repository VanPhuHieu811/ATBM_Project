using ATBM_Project.Data;
using ATBM_Project.Utilities;
using System;
using System.Data;

namespace ATBM_Project.Presenters
{
    public class BackupPresenter
    {
        public DataTable GetTables()
        {
            string sql = "SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME";
            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);
        }

        public string ExecuteFlashback(string tableName, string timestamp)
        {
            try
            {
                string sqlEnable = $"ALTER TABLE ADMIN.{tableName} ENABLE ROW MOVEMENT";
                OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sqlEnable);

                string sqlFlashback = $"FLASHBACK TABLE ADMIN.{tableName} TO TIMESTAMP TO_TIMESTAMP('{timestamp}', 'YYYY-MM-DD HH24:MI:SS')";
                OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sqlFlashback);

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}