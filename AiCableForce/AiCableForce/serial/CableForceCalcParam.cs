namespace AiCableForce.serial
{
    public class CableForceCalcParam
    {
        public static CableForceCalcParam Default { get { return new CableForceCalcParam { UsePsd = false, SelfPower = 5 }; } }

        public bool UsePsd;                     // 使用经典谱估计，否则使用幅频曲线评估

        public int SelfPower;                   // 倍增次数

        public int SmoothPower;                 // 平滑次数

        public double EstimateFreq;             // 预估一阶固有频率 Hz

        public double FreqTolerance;             // 容许频率误差 Hz

        public double UnitWeight;               // 索的容重/线密度 kg/m

        public double CableLength;              // 索的长度 m

        public double PeakThreshod;             // 峰值查找时的阈值设定(最大值倍数 0~1)
    }

    public class CableForceCalcResult
    {
        public double Force;                    // 计算索力值 kN

        public double MasterFreq;               // 主振频率/基频/一阶固有频率

        public double SecondOrderFreq;          // 二阶频率
    }
}
