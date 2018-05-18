using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AiCableForce.serial;

namespace SecondaryNoise
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCalc_Click(object sender, EventArgs e)
        {
            lbRes.Text = "";
            if (openFileDialog1.ShowDialog(this) != DialogResult.OK) return;
            var reader = new VibReader(openFileDialog1.FileName);
            try
            {
                var data = reader.Read();
                var sn = new AiCableForce.noise.SecondaryNoise();
                var d = sn.Calc(data);
                lbRes.Text = string.Format("{0:####.###} dB", d);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"读取或计算出错:" + ex.Message);
            }
        }
    }
}
