using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DAASView
{
    public partial class MainFrm : Form
    {
        public MainFrm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dgv.DataBindingComplete += dgv_DataBindingComplete;
            dgv.CellContentClick += dgv_CellContentClick;
        }

        void dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgv.Columns[e.ColumnIndex].Name == "data")
            {
                var data = dgv[e.ColumnIndex, e.RowIndex].Value as double[];
                if (data != null)
                {
                    var str = string.Join("\n", data);
                    new TextWin(str).Show(this);
                }
            }
        }

        void dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgv.Columns["max"].HeaderText = @"max 最大";
            dgv.Columns["pp"].HeaderText = @"pp 峰峰";
            foreach (DataGridViewRow r in dgv.Rows)
            {
                r.Cells["data"] = new DataGridViewButtonCell();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                var path = folderBrowserDialog1.SelectedPath;

                var tb = new DataTable();
                tb.Columns.Add("file");
                tb.Columns.Add("module");
                tb.Columns.Add("ch");
                tb.Columns.Add("count", typeof(long)); // 点数
                tb.Columns.Add("max", typeof(double));
                tb.Columns.Add("pp", typeof(double));
                tb.Columns.Add("time");
                tb.Columns.Add("data", typeof(object));

                LoadPath(tb, path, "*.dat", ReadCloudVibFile);
                LoadPath(tb, path, "*.odb", ReadDAASVibFile);

                dgv.DataSource = tb;
                dgv.Update();
            }
        }

        public static void ExportDir(string path, string dest)
        {
            if (!Directory.Exists(dest)) Directory.CreateDirectory(dest);
            var tb = new DataTable();
            tb.Columns.Add("file");
            tb.Columns.Add("module");
            tb.Columns.Add("ch");
            tb.Columns.Add("count", typeof(long)); // 点数
            tb.Columns.Add("max", typeof(double));
            tb.Columns.Add("pp", typeof(double));
            tb.Columns.Add("time");
            tb.Columns.Add("data", typeof(object));

            LoadPath(tb, path, "*.dat", ReadCloudVibFile);
            LoadPath(tb, path, "*.odb", ReadDAASVibFile);

            Export(dest, tb);
        }

        private delegate bool ReadVib(
            string file, out CloudVibFileTitle title, out double[] data, out string module, out string err);

        public static void LoadAndExport(FileInfo fi, string dest)
        {
            CloudVibFileTitle title;
            double[] org;
            string err;
            string mod;
            switch (fi.Extension)
            {
                case ".dat":
                    if (!ReadCloudVibFile(fi.FullName, out title, out org, out mod, out err))
                    {
                        return;
                    }
                    break;
                case ".odb":
                    if (!ReadDAASVibFile(fi.FullName, out title, out org, out mod, out err))
                    {
                        return;
                    }
                    break;
                case "":
                    if (!ReadProcessedVibFile(fi.FullName, out title, out org, out mod, out err))
                    {
                        return;
                    }
                    break;
                default:
                    org = null;
                    break;
            }
            if (org != null)
                using (var sw = new StreamWriter(Path.Combine(dest, fi.Name + ".txt")))
                {
                    sw.Write(string.Join("\r\n", org));
                }
        }

        private static void LoadPath(DataTable tb, string p, string searchPattern, ReadVib rb)
        {
            var dir = new DirectoryInfo(p);
            var files = dir.GetFiles(searchPattern);
            foreach (var fi in files)
            {
                try
                {
                    CloudVibFileTitle title;
                    double[] org;
                    string err;
                    string mod;
                    if (!rb(fi.FullName, out title, out org, out mod, out err))
                    {
                        continue;
                    }
                    var ch = title.CHNum;
                    var time = new DateTime(2000 + title.year, title.mon, title.day, title.hour, title.min, title.sec);
                    var pp = org.Max() - org.Min(); // 峰峰
                    var max = org.OrderBy(Math.Abs).Last();

                    var nr = tb.NewRow();
                    nr["file"] = fi.Name;
                    nr["module"] = mod;
                    nr["ch"] = ch;
                    nr["count"] = org.Length;
                    nr["max"] = max;
                    nr["pp"] = pp;
                    nr["time"] = time;
                    nr["data"] = org;
                    tb.Rows.Add(nr);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }


        /// <summary>
        /// 解析云振动数据文件
        /// </summary>
        /// <param name="file">文件</param>
        /// <param name="title">out 振动头信息</param>
        /// <param name="data">out 振动数据</param>
        /// <param name="err">out 错误信息</param>
        /// <param name="module">out 转换后的模块号</param>
        /// <returns></returns>
        private static bool ReadCloudVibFile(string file, out CloudVibFileTitle title, out double[] data, out string module, out string err)
        {
            try
            {
                err = string.Empty;
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var br = new BinaryReader(fs);
                    var t = br.ReadUInt16();
                    var l = br.ReadUInt16();
                    title = (CloudVibFileTitle)StructConvert.BytesToStruct(br.ReadBytes(l), typeof(CloudVibFileTitle));
                    var cnt = title.L_Date / 4;
                    data = new double[cnt];
                    for (var i = 0; i < cnt; i++)
                    {
                        data[i] = br.ReadSingle();
                    }
                    fs.Close();
                }
                module = title.DeviceID.ToString();
                return true;
            }
            catch (Exception ex)
            {
                title = new CloudVibFileTitle();
                data = null;
                err = ex.Message;
                module = "";
                return false;
            }
        }

        private static bool ReadDAASVibFile(string file, out CloudVibFileTitle title, out double[] data, out string module, out string err)
        {

            try
            {
                err = string.Empty;
                var idx = file.LastIndexOf(".", StringComparison.Ordinal);
                var sdb = file.Substring(0, idx) + ".sdb";
                data = ReadF2Datum(file);
                var cfg = ReadF2Param(sdb);
                int ch;
                DateTime time;
                GetVibSensorInfoFormFilename(file, out module, out ch, out time);
                title = new CloudVibFileTitle()
                {
                    CHNum = (byte)ch,
                    DeviceID = 0,
                    SampleFreq = (float)cfg.diSampleFreq,
                    year = (byte)(time.Year - 2000),
                    mon = (byte)time.Month,
                    day = (byte)time.Day,
                    hour = (byte)time.Hour,
                    min = (byte)time.Minute,
                    sec = (byte)time.Second
                };
                return true;
            }
            catch (Exception ex)
            {
                title = new CloudVibFileTitle();
                data = null;
                err = ex.Message;
                module = "";
                return false;
            }
        }

        private static bool ReadProcessedVibFile(string file, out CloudVibFileTitle title, out double[] data, out string module, out string err)
        {
            title = new CloudVibFileTitle();
            module = "";
            err = "";
            try
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var n = fs.Length / sizeof(float);
                    data = new double[n];
                    using (var br = new BinaryReader(fs))
                    {
                        for (var i = 0; i < n; i++)
                        {
                            data[i] = br.ReadSingle();
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                data = null;
                err = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 从云振动数据文件名获取传感器信息
        /// </summary>
        /// <param name="file">in 文件名</param>
        /// <param name="mod">out 模块号</param>
        /// <param name="ch">out 通道号</param>
        /// <param name="time">out 采集时间</param>
        /// <returns>成功true 失败false</returns>
        private static bool GetVibSensorInfoFormFilename(string file, out string mod, out int ch, out DateTime time)
        {
            try
            {
                mod = "";
                ch = 0;
                time = new DateTime();
                var f = Path.GetFileNameWithoutExtension(file);
                if (f == null) return false;
                var parts = f.Split('_');
                mod = parts[0];
                ch = int.Parse(parts[1]);
                time = DateTime.ParseExact(parts[2], "yyyyMMddHHmmssfff", null);
                return true;
            }
            catch (Exception)
            {
                mod = "";
                ch = 0;
                time = DateTime.MinValue;
                return false;
            }
        }

        /// <summary>
        /// 读取F2配置
        /// </summary>
        /// <param name="cfile"></param>
        /// <returns></returns>
        private static F2VibStruct ReadF2Param(string cfile)
        {
            using (var fs = new FileStream(cfile, FileMode.Open, FileAccess.Read))
            {
                var br = new BinaryReader(fs);
                var res = (F2VibStruct)StructConvert.BytesToStruct(br.ReadBytes((int)fs.Length), typeof(F2VibStruct));
                br.Close();
                fs.Close();
                return res;
            }
        }

        /// <summary>
        /// F2软件生成的振动数据格式
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct F2VibStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
            public string bfType;                           // 采样数据标识
            public short diVer;                             // 版本号
            public double diSampleFreq;                     // 采样频率
            public int diSize;                              // 采样点数
            public double diSensitivity;                    // 灵敏度
            public byte diSensorType;                       // 传感器类型
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string diTestPointNum;                   // 测点号
            public int diMultiple;                          // 放大倍数
            public double diFilter;                         // 滤波频率
            public byte diUnit;                             // 工程单位
            public short diADBit;                           // AD精度
            public byte diMethod;                           // 采样方式
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string diRemark;                         // 备注
        }

        /// <summary>
        /// 读取F2数据
        /// </summary>
        /// <param name="dfile"></param>
        /// <returns></returns>
        private static double[] ReadF2Datum(string dfile)
        {
            double[] ret;
            using (var fs = new FileStream(dfile, FileMode.Open, FileAccess.Read))
            {
                var n = fs.Length / sizeof(double);
                ret = new double[n];
                var br = new BinaryReader(fs);
                for (var i = 0; i < n; i++)
                {
                    ret[i] = br.ReadDouble();
                }
            }
            return ret;
        }

        private static void Export(string path, DataTable tb)
        {
            foreach (DataRow nr in tb.Rows)
            {
                try
                {
                    var name = nr["file"];
                    var org = nr["data"] as double[];
                    using (var sw = new StreamWriter(Path.Combine(path, name + ".txt")))
                    {
                        sw.Write(string.Join("\r\n", org));
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private void exportTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog(this) == DialogResult.OK)
            {
                var path = folderBrowserDialog2.SelectedPath;
                var tb = dgv.DataSource as DataTable;
                Export(path, tb);
            }
            MessageBox.Show(@"导出完成!");
        }

        private void exportDirToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            new VibExporter().ShowDialog(this);
        }
    }


    /// <summary>
    /// 云网关振动数据格式
    /// </summary>
    public struct CloudVibFileTitle
    {
        public byte diVer; //版本号
        public byte CHNum; //通道号
        public ushort DeviceID; //设备ID
        public float SampleFreq; //采样频率
        public float FilterFreq; // 滤波频率
        public byte GainAmplifier; //放大倍数
        public byte TriggerType; //采样方式
        public byte year; // 采集时刻:年
        public byte mon; // 采集时刻:月
        public byte day; // 采集时刻:日
        public byte hour; // 采集时刻:时
        public byte min; // 采集时刻:分
        public byte sec; // 采集时刻:秒
        public uint Reserved1;
        public uint Reserved2;
        public ushort Reserved3;
        public ushort T_Data;
        public uint L_Date; // 数据区数据长度
    }
}
