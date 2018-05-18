using System;
using System.Runtime.InteropServices;

namespace AiCableForce.serial
{
    /// <summary>
    /// 配置文件信息保存类
    /// 2014.07.23 MODIFIED
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct sdb
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string bfType;                           // 采样数据标识
        public Int16 diVer;                             // 版本号
        public double diSampleFreq;                     // 采样频率
        public Int32 diSize;                            // 采样点数
        public double diSensitivity;                    // 灵敏度
        public byte diSensorType;                       // 传感器类型
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string diTestPointNum;                   // 测点号
        public Int32 diMultiple;                        // 放大倍数
        public double diFilter;                         // 滤波频率
        public byte diUnit;                             // 工程单位
        public Int16 diADBit;                           // AD精度
        public byte diMethod;                           // 采样方式
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string diRemark;                         // 备注
    }
}
