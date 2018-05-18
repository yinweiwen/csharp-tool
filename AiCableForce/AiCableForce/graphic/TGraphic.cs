namespace AiCableForce.graphic
{
    /// <summary>
    /// 图形类型
    /// (标识图形单位信息)
    /// </summary>
    public enum TGraphic
    {
        /// <summary>
        /// 原始数据
        /// </summary>
        Original,
        /// <summary>
        /// 频谱(峰)
        /// </summary>
        FreqSpectrum,
        /// <summary>
        /// 频谱(Rms)
        /// </summary>
        FreqSpectrumRms,
        /// <summary>
        /// 频谱(功率谱)
        /// </summary>
        FreqSpectrumPower,
        /// <summary>
        /// 频谱(1/3倍频程)
        /// </summary>
        FreqSpectrumOctave,
        /// <summary>
        /// 频谱(功率谱密度)
        /// </summary>
        FreqSpectrumPSD,
        /// <summary>
        /// 自相关
        /// </summary>
        SelfCorrelation,
        /// <summary>
        /// 互相关
        /// </summary>
        CrossCorrelation,
        /// <summary>
        /// 波形计数
        /// </summary>
        WaveCount,
        /// <summary>
        /// 利萨如XY
        /// </summary>
        LissajousXY,
        /// <summary>
        /// 利萨如YX
        /// </summary>
        LissajousYX,
        /// <summary>
        /// 互功率谱-幅频
        /// </summary>
        CrossSpecAmp,
        /// <summary>
        /// 互功率谱-相频
        /// </summary>
        CrossSpecPhase,
        /// <summary>
        /// 互功率谱-奈奎斯特
        /// </summary>
        CrossSpecNyq,
        /// <summary>
        /// 互功率谱-实部
        /// </summary>
        CrossSpecReal,
        /// <summary>
        /// 互功率谱-虚部
        /// </summary>
        CrossSpecImag,
        /// <summary>
        /// 互功率谱-相干幅频
        /// </summary>
        CrossSpecCoheAmp,
        /// <summary>
        /// 互功率谱-相干
        /// </summary>
        CrossSpecCohe,
        /// <summary>
        /// 传递函数-幅频
        /// </summary>
        TransFuncAmp,
        /// <summary>
        /// 传递函数-相频
        /// </summary>
        TransFuncPhase,
        /// <summary>
        /// 传递函数-奈奎斯特
        /// </summary>
        TransFuncNyq,
        /// <summary>
        /// 传递函数-实部
        /// </summary>
        TransFuncReal,
        /// <summary>
        /// 传递函数-虚部
        /// </summary>
        TransFuncImag,
        /// <summary>
        /// 传递函数-相干幅频
        /// </summary>
        TransFuncCoheAmp,
        /// <summary>
        /// 传递函数-相干
        /// </summary>
        TransFuncCohe,
        /// <summary>
        /// 时域微积分
        /// </summary>
        TimeDiffAndInter,
        /// <summary>
        /// 频域微积分
        /// </summary>
        FreqDiffAndInter,
        /// <summary>
        /// 概率密度
        /// </summary>
        ProbabilityDensity,
        /// <summary>
        /// 概率分布
        /// </summary>
        ProbabilityDistribution,
    }
}
