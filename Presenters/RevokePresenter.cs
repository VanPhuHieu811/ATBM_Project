using System;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;

namespace ATBM_Project.Presenters
{
    public class RevokePresenter
    {
        public bool RevokePrivilege(string privilegeName, string privilegeType, string tableName, string grantee)
        {
            string normalizedPrivilege = (privilegeName ?? string.Empty).Trim();
            string normalizedType = (privilegeType ?? "SYSTEM").Trim().ToUpperInvariant();
            string normalizedGrantee = (grantee ?? string.Empty).Trim();
            string normalizedTableName = (tableName ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(normalizedPrivilege) || string.IsNullOrEmpty(normalizedGrantee))
            {
                throw new Exception("Thiếu thông tin privilege hoặc grantee.");
            }

            string sql;
            if (normalizedType == "OBJECT")
            {
                if (string.IsNullOrEmpty(normalizedTableName) || !normalizedTableName.Contains("."))
                {
                    throw new Exception("Tên object không hợp lệ. Dùng dạng OWNER.TABLE_NAME.");
                }

                sql = $"REVOKE {normalizedPrivilege} ON {normalizedTableName} FROM {normalizedGrantee}";
            }
            else
            {
                sql = $"REVOKE {normalizedPrivilege} FROM {normalizedGrantee}";
            }

            try
            {
                using (OracleConnection conn = DBConfig.GetConnection())
                using (OracleCommand cmd = new OracleCommand(sql, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"[SQL Executed: {sql}] - {ex.Message}");
            }
        }
    }
}
