using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WorkSpace.GameFunction.Lerp;

namespace WorkSpace.GameFunction.BehavioralFlow
{
    public class KeyFrameAnimator : IRange<float>
    {
        private KeyFrameAnimator(float beginValue, float endValue, IEnumerable<KeyFrames> collection, TimeInputMode timeInputMode)
        {
            _keyTransforms = new List<KeyFrames>(collection);
            TimeInputMode = CheckTimeInputMode(_keyTransforms, 0.001F);

            BeginValue = beginValue;
            EndValue = endValue;


            if (timeInputMode != TimeInputMode)
            {
                Debug.LogWarning($"您指定的模式为\"{timeInputMode}\"但检测结果为\"{TimeInputMode}\"");
            }
        }


        private readonly List<KeyFrames> _keyTransforms;
        private float _value;


        public TimeInputMode TimeInputMode { get; private set; }


        public float Value
        {
            get => _value;
            set => SetTime(Mathf.Clamp(value, BeginValue, EndValue));
        }


        public float BeginValue { get; }


        public float EndValue { get; }


        public float Lerp(float t) => Value = Mathf.Lerp(BeginValue, EndValue, t);

        public void SetTime(float time, TimeInputMode inputMode = TimeInputMode.None)
        {
            if (inputMode != TimeInputMode.None)
            {
                TimeInputMode = inputMode;
            }

            switch (TimeInputMode)
            {
                case TimeInputMode.LinearProgression:
                    SetLinerTime(time);
                    return;

                case TimeInputMode.NonLinearSampling:
                    SetAnyTime(time);
                    return;
            }
        }

        
        
        #region 辅助方法

        private void SetAnyTime(float time)
        {
            foreach (var kf in _keyTransforms)
            {
                if (time <= kf.BeginValue)
                {
                    kf.SetBeginValue();
                    continue;
                }

                if (time > kf.BeginValue && time < kf.EndValue)
                {
                    SetTime(kf, time);
                    continue;
                }

                if (time >= kf.EndValue)
                {
                    kf.SetEndTime();
                }
            }
        }

        private void SetLinerTime(float time)
        {
            var left = 0;
            var right = _keyTransforms.Count - 1;

            while (left <= right)
            {
                var mid = left + (right - left) / 2;

                var midKf = _keyTransforms[mid];

                // 判断是否在区间
                if (time >= midKf.BeginValue && time <= midKf.EndValue)
                {
                    midKf.Value = time;
                    return;
                }

                // 未到这个区间
                if (time < midKf.BeginValue)
                {
                    right = mid - 1;
                }
                else
                    // if (time > midKf.EndTime) // 超过了这个区间
                {
                    left = mid + 1;
                }
            }
        }


        private static TimeInputMode CheckTimeInputMode(IReadOnlyList<KeyFrames> collection, float tolerance)
        {
            if (collection == null || collection.Count == 0)
            {
                Debug.LogError("无有效关键帧!");
                return TimeInputMode.None;
            }

            switch (collection.Count)
            {
                case 1:
                    return TimeInputMode.LinearProgression;
                case 2:
                    return collection[1].BeginValue - collection[0].EndValue > tolerance ? TimeInputMode.NonLinearSampling : TimeInputMode.LinearProgression;
            }

            var lastEndTime = collection[0].EndValue;

            for (var index = 1; index < collection.Count; index++)
            {
                if (Mathf.Abs(collection[index].BeginValue - lastEndTime) > tolerance)
                {
                    return TimeInputMode.NonLinearSampling;
                }

                lastEndTime = collection[index].EndValue;
            }

            return TimeInputMode.LinearProgression;
        }

        private static void SetTime(KeyFrames keyFrames, float time)
        {
            if (keyFrames.NotModify)
            {
                return;
            }

            keyFrames.Value = time;
        }

        #endregion


        public static KeyFrameAnimator Create(float beginTime, params (ILerpObject lerpObject, float endTime)[] kfs)
        {
            var list = new List<KeyFrames>(kfs.Length);

            var left = beginTime;
            foreach (var (lerpObject, right) in kfs)
            {
                list.Add(new KeyFrames(left, right, lerpObject));
                left = right;
            }

            return new KeyFrameAnimator(0F, kfs[^1].endTime, list, TimeInputMode.LinearProgression);
        }

        public static KeyFrameAnimator Create(params (ILerpObject, float, float )[] kfs)
        {
            var beginTime = float.MaxValue;
            var endTime = float.MinValue;

            var list = new List<KeyFrames>(kfs.Length);
            foreach (var (lO, begin, end) in kfs)
            {
                if (end >= endTime)
                {
                    endTime = end;
                }

                if (begin <= beginTime)
                {
                    beginTime = begin;
                }

                list.Add(new KeyFrames(begin, end, lO));
            }

            return new KeyFrameAnimator(beginTime, endTime, list, TimeInputMode.NonLinearSampling);
        }
    }
}