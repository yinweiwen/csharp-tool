using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DAASView
{
    public partial class VibExporter : Form
    {
        public VibExporter()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                txPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnBrowse2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                tbDest.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!bgw.IsBusy)
            {
                if (string.IsNullOrEmpty(txPath.Text) || string.IsNullOrEmpty(tbDest.Text))
                {
                    MessageBox.Show(@"输入为空!!");
                    return;
                }
                bgw.RunWorkerAsync();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                bgw.CancelAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {
            var dest = tbDest.Text;
            var path = txPath.Text.Trim();
            var dir = new DirectoryInfo(path);
            var dirs = dir.GetDirectories("*", SearchOption.AllDirectories);
            var total1 = dirs.Count();
            var index1 = 0;
            BeginInvoke(new Action(() => { pb1.Maximum = total1; }));
            foreach (var subDir in dirs)
            {
                index1++;
                var files = subDir.GetFiles().Where(s => string.IsNullOrEmpty(s.Extension) || s.Extension == ".odb" || s.Extension == ".dat").ToArray();
                var total2 = files.Count();
                var destDir = Path.Combine(dest, subDir.Name);
                if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                BeginInvoke(new Action(() => { pb2.Value = 0; pb2.Maximum = total2; }));
                for (var i = 0; i < total2; i++)
                {
                    if (bgw.CancellationPending) return;
                    var f = files[i];
                    try
                    {
                        MainFrm.LoadAndExport(f, destDir);
                        BeginInvoke(new Action(() => { lb.Text = f.Name; }));
                    }
                    catch (Exception ex)
                    {
                        BeginInvoke(new Action(() => { lb.Text = f.Name + @" " + ex.Message; }));
                        // ignored
                    }
                    var i1 = i;
                    BeginInvoke(new Action(() => { pb2.Value = (i1 + 1); }));
                }

                var index2 = index1;
                BeginInvoke(new Action(() => { pb1.Value = index2; }));
            }
        }

        private void bgw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                BeginInvoke(new Action(() => { lb.Text = @"Canceled"; }));
            }
            else if (e.Error != null)
            {
                BeginInvoke(new Action(() => { lb.Text = @"错误: " + e.Error.Message; }));
            }
            else
            {
                BeginInvoke(new Action(() => { lb.Text = @"Finished"; }));
            }
        }
    }
}
