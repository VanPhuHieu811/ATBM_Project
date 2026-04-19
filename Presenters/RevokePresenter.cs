using System;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;

namespace ATBM_Project.Presenters
{
    public class RevokePresenter
    {
        public bool RevokePrivilege(string privilegeName, string privilegeType, string tableName, string grantee)
        {
            string normalizedPrivilege = (privilegeName ?? string.Empty).Trim().ToUpperInvariant();
            string normalizedType = (privilegeType ?? "SYSTEM").Trim().ToUpperInvariant();
            string normalizedGrantee = (grantee ?? string.Empty).Trim().ToUpper();
            string normalizedTableName = (tableName ?? string.Empty).Trim();

            if (string.IsNullOrEmpty(normalizedPrivilege) || string.IsNullOrEmpty(normalizedGrantee))
            {
                throw new Exception("Thiếu thông tin privilege hoặc grantee.");
            }

            string sql;
            if (normalizedType == "OBJECT" || normalizedType == "COLUMN")
            {
                if (string.IsNullOrEmpty(normalizedTableName) || !normalizedTableName.Contains("."))
                {
                    throw new Exception("Tên object không hợp lệ. Dùng dạng OWNER.TABLE_NAME hoặc OWNER.TABLE_NAME (COL_NAME).");
                }

                // Oracle không cho REVOKE UPDATE/REFERENCES theo từng cột ma phai full table
                string actualTableName = normalizedTableName;
                int idx = normalizedTableName.IndexOf('(');
                if (idx > 0)
                {
                    actualTableName = normalizedTableName.Substring(0, idx).Trim();
                }

                sql = $"REVOKE {normalizedPrivilege} ON {actualTableName} FROM {normalizedGrantee}";
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
