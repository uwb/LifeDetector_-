using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RADARMRM
{
    public class LgUser
    {
        public string sIndex;
        public string sName;
        public string sId;
        public string sPass;
        public string sRole;

        public bool isAdmin()
        {
            return (sRole == "0");
        }
        public bool isOperator()
        {
            return (sRole == "1");
        }

    }
}
