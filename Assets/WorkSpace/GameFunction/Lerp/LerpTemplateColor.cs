using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateColor : LerpTemplate<Color>
    {
        public LerpTemplateColor(LerpGet<Color> getter, LerpSet<Color> setter, Color begin, Color end) : base(getter, setter, begin, end)
        {
        }

        public override float Value
        {
            get => (Mathf.Clamp01((Property.r - BeginValue.r) / (EndValue.r - BeginValue.r)) +
                    Mathf.Clamp01((Property.g - BeginValue.g) / (EndValue.g - BeginValue.g)) +
                    Mathf.Clamp01((Property.b - BeginValue.b) / (EndValue.b - BeginValue.b))) / 3f;

            set => Lerp(value);
        }

        public override Color Lerp(float t) => Property = Color.Lerp(BeginValue, EndValue, t);
    }
}