using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Input;

namespace RADARMRM
{
    public partial class Form1 : Form
    {
        //SqlClient sql;
        List<Radar> radarList = new List<Radar>();
        int progress = 0;
        int connectDelay = 5;
        bool blink = false;
        bool sirenBlink = false;
        bool radarFindTarget = false;
        bool vibFindTarget = false;
        string[] radarIp = new string[3];
        string localIp;
        TcpListener tcpServer;
        TcpClient tcpReceiver;
        NetworkStream tcpStream;
        UdpClient udpServer;
        IPEndPoint remoteIpEnd;
        Task receivingTask, udpReceiveTask;
        bool receivingRunning = true;
        SerialPort port = new SerialPort();
        byte[] serialBuffer = new byte[32];
        int bytesRead;
 //       bool expectingZero;
        bool vibReady = false;
        bool vibRunning = false;
        bool showMessageBox = false;
        int vibResult = -1, vibCD = 300;
        string plateNumber, inspectorName;
        int currentStringLength;
        SoundPlayer searchSound = new SoundPlayer("search.wav");
        SoundPlayer passSound = new SoundPlayer("pass.wav");
        int serialProgress = 0;

        string[] plateInitial, inspectors;
        string comName, LedIp;
        string sqlConnectionString = null;
        int ledBaudRate = 115200;
        LedDisplay led;
        int ledClearCD = 0, ledReportCD = 0, restartCD = 0;
        string passText, vibText, radarText;
        bool enableRemoteClick = true;

        int[,] configIntegration = new int[3,6], configSensitivity = new int[3,6];
        Single[,] configStart = new Single[3,6], configRange = new Single[3,6];
        //const float pixelPerMeter = 10;

        //bool lifeStarted = false;
        
        int carType;

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.WindowState = FormWindowState.Normal;
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void connectAllButton_Click(object sender, EventArgs e)
        {
            int i = comboBox1.SelectedIndex;
            radarList[0].LifeAverageDepth = configIntegration[0, i];
            radarList[0].LifeSensitivity = configSensitivity[0, i];
            radarList[0].LifeStart = Convert.ToSingle(configStart[0, i]);
            radarList[0].LifeRange = Convert.ToSingle(configRange[0, i]);
            radarList[1].LifeAverageDepth = configIntegration[1, i];
            radarList[1].LifeSensitivity = configSensitivity[1, i];
            radarList[1].LifeStart = Convert.ToSingle(configStart[1, i]);
            radarList[1].LifeRange = Convert.ToSingle(configRange[1, i]);
            radarList[2].LifeAverageDepth = configIntegration[2, i];
            radarList[2].LifeSensitivity = configSensitivity[2, i];
            radarList[2].LifeStart = Convert.ToSingle(configStart[2, i]);
            radarList[2].LifeRange = Convert.ToSingle(configRange[2, i]);


            //radarList[0].LifeAverageDepth = Convert.ToInt32(IntegrationNumeric.Value);
            //radarList[0].LifeSensitivity = Convert.ToInt32(numericUpDown1.Value);
            //radarList[0].LifeStart = Convert.ToSingle(numericUpDown2.Value);
            //radarList[0].LifeRange = Convert.ToSingle(numericUpDown3.Value);
            //radarList[1].LifeAverageDepth = Convert.ToInt32(IntegrationNumeric1.Value);
            //radarList[1].LifeSensitivity = Convert.ToInt32(numericUpDown7.Value);
            //radarList[1].LifeStart = Convert.ToSingle(numericUpDown6.Value);
            //radarList[1].LifeRange = Convert.ToSingle(numericUpDown5.Value);
            //radarList[2].LifeAverageDepth = Convert.ToInt32(numericUpDown4.Value);
            //radarList[2].LifeSensitivity = Convert.ToInt32(numericUpDown10.Value);
            //radarList[2].LifeStart = Convert.ToSingle(numericUpDown9.Value);
            //radarList[2].LifeRange = Convert.ToSingle(numericUpDown8.Value);

            graphicTimer.Start();
            led.LedSend("等待检测");
            led.LedSendSound("等待检测");
            restartCD = 10;
            progress = 0;
            connectDelay = 5;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            checkBox3.Enabled = false;
            startButton.Enabled = false;
            foreach (Radar r in radarList)
            {
                r.OperationMode = 1;
                r.MRMCodeChannel = (byte)(radarList.IndexOf(r) + 1);
                r.ConfigurationRequest();
            }
        }

        public void UpdateStatusSync(string msg)
        {
            statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + msg + Environment.NewLine);
        }

        public void UpdateStatus(string msg)
        {          
            //if (this.InvokeRequired)
            try
            {
                BeginInvoke((MethodInvoker)(() => statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + msg + Environment.NewLine)));
            }
            catch (InvalidOperationException) { }
                //else
                //statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + msg + Environment.NewLine);
        }

        public void UpdateReport(string msg)
        {
            BeginInvoke((MethodInvoker)(() => reportText.AppendText(msg)));
        }

        public void UpdateData(int[] data)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                chart2.Series[0].Points.DataBindY(data);// why 0 no 1

            }));
        }

        public void UpdateRData(int[] data)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                chart2.Series[1].Points.DataBindY(data);// why 0 no 1

            }));
        }

        public void Updatechart2_XAxis(int scanLength)
        {
            BeginInvoke((MethodInvoker)(() =>
            {
                chart2.ChartAreas[0].AxisX.Maximum = scanLength;// why 0 no 1

            }));            
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            enableRemoteClick = false;
            plateNumber = (string)plateInitialCombo.SelectedItem + plateNumberText.Text;
            inspectorName = (string)inspectorCombo.SelectedItem;
            startButton.Enabled = false;
            startButton.BackColor = Color.Red;
            radarFindTarget = false;
            vibFindTarget = false;
            string tempDisplay = String.Format("{0,-20}{1,-14}{2,-25}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "开始扫描", inspectorName);
            currentStringLength = tempDisplay.Length;
            reportText.AppendText(tempDisplay);

            int i = comboBox1.SelectedIndex;
            radarList[0].LifeAverageDepth = configIntegration[0, i];
            radarList[0].LifeSensitivity = configSensitivity[0, i];
            radarList[0].LifeStart = Convert.ToSingle(configStart[0, i]);
            radarList[0].LifeRange = Convert.ToSingle(configRange[0, i]);
            radarList[1].LifeAverageDepth = configIntegration[1, i];
            radarList[1].LifeSensitivity = configSensitivity[1, i];
            radarList[1].LifeStart = Convert.ToSingle(configStart[1, i]);
            radarList[1].LifeRange = Convert.ToSingle(configRange[1, i]);
            radarList[2].LifeAverageDepth = configIntegration[2, i];
            radarList[2].LifeSensitivity = configSensitivity[2, i];
            radarList[2].LifeStart = Convert.ToSingle(configStart[2, i]);
            radarList[2].LifeRange = Convert.ToSingle(configRange[2, i]);

            //radarList[0].LifeAverageDepth = Convert.ToInt32(IntegrationNumeric.Value);
            //radarList[0].LifeSensitivity = Convert.ToInt32(numericUpDown1.Value);
            //radarList[0].LifeStart = Convert.ToSingle(numericUpDown2.Value);
            //radarList[0].LifeRange = Convert.ToSingle(numericUpDown3.Value);
            //radarList[1].LifeAverageDepth = Convert.ToInt32(IntegrationNumeric1.Value);
            //radarList[1].LifeSensitivity = Convert.ToInt32(numericUpDown7.Value);
            //radarList[1].LifeStart = Convert.ToSingle(numericUpDown6.Value);
            //radarList[1].LifeRange = Convert.ToSingle(numericUpDown5.Value);
            //radarList[2].LifeAverageDepth = Convert.ToInt32(numericUpDown4.Value);
            //radarList[2].LifeSensitivity = Convert.ToInt32(numericUpDown10.Value);
            //radarList[2].LifeStart = Convert.ToSingle(numericUpDown9.Value);
            //radarList[2].LifeRange = Convert.ToSingle(numericUpDown8.Value);
            foreach (Radar r in radarList)
            {
                r.LifeDetectedDistance = -1;
            }
            if (radarList[0].MRMBackgroundReady && checkBox1.Checked)
                radarList[0].StartLife();
            if (radarList[1].MRMBackgroundReady && checkBox2.Checked)
                radarList[1].StartLife();
            if (radarList[2].MRMBackgroundReady && checkBox3.Checked)
                radarList[2].StartLife();
            if (vibReady && checkBox4.Checked)
            {
                byte[] buffer = new byte[9];
                buffer[0] = 0;
                Buffer.BlockCopy(BitConverter.GetBytes(844), 0, buffer, 1, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                udpServer.Send(buffer, 9, remoteIpEnd);
                vibResult = -1;
                vibCD = 300;
                vibRunning = true;
            }
            progress = 1;
            led.LedSend("开始扫描");
            led.LedSendSound("开始扫描");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Radar r in radarList)
            {
                if (r.LifeRunning)
                    r.Stop();                
            }
            receivingRunning = false;
            if (tcpReceiver != null)
                tcpReceiver.Close();
            //sql.Close();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {            
            radarFindTarget = false;
            vibFindTarget = false;
            ScanningPic.Refresh();
            foreach (Radar r in radarList)
            {
                if (r.LifeRunning)
                    r.Stop();
            }
            vibRunning = false;
            byte[] buffer = new byte[9];
            buffer[0] = 1;
            
            if (udpServer != null)
            {
                cancelTimer.Start();
                udpServer.Send(buffer, 9, remoteIpEnd);
            }
            enableRemoteClick = true;
            if (progress == 2)
            {
                string tempDisplay = String.Format("{0,-20}{1,-14}{2,-19}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "检测终止，请重新操作", inspectorName);
                currentStringLength = 0;
                reportText.AppendText(tempDisplay + Environment.NewLine);
                startButton.Enabled = true;
                startButton.BackColor = SystemColors.Control;
                restartCD = 50;
                enableRemoteClick = true;
                progress = 3;
                led.LedSend("检测终止，请重新操作");
                led.LedSendSound("检测终止，请重新操作");
            }            
            //led.LedSendText("停止检测,本次检测异常,请重新检测");
        }        

        //private void ScanningPic_Paint(object sender, PaintEventArgs e)
        //{
        //    if (blink)
        //    {
        //        if (radarList[0].LifeRunning)
        //        {
        //            int width = 251 / radarList[0].LifeSections;
        //            int start = 34 + 251 * radarList[0].CurrentLifeSection / radarList[0].LifeSections;
        //            e.Graphics.FillRectangle(new SolidBrush(Color.Blue), start, 14, width, 109);                    
        //        }
        //        if (radarList[1].LifeRunning)
        //        {
        //            int height = 109 / radarList[1].LifeSections;
        //            int start = 14 + 109 * radarList[1].CurrentLifeSection / radarList[1].LifeSections;
        //            e.Graphics.FillRectangle(new SolidBrush(Color.Blue), 34, start, 125, height);
        //        }
        //        if (radarList[2].LifeRunning)
        //        {
        //            int height = 109 / radarList[2].LifeSections;
        //            int start = 14 + 109 * radarList[2].CurrentLifeSection / radarList[2].LifeSections;
        //            e.Graphics.FillRectangle(new SolidBrush(Color.Blue), 159, start, 126, height);
        //        }
        //    }

        //    if (radarFindTarget)
        //    {                
        //        if (radarList[0].LifeDetectedDistance != -1)
        //        {
        //            int position = (int)((radarList[0].LifeDetectedDistance - radarList[0].LifeStart) / radarList[0].LifeRange * 251 + 34);
        //            //String distance = radarList[0].LifeDetectedDistance.ToString("#.0") + "m";
        //            e.Graphics.FillRectangle(new SolidBrush(Color.Red), position - 5, 14, 10, 109);
        //            //e.Graphics.DrawString(distance, new Font("Ariel", 16), new SolidBrush(Color.Red), position - 25, 88);
        //        }
        //        if (radarList[1].LifeDetectedDistance != -1)
        //        {
        //            int position = (int)((radarList[1].LifeDetectedDistance - radarList[1].LifeStart) / radarList[1].LifeRange * 109 + 14);
        //            e.Graphics.FillRectangle(new SolidBrush(Color.Red), 34, position - 5, 125, 10);                    
        //        }
        //        if (radarList[2].LifeDetectedDistance != -1)
        //        {
        //            int position = (int)((radarList[2].LifeDetectedDistance - radarList[2].LifeStart) / radarList[2].LifeRange * 109 + 14);
        //            e.Graphics.FillRectangle(new SolidBrush(Color.Red), 159, position - 5, 126, 10);
        //        }
        //    }            
        //}

        private void graphicTimer_Tick(object sender, EventArgs e)
        {
            string sqlOverall = null, sqlRadar = null, sqlVibration = null;
            //if (ledClearCD-->0)
            //{
            //    if (ledClearCD == 0)
            //    {
            //        led.LedClear();
            //        led.LedSetTime();
            //    }
                    
            //}
            switch (progress)
            {
                case 0: //waiting connect                    
                    int readyNO = 0;
                    if (radarList[0].MRMBackgroundReady)
                    {
                        readyNO++;
                        checkBox1.Enabled = true;
                    }
                    if (radarList[1].MRMBackgroundReady)
                    {
                        readyNO++;
                        checkBox2.Enabled = true;
                    }
                    if (radarList[2].MRMBackgroundReady)
                    {
                        readyNO++;
                        checkBox3.Enabled = true;
                    }
                    if (vibReady == true)
                    {
                        readyNO++;
                        checkBox4.Enabled = true;
                    }
                    connectDelay--;
                    if ((connectDelay == 0) || (readyNO == 4))
                    {                        
                        if (readyNO != 0)
                        {
                            progress = 3;
                            connectAllButton.BackColor = Color.Green;
                            startButton.Enabled = true;
                        }
                    }
                    break;
                case 3: //idle state
                    if (restartCD-- == 0)
                    {
                        led.LedSend("准备扫描");
                        led.LedSendSound("准备扫描");
                    }
                    //led.LedSendText("准备扫描，请工作人员站到指定区域");
                    //led.LedClear();
                    if (restartCD < 0)
                        restartCD = -1;
                    break;
                case 1: //waiting for start state                    
                    //ledReportCD = 10;
                    foreach (Radar r in radarList)
                    {
                        if (r.LifeRunning)
                        {
                            progress = 2;
                            break; //when one radar start scanning, switch to scan monitor
                        }
                    }
                    if (vibRunning)
                        progress = 2;
                    break;
                case 2: //scan monitor
                    /*if (ledReportCD-- == 10)
                        led.LedSendText("正在扫描");
                    if (ledReportCD == 0)
                        ledReportCD = 10;*/
                    string report1 = "号雷达";
                    int textAlignReduction = 0;
                    int runningNO = 0;

                    if (vibRunning)
                        vibCD--;
                    if (vibCD == 0)
                    {
                        vibRunning = false;
                        statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "微震响应超时" + Environment.NewLine);
                        vibCD = -1;
                    }
                    if (vibResult != -1)
                        vibRunning = false;

                    if (blink)
                        blink = false;
                    else
                        blink = true;
                    if (radarList[2].LifeRunning)
                    {
                        runningNO++;
                        report1 = "3" + report1;
                    }
                    if (radarList[1].LifeRunning)
                    {                        
                        if (runningNO > 0)
                            report1 = "," + report1;
                        report1 = "2" + report1;
                        runningNO++;
                    }
                    if (radarList[0].LifeRunning)
                    {                        
                        if (runningNO > 0)
                            report1 = "," + report1;
                        report1 = "1" + report1;
                        runningNO++;
                    }
                    if (runningNO > 0 && radarProgress.Style != ProgressBarStyle.Marquee)
                        radarProgress.Style = ProgressBarStyle.Marquee;
                    if (vibRunning)
                    {
                        if (runningNO > 0)
                        {
                            report1 = "微震," + report1;
                            textAlignReduction = 8;
                        }
                        else
                        {
                            report1 = "微震";
                            textAlignReduction = 5;
                        }
                        runningNO++;
                    }
                    if (runningNO > 0)
                    {
                        report1 = report1 + "检测中";
                        reportText.Text = reportText.Text.Remove(reportText.Text.Length - currentStringLength);
                        //string tempDisplay = String.Format("{0,-20}{1,-14}{2,-" + (29 - textAlignReduction).ToString() + "}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, report1, inspectorName);

                        string tempDisplay = String.Format("{0,-20}{1,-14}{2,-25}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "正在检测", inspectorName);

                        currentStringLength = tempDisplay.Length;
                        reportText.AppendText(tempDisplay);
                    }
                    
                    if (!radarFindTarget)
                    {
                        foreach (Radar r in radarList)
                        {
                            if (r.LifeDetectedDistance != -1)
                            {
                                radarFindTarget = true;
                                ledClearCD = 300;
                                //led.LedSend(radarText);
                                break;
                            }
                        }  
                    } 
                    if (!vibFindTarget)
                    {
                        if (vibResult == 2)
                        {
                            ledClearCD = 300;
                            led.LedSend(vibText);
                            led.LedSendSound(vibText);
                            vibFindTarget = true;
                        }
                    }                   
                    if (runningNO == 0)
                    {
                        if (radarProgress.Style == ProgressBarStyle.Marquee)
                            radarProgress.Style = ProgressBarStyle.Blocks;
                        reportText.Text = reportText.Text.Remove(reportText.Text.Length - currentStringLength);
                        string displayString = null;
                        string deviceString = null;
                        if (vibFindTarget || (!checkBox4.Checked && radarFindTarget))
                        {
                            sqlOverall = "报警";
                            if (radarFindTarget)
                            {
                                sqlRadar = "号雷达检测到生命迹象";
                                if (radarList[2].LifeDetectedDistance != -1)
                                    sqlRadar = "3" + sqlRadar;
                                if (radarList[1].LifeDetectedDistance != -1)
                                    sqlRadar = "2" + sqlRadar;
                                if (radarList[0].LifeDetectedDistance != -1)
                                    sqlRadar = "1" + sqlRadar;
                            }
                            if (vibFindTarget)
                                sqlVibration = "微震检测到生命迹象";
                            if (radarFindTarget)
                            {
                                if (!vibFindTarget)
                                    deviceString = "（雷达）";
                            }
                            else if (vibFindTarget)
                                deviceString = "（微震）";

                            searchSound.Play();
                            //if (deviceString == null)
                            //    displayString = String.Format("{0,-20}{1,-14}{2,-20}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "检测停止，检测异常", inspectorName);
                            //else
                            //    displayString = String.Format("{0,-20}{1,-14}{2,-16}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "检测停止，检测异常" + deviceString, inspectorName);

                            displayString = String.Format("{0,-20}{1,-14}{2,-19}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "检测异常，请人工检查", inspectorName);

                            led.LedSend("检测结束，卸货检查");
                            led.LedSendSound("检测结束卸货检查");
                            //led.LedSend("检测结束,本次检测异常,请人工排查");
                            showMessageBox = true;
                        }
                        else if (vibCD == -1)
                        {
                            statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "串口通信超时，请检查微震串口通信或重启程序" + Environment.NewLine);
                            displayString = String.Format("{0,-20}{1,-14}{2,-20}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "检测停止，系统故障", inspectorName);

                            displayString = String.Format("{0,-20}{1,-14}{2,-12}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "通信超时，请检查微震服务器通信连接", inspectorName);
                            led.LedSend("通信超时，请检查微震服务器通信连接");
                            led.LedSendSound("通信超时请检查微震服务器通信连接");
                        }
                        else if (vibResult == 3)
                        {
                            displayString = String.Format("{0,-20}{1,-14}{2,-19}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "操作错误，请重新检测", inspectorName);
                            led.LedSend("操作错误，请重新检测");
                            led.LedSendSound("操作错误请重新检测");
                        }
                        else
                        {
                            sqlOverall = "放行";
                            passSound.Play();
                            ledClearCD = 300;
                            led.LedSend(passText);
                            led.LedSendSound(passText);
                            //displayString = String.Format("{0,-20}{1,-14}{2,-20}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "检测结束，检测正常", inspectorName);

                            displayString = String.Format("{0,-20}{1,-14}{2,-21}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "检测结束，请放行", inspectorName);

                            //led.LedSend("请放行");
                        }

                        //sql.Insert(sqlOverall, sqlRadar, sqlVibration, plateInitial + plateNumber);
                        m_pDbSql.InsertRaderMessage(sqlOverall, sqlRadar, sqlVibration, plateInitial + plateNumber);
                        currentStringLength = 0;
                        reportText.AppendText(displayString + Environment.NewLine);
                        startButton.Enabled = true;
                        startButton.BackColor = SystemColors.Control;
                        restartCD = 50;
                        enableRemoteClick = true;
                        progress = 3;

                        if (logCheck.Checked)
                        {
                            try
                            {
                                using (StreamWriter sw = new StreamWriter("log.txt", true))
                                {
                                    sw.WriteLine(displayString);
                                }
                            }
                            catch (Exception)
                            {
                                statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "记录日志失败" + Environment.NewLine);
                            }
                        }
                    }                    
                    //ScanningPic.Refresh();
                    if (showMessageBox)
                    {
                        showMessageBox = false;
                        graphicTimer.Enabled = false;
                        MessageBox.Show("检测结束,本次检测异常,请人工排查");
                        graphicTimer.Enabled = true;
                    }
                    break;
                default:
                    break;
            }
            if (vibFindTarget || (!checkBox4.Checked && radarFindTarget))
            {
                sirenBlink = !sirenBlink;
                if (sirenBlink)
                    pictureBox1.Image = RADARMRM.Properties.Resources.emergency_on;
                else
                    pictureBox1.Image = RADARMRM.Properties.Resources.emergency_off;
            }
            else
                pictureBox1.Image = RADARMRM.Properties.Resources.emergency_off;

            pictureBox1.Refresh();
            //if (radarList[0].LifeRunning)
            //{
            //    lifeStarted = true;
            //    reportText.Text = reportText.Text.Remove(reportText.Text.Length - 81);
            //    if (radarList[0].LifeDetectedDistance != -1)
            //    {
            //        reportText.AppendText(String.Format("{0,-24}{1,-10}{2,-29}{3,-17}\n", DateTime.Now, "XXXXXXX", "发现生命体", "XXX"));
            //    }
            //    else
            //    {
            //        reportText.AppendText(String.Format("{0,-24}{1,-10}{2,-22}{3,-24}\n", DateTime.Now, "XXXXXXX", "正在扫描" + radarList[0].LifeSections.ToString() + "段中第" + (radarList[0].CurrentLifeSection + 1).ToString() + "段", "XXX"));
            //    }
            //}
            //else if (lifeStarted)
            //{
            //    lifeStarted = false;
            //    reportText.Text = reportText.Text.Remove(reportText.Text.Length - 81);
            //    reportText.AppendText(String.Format("{0,-24}{1,-10}{2,-21}{3,-8}\n", DateTime.Now, "XXXXXXX", "完成，未发现生命体", "XXX"));
            //}

        }

        public void RemoteStartAsync()
        {
            this.BeginInvoke((MethodInvoker)RemoteStart);
        }

        private void RemoteStart()
        {
            enableRemoteClick = false;
            plateNumber = (string)plateInitialCombo.SelectedItem + plateNumberText.Text;
            inspectorName = (string)inspectorCombo.SelectedItem;
            startButton.Enabled = false;
            startButton.BackColor = Color.Red;
            radarFindTarget = false;
            vibFindTarget = false;
            string tempDisplay = String.Format("{0,-20}{1,-14}{2,-25}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "开始扫描", inspectorName);
            currentStringLength = tempDisplay.Length;
            reportText.AppendText(tempDisplay);
            //radarList[0].LifeAverageDepth = Convert.ToInt32(IntegrationNumeric.Value);
            //radarList[0].LifeSensitivity = Convert.ToInt32(numericUpDown1.Value);
            //radarList[0].LifeStart = Convert.ToSingle(numericUpDown2.Value);
            //radarList[0].LifeRange = Convert.ToSingle(numericUpDown3.Value);
            //radarList[1].LifeAverageDepth = Convert.ToInt32(IntegrationNumeric1.Value);
            //radarList[1].LifeSensitivity = Convert.ToInt32(numericUpDown7.Value);
            //radarList[1].LifeStart = Convert.ToSingle(numericUpDown6.Value);
            //radarList[1].LifeRange = Convert.ToSingle(numericUpDown5.Value);
            //radarList[2].LifeAverageDepth = Convert.ToInt32(numericUpDown4.Value);
            //radarList[2].LifeSensitivity = Convert.ToInt32(numericUpDown10.Value);
            //radarList[2].LifeStart = Convert.ToSingle(numericUpDown9.Value);
            //radarList[2].LifeRange = Convert.ToSingle(numericUpDown8.Value);
            //if (radarList[0].MRMBackgroundReady && checkBox1.Checked)
            //    radarList[0].StartLife();
            //if (radarList[1].MRMBackgroundReady && checkBox2.Checked)
            //    radarList[1].StartLife();
            //if (radarList[2].MRMBackgroundReady && checkBox3.Checked)
            //    radarList[2].StartLife();
            //if (vibReady && checkBox4.Checked)
            //{
            //    byte[] buffer = new byte[9];
            //    buffer[0] = 0;
            //    Buffer.BlockCopy(BitConverter.GetBytes(844), 0, buffer, 1, 4);
            //    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
            //    udpServer.Send(buffer, 9, remoteIpEnd);
            //    vibResult = -1;
            //    vibCD = 300;
            //    vibRunning = true;
            //}
            led.LedSend("开始扫描");
            led.LedSendSound("开始扫描");
            //lifeStarted = true;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            FrmConfig frm = new FrmConfig();
           
            int n = comboBox1.SelectedIndex;
            frm.cfgInt[0] = configIntegration[0, n];
            frm.cfgInt[1] = configSensitivity[0, n];
            frm.cfgFloat[0] = Convert.ToSingle(configStart[0, n]);
            frm.cfgFloat[1] = Convert.ToSingle(configRange[0, n]);
            frm.cfgInt[2] = configIntegration[1, n];
            frm.cfgInt[3] = configSensitivity[1, n];
            frm.cfgFloat[2] = Convert.ToSingle(configStart[1, n]);
            frm.cfgFloat[3] = Convert.ToSingle(configRange[1, n]);
            frm.cfgInt[4] = configIntegration[2, n];
            frm.cfgInt[5] = configSensitivity[2, n];
            frm.cfgFloat[4] = Convert.ToSingle(configStart[2, n]);
            frm.cfgFloat[5] = Convert.ToSingle(configRange[2, n]);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                string temp;
                int index = comboBox1.SelectedIndex;
                configIntegration[0, index] = frm.cfgInt[0];
                configSensitivity[0, index] = frm.cfgInt[1];
                configStart[0, index] = frm.cfgFloat[0];
                configRange[0, index] = frm.cfgFloat[1];
                configIntegration[1, index] = frm.cfgInt[2];
                configSensitivity[1, index] = frm.cfgInt[3];
                configStart[1, index] = frm.cfgFloat[2];
                configRange[1, index] = frm.cfgFloat[3];
                configIntegration[2, index] = frm.cfgInt[4];
                configSensitivity[2, index] = frm.cfgInt[5];
                configStart[2, index] = frm.cfgFloat[4];
                configRange[2, index] = frm.cfgFloat[5];

                try
                {
                    using (StreamWriter sw = new StreamWriter("config.txt", false))
                    {
                        sw.WriteLine(sqlConnectionString);
                        sw.WriteLine(comName);
                        sw.WriteLine(LedIp);
                        sw.WriteLine(ledBaudRate.ToString());
                        sw.WriteLine(localIp);
                        for (int i = 0; i < 3; i++)
                        {
                            sw.WriteLine(radarIp[i]);
                            temp = "";
                            for (int j = 0; j < 5; j++)
                                temp += configIntegration[i, j] + " ";
                            sw.WriteLine(temp);
                            temp = "";
                            for (int j = 0; j < 5; j++)
                                temp += configSensitivity[i, j] + " ";
                            sw.WriteLine(temp);
                            temp = "";
                            for (int j = 0; j < 5; j++)
                                temp += configStart[i, j] + " ";
                            sw.WriteLine(temp);
                            temp = "";
                            for (int j = 0; j < 5; j++)
                                temp += configRange[i, j] + " ";
                            sw.WriteLine(temp);
                        }
                        sw.WriteLine(comboBox1.SelectedIndex.ToString());
                        temp = "";
                        foreach (string s in plateInitial)
                        {
                            temp += s + " ";
                        }
                        sw.WriteLine(temp);
                        temp = "";
                        foreach (string s in inspectors)
                        {
                            temp += s + " ";
                        }
                        sw.WriteLine(temp);
                        sw.WriteLine(passText);
                        sw.WriteLine(vibText);
                        sw.WriteLine(radarText);
                    }
                }
                catch (Exception)
                {
                    statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "保存配置失败" + Environment.NewLine);
                }
            }






            //string temp;
            //int index = comboBox1.SelectedIndex;
            //configIntegration[0,index] = Convert.ToInt32(IntegrationNumeric.Value);
            //configSensitivity[0,index] = Convert.ToInt32(numericUpDown1.Value);
            //configStart[0,index] = Convert.ToSingle(numericUpDown2.Value);
            //configRange[0,index] = Convert.ToSingle(numericUpDown3.Value);
            //configIntegration[1,index] = Convert.ToInt32(IntegrationNumeric1.Value);
            //configSensitivity[1,index] = Convert.ToInt32(numericUpDown7.Value);
            //configStart[1,index] = Convert.ToSingle(numericUpDown6.Value);
            //configRange[1,index] = Convert.ToSingle(numericUpDown5.Value);
            //configIntegration[2,index] = Convert.ToInt32(numericUpDown4.Value);
            //configSensitivity[2,index] = Convert.ToInt32(numericUpDown10.Value);
            //configStart[2,index] = Convert.ToSingle(numericUpDown9.Value);
            //configRange[2,index] = Convert.ToSingle(numericUpDown8.Value);

            //try
            //{
            //    using (StreamWriter sw = new StreamWriter("config.txt", false))
            //    {
            //        sw.WriteLine(sqlConnectionString);
            //        sw.WriteLine(comName);
            //        sw.WriteLine(LedComName);
            //        sw.WriteLine(ledBaudRate.ToString());
            //        sw.WriteLine(localIp);
            //        for (int i = 0; i < 3; i++)
            //        {
            //            sw.WriteLine(radarIp[i]);
            //            temp = "";
            //            for (int j = 0; j < 5; j++)
            //                temp += configIntegration[i, j] + " ";
            //            sw.WriteLine(temp);
            //            temp = "";
            //            for (int j = 0; j < 5; j++)
            //                temp += configSensitivity[i, j] + " ";
            //            sw.WriteLine(temp);
            //            temp = "";
            //            for (int j = 0; j < 5; j++)
            //                temp += configStart[i, j] + " ";
            //            sw.WriteLine(temp);
            //            temp = "";
            //            for (int j = 0; j < 5; j++)
            //                temp += configRange[i, j] + " ";
            //            sw.WriteLine(temp);
            //        }                    
            //        sw.WriteLine(comboBox1.SelectedIndex.ToString());
            //        temp = "";
            //        foreach(string s in plateInitial)
            //        {
            //            temp += s + " ";
            //        }
            //        sw.WriteLine(temp);
            //        temp = "";
            //        foreach (string s in inspectors)
            //        {
            //            temp += s + " ";
            //        }
            //        sw.WriteLine(temp);
            //        sw.WriteLine(passText);
            //        sw.WriteLine(vibText);
            //        sw.WriteLine(radarText);
            //    }
            //}
            //catch (Exception)
            //{
            //    statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "保存配置失败" + Environment.NewLine);
            //}





            //byte averageDepth1 = (byte)IntegrationNumeric.Value;
            //byte sensitivity1 = (byte)numericUpDown1.Value;
            //byte mrmRange1 = (byte)((Convert.ToSingle(numericUpDown3.Value) + 0.7) / 0.8);
            //int start1 = 11000 + (int)(Convert.ToSingle(numericUpDown2.Value) * 6671);
            //byte averageDepth2 = (byte)IntegrationNumeric1.Value;
            //byte sensitivity2 = (byte)numericUpDown7.Value;
            //byte mrmRange2 = (byte)((Convert.ToSingle(numericUpDown5.Value) + 0.7) / 0.8);
            //int start2 = 11000 + (int)(Convert.ToSingle(numericUpDown6.Value) * 6671);
            //radar.RemoteRequest(mrmRange1, sensitivity1, averageDepth1, start1, mrmRange2, sensitivity2, averageDepth2, start2);
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                label10.BackColor = Color.FromArgb(128, 128, 255);
            }
            else
            {
                label10.BackColor = Color.FromArgb(255, 0, 0);
            }
        }

        
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (false == m_lgUser.isAdmin())
            {
                button1.Enabled = false;
            }

            logoPicture.Image = new Bitmap("logo.png");
            remotePicture.Image = new Bitmap("MVRemote.jpg");
            reportText.AppendText(Environment.NewLine + String.Format("{0,-20}{1,-15}{2,-29}{3}\n", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), "XXXXXXX", "1,2,3", "XXX"));
            try
            {
                using (StreamReader sr = new StreamReader("config.txt"))
                {
                    String[] args;
                    sqlConnectionString = sr.ReadLine();
                    comName = sr.ReadLine();
                    LedIp = sr.ReadLine();
                    ledBaudRate = int.Parse(sr.ReadLine());
                    localIp = sr.ReadLine();
                    for (int i = 0; i < 3; i++)
                    {
                        radarIp[i] = sr.ReadLine();
                        args = sr.ReadLine().Split(new char[] { ' ', ',' });
                        for (int j = 0; j < 5; j++)
                            configIntegration[i, j] = int.Parse(args[j]);
                        args = sr.ReadLine().Split(new char[] { ' ', ',' });
                        for (int j = 0; j < 5; j++)
                            configSensitivity[i, j] = int.Parse(args[j]);
                        args = sr.ReadLine().Split(new char[] { ' ', ',' });
                        for (int j = 0; j < 5; j++)
                            configStart[i, j] = Single.Parse(args[j]);
                        args = sr.ReadLine().Split(new char[] { ' ', ',' });
                        for (int j = 0; j < 5; j++)
                            configRange[i, j] = Single.Parse(args[j]);
                        Radar r = new Radar(new IPEndPoint(IPAddress.Parse(radarIp[i]), 21000), this, i + 1);
                        r.OperationMode = 1;
                        r.MRMCodeChannel = (byte)(i + 1);
                        radarList.Add(r);
                    }
                    comboBox1.SelectedIndex = int.Parse(sr.ReadLine());
                    carType = comboBox1.SelectedIndex;
                    plateInitial = sr.ReadLine().Split(new char[] { ' ', ',' });
                    foreach (string s in plateInitial)
                    {
                        plateInitialCombo.Items.Add(s);
                    }
                    plateInitialCombo.SelectedIndex = 0;
                    inspectors = sr.ReadLine().Split(new char[] { ' ', ',' });
                    foreach (string s in inspectors)
                    {
                        inspectorCombo.Items.Add(s);
                    }
                    inspectorCombo.SelectedIndex = 0;
                    passText = sr.ReadLine();
                    vibText = sr.ReadLine();
                    radarText = sr.ReadLine();
                }
            }
            catch (Exception)
            {
                statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "配置文件读取异常" + Environment.NewLine);
            }
            led = new LedDisplay(this, LedIp);
            port.PortName = comName;
            port.BaudRate = 19200;
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.DataBits = 8;
            port.Handshake = Handshake.None;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try { port.Open(); }
            catch (Exception)
            {
                statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "串口打开失败，微震无法正常使用" + Environment.NewLine);
                graphicTimer.Start();
                connectAllButton.Enabled = true;
                timeTimer.Start();
                return;
            }
            bytesRead = -1;
            try
            {
                tcpServer = new TcpListener(IPAddress.Parse(localIp), 12000);
                tcpServer.Start();
                tcpServer.BeginAcceptTcpClient(new AsyncCallback(endConnection), null);
                udpServer = new UdpClient(new IPEndPoint(IPAddress.Parse(localIp), 13000));
                udpReceiveTask = Task.Factory.StartNew(udpReceiving, TaskCreationOptions.LongRunning);
            }
            catch (Exception) { statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "微震服务器初始化失败，请检查配置文件后重启程序" + Environment.NewLine); }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            
            //sql = new SqlClient(this, sqlConnectionString);

            connectAllButton.Enabled = true;
            timeTimer.Start();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                label12.BackColor = Color.FromArgb(128, 128, 255);
            }
            else
            {
                label12.BackColor = Color.FromArgb(255, 0, 0);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                label13.BackColor = Color.FromArgb(128, 128, 255);
            }
            else
            {
                label13.BackColor = Color.FromArgb(255, 0, 0);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            startButton.Enabled = false;
            int i = comboBox1.SelectedIndex;
            carType = i;
            //IntegrationNumeric.Value = configIntegration[0,i];
            //numericUpDown1.Value = configSensitivity[0, i];
            //numericUpDown2.Value = Convert.ToDecimal(configStart[0, i]);
            //numericUpDown3.Value = Convert.ToDecimal(configRange[0, i]);
            //IntegrationNumeric1.Value = configIntegration[1, i];
            //numericUpDown7.Value = configSensitivity[1, i];
            //numericUpDown6.Value = Convert.ToDecimal(configStart[1, i]);
            //numericUpDown5.Value = Convert.ToDecimal(configRange[1, i]);
            //numericUpDown4.Value = configIntegration[2, i];
            //numericUpDown10.Value = configSensitivity[2, i];
            //numericUpDown9.Value = Convert.ToDecimal(configStart[2, i]);
            //numericUpDown8.Value = Convert.ToDecimal(configRange[2, i]);
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    checkBox1.Checked = true;
                    checkBox2.Checked = false;
                    checkBox3.Checked = false;
                    break;
                case 1:
                    checkBox1.Checked = true;
                    checkBox2.Checked = false;
                    checkBox3.Checked = false;
                    break;
                case 2:
                    checkBox1.Checked = false;
                    checkBox2.Checked = false;
                    checkBox3.Checked = true;
                    break;
                case 3:
                    checkBox1.Checked = false;
                    checkBox2.Checked = true;
                    checkBox3.Checked = true;
                    break;
                case 4:
                    checkBox1.Checked = true;
                    checkBox2.Checked = false;
                    checkBox3.Checked = false;
                    break;
                case 5:
                    checkBox1.Checked = true;
                    checkBox2.Checked = false;
                    checkBox3.Checked = false;
                    break;
            }
            if (vibReady)
            {
                byte[] buffer = new byte[9];
                buffer[0] = 0;
                if (comboBox1.SelectedIndex == 0)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(93), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else if (comboBox1.SelectedIndex == 3)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(442), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else if (comboBox1.SelectedIndex == 4)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(442), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else if (comboBox1.SelectedIndex == 5)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(615), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(266), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                    udpServer.Send(buffer, 9, remoteIpEnd);
                }
        }

        private void endConnection(IAsyncResult ar)
        {
            tcpReceiver = tcpServer.EndAcceptTcpClient(ar);
            tcpStream = tcpReceiver.GetStream();
            this.BeginInvoke((MethodInvoker)(() => this.statusTextBox.AppendText(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "微震主机上线" + Environment.NewLine)));
            receivingRunning = true;
            receivingTask = Task.Factory.StartNew(Receiving, TaskCreationOptions.LongRunning);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FrmUserManage frm = new FrmUserManage(this.m_pDbSql);
            frm.ShowDialog();
        }

        private void Receiving()
        {
            byte[] longBuffer = new byte[1024];
            while (receivingRunning)
            {                
                byte[] buffer = new byte[4];
                try
                {
                    tcpStream.Read(buffer, 0, 4);
                }
                catch (IOException)
                {
                    UpdateStatus("微震主机离线");
                    tcpReceiver.Close();
                    vibReady = false;
                    try
                    {                        
                        tcpServer.Start();
                        tcpServer.BeginAcceptTcpClient(new AsyncCallback(endConnection), null);
                    }
                    catch (Exception) { this.UpdateStatus(String.Format("{0,-20}", DateTime.Now.ToString("yyyy/MM/dd HH:mm")) + "远程tcp连接断开" + Environment.NewLine); }
                    return;
                }
                int len = BitConverter.ToInt32(buffer, 0);
                MemoryStream ms = new MemoryStream(len);
                while (len != 0)
                {
                    int bytesToRead = 1024;
                    if (len < 1024)
                        bytesToRead = len;
                    int readLen = tcpStream.Read(longBuffer, 0, bytesToRead);
                    ms.Write(longBuffer, 0, readLen);
                    len -= readLen;
                }
                //this.BeginInvoke((MethodInvoker)(() => statusText.AppendText("Finish a image.\n")));
                //this.BeginInvoke((MethodInvoker)(() => statusText.AppendText("Start receive image size " + len.ToString() + ".\n")));
                //ns.CopyTo(ms, 256);
                //this.BeginInvoke((MethodInvoker)(() => statusText.AppendText("Finish a image.\n")));
                Bitmap bmpDisplay = new Bitmap(ms);
                this.BeginInvoke((MethodInvoker)(() => remotePicture.Image = bmpDisplay));
            }
        }

        private void udpReceiving()
        {   
            while (receivingRunning)
            {
                byte[] tempbuffer = udpServer.Receive(ref remoteIpEnd);
                UpdateStatus("UDP远程终端刷新");
                vibReady = true;
                byte[] buffer = new byte[9];
                buffer[0] = 0;
                if (carType == 0)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(93), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else if (carType == 3)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(266), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else if (carType == 4)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(442), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else if (carType == 5)
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(615), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                else
                {
                    Buffer.BlockCopy(BitConverter.GetBytes(442), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(55), 0, buffer, 5, 4);
                }
                udpServer.Send(buffer, 9, remoteIpEnd);
            }
        }

        private void remotePicture_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (remoteIpEnd != null)
            {
                byte[] buffer = new byte[9];
                if (e.Button == MouseButtons.Left)
                {
                    buffer[0] = 0;
                    Buffer.BlockCopy(BitConverter.GetBytes(e.X), 0, buffer, 1, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(e.Y), 0, buffer, 5, 4);
                    if ((e.X > 810) && (e.X < 979) && (e.Y > 24) && (e.Y < 86) && (progress == 1))
                    {
                        plateNumber = (string)plateInitialCombo.SelectedItem + plateNumberText.Text;
                        inspectorName = (string)inspectorCombo.SelectedItem;
                        startButton.Enabled = false;
                        startButton.BackColor = Color.Red;
                        vibFindTarget = false;
                        string tempDisplay = String.Format("{0,-20}{1,-14}{2,-25}{3}", DateTime.Now.ToString("yyyy/MM/dd HH:mm"), plateNumber, "开始扫描", inspectorName);
                        currentStringLength = tempDisplay.Length;
                        reportText.AppendText(tempDisplay);
                        vibResult = -1;
                        vibCD = 300;
                        vibRunning = true;
                    }
                }
                //else
                //{
                //    buffer[0] = 1;
                //    cancelTimer.Start();
                //}
                udpServer.Send(buffer, 9, remoteIpEnd);
            }            
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            while (true)
            {
                int buffer = port.ReadByte();
                if (buffer == -1)
                    return;
                else
                {
                    if(buffer!=0)
                    {
                        if (bytesRead == -1)
                        {
                            if (buffer == 0x24)
                                bytesRead = 0;
                        }
                        else
                        {
                            if (buffer == 0x24)
                            {
                                //UpdateStatus("\n" + BitConverter.ToString(serialBuffer, 0, 8) + "\n");
                                if (bytesRead > 2)
                                {
                                    if (serialBuffer[2] == 0x50) //no target found
                                    {
                                        vibResult = 1;
                                        bytesRead = -1;
                                        UpdateStatus("微震未发现目标");
                                    }
                                    else if (serialBuffer[2] == 0x53) //find target
                                    {
                                        vibResult = 2;
                                        bytesRead = -1;
                                        UpdateStatus("微震发现目标");
                                    }
                                    else if (serialBuffer[2] == 0x45)   //cancel
                                    {
                                        vibResult = 3;
                                        bytesRead = -1;
                                        UpdateStatus("微震检测终止");
                                    }
                                    else
                                        bytesRead = 0;
                                }
                                else
                                    bytesRead = 0;
                            }
                            else
                            {                                
                                serialBuffer[bytesRead] = (byte)buffer;
                                bytesRead++;
                                if (bytesRead >= 32)
                                    bytesRead = -1;
                            }
                        }
                    }
                    //if (expectingZero)
                    //{
                    //    if (buffer == 0)
                    //        expectingZero = false;
                    //    else
                    //        bytesRead = -1;
                    //}
                    //else
                    //{
                    //    if (buffer == 0)
                    //        bytesRead = -1;
                    //    else
                    //    {
                    //        expectingZero = true;
                    //        if (bytesRead == -1)
                    //        {
                    //            if (buffer == 0x24)
                    //                bytesRead = 0;
                    //        }
                    //        else
                    //        {
                    //            if (buffer == 0x24)
                    //            {
                    //                //UpdateStatus("\n" + BitConverter.ToString(serialBuffer, 0, 8) + "\n");
                    //                if (bytesRead > 2)
                    //                {
                    //                    if (serialBuffer[2] == 0x50) //no target found
                    //                    {
                    //                        vibResult = 1;
                    //                        UpdateStatus("微震未发现目标");
                    //                    }
                    //                    else if (serialBuffer[2] == 0x53) //find target
                    //                    {
                    //                        vibResult = 2;
                    //                        UpdateStatus("微震发现目标");
                    //                    }
                    //                    else if (serialBuffer[2] == 0x45)   //cancel
                    //                    {
                    //                        vibResult = 3;
                    //                        UpdateStatus("微震检测终止");
                    //                    }
                    //                    else
                    //                        bytesRead = 0;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                if (bytesRead >= 32)
                    //                    bytesRead = -1;
                    //                else
                    //                {
                    //                    serialBuffer[bytesRead] = (byte)buffer;
                    //                    bytesRead++;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                }

            }
        }

        private void cancelTimer_Tick(object sender, EventArgs e)
        {
            cancelTimer.Stop();
            byte[] buffer = new byte[9];
            buffer[0] = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(240), 0, buffer, 1, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(264), 0, buffer, 5, 4);
            udpServer.Send(buffer, 9, remoteIpEnd);
        }

        private void openLogButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("log.txt");
        }

        private void timeTimer_Tick(object sender, EventArgs e)
        {
            timeLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
        }




        private LgUser m_lgUser = new LgUser();
        private MssqlHelper m_pDbSql = null;

        public void SetMssql(MssqlHelper p)
        {
            this.m_pDbSql = p;
        }

        public void SetUserInfo(string sName, string sId, string sPass, string sRole)
        {
            m_lgUser.sName = sName;
            m_lgUser.sId = sId;
            m_lgUser.sPass = sPass;
            m_lgUser.sRole = sRole;

        }

    }
}
