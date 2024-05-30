namespace WorkSpace.GameFunction.Lerp
{
    public abstract class LerpTemplate<TType> : IRange<TType>
    {
        private readonly LerpGet<TType> _getProperty;
        private readonly LerpSet<TType> _setProperty;

        // 0-1
        public abstract float Value { get; set; }


        public TType BeginValue { get; }
        public TType EndValue { get; }


        public abstract TType Lerp(float t);


        public virtual TType Property
        {
            get => _getProperty.Invoke();
            set => _setProperty.Invoke(value);
        }

        protected LerpTemplate(LerpGet<TType> getter, LerpSet<TType> setter, TType begin, TType end)
        {
            _getProperty = getter;
            _setProperty = setter;

            BeginValue = begin;
            EndValue = end;
        }
    }
}