using ATBM_Project.Data;
using ATBM_Project.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATBM_Project.Presenters
{
    public class AuditPresenter
    {
        public DataTable GetStandardAudit()
        {
            string sql = "SELECT USERNAME, ACTION_NAME, OBJ_NAME, TIMESTAMP, RETURNCODE FROM DBA_AUDIT_TRAIL ORDER BY TIMESTAMP DESC";
            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql); 
        }

        public DataTable GetFGAAudit()
        {
            string sql = "SELECT DB_USER, OBJECT_NAME, POLICY_NAME, SQL_TEXT, TIMESTAMP FROM DBA_FGA_AUDIT_TRAIL ORDER BY TIMESTAMP DESC";
            return OracleHelper.ExecuteTextReader(DBConfig.ConnectionString, sql);
        }
    }
}