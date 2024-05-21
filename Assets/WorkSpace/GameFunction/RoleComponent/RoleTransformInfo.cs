using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WorkSpace.StructData;

namespace WorkSpace.GameFunction.RoleComponent
{
    public static class RoleTransformInfo
    {
        public static Dictionary<string, SpriteAsset> Load(JObject roleTransformInfoJObject)
        {
            var jsonData = new Dictionary<string, SpriteAsset>();
            foreach (var property in roleTransformInfoJObject.Properties())
            {
                var data = property.Value["data"];
                if (data is not JArray dataArray)
                {
                    Debug.LogWarning($"属性:\"{property.Name}\"不是一个数组!");
                    jsonData.Add(property.Name, null);
                    continue;
                }

                var transformInfos = new TransformInfo[dataArray.Count];
                for (var i = 0; i < transformInfos.Length; i++)
                {
                    if (dataArray[i] is not JObject target)
                    {
                        Debug.LogWarning($"属性\"{property.Name}->data\"中的数据不是一个对象");
                        target = new JObject();
                    }

                    transformInfos[i] = new TransformInfo(target);
                }

                var type = property.Value["type"]?.Value<string>();
                if (string.IsNullOrEmpty(type))
                {
                    Debug.LogWarning($"属性\"{property.Value}\"缺少\"type\"属性!");
                    type = "null";
                }

                var spriteAsset = new SpriteAsset(type, new Data(transformInfos));
                jsonData.Add(property.Name, spriteAsset);
            }

            return jsonData;
        }
    }


    public record SpriteAsset(string Type, Data Data)
    {
        public string Type { get; private set; } = Type;
        public Data Data { get; private set; } = Data;
    }

    public record Data(TransformInfo[] TransformInfoData)
    {
        public TransformInfo[] TransformInfoData { get; private set; } = TransformInfoData;
    }
}