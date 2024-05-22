using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WorkSpace.GameFunction.Lerp
{
    public static class Lerp
    {
        public static ILerpValue LerpBuild(LerpGet<float> getter, LerpSet<float> setter, float start, float end)
        {
            return new LerpTemplateFloat(getter, setter, start, end);
        }

        public static ILerpValue LerpBuild(LerpGet<Vector2> getter, LerpSet<Vector2> setter, Vector2 start, Vector2 end)
        {
            return new LerpTemplateVector2(getter, setter, start, end);
        }

        public static ILerpValue LerpBuild(LerpGet<Vector3> getter, LerpSet<Vector3> setter, Vector3 start, Vector3 end)
        {
            return new LerpTemplateVector3(getter, setter, start, end);
        }

        public static ILerpValue LerpBuild(LerpGet<Color> getter, LerpSet<Color> setter, Color start, Color end)
        {
            return new LerpTemplateColor(getter, setter, start, end);
        }


        public static ILerpValue GalgameAnchoredPosition(this RectTransform rectTransform, Vector2 start, Vector2 end)
        {
            return LerpBuild(() => rectTransform.anchoredPosition, value => rectTransform.anchoredPosition = value, start, end);
        }

        public static ILerpValue GalgameAnchoredPosition(this RectTransform rectTransform, Vector2 end)
        {
            var start = rectTransform.anchoredPosition;
            return LerpBuild(() => rectTransform.anchoredPosition, value => rectTransform.anchoredPosition = value, start, end);
        }


        public static ILerpValue GalgameSizeDelta(this RectTransform rectTransform, Vector2 start, Vector2 end)
        {
            return LerpBuild(() => rectTransform.sizeDelta, value => rectTransform.sizeDelta = value, start, end);
        }

        public static ILerpValue GalgameSizeDelta(this RectTransform rectTransform, Vector2 end)
        {
            var start = rectTransform.sizeDelta;
            return LerpBuild(() => rectTransform.sizeDelta, value => rectTransform.sizeDelta = value, start, end);
        }


        public static ILerpValue GalgameEulerAngles(this Transform transform, Vector3 start, Vector3 end)
        {
            return LerpBuild(() => transform.eulerAngles, value => transform.eulerAngles = value, start, end);
        }

        public static ILerpValue GalgameEulerAngles(this Transform transform, Vector3 end)
        {
            var start = transform.eulerAngles;
            return LerpBuild(() => transform.eulerAngles, value => transform.eulerAngles = value, start, end);
        }


        public static ILerpValue GalgameDepth(this Transform transform, float start, float end)
        {
            return LerpBuild(() => transform.position.z, v =>
            {
                var position = transform.position;
                transform.position = new Vector3(position.x, position.y, v);
            }, start, end);
        }

        public static ILerpValue GalgameDepth(this Transform transform, float end)
        {
            var start = transform.position.z;
            return LerpBuild(() => transform.position.z, v =>
            {
                var position = transform.position;
                transform.position = new Vector3(position.x, position.y, v);
            }, start, end);
        }


        public static ILerpValue GalgameColor(this Image image, Color start, Color end)
        {
            return LerpBuild(() => image.color, v => image.color = new Color(v.r, v.g, v.b, image.color.a), start, end);
        }

        public static ILerpValue GalgameColor(this Image image, Color end)
        {
            var start = image.color;
            return LerpBuild(() => image.color, v => image.color = new Color(v.r, v.g, v.b, image.color.a), start, end);
        }


        public static ILerpValue GalgameAlpha(this Image image, float start, float end)
        {
            return LerpBuild(() => image.color.a, value => image.color = new Color(image.color.r, image.color.g, image.color.b, value), start, end);
        }

        public static ILerpValue GalgameAlpha(this Image image, float end)
        {
            var start = image.color.a;
            return LerpBuild(() => image.color.a, value => image.color = new Color(image.color.r, image.color.g, image.color.b, value), start, end);
        }


        public static ILerpValue GalgameColorA(this Image image, Color start, Color end)
        {
            return LerpBuild(() => image.color, value => image.color = value, start, end);
        }

        public static ILerpValue GalgameColorA(this Image image, Color end)
        {
            var start = image.color;
            return LerpBuild(() => image.color, value => image.color = value, start, end);
        }


        public static void GalgameSetValue(this IEnumerable<ILerpValue> collects, float value)
        {
            foreach (var item in collects)
                item.Value = value;
        }
    }
}