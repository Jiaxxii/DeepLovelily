using UnityEngine;

namespace WorkSpace.GameFunction.Lerp
{
    public class LerpTemplateVector3 : LerpTemplate<Vector3>
    {
        public LerpTemplateVector3(LerpGet<Vector3> getter, LerpSet<Vector3> setter, Vector3 start, Vector3 end) : base(getter, setter, start, end)
        {
        }

        public override float Value
        {
            get => (Mathf.Lerp(StartValue.x, EndValue.x, Property.x) + Mathf.Lerp(StartValue.y, EndValue.y, Property.y) + Mathf.Lerp(StartValue.z, EndValue.z, Property.z)) * 0.5f;
            set => Property = Vector3.Lerp(StartValue, EndValue, value);
        }
    }
}