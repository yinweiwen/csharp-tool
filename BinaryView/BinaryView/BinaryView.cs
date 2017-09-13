using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BinaryView
{
    public partial class BinaryView : Form
    {
        public BinaryView()
        {
            InitializeComponent();
            this.richTextBox1.AllowDrop = true;
            this.richTextBox1.DragEnter += RichTextBox1_DragEnter;
            this.richTextBox1.DragDrop += RichTextBox1_DragDrop;
        }

        private void RichTextBox1_DragDrop(object sender, DragEventArgs e)
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            OpenFile(path);
        }

        private void RichTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link; //重要代码：表明是链接类型的数据，比如文件路径
            else e.Effect = DragDropEffects.None;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                OpenFile(dialog.FileName);
            }
        }

        private void OpenFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bts = new byte[fs.Length];
                fs.Read(bts, 0, (int)fs.Length);
                var str = BytesToHexStr(bts);
                this.richTextBox1.Text = str;
            }
        }
        
        // FD010022 ... 16进制文本转 byte数组
        public static byte[] ToBytes(string str)
        {
            int len = str.Length / 2;
            byte[] buff = new byte[len];
            for (int i = 0; i < len; i++)
            {
                buff[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }
            return buff;
        }

        public static byte[] StrToToHexByte(string hexString)
        {
            string str = hexString;
            str = str.Replace(" ", "").Replace("\n", "").Replace("\r", "");
            return ToBytes(str);
        }

        public static string BytesToHexStr(byte[] da, string separator = " ")
        {
            return BytesToHexStr(da, 0, da.Length, separator);
        }

        public static string BytesToHexStr(byte[] da, int len, string separator = " ")
        {
            len = len == 0 ? da.Length : len;
            if (len < da.Length)
                return BytesToHexStr(da, 0, len, separator) + " ...";
            return BytesToHexStr(da, 0, da.Length, separator);
        }

        public static string BytesToHexStr(byte[] da, int start, int length, string separator = " ")
        {
            if (da == null || da.Length == 0)
            {
                return "";
            }
            string str = "";
            if (separator == null)
            {
                separator = " ";
            }
            for (int i = start; i < (start + length); i++)
            {
                str = str + Convert.ToString(da[i], 0x10).PadLeft(2, '0') + separator;
            }
            if (length > 0 && separator.Length > 0)
            {
                str = str.Remove(str.Length - separator.Length);
            }
            return str;
        }
    }
}
