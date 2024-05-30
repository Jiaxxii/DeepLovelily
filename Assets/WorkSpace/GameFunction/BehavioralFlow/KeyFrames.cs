using System;
using UnityEngine;
using WorkSpace.GameFunction.Lerp;

namespace WorkSpace.GameFunction.BehavioralFlow
{
    public class KeyFrames : IRange<float>
    {
        public KeyFrames(float beginValue, float endValue, ILerpObject lerpObject)
        {
            BeginValue = beginValue;
            EndValue = endValue;
            _lerpObject = lerpObject;
            if (BeginValue > EndValue)
            {
                (BeginValue, EndValue) = (EndValue, BeginValue);
            }

            Value = 0;
        }
        
        

        private readonly ILerpObject _lerpObject;

        private float _value;
        
        
        public float BeginValue { get; }
        public float EndValue { get; }



        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, BeginValue, EndValue).MapRange01(BeginValue, EndValue);
                _lerpObject.Value = _value;
            }
        }


        public float Lerp(float t) => Value = Mathf.Lerp(BeginValue, EndValue, t);

        public void SetBeginValue()
        {
            if (NotModify)
                return;

            Value = BeginValue;
        }

        public void SetEndTime()
        {
            if (NotModify)
                return;

            Value = EndValue;
        }


        public bool NotModify => _lerpObject == null;
    }
}