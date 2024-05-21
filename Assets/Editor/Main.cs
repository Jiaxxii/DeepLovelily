using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;


namespace Editor
{
    public static class Main
    {
        // [MenuItem("Tools/ReNameFile")]
        // public static void ReNameFile()
        // {
        //     const string name = "yuk";
        //     foreach (var file in Directory.GetFiles(Path.Combine(Application.dataPath, "Images", "ROLE", name), "*.png"))
        //     {
        //         var newName = Path.GetFileName(file).Replace("優子", name + '_');
        //         File.Move(file, Path.Combine(Application.dataPath, "Images", "ROLE", name, newName));
        //         AssetDatabase.Refresh();
        //     }
        // }

        // [MenuItem("Tools/路径名称修改")]
        // public static void 路径名称修改()
        // {
        //     var filePath = Path.Combine(Application.dataPath, "Resources", "Transform",
        //         "ai faces test.json");
        //     var jsonData = JObject.Parse(File.ReadAllText(filePath));
        //
        //     var conbimoJsonData = new JObject();
        //     foreach (var data in jsonData.Properties())
        //     {
        //         // 侧面脸部 偏移 32.7
        //         var dataArray = new JArray();
        //         foreach (var item in data.Value["data"] as JArray)
        //         {
        //             dataArray.Add(new JObject
        //             {
        //                 ["path"] = item["path"].Value<string>(),
        //                 ["x"] = data.Name.Contains("ai_b_") && data.Value["type"].Value<string>() == "Faces"
        //                     ? float.Parse((item["x"].Value<float>() - 32.7).ToString("F4"))
        //                     : item["x"].Value<float>(),
        //                 ["y"] = item["y"].Value<float>(),
        //                 ["width"] = item["width"].Value<float>(),
        //                 ["height"] = item["height"].Value<float>(),
        //             });
        //         }
        //
        //         conbimoJsonData[data.Name] = new JObject
        //         {
        //             ["type"] = data.Value["type"].Value<string>(),
        //             ["data"] = dataArray
        //         };
        //     }
        //
        //     File.Copy(filePath,
        //         Path.Combine(Application.dataPath, "Resources", "Temp", "ai faces test_set float.json"));
        //
        //     File.WriteAllText(filePath, conbimoJsonData.ToString(Formatting.Indented));
        // }


        [MenuItem("Tools/SetJson")]
        public static void SetJson()
        {
            var filePath = Path.Combine(Application.dataPath, "Resources", "Transform"
                , "yzw faces test.json");

            var select = EditorUtility.DisplayDialog("是否删除", $"将修改\"{filePath.Replace('\\', '/')}\"文件", "确定", "取消");

            if (!select) return;

            var combineData = new JObject();
            var rawContent = File.ReadAllText(filePath);
            Debug.Log(rawContent);
            foreach (var jsonItem in JObject.Parse(rawContent))
            {
                if (jsonItem.Value is not JArray jArrayData)
                {
                    throw new ArgumentException($"json属性\"{jsonItem.Key}\"格式不是一个数组!");
                }

                var bodyType = jArrayData.Count > 1 ? "Faces" : "Body";
                combineData.Add(jsonItem.Key, new JObject
                {
                    ["type"] = bodyType,
                    ["data"] = jArrayData
                });
            }

            File.WriteAllText(filePath, combineData.ToString(Formatting.Indented));
            Debug.Log($"完成!\"<color=green>{filePath.Replace('\\', '/')}</color>\"");
        }

