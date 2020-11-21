using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RADARMRM
{
    public partial class FrmConfig : Form
    {
        public int [] cfgInt = new int[6];
        public float[] cfgFloat = new float[6];

        public FrmConfig()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cfgInt[0] = Convert.ToInt32(IntegrationNumeric.Value);
            cfgInt[1] = Convert.ToInt32(numericUpDown1.Value);
            cfgFloat[0] = Convert.ToSingle(numericUpDown2.Value);
            cfgFloat[1] = Convert.ToSingle(numericUpDown3.Value);
            cfgInt[2] = Convert.ToInt32(IntegrationNumeric1.Value);
            cfgInt[3] = Convert.ToInt32(numericUpDown7.Value);
            cfgFloat[2] = Convert.ToSingle(numericUpDown6.Value);
            cfgFloat[3] = Convert.ToSingle(numericUpDown5.Value);
            cfgInt[4] = Convert.ToInt32(numericUpDown4.Value);
            cfgInt[5] = Convert.ToInt32(numericUpDown10.Value);
            cfgFloat[4] = Convert.ToSingle(numericUpDown9.Value);
            cfgFloat[5] = Convert.ToSingle(numericUpDown8.Value);

            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void FrmConfig_Load(object sender, EventArgs e)
        {

            IntegrationNumeric.Value = cfgInt[0];
            numericUpDown1.Value = cfgInt[1];
            numericUpDown2.Value = Convert.ToDecimal(cfgFloat[0]);
            numericUpDown3.Value = Convert.ToDecimal(cfgFloat[1]);
            IntegrationNumeric1.Value = cfgInt[2];
            numericUpDown7.Value = cfgInt[3];
            numericUpDown6.Value = Convert.ToDecimal(cfgFloat[2]);
            numericUpDown5.Value = Convert.ToDecimal(cfgFloat[3]);
            numericUpDown4.Value = cfgInt[4];
            numericUpDown10.Value = cfgInt[5];
            numericUpDown9.Value = Convert.ToDecimal(cfgFloat[4]);
            numericUpDown8.Value = Convert.ToDecimal(cfgFloat[5]);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void radDropDownList8_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {

        }
    }
}
