using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateFloat : LerpTemplate<float>
    {
        public LerpTemplateFloat(LerpGet<float> getter, LerpSet<float> setter, float begin, float end) : base(getter, setter, begin, end)
        {
        }

        public override float Value
        {
            get => Mathf.InverseLerp(BeginValue, EndValue, Property);
            set => Lerp(value);
        }

        public override float Lerp(float t) => Property = Mathf.Lerp(BeginValue, EndValue, t);
    }
}