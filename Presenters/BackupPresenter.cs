using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace ATBM_Project.Presenters
{
    public class BackupPresenter
    {
        private string GetDynamicTableFilter()
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
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(ATBM_Project.Data.DBConfig.ConnectionString))
                {
                    conn.Open();
                    string query = $"SELECT TABLE_NAME FROM ({GetDynamicTableFilter()}) ORDER BY TABLE_NAME";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch { }
            return dt;
        }

        public DataTable GetTablePreview(string tableName)
        {
            DataTable dt = new DataTable();
            try
            {
                using (OracleConnection conn = new OracleConnection(ATBM_Project.Data.DBConfig.ConnectionString))
                {
                    conn.Open();
                    string query = $"SELECT * FROM {tableName} WHERE ROWNUM <= 100";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                }
            }
            catch { }
            return dt;
        }

        public string ExecuteFlashback(string tableName, string timeStr)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(ATBM_Project.Data.DBConfig.ConnectionString))
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand($"ALTER TABLE {tableName} ENABLE ROW MOVEMENT", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string query = $"FLASHBACK TABLE {tableName} TO TIMESTAMP TO_TIMESTAMP('{timeStr}', 'YYYY-MM-DD HH24:MI:SS')";
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    using (OracleCommand cmd = new OracleCommand($"ALTER TABLE {tableName} DISABLE ROW MOVEMENT", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}