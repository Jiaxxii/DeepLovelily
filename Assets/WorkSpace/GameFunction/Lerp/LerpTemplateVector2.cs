using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateVector2 : LerpTemplate<Vector2>
    {
        public LerpTemplateVector2(LerpGet<Vector2> getter, LerpSet<Vector2> setter, Vector2 start, Vector2 end) : base(getter, setter, start, end)
        {
        }

        public override float Value
        {
            get => (Mathf.Lerp(StartValue.x, EndValue.x, Property.x) + Mathf.Lerp(StartValue.y, EndValue.y, Property.y)) * 0.5f;
            set => Property = Vector2.Lerp(StartValue, EndValue, value);
        }
    }
}