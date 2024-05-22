namespace WorkSpace.GameFunction.Lerp
{
    public abstract class LerpTemplate<TType> : ILerpValue
    {
        private readonly LerpGet<TType> _getProperty;
        private readonly LerpSet<TType> _setProperty;

        // 0-1
        public abstract float Value { get; set; }


        public TType StartValue { get; }
        public TType EndValue { get;  }


        public virtual TType Property
        {
            get => _getProperty.Invoke();
            set => _setProperty.Invoke(value);
        }

        protected LerpTemplate(LerpGet<TType> getter, LerpSet<TType> setter, TType start, TType end)
        {
            _getProperty = getter;
            _setProperty = setter;

            StartValue = start;
            EndValue = end;
        }
    }
}