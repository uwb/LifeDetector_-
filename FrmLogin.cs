using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RADARMRM
{
    public partial class FrmLogin : Form
    {
        public string m_sUser;
        public string m_sPass;
        public string m_sRole;
        public string m_sName;
        public MssqlHelper pDbSql = null;

        public FrmLogin()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {
            label1.BackColor = Color.Transparent;
            label1.ForeColor = Color.White;

            label2.BackColor = Color.Transparent;
            label2.ForeColor = Color.White;

            label3.BackColor = Color.Transparent;
            label3.Text = "";

            label4.BackColor = Color.Transparent;
            label4.ForeColor = Color.White;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //this.DialogResult = DialogResult.OK;
            //return;
           
            m_sUser = textBox1.Text.Trim();
            if (m_sUser.Length == 0)
            {
                MessageOut(@"请输入用户账号！");
                return;
            }
            m_sPass = textBox2.Text.Trim();
            if (m_sPass.Length == 0)
            {
                MessageOut(@"请输入用户密码！");
                return;
            }

            MessageOut(@"正在连接数据服务......");
            this.label3.Refresh();

            string strCnn = "";
            using (StreamReader sr = new StreamReader("config.txt"))
            {
                strCnn = sr.ReadLine();
            }
            if (strCnn.Trim().Length == 0)
            {
                MessageOut( @"数据库连接参数为空");
                return;
            }

           pDbSql = new MssqlHelper(strCnn);

            string strSql = "SELECT * FROM BT_USER_REGISTER WHERE UserID='" + m_sUser + "'";
            DataSet ds = pDbSql.GetDataSet(strSql);

            if (ds == null)
            {
                MessageOut(@"数据服务连接失败！请检查系统配置参数。");
                return;
            }

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count == 0)
                {
                    MessageOut( @"输入的用户名称不正确！");
                    
                    return;
                }

                DataRow DR = ds.Tables[0].Rows[0];
                if (DR[2].ToString() != m_sPass)
                {
                    MessageOut(@"输入的用户密码不正确(注意字符大小写)！");
                    
                    return;
                }
                m_sName = DR[3].ToString();
                m_sRole = DR[4].ToString();
            }
            else
            {
                MessageOut(@"用户账号未注册");
                
                return;
            }

            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (pDbSql != null)
            {
                pDbSql.Replace();
            }
            this.DialogResult = DialogResult.Cancel;
            Application.Exit();
        }

        private void MessageOut(string s)
        {
            label3.Text = s;
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            MessageOut("");
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            MessageOut("");
        }


    }
}
