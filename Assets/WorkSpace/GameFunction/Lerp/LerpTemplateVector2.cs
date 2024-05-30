using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateVector2 : LerpTemplate<Vector2>
    {
        public LerpTemplateVector2(LerpGet<Vector2> getter, LerpSet<Vector2> setter, Vector2 begin, Vector2 end) : base(getter, setter, begin, end)
        {
        }

        public override float Value
        {
            get => (Mathf.Lerp(BeginValue.x, EndValue.x, Property.x) + Mathf.Lerp(BeginValue.y, EndValue.y, Property.y)) * 0.5f;
            set => Lerp(value);
        }

        public override Vector2 Lerp(float t) => Property = Vector2.Lerp(BeginValue, EndValue, t);
    }
}