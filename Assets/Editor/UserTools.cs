using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public class UserTools : EditorWindow
    {
        private RectTransform _firstRoleContentRoot, _lastRoleContentRoot;
        private string _firstSpriteName, _lastSpriteName;

        private Vector2 _lastRoleContentOffset;

        private string _jsonData;

        private string _path;

        private Vector2 _bodySize;
        private int _currentIndex;

        // private readonly List<Image> _parts = new();

        [MenuItem("Tools/GalGame工具组 %#&G")]
        private static void ShowWindow()
        {
            var window = GetWindow<UserTools>();
            window.titleContent = new GUIContent("TITLE");

            window.Show();
        }

        private void OnEnable()
        {
            _path = PlayerPrefs.GetString("SAVE_PATH", string.Empty);
            _firstSpriteName = PlayerPrefs.GetString("SAVE_FIRST_SPRITE_NAME", string.Empty);
            _lastSpriteName = PlayerPrefs.GetString("SAVE_LAST_SPRITE_NAME", string.Empty);

            _bodySize = new Vector2(PlayerPrefs.GetFloat("SAVE_BODY_SIZE_X", 0), PlayerPrefs.GetFloat("SAVE_BODY_SIZE_Y", 0));

            _currentIndex = PlayerPrefs.GetInt("LAST_INDEX", 0);
        }

        private void OnGUI()
        {
            _firstRoleContentRoot = EditorGUILayout.ObjectField("根节点-1", _firstRoleContentRoot, typeof(RectTransform), true) as RectTransform;
            _lastRoleContentRoot = EditorGUILayout.ObjectField("根节点-2", _lastRoleContentRoot, typeof(RectTransform), true) as RectTransform;

            _lastRoleContentOffset = EditorGUILayout.Vector2Field("-2 偏移", _lastRoleContentOffset);

            EditorGUILayout.Space(10);
            _firstSpriteName = EditorGUILayout.TextField("保存精灵名称", _firstSpriteName);
            _lastSpriteName = EditorGUILayout.TextField("保存精灵名称", _lastSpriteName);

            EditorGUILayout.Space(10);
            _path = EditorGUILayout.TextField("路径:", _path);

            EditorGUILayout.Space(10);
            _bodySize = EditorGUILayout.Vector2Field("身体排除", _bodySize);

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField($"索引:{_currentIndex}");

            if (_firstRoleContentRoot == null)
                return;

            EditorGUILayout.Space(10);
            if (GUILayout.Button("索引更新"))
            {
                var match = Regex.Match(_firstSpriteName, @"\w+_\w+_(?<index>\d{4})").Groups["index"];
                if (!int.TryParse(match.Value, out var currentIndex))
                {
                    Debug.LogWarning($"匹配索引失败:{_firstSpriteName}");
                    return;
                }

                if (currentIndex % 2 != 0)
                {
                    Debug.LogError($"断言失败!{currentIndex}必须能被2整除!");
                    return;
                }

                _currentIndex = (currentIndex += 2) / 2;
                PlayerPrefs.SetInt("LAST_INDEX", _currentIndex);
                _firstSpriteName = _firstSpriteName.Replace(match.Value, currentIndex.ToString("0000"));
                _lastSpriteName = _lastSpriteName.Replace(match.Value, currentIndex.ToString("0000"));
            }

            if (!GUILayout.Button("获取")) return;

            if (!_firstRoleContentRoot.gameObject.activeSelf)
            {
                var select = EditorUtility.DisplayDialog("读取非激活状态的对象", $"对象\"{_firstRoleContentRoot.gameObject.name}\"处于非激活状态,这可能不是预期的!", "确定", "取消");

                if (!select) return;
            }

            // var assembly = Assembly.GetAssembly(typeof(SceneView));
            // var type = assembly.GetType("UnityEditor.LogEntries");
            // var method = type.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
            // method.Invoke(null, null);

            // var facesJson = new JArray();
            // ReadJson(_firstRoleContentRoot, facesJson);
            //
            // if (!ReadJson(_lastRoleContentRoot, facesJson))
            // {
            //     Debug.LogWarning("根节点-2未指定");
            // }

            // 先获取之前的数据
            var savePath = Path.Combine(Application.dataPath, _path);

            var jObjectData = File.Exists(savePath)
                ? JObject.Parse(File.ReadAllText(savePath))
                : new JObject();

            var spriteFirstName = _firstSpriteName;
            var spriteLastName = _lastSpriteName;

            var index = 1;
            while (!jObjectData.TryAdd(spriteFirstName, ReadJson(_firstRoleContentRoot, Vector2.zero)))
            {
                spriteFirstName = $"{_firstSpriteName}_{index++}";
            }

            Debug.Log($"名称为:<color=red>{spriteFirstName}</color>");

            if (_lastRoleContentRoot != null)
            {
                index = 1;
                while (!jObjectData.TryAdd(spriteLastName, ReadJson(_lastRoleContentRoot, _lastRoleContentOffset)))
                {
                    spriteLastName = $"{_lastSpriteName}_{index++}";
                }

                Debug.Log($"名称为:<color=red>{spriteLastName}</color>");
            }


            File.WriteAllText(savePath, jObjectData.ToString(Formatting.Indented));
            Debug.Log($"保存成功:{savePath}");

            PlayerPrefs.SetString("SAVE_PATH", _path);
            PlayerPrefs.SetString("SAVE_FIRST_SPRITE_NAME", _firstSpriteName);
            PlayerPrefs.SetString("SAVE_LAST_SPRITE_NAME", _lastSpriteName);
            PlayerPrefs.SetFloat("SAVE_BODY_SIZE_X", _bodySize.x);
            PlayerPrefs.SetFloat("SAVE_BODY_SIZE_Y", _bodySize.y);


            AssetDatabase.Refresh();
            return;

            JArray ReadJson(Transform root, Vector2 offset)
            {
                var faceItemArray = new JArray();
                for (var i = 0; i < root.childCount; i++)
                {
                    if (!root.GetChild(i).gameObject.activeSelf) continue;

                    var current = root.GetChild(i).GetComponent<Image>();
                    var pos = current.rectTransform.anchoredPosition;
                    var size = current.rectTransform.sizeDelta;

                    if (size.x * size.y > _bodySize.x * _bodySize.y)
                    {
                        Debug.LogWarning($"跳过\"{current.gameObject.name}\"因为它的Image大于 {size}");
                        continue;
                    }

                    faceItemArray.Add(new JObject
                    {
                        ["path"] = current.sprite.name,
                        ["x"] = pos.x + offset.x,
                        ["y"] = pos.y + offset.y,
                        ["width"] = size.x,
                        ["height"] = size.y,
                    });

                    // _parts.Add(current);
                }

                return faceItemArray;
            }
        }
    }
}