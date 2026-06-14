using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Data;

namespace ATBM_Project.Utilities
{
    /// <summary>
    /// Class helper để kết nối Oracle và thực thi các lệnh SQL/SP
    /// Hỗ trợ ExecuteNonQuery, ExecuteReader, ExecuteScalar
    /// </summary>
    public static class OracleHelper
    {
        /// <summary>
        /// Thực thi một Stored Procedure không trả về dữ liệu
        /// Trả về số dòng bị ảnh hưởng (hoặc -1 nếu lỗi)
        /// </summary>
        public static int ExecuteNonQuery(string connectionString, string spName, OracleParameter[] parameters = null)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        conn.Close();

                        return result;
                    }
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Lỗi Oracle: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kết nối Oracle: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Thực thi một Stored Procedure trả về dữ liệu
        /// Trả về DataTable với kết quả từ RefCursor
        /// </summary>
        public static DataTable ExecuteReader(string connectionString, string spName, OracleParameter[] parameters = null)
        {
            DataTable dt = new DataTable();

            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        conn.Open();

                        using (OracleDataAdapter adapter = new OracleDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }

                        conn.Close();
                    }
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Lỗi Oracle: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kết nối Oracle: " + ex.Message, ex);
            }

            return dt;
        }

        /// <summary>
        /// Thực thi một câu lệnh SQL SELECT và trả về một giá trị
        /// Dùng cho các hàm OLS như FN_BUILD_LABEL
        /// </summary>
        public static object ExecuteScalar(string connectionString, string query, OracleParameter[] parameters = null)
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.CommandType = CommandType.Text;

                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        conn.Close();

                        return result;
                    }
                }
            }
            catch (OracleException ex)
            {
                throw new Exception("Lỗi Oracle: " + ex.Message, ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kết nối Oracle: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Lấy giá trị chuỗi Label từ hàm FN_BUILD_LABEL
        /// Dùng để hiển thị preview Label trong form
        /// </summary>
        public static string GetBuildLabel(string connectionString, string level, string compartment, string group)
        {
            try
            {
                string query = "SELECT ADMIN.FN_BUILD_LABEL(:lv, :cp, :gr) FROM DUAL";

                OracleParameter[] parameters = new OracleParameter[]
                {
            new OracleParameter(":lv", OracleDbType.Varchar2) { Value = level ?? "" },
            new OracleParameter(":cp", OracleDbType.Varchar2) { Value = compartment ?? "" },  // "" thay vì DBNull
            new OracleParameter(":gr", OracleDbType.Varchar2) { Value = group ?? "" }         // "" thay vì DBNull
                };

                object result = ExecuteScalar(connectionString, query, parameters);
                return result?.ToString() ?? level; // fallback về level nếu lỗi
            }
            catch (Exception ex)
            {
                // Log ra để debug thay vì nuốt lỗi
                System.Diagnostics.Debug.WriteLine($"GetBuildLabel error: {ex.Message}");
                return level; // fallback về level
            }
        }
        /// <summary>
        /// Lấy tên user hiện tại từ session
        /// SELECT SYS_CONTEXT('USERENV','SESSION_USER') FROM DUAL
        /// </summary>
        public static string GetCurrentUser(string connectionString)
        {
            try
            {
                string query = "SELECT SYS_CONTEXT('USERENV','SESSION_USER') FROM DUAL";
                object result = ExecuteScalar(connectionString, query);
                return result?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}
