using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WorkSpace.StructData;

namespace WorkSpace
{
    public static class Expand
    {
        public static IEnumerable<KeyValuePair<string, TransformInfo[]>> JObjectToKeyValuePair(this JObject jObject)
        {
            var list = new List<KeyValuePair<string, TransformInfo[]>>(jObject.Count);
            foreach (var item in jObject.Properties())
            {
                if (item.Value is not JArray componentPair)
                {
                    throw new InvalidOperationException($"\"{item.Value}\"不是一个Array!");
                }

                var component = new TransformInfo[componentPair.Count];
                for (var i = 0; i < componentPair.Count; i++)
                {
                    if (componentPair[i] is JObject jTransformInfo)
                    {
                        component[i] = new TransformInfo(jTransformInfo);
                    }
                    else
                    {
                        Debug.LogWarning($"\"{item.Value}\"数组中元素[{i}]不是一个对象!");
                        component[i] = new TransformInfo();
                    }
                }


                list.Add(
                    new KeyValuePair<string, TransformInfo[]>(item.Name, component));
            }

            return list;
        }

        public static float Vector2Dot(this Vector2 vector) => vector.x * vector.y;
        public static float Vector3Dot(this Vector3 vector) => vector.x * vector.y * vector.z;

        public static float MapRange(this float value, float sourceMin, float sourceMax, float targetMin, float targetMax)
        {
            return targetMin + (value - sourceMin) * (targetMax - targetMin) / (sourceMax - sourceMin);
        }

        public static float MapRange01(this float value, float sourceMin, float sourceMax)
        {
            return (value - sourceMin) / (sourceMax - sourceMin);
        }

        public static float Map01Range(this float value, float targetMin, float targetMax)
        {
            return targetMin + value * (targetMax - targetMin);
        }
    }
}