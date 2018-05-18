using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using AiCableForce.graphic;
using AiCableForce.serial;

namespace AiLaboratory
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            textBox1.Text = new IniFileOperation().IniReadValue("Main", "Path", "");
            tbCableLen.Text = new IniFileOperation().IniReadValue("Main", "索长", "0");
            tbCableWeight.Text = new IniFileOperation().IniReadValue("Main", "索重", "0");
            tbJipin.Text = new IniFileOperation().IniReadValue("Main", "基频", "0");
            tbWucha.Text = new IniFileOperation().IniReadValue("Main", "误差", "0");
            cbUsePsd.Checked = new IniFileOperation().IniReadBool("Main", "PSD", false);
            cbxDownsampling.Text = new IniFileOperation().IniReadValue("Main", "DownSampling", "1");
            tbSelfPower.Text = new IniFileOperation().IniReadValue("Main", "SelfPower", "5");
            tbSmoothPower.Text = new IniFileOperation().IniReadValue("Main", "SmoothPower", "0");
            cbShowGraphic.Checked = new IniFileOperation().IniReadBool("Main", "Graphic", true);
            tbExportTo.Text = new IniFileOperation().IniReadValue("Main", "ExportTo", "");
        }

        void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var index = listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                Do();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "Path", textBox1.Text);
            listBox2.Items.Clear();
            listBox2.Items.Add("");
            try
            {
                var di = new DirectoryInfo(textBox1.Text);
                var jis = di.GetDirectories();

                foreach (var d in jis)
                {
                    listBox2.Items.Add(d.Name);
                }
            }
            catch (Exception)
            {
                // ignored
            }
            listBox2.SelectedIndex = 0;
        }

        void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem == null) return;

            listBox1.Items.Clear();
            try
            {
                var di = new DirectoryInfo(Path.Combine(textBox1.Text, listBox2.SelectedItem.ToString()));
                var files = di.GetFiles("*.odb");
                listBox1.Items.AddRange(files);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void btnSelectNext_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            int si;
            if (listBox1.SelectedIndex + 1 >= listBox1.Items.Count)
                si = 0;
            else si = listBox1.SelectedIndex + 1;
            listBox1.SelectedItems.Clear();
            listBox1.SelectedIndex = si;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void tbCableLen_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "索长", tbCableLen.Text);
        }

        private void tbCableWeight_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "索重", tbCableWeight.Text);
        }

        private void tbJipin_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "基频", tbJipin.Text);
        }

        private void tbWucha_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "误差", tbWucha.Text);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "PSD", cbUsePsd.Checked.ToString());
        }

        private void tbSelfPower_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "SelfPower", tbSelfPower.Text);
        }

        private void tbExportTo_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "ExportTo", tbExportTo.Text);
        }

        private void tbSmoothPower_TextChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "SmoothPower", tbSmoothPower.Text);
        }

        private void cbShowGraphic_CheckedChanged(object sender, EventArgs e)
        {
            new IniFileOperation().IniWriteValue("Main", "Graphic", cbShowGraphic.Checked.ToString());
        }

        private void tbMasterFreq_TextChanged(object sender, EventArgs e)
        {
            var para = new CableForceCalcParam
            {
                UsePsd = cbUsePsd.Checked,
                SelfPower = int.Parse(tbSelfPower.Text),
                SmoothPower = int.Parse(tbSmoothPower.Text),
                UnitWeight = double.Parse(tbCableWeight.Text),
                CableLength = double.Parse(tbCableLen.Text),
                EstimateFreq = double.Parse(tbJipin.Text),
                FreqTolerance = double.Parse(tbWucha.Text)
            };
            double freq = 0;
            double.TryParse(tbMasterFreq.Text, out freq);

            var t = 4 * para.UnitWeight * para.CableLength * para.CableLength * freq * freq / 1000;
            tbForce.Text = t.ToString(CultureInfo.InvariantCulture);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Do();
        }

        public void Do()
        {
            try
            {
                Calc();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            new IniFileOperation().IniWriteValue("Main", "DownSampling", cbxDownsampling.Text);
        }

        private void Calc()
        {
            if (listBox1.SelectedItems.Count == 0) return;
            CableForceCalcResult cres;
            if (listBox1.SelectedItems.Count == 1)
            {
                var fi = listBox1.SelectedItem as FileInfo;
                if (fi == null) return;
                var path = fi.FullName;
                Clipboard.SetDataObject(fi.Name);
                var data = new VibReader(path).Read();
                DownSampling(ref data);
                cres = new Graphic(Path.GetFileName(path)).ShowAll(data, new CableForceCalcParam
                {
                    UsePsd = cbUsePsd.Checked,
                    SelfPower = int.Parse(tbSelfPower.Text),
                    SmoothPower = int.Parse(tbSmoothPower.Text),
                    UnitWeight = double.Parse(tbCableWeight.Text),
                    CableLength = double.Parse(tbCableLen.Text),
                    EstimateFreq = double.Parse(tbJipin.Text),
                    FreqTolerance = double.Parse(tbWucha.Text)
                }, cbShowGraphic.Checked);
            }
            else
            {
                var fi1 = listBox1.SelectedItems[0] as FileInfo;
                var fi2 = listBox1.SelectedItems[1] as FileInfo;
                if (fi1 == null || fi2 == null) return;
                var data1 = new VibReader(fi1.FullName).Read();
                var data2 = new VibReader(fi2.FullName).Read();
                DownSampling(ref data1);
                DownSampling(ref data2);
                cres = new Graphic(fi1.Name + ";" + fi2.Name).ShowMulti(data1, data2, new CableForceCalcParam
                {
                    UsePsd = cbUsePsd.Checked,
                    SmoothPower = int.Parse(tbSmoothPower.Text),
                    UnitWeight = double.Parse(tbCableWeight.Text),
                    CableLength = double.Parse(tbCableLen.Text),
                    EstimateFreq = double.Parse(tbJipin.Text),
                    FreqTolerance = double.Parse(tbWucha.Text)
                }, cbShowGraphic.Checked);
            }
            tbMasterFreq.Text = cres.MasterFreq.ToString(CultureInfo.InvariantCulture);
            tbForce.Text = cres.Force.ToString(CultureInfo.InvariantCulture);
        }

        private void DownSampling(ref VibData vd)
        {
            var ds = int.Parse(cbxDownsampling.Text);
            if (ds == 1) return;
            if (((int)vd.Sdb.diSampleFreq) % ds != 0) throw new Exception("down sample multiple err.");
            var nfs = ((int)vd.Sdb.diSampleFreq) / ds;

            var ls = new List<double>();
            for (var i = 0; i < vd.Odb.Length; i += ds)
            {
                ls.Add(vd.Odb[i]);
            }
            vd.Sdb.diSampleFreq = nfs;
            vd.Odb = ls.ToArray();
        }

        private void btnExportTo_Click(object sender, EventArgs e)
        {
            try
            {
                var spls = tbExportTo.Text.Split(',');
                var path = spls[0];
                var mod = spls[1];
                var ch = spls[2];
                var fi = listBox1.SelectedItems[0] as FileInfo;
                if (fi == null) throw new Exception("selection is empty.");
                var fix = new FileInfo(fi.FullName.Replace(".odb", ".sdb"));
                if (!fix.Exists) throw new Exception(".sdb not exists");
                var npath = Path.Combine(path, string.Format("{0}_{1}_{2:yyyyMMddHHmmssfff}", mod, ch, DateTime.Now));
                fi.CopyTo(npath + ".odb", true);
                fix.CopyTo(npath + ".sdb", true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