        [MenuItem("Tools/Combine")]
        public static void Combine()
        {
            // var queue = new Queue<string>(new[]
            // {
            //     "_a_0_1426",
            //     "_a_0_1428",
            //     "_a_0_1430",
            //     "_a_0_1432",
            //     "_a_0_1479",
            //
            //     "_c_0_4530",
            //     "_c_0_4534",
            //     "_c_0_4538",
            //     "_c_0_4542",
            //     "_c_0_4630",
            // });

            var queuePair = new Queue<(string first, string last)>(new[]
            {
                ("_a_0_749", ""),
                ("_a_0_751", ""),
            });

            var combineData = new JObject();
            // 找出旧的数据

            var oldData = JObject.Parse(Resources.Load<TextAsset>("yzw position").text);

            var regex = new Regex(@"_(?<bodyType>\w)_\d_(?<index>\d{3,4})");

            var startIndex = 0;
            while (queuePair.Count != 0)
            {
                var pair = queuePair.Dequeue();

                Add(pair.first);

                if (string.IsNullOrEmpty(pair.last))
                {
                    startIndex += 2;
                    continue;
                }

                Add(pair.last);
                startIndex += 2;
            }


            // 更新新数据中的索引 索引 = 自身索引 + queuePair.Count * pair

            foreach (var item in JObject.Parse(Resources.Load<TextAsset>("Transform/yzw faces").text))
            {
                var match = Regex.Match(item.Key, @"(?<front>\w+_\w_)(?<currentIndex>\d{3,4})");

                Debug.Assert(!string.IsNullOrEmpty(match.Groups["currentIndex"].Value), $"文件命名不规范!\"{item.Key}\"");
                var currentIndex = startIndex - 2 + int.Parse(match.Groups["currentIndex"].Value);

                var propertyName = $"{match.Groups["front"]}{currentIndex:0000}";

                combineData[propertyName] = item.Value as JArray;
            }

            var savePath = Path.Combine(Application.dataPath, "Resources", "Transform", "yzw faces test.json");
            File.WriteAllText(savePath, combineData.ToString(Formatting.Indented));

            Debug.Log($"文件保存在:\"<color=green>{savePath}</color>\"");
            AssetDatabase.Refresh();

            return;

            void Add(string propertyName)
            {
                if (!oldData.TryGetValue(propertyName, out var jObject))
                {
                    Debug.LogError($"旧数据中无键\"{propertyName}\"");
                    return;
                }

                var bodyType = regex.Match(propertyName).Groups["bodyType"].Value.Contains('a') ? 'a' : 'b';

                combineData.Add($"yzw_{bodyType}_{startIndex:0000}", new JArray
                {
                    new JObject
                    {
                        ["path"] = $"yzw{propertyName}",
                        ["x"] = jObject["x"].Value<float>(),
                        ["y"] = jObject["y"].Value<float>(),
                        ["width"] = jObject["width"].Value<float>(),
                        ["height"] = jObject["height"].Value<float>(),
                    }
                });
            }
        }

        [MenuItem("Tools/ShowBody")]
        public static async void ShowBody()
        {
            var root = Selection.transforms[0] as RectTransform;

            var initTransformData = JObject.Parse(Resources.Load<TextAsset>("ai position").text);
            Debug.Log("json已经成功加载");

            var sw = Stopwatch.StartNew();

            Debug.Log("开始加载精灵图集......");
            var handle = Addressables.LoadAssetsAsync<Sprite>("ai", null);
            var res = await handle.Task;

            sw.Stop();
            Debug.Log($"精灵图集加载完毕!耗时:{sw.ElapsedMilliseconds} ms");

            sw.Restart();

            var startX = 0f;

            foreach (var sprite in res)
            {
                var content = Object.Instantiate(Resources.Load<GameObject>("Role Content Root"), parent: root).transform as RectTransform;

                var roleTemplate = Object.Instantiate(Resources.Load<GameObject>("Role Template"), parent: content).GetComponent<Image>();

                if (root == null || content == null || roleTemplate == null)
                {
                    Object.Destroy(content);
                    Debug.LogWarning("排除一个实例，因为它不符合目标预制体!");
                    continue;
                }

                Debug.Log("节点首次创建成功......");

                var key = sprite.name.Substring(sprite.name.IndexOf('_'));

                var toKen = initTransformData[key];

                if (toKen == null)
                {
                    Debug.LogWarning($"json中没有属性\"{key}\",初始化失败该节点被删除!");
                    Object.Destroy(content);
                    continue;
                }

                Debug.Log("成功找到token......");

                var origin = (x: toKen["x"]?.Value<float>(), y: toKen["y"]?.Value<float>());

                if (origin.x is null || origin.y is null)
                {
                    Debug.LogWarning($"json->{key} 解析失败缺少参数");
                    origin = (0, 0);
                }

                Debug.Log("origin已获取......");

                var size = (width: toKen["width"]?.Value<float>(), height: toKen["height"]?.Value<float>());
                if (size.width is null || size.height is null)
                {
                    Debug.LogWarning($"json->{key} 解析失败缺少参数");
                    size = (100, 100);
                }

                Debug.Log("size......");

                roleTemplate.rectTransform.anchoredPosition = new Vector2(origin.x.Value + startX, origin.y.Value);
                roleTemplate.rectTransform.sizeDelta = new Vector2(size.width.Value, size.height.Value);
                roleTemplate.sprite = sprite;
                roleTemplate.gameObject.SetActive(true);

                startX += size.width.Value;

                Debug.Log("设置成功!");
            }

            sw.Stop();
            Debug.LogWarning($"创建结束,耗时:{sw.ElapsedMilliseconds} ms");
        }

        [MenuItem("Tools/ReName")]
        public static void ReName()
        {
            var objs = Selection.objects;
            foreach (var obj in objs)
            {
                if (obj == null)
                {
                    Debug.LogWarning("未选择任何对象!");
                    return;
                }

                if (obj is not GameObject gameObject)
                {
                    Debug.LogWarning($"\"{obj.name}\"不是游戏对象!");
                    return;
                }

                if (!gameObject.TryGetComponent<Image>(out var image))
                {
                    Debug.LogWarning($"\"{gameObject.name}\"未挂载\"{nameof(Image)}\"组件!");
                    return;
                }

                var name = image.sprite.name;
                if (string.IsNullOrEmpty(name))
                {
                    Debug.LogWarning($"\"{gameObject.name}->{nameof(image)}\"原图像不存在!");
                    return;
                }

                gameObject.name = name;
            }
        }

        [MenuItem("Tools/TransformToJson")]
        public static void TransformToJson()
        {
            var transforms = Selection.transforms;

            var json = new JObject();
            var count = 0;

            var st = Stopwatch.StartNew();
            foreach (var tf in transforms)
            {
                if (tf is not RectTransform rectTransform)
                    continue;

                var match = Regex.Match(tf.name, @"(?<name>.+?)(?<index>_\w_\d+_\d{3,})");

                json[match.Groups["index"].Value] = new JObject
                {
                    ["x"] = rectTransform.anchoredPosition.x,
                    ["y"] = rectTransform.anchoredPosition.y,
                    ["width"] = rectTransform.sizeDelta.x,
                    ["height"] = rectTransform.sizeDelta.y
                };
                count++;
            }

            var fileName = Regex.Match(transforms[0].name, @"(?<name>.+?)(?<index>_\w_\d+_\d{3,})").Groups["name"].Value;
            var filePath = Path.Combine(Application.dataPath, "Resources", $"{fileName} position.json");
            // File.Create(filePath).Close();
            File.WriteAllText(filePath, json.ToString(Formatting.Indented));

            st.Stop();
            Debug.Log($"共<color=#0000ff>{count}</color>项,耗时:<color=red>{st.ElapsedMilliseconds}</color>ms.保存到\"<color=green>{filePath}</color>\"");
            AssetDatabase.Refresh();
        }


        [MenuItem("Tools/SetPosition")]
        public static void SetPosition()
        {
            var select = EditorUtility.DisplayDialog("是否删除", "是否执行修改位置？", "确定", "取消");

            if (!select) return;


            // 名称 （位置，大小）
            var map = new Dictionary<string, (float x, float y, float width, float height)>();

            foreach (KeyValuePair<string, JToken> pair in JsonDataCombine())
            {
                var posInfo = (pair.Value["x"].Value<float>(), pair.Value["y"].Value<float>(), pair.Value["width"].Value<float>(), pair.Value["height"].Value<float>());
                Debug.AssertFormat(map.TryAdd(pair.Key, posInfo), "重复的Key\"{0}\"", pair.Key);
            }

            var root = Selection.activeTransform;
            for (var i = 0; i < root.childCount; i++)
            {
                var currentTf = root.GetChild(i) as RectTransform ?? throw new System.InvalidCastException();

                // 游戏对象"真奈美b_0_5544"未在映射中!
                var key = Regex.Match(currentTf.name, @"_\w_\d+_\d{3,}").Value;
                if (!map.TryGetValue(key, out var info))
                {
                    Debug.LogWarning($"游戏对象\"{currentTf.name}(捕获:\"{key}\")\"未在映射中!");
                }

                // record object info
                Undo.RecordObject(currentTf.gameObject, "modify transform");
                EditorUtility.SetDirty(currentTf.gameObject);

                // size
                currentTf.sizeDelta = new Vector2(info.width, info.height);

                // Position
                currentTf.anchoredPosition = new Vector2(info.x, info.y);
            }


            return;


            JObject JsonDataCombine()
            {
                var ai = JObject.Parse(Resources.Load<TextAsset>("ai position").text);
                foreach (var property in JObject.Parse(Resources.Load<TextAsset>("sin position").text).Properties())
                {
                    if (ai.ContainsKey(property.Name)) continue;
                    ai[property.Name] = property.Value;
                }

                foreach (var property in JObject.Parse(Resources.Load<TextAsset>("yuk position").text).Properties())
                {
                    if (ai.ContainsKey(property.Name)) continue;
                    ai[property.Name] = property.Value;
                }

                foreach (var property in JObject.Parse(Resources.Load<TextAsset>("yzw position").text).Properties())
                {
                    if (ai.ContainsKey(property.Name)) continue;
                    ai[property.Name] = property.Value;
                }

                return ai;
            }
        }
    }
}