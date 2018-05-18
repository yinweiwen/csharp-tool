using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AiLaboratory
{
    class IniFileOperation
    {
        /// <summary>
        /// INI文件名
        /// </summary>
        public string path;

        //声明读写INI文件的API函数     
        [DllImport("kernel32")]

        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]

        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        #region 构造函数

        /// <summary>
        /// 类的构造函数，传递INI文件名
        /// </summary>
        public IniFileOperation()
        {

            path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            if (!System.IO.File.Exists(path))
            {
                System.IO.File.Create(path).Close();//Modify@2013.5.2 必须关闭文件流
            }
        }

        #endregion

        #region 方法
        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="Section">分类</param>
        /// <param name="Key">关键字</param>
        /// <param name="Value">值</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        /// <summary>
        /// 读INI文件
        /// </summary>
        /// <param name="Section">分类</param>
        /// <param name="Key">关键字</param>
        /// <returns>值</returns>
        public string IniReadValue(string Section, string Key)
        {

            StringBuilder temp = new StringBuilder(255);

            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);

            return temp.ToString();

        }
        /// <summary>
        /// 读取INI文件字段字符串（包含默认值）
        /// </summary>
        public string IniReadValue(string Section, string Key, string DefalutVal)
        {
            string ret = IniReadValue(Section, Key);
            if (string.IsNullOrEmpty(ret))
            {
                return DefalutVal;
            }
            return (ret);
        }
        /// <summary>
        /// 读取INI文件字段整型数据
        /// </summary>
        public int IniReadInt(string Section, string Key, int defaultVal)
        {
            string tmp = IniReadValue(Section, Key);
            int res = defaultVal;
            if (!int.TryParse(tmp, out res))
            {
                res = defaultVal;
            }
            return res;
        }
        /// <summary>
        /// 读取INI文件时间字段
        /// </summary>
        public DateTime IniReadDateTime(string Section, string Key, string defaultVal)
        {
            string tmp = IniReadValue(Section, Key);
            DateTime res;
            if (!DateTime.TryParse(tmp, out res))
            {
                DateTime.TryParse(defaultVal, out res);
            }
            return res;
        }
        /// <summary>
        /// 读取INI文件浮点型字段
        /// </summary>
        public float IniReadFloat(string Section, string Key, float defaultVal)
        {
            string tmp = IniReadValue(Section, Key);
            float res = defaultVal;
            if (!float.TryParse(tmp, out res))
            {
                res = defaultVal;
            }
            return res;
        }
        /// <summary>
        /// 读取INI文件布尔型字段(默认值为false)
        /// </summary>
        public bool IniReadBool(string Section, string Key)
        {
            int tmp = IniReadInt(Section, Key, 0);
            if (tmp == 0) return false;
            return true;
        }
        /// <summary>
        /// 读取INI文件布尔型字段(默认值为false)
        /// </summary>
        public bool IniReadBool(string Section, string Key, bool defaultVal)
        {
            int tmp = IniReadInt(Section, Key, -1);
            if (tmp == -1) return defaultVal;
            return tmp != 0;
        }
        #endregion
    }
}
