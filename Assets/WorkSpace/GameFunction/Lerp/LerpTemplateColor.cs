using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateColor : LerpTemplate<Color>
    {
        public LerpTemplateColor(LerpGet<Color> getter, LerpSet<Color> setter, Color start, Color end) : base(getter, setter, start, end)
        {
        }

        public override float Value
        {
            get => (Mathf.Clamp01((Property.r - StartValue.r) / (EndValue.r - StartValue.r)) +
                    Mathf.Clamp01((Property.g - StartValue.g) / (EndValue.g - StartValue.g)) +
                    Mathf.Clamp01((Property.b - StartValue.b) / (EndValue.b - StartValue.b))) / 3f;

            set => Property = Color.Lerp(StartValue, EndValue, value);
        }
    }
}