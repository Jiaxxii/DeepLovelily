namespace WorkSpace.GameFunction.BehavioralFlow
{
    public enum TimeInputMode
    {
        None,

        /// <summary>
        /// 表示输入的Time将会是从start开始到end结束的线性输入的
        /// <para>
        /// 适用于动画是逐帧播放的，适用于自动播放模式。
        /// </para>
        /// </summary>
        LinearProgression,

        /// <summary>
        /// 表示输入的Time将会从start到end之间非线性取值
        /// <para>
        /// 适用于动画是会跳转的 (开销大)。
        /// </para>
        /// </summary>
        NonLinearSampling
    }
}