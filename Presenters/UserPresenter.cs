using System.Collections.Generic;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using ATBM_Project.Models;
using ATBM_Project.Database;
using ATBM_Project.Data;

namespace ATBM_Project.Presenters
{
    public class UserPresenter
    {
        public List<UserOracle> GetUserList()
        {
            List<UserOracle> list = new List<UserOracle>();

            using (OracleConnection conn = DBConfig.GetConnection())
            {
                conn.Open();
                
                string sql = "SELECT username, created, account_status FROM dba_users";
                OracleCommand cmd = new OracleCommand(sql, conn);

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UserOracle
                        {
                            Username = reader["USERNAME"].ToString(),
                            CreatedDate = reader["CREATED"].ToString(),
                            AccountStatus = reader["ACCOUNT_STATUS"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}