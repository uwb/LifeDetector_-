using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RADARMRM
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            MssqlHelper pDbSql = null;
            FrmLogin frmLogin = new FrmLogin();
            if (frmLogin.ShowDialog() == DialogResult.OK)
            {
                pDbSql = frmLogin.pDbSql;

                Form1 frmMain = new Form1();
                frmMain.SetMssql(pDbSql);
                frmMain.SetUserInfo(frmLogin.m_sName, frmLogin.m_sUser, frmLogin.m_sPass, frmLogin.m_sRole);
                Application.Run(frmMain);
            }

            if (pDbSql != null)
            {
                pDbSql.Replace();
            }

        }
    }
}
