namespace WorkSpace.GameFunction.Lerp
{
    public interface IRange<out T> : ILerpObject
    {
        /// <summary>
        /// 起始
        /// </summary>
        public T BeginValue { get; }
        
        /// <summary>
        /// 结束
        /// </summary>
        public T EndValue { get; }

        /// <summary>
        /// 在 <see cref="BeginValue"/> 与 <see cref="EndValue"/> 之间差值 <paramref name="t"/>
        /// </summary>
        /// <param name="t"> range:[0-1] </param>
        /// <returns></returns>
        public T Lerp(float t);
    }
}