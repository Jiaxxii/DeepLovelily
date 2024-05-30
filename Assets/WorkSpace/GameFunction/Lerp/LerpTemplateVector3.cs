using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateVector3 : LerpTemplate<Vector3>
    {
        public LerpTemplateVector3(LerpGet<Vector3> getter, LerpSet<Vector3> setter, Vector3 begin, Vector3 end) : base(getter, setter, begin, end)
        {
        }

        public override float Value
        {
            get => (Mathf.Lerp(BeginValue.x, EndValue.x, Property.x) + Mathf.Lerp(BeginValue.y, EndValue.y, Property.y) + Mathf.Lerp(BeginValue.z, EndValue.z, Property.z)) * 0.5f;
            set => Lerp(value);
        }

        public override Vector3 Lerp(float t) => Property = Vector3.Lerp(BeginValue, EndValue, t);
    }
}