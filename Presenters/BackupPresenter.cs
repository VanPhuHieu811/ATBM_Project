using ATBM_Project.Data;
using ATBM_Project.Utilities;
using System;
using System.Data;

namespace ATBM_Project.Presenters
{
    public class BackupPresenter
    {
        public bool ExecuteFlashback(string tableName, string timestamp)
        {
            try
            {
                string sqlEnable = $"ALTER TABLE ADMIN.{tableName} ENABLE ROW MOVEMENT";
                OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sqlEnable);

                string sqlFlashback = $"FLASHBACK TABLE ADMIN.{tableName} TO TIMESTAMP TO_TIMESTAMP('{timestamp}', 'YYYY-MM-DD HH24:MI:SS')";
                OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sqlFlashback);
                return true;
            }
            catch { return false; }
        }

        public DataTable GetTablePreview(string tableName)
        {
            string sql = $"SELECT * FROM ADMIN.{tableName} WHERE ROWNUM <= 50";
            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);
        }
    }
}