namespace RADARMRM
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.connectAllButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.statusTextBox = new System.Windows.Forms.TextBox();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.graphicTimer = new System.Windows.Forms.Timer(this.components);
            this.reportText = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.ScanningPic = new System.Windows.Forms.PictureBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.remotePicture = new System.Windows.Forms.PictureBox();
            this.label17 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.inspectorCombo = new System.Windows.Forms.ComboBox();
            this.plateNumberText = new System.Windows.Forms.TextBox();
            this.plateInitialCombo = new System.Windows.Forms.ComboBox();
            this.cancelTimer = new System.Windows.Forms.Timer(this.components);
            this.label18 = new System.Windows.Forms.Label();
            this.logCheck = new System.Windows.Forms.CheckBox();
            this.openLogButton = new System.Windows.Forms.Button();
            this.timeLabel = new System.Windows.Forms.Label();
            this.timeTimer = new System.Windows.Forms.Timer(this.components);
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.logoPicture = new System.Windows.Forms.PictureBox();
            this.radarProgress = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScanningPic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.remotePicture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoPicture)).BeginInit();
            this.SuspendLayout();
            // 
            // connectAllButton
            // 
            this.connectAllButton.AutoSize = true;
            this.connectAllButton.BackColor = System.Drawing.SystemColors.Control;
            this.connectAllButton.Enabled = false;
            this.connectAllButton.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.connectAllButton.Location = new System.Drawing.Point(115, 88);
            this.connectAllButton.Name = "connectAllButton";
            this.connectAllButton.Size = new System.Drawing.Size(75, 25);
            this.connectAllButton.TabIndex = 0;
            this.connectAllButton.Text = "连接";
            this.connectAllButton.UseVisualStyleBackColor = false;
            this.connectAllButton.Click += new System.EventHandler(this.connectAllButton_Click);
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.SystemColors.Control;
            this.startButton.Enabled = false;
            this.startButton.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.startButton.Location = new System.Drawing.Point(196, 88);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(75, 25);
            this.startButton.TabIndex = 2;
            this.startButton.Text = "开始";
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
            this.stopButton.Font = new System.Drawing.Font("宋体", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.stopButton.Location = new System.Drawing.Point(277, 88);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 25);
            this.stopButton.TabIndex = 3;
            this.stopButton.Text = "停止";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // statusTextBox
            // 
            this.statusTextBox.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.statusTextBox.Location = new System.Drawing.Point(28, 644);
            this.statusTextBox.Multiline = true;
            this.statusTextBox.Name = "statusTextBox";
            this.statusTextBox.Size = new System.Drawing.Size(454, 176);
            this.statusTextBox.TabIndex = 1;
            // 
            // chart2
            // 
            chartArea1.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chartArea1.AxisX.MajorGrid.Interval = 0D;
            chartArea1.AxisX.MajorGrid.IntervalOffset = 0D;
            chartArea1.AxisX.MajorGrid.IntervalOffsetType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorGrid.IntervalType = System.Windows.Forms.DataVisualization.Charting.DateTimeIntervalType.Auto;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea1);
            this.chart2.Location = new System.Drawing.Point(1123, 9);
            this.chart2.Name = "chart2";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Name = "Series2";
            this.chart2.Series.Add(series1);
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(346, 58);
            this.chart2.TabIndex = 4;
            this.chart2.Text = "chart2";
            this.chart2.Visible = false;
            // 
            // graphicTimer
            // 
            this.graphicTimer.Interval = 200;
            this.graphicTimer.Tick += new System.EventHandler(this.graphicTimer_Tick);
            // 
            // reportText
            // 
            this.reportText.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.reportText.Location = new System.Drawing.Point(27, 416);
            this.reportText.Multiline = true;
            this.reportText.Name = "reportText";
            this.reportText.Size = new System.Drawing.Size(454, 153);
            this.reportText.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.Yellow;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Location = new System.Drawing.Point(27, 395);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(118, 18);
            this.label5.TabIndex = 16;
            this.label5.Text = "时间";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.Yellow;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Location = new System.Drawing.Point(151, 394);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 18);
            this.label6.TabIndex = 17;
            this.label6.Text = "车牌号";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.Yellow;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Location = new System.Drawing.Point(239, 394);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(168, 18);
            this.label7.TabIndex = 18;
            this.label7.Text = "状态";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Yellow;
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Location = new System.Drawing.Point(413, 394);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 18);
            this.label8.TabIndex = 19;
            this.label8.Text = "检查员";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(106, 42);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 25);
            this.saveButton.TabIndex = 26;
            this.saveButton.Text = "雷达配置";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(286, 133);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 19);
            this.label9.TabIndex = 27;
            this.label9.Text = "3号雷达";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(115, 133);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 19);
            this.label11.TabIndex = 29;
            this.label11.Text = "2号雷达";
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Highlight;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.ForeColor = System.Drawing.Color.White;
            this.textBox1.Location = new System.Drawing.Point(33, 203);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(23, 91);
            this.textBox1.TabIndex = 30;
            this.textBox1.Text = "1\r\n号\r\n雷\r\n达";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(383, 23);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(18, 18);
            this.label10.TabIndex = 31;
            this.label10.Text = "1";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(423, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(18, 18);
            this.label12.TabIndex = 32;
            this.label12.Text = "2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label13.Location = new System.Drawing.Point(461, 23);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(18, 18);
            this.label13.TabIndex = 33;
            this.label13.Text = "3";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(385, 50);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 34;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Enabled = false;
            this.checkBox2.Location = new System.Drawing.Point(425, 50);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 35;
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Checked = true;
            this.checkBox3.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox3.Enabled = false;
            this.checkBox3.Location = new System.Drawing.Point(463, 50);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 36;
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::RADARMRM.Properties.Resources.emergency_off;
            this.pictureBox1.Location = new System.Drawing.Point(565, 22);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 37;
            this.pictureBox1.TabStop = false;
            // 
            // ScanningPic
            // 
            this.ScanningPic.Image = global::RADARMRM.Properties.Resources.car;
            this.ScanningPic.InitialImage = null;
            this.ScanningPic.Location = new System.Drawing.Point(71, 158);
            this.ScanningPic.Name = "ScanningPic";
            this.ScanningPic.Size = new System.Drawing.Size(407, 185);
            this.ScanningPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ScanningPic.TabIndex = 11;
            this.ScanningPic.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "小型箱货",
            "中型箱货车",
            "栅栏车",
            "大型货车",
            "集装箱车",
            "大巴车"});
            this.comboBox1.Location = new System.Drawing.Point(236, 44);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 22);
            this.comboBox1.TabIndex = 46;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // remotePicture
            // 
            this.remotePicture.Location = new System.Drawing.Point(565, 108);
            this.remotePicture.Name = "remotePicture";
            this.remotePicture.Size = new System.Drawing.Size(933, 800);
            this.remotePicture.TabIndex = 47;
            this.remotePicture.TabStop = false;
            this.remotePicture.MouseClick += new System.Windows.Forms.MouseEventHandler(this.remotePicture_MouseClick);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label17.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(499, 23);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(42, 18);
            this.label17.TabIndex = 48;
            this.label17.Text = "微震";
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Checked = true;
            this.checkBox4.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox4.Enabled = false;
            this.checkBox4.Location = new System.Drawing.Point(509, 50);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 49;
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // inspectorCombo
            // 
            this.inspectorCombo.FormattingEnabled = true;
            this.inspectorCombo.Location = new System.Drawing.Point(404, 586);
            this.inspectorCombo.Name = "inspectorCombo";
            this.inspectorCombo.Size = new System.Drawing.Size(74, 20);
            this.inspectorCombo.TabIndex = 50;
            // 
            // plateNumberText
            // 
            this.plateNumberText.Location = new System.Drawing.Point(148, 586);
            this.plateNumberText.Name = "plateNumberText";
            this.plateNumberText.Size = new System.Drawing.Size(82, 21);
            this.plateNumberText.TabIndex = 51;
            this.plateNumberText.Text = "XXXXXX";
            // 
            // plateInitialCombo
            // 
            this.plateInitialCombo.FormattingEnabled = true;
            this.plateInitialCombo.Location = new System.Drawing.Point(105, 587);
            this.plateInitialCombo.Name = "plateInitialCombo";
            this.plateInitialCombo.Size = new System.Drawing.Size(40, 20);
            this.plateInitialCombo.TabIndex = 52;
            // 
            // cancelTimer
            // 
            this.cancelTimer.Interval = 500;
            this.cancelTimer.Tick += new System.EventHandler(this.cancelTimer_Tick);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(241, 590);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 12);
            this.label18.TabIndex = 53;
            this.label18.Text = "记录日志";
            // 
            // logCheck
            // 
            this.logCheck.AutoSize = true;
            this.logCheck.Checked = true;
            this.logCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logCheck.Location = new System.Drawing.Point(300, 590);
            this.logCheck.Name = "logCheck";
            this.logCheck.Size = new System.Drawing.Size(15, 14);
            this.logCheck.TabIndex = 54;
            this.logCheck.UseVisualStyleBackColor = true;
            // 
            // openLogButton
            // 
            this.openLogButton.Location = new System.Drawing.Point(323, 585);
            this.openLogButton.Name = "openLogButton";
            this.openLogButton.Size = new System.Drawing.Size(75, 23);
            this.openLogButton.TabIndex = 55;
            this.openLogButton.Text = "打开日志";
            this.openLogButton.UseVisualStyleBackColor = true;
            this.openLogButton.Click += new System.EventHandler(this.openLogButton_Click);
            // 
            // timeLabel
            // 
            this.timeLabel.AutoSize = true;
            this.timeLabel.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.timeLabel.Location = new System.Drawing.Point(99, 7);
            this.timeLabel.Name = "timeLabel";
            this.timeLabel.Size = new System.Drawing.Size(53, 19);
            this.timeLabel.TabIndex = 57;
            this.timeLabel.Text = "2017";
            // 
            // timeTimer
            // 
            this.timeTimer.Interval = 1000;
            this.timeTimer.Tick += new System.EventHandler(this.timeTimer_Tick);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.Yellow;
            this.label21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label21.Location = new System.Drawing.Point(151, 623);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(331, 18);
            this.label21.TabIndex = 60;
            this.label21.Text = "状态信息";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.Yellow;
            this.label22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label22.Location = new System.Drawing.Point(28, 623);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(118, 18);
            this.label22.TabIndex = 59;
            this.label22.Text = "时间";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // logoPicture
            // 
            this.logoPicture.ErrorImage = ((System.Drawing.Image)(resources.GetObject("logoPicture.ErrorImage")));
            this.logoPicture.InitialImage = null;
            this.logoPicture.Location = new System.Drawing.Point(1123, 9);
            this.logoPicture.Name = "logoPicture";
            this.logoPicture.Size = new System.Drawing.Size(320, 70);
            this.logoPicture.TabIndex = 62;
            this.logoPicture.TabStop = false;
            // 
            // radarProgress
            // 
            this.radarProgress.Location = new System.Drawing.Point(71, 349);
            this.radarProgress.MarqueeAnimationSpeed = 20;
            this.radarProgress.Name = "radarProgress";
            this.radarProgress.Size = new System.Drawing.Size(407, 23);
            this.radarProgress.TabIndex = 63;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(21, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 25);
            this.button1.TabIndex = 64;
            this.button1.Text = "用户管理";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(1367, 872);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.radarProgress);
            this.Controls.Add(this.logoPicture);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.timeLabel);
            this.Controls.Add(this.openLogButton);
            this.Controls.Add(this.logCheck);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.plateInitialCombo);
            this.Controls.Add(this.plateNumberText);
            this.Controls.Add(this.inspectorCombo);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.remotePicture);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.reportText);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.statusTextBox);
            this.Controls.Add(this.stopButton);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.connectAllButton);
            this.Controls.Add(this.ScanningPic);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 1267, 667);
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "融合式车辆安检生命探测系统";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Shown += new System.EventHandler(this.Form1_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ScanningPic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.remotePicture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.logoPicture)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectAllButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button stopButton;
        //  private System.Windows.Forms.DataVisualization.Charting.Chart lineChart;
        //private System.Windows.Forms.DataVisualization.Charting.Chart lineChart;
        private System.Windows.Forms.TextBox statusTextBox;
        // private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.PictureBox ScanningPic;
        private System.Windows.Forms.Timer graphicTimer;
        private System.Windows.Forms.TextBox reportText;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.PictureBox remotePicture;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.ComboBox inspectorCombo;
        private System.Windows.Forms.TextBox plateNumberText;
        private System.Windows.Forms.ComboBox plateInitialCombo;
        private System.Windows.Forms.Timer cancelTimer;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.CheckBox logCheck;
        private System.Windows.Forms.Button openLogButton;
        private System.Windows.Forms.Label timeLabel;
        private System.Windows.Forms.Timer timeTimer;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.PictureBox logoPicture;
        private System.Windows.Forms.ProgressBar radarProgress;
        private System.Windows.Forms.Button button1;
    }
}

