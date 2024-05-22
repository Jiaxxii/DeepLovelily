using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateFloat : LerpTemplate<float>
    {
        public LerpTemplateFloat(LerpGet<float> getter, LerpSet<float> setter, float start, float end) : base(getter, setter, start, end)
        {
        }

        public override float Value
        {
            get => Mathf.InverseLerp(StartValue, EndValue, Property);
            set => Property = Mathf.Lerp(StartValue, EndValue, value);
        }
    }
}