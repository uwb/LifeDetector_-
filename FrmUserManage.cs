using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace RADARMRM
{
    public partial class FrmUserManage : Form
    {
        private MssqlHelper m_pDbMSSql = null;

        private ArrayList m_arryUsers = new ArrayList();
        private int nCurId = -1;

        public FrmUserManage(MssqlHelper p)
        {
            InitializeComponent();
            m_pDbMSSql = p;
        }

        private void FrmUserManage_Load(object sender, EventArgs e)
        {
            InitList();

            UserRole.Items.Add("管理员");
            UserRole.Items.Add("操作员");
            lbmsg.Text = "";
        }

        private void InitList()
        {
            if (m_pDbMSSql == null)
            {
                return;
            }
            string sSql = "SELECT * FROM BT_USER_REGISTER";
            DataSet ds = m_pDbMSSql.GetDataSet(sSql);
            if (ds == null) return;
            if (ds.Tables.Count == 0) return;
            if (ds.Tables[0].Rows.Count == 0) return;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                DataRow DR = ds.Tables[0].Rows[i];

                LgUser lu = new LgUser();
                lu.sIndex = DR[0].ToString();
                lu.sId = DR[1].ToString();
                lu.sPass = DR[2].ToString();
                lu.sName = DR[3].ToString();
                lu.sRole = DR[4].ToString();

                m_arryUsers.Add(lu);

                listBox1.Items.Add(DR[3].ToString() + "("+DR[1].ToString()+")");
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int nSel = listBox1.SelectedIndex;
            if (nSel < 0) return;
            LgUser lu = (LgUser)m_arryUsers[nSel];
            textBox1.Text = lu.sId;
            textBox2.Text = lu.sName;
            int m = Convert.ToInt32(lu.sRole);
            if (m >= 0 && m < 3)
            {
                UserRole.SelectedIndex = m;
            }
            nCurId = nSel;
            lbmsg.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                lbmsg.Text = "用户登录账号不能为空！";
                return;
            }
            if (textBox2.Text == "")
            {
                lbmsg.Text = "请输入用户真实姓名！";
                return;
            }
            if (UserRole.Text == "")
            {
                lbmsg.Text = "请选择用户角色！";
                return;
            }

            lbmsg.Text = "正在保存用户信息...";
            lbmsg.Refresh();

            if (nCurId == -1)
            {
                string strSql = "INSERT INTO BT_USER_REGISTER(UserID,UserPWS,UserName,UserRole) VALUES('" + textBox1.Text + "','1234','" + textBox2.Text + "','" + UserRole.SelectedIndex.ToString() + "')";
                if (m_pDbMSSql.ExecuteSql(strSql))
                {
                    LgUser lu = new LgUser();
                    
                    lu.sId = textBox1.Text;
                    lu.sName = textBox2.Text;
                    lu.sRole = UserRole.SelectedIndex.ToString();

                    m_arryUsers.Add(lu);
                    listBox1.Items.Add(lu.sName + "(" + lu.sId + ")");

                    lbmsg.Text = "完成创建新用户";
                }
                else
                {
                    lbmsg.Text = "创建新用户失败";
                }
                
            }
            else
            {
                int nSel = listBox1.SelectedIndex;
                LgUser lu = (LgUser)m_arryUsers[nSel];

                string strSql = "UPDATE BT_USER_REGISTER SET UserName='" + textBox2.Text + "',UserRole='" + UserRole.SelectedIndex.ToString() + "' WHERE ID=" + lu.sIndex;
                if (m_pDbMSSql.ExecuteSql(strSql))
                {
                    lu.sId = textBox1.Text;
                    lu.sName = textBox2.Text;
                    lu.sRole = UserRole.SelectedIndex.ToString();
                    listBox1.Items[nSel] = lu.sName + "(" + lu.sId + ")";

                    lbmsg.Text = "完成保存用户信息";
                }
                else
                {
                    lbmsg.Text = "保存用户信息失败！";
                }
                
            }

            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sUserId = "";
            int m = 1000;
            while (m-- == 0)
            {
                sUserId = GetRandomArray(9, 4);
                if (TesttingUserID(sUserId))
                    break;
            }

            nCurId = -1;
            textBox1.Text = GetRandomArray(9, 4);
            textBox2.Text = "";
            lbmsg.Text = "";
            listBox1.SelectedIndex = -1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int nSel = listBox1.SelectedIndex;
            if (nSel < 0)
            {
                lbmsg.Text = "请选择要删除的用户...";
                return;
            }
            if (MessageBox.Show("确定删除用户[" + listBox1.Text + "]？删除不可恢复", "信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
            {
                lbmsg.Text = "正在删除用户信息...";
                lbmsg.Refresh();

                LgUser lu = (LgUser)m_arryUsers[nSel];
                string strSql = "DELETE FROM BT_USER_REGISTER WHERE ID=" + lu.sIndex;
                if (m_pDbMSSql.ExecuteSql(strSql))
                {
                    listBox1.Items.RemoveAt(nSel);
                    m_arryUsers.RemoveAt(nSel);
                    m_arryUsers.TrimToSize();

                    textBox1.Text = "";
                    textBox2.Text = "";

                    lbmsg.Text = "完成删除用户信息";
                }
                else
                {
                    lbmsg.Text = "删除成败";
                }
                
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int nSel = listBox1.SelectedIndex;
            if (nSel < 0)
            {
                lbmsg.Text = "请选择需要初始化密码的用户...";
                return;
            }
            if (MessageBox.Show("确定初始化用户[" + listBox1.Text + "]密码吗？", "信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk) == DialogResult.OK)
            {
                lbmsg.Text = "正在初始化密码...";
                lbmsg.Refresh();

                LgUser lu = (LgUser)m_arryUsers[nSel];
                string strSql = "UPDATE BT_USER_REGISTER SET UserPWS='1234' WHERE ID=" + lu.sIndex;
                if (!m_pDbMSSql.ExecuteSql(strSql))
                {
                    lbmsg.Text = @"初始化密码失败！";
                }
                else
                {
                    lbmsg.Text = "用户密码[" + listBox1.Text + "]初始为1234";
                }
            }
        }  

        private string GetRandomArray(int maxNumber, int count)
        {
            string srt = ""; 

            int[] array = new int[maxNumber];   
            for (int i = 0; i < maxNumber; i++) 
                array[i] = i + 1;

            Random rnd = new Random();
            for (int j = 0; j < count; j++)
            {
                int index = rnd.Next(j, maxNumber);   
                int temp = array[index];   
                srt += temp.ToString();
                array[index] = array[j];  
                array[j] = temp;   
            }
            return srt;
        }

        private bool TesttingUserID(string sUserId)
        {
            string strSql = "SELECT * FROM BT_USER_REGISTER WHERE UserID='" + sUserId + "'";
            DataSet ds = m_pDbMSSql.GetDataSet(strSql);

            if (ds == null) return true;
            if (ds.Tables.Count == 0) return true;

            if (ds.Tables[0].Rows.Count != 0) return false;
           
            return true;
        }

        private void UserRole_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
