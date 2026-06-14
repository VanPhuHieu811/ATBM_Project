using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATBM_Project.Views
{
    public interface ILogoutSupport
    {
        event EventHandler LogoutRequested;
    }
}
