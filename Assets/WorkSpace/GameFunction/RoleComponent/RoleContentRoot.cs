using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using WorkSpace.Global.CharacterIllustration;
using WorkSpace.StructData.SO;

namespace WorkSpace.GameFunction.RoleComponent
{
    public class RoleContentRoot : MonoBehaviour
    {
        /// <summary>
        /// 一些预制体资源
        /// </summary>
        private static PreformScriptableObject _preformScriptableObject;

        /// <summary>
        /// key: typeCode
        /// value: 精灵组信息
        /// </summary>
        private Dictionary<string, SpriteAsset> _spriteAssetsTransformInfoMap;

        /// <summary>
        /// key:图片源名称
        /// value:图片引用(未加载)
        /// </summary>
        private readonly Dictionary<string, SpriteResourceLoader> _refAssetsMap = new();

        // 用于存放上一次的使用的 typeCode
        // 如果使用时有相等则不再重复加载
        private (string bodyCode, string faceCode) _lastTypeCode = (string.Empty, string.Empty);


        // UNITY 面板引用 (在预制体中)
        [SerializeField] private RoleBodyType body;
        [SerializeField] private RoleBodyType faces;
        //


        /// <summary>
        /// 强烈建议 <see cref="Type"/> 名称与要加载的资源标签名称相同
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// 引用状态
        /// </summary>
        public RefState State { get; private set; }

        /// <summary>
        /// Body 节点控制器
        /// </summary>
        public RoleBodyType Body
        {
            get => body;
            private set => body = value;
        }

        /// <summary>
        /// Faces 节点控制器
        /// </summary>
        public RoleBodyType Faces
        {
            get => faces;
            private set => faces = value;
        }

        /// <summary>
        /// 一些预制体资源
        /// </summary>
        public static PreformScriptableObject PreformScriptableObject
        {
            get
            {
                if (_preformScriptableObject != null)
                {
                    return _preformScriptableObject;
                }

                return _preformScriptableObject = Resources.Load<PreformScriptableObject>("New PreformData");
            }
        }

        /// <summary>
        /// <para>  *必须调用</para>
        /// 异步加载资源类型为 <see cref="Sprite"/> 并且标签为 <see cref="label"/>
        /// <para>(不指定时使用 <see cref="Type"/>) 作为 <see cref="label"/></para>
        /// </summary>
        /// <param name="label">资源标签名称</param>
        /// <returns>此方法需要配合协程使用</returns>
        public IEnumerator LoadRefAssetsAsync(string label = null)
        {
            State = RefState.None;
            if (string.IsNullOrEmpty(label) || string.IsNullOrWhiteSpace(label))
            {
                label = Type;
            }

            var refsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Sprite));

            State = RefState.Loading;
            yield return refsHandle;

            if (refsHandle.Status != AsyncOperationStatus.Succeeded)
            {
                State = RefState.Fail;
            }

            foreach (var assetRef in refsHandle.Result)
            {
                // assetRef.PrimaryKey 返回图片的源名称
                // (前提需要图片命名遵循本项目的命名规范)
                _refAssetsMap.Add(assetRef.PrimaryKey, new SpriteResourceLoader(assetRef));
            }

            State = RefState.Ok;
            Addressables.Release(refsHandle);
        }

        /// <summary>
        /// 显示立绘
        /// </summary>
        /// <param name="bodyCode">身体</param>
        /// <param name="faceCode">脸部</param>
        /// <returns>此方法需要配合协程使用</returns>
        public IEnumerator Display(string bodyCode, string faceCode)
        {
            if (State == RefState.None)
            {
                Debug.LogWarning("资源引用未加载,尝试加载中......");
                yield return LoadRefAssetsAsync(Type);
                yield return Display(bodyCode, faceCode);
                yield break;
            }

            if (State == RefState.Loading)
            {
                Debug.LogWarning("资源引用未加载完成,尝试等待中......");
                yield return new WaitUntil(() => State != RefState.Loading);
                yield return Display(bodyCode, faceCode);
                yield break;
            }

            if (State == RefState.Fail)
            {
                Debug.LogError("资源加载失败!");
                yield break;
            }

            // 如何 typeCode 与上一次一样就没必要设置了，同时也可以提供性能
            if (_lastTypeCode.bodyCode != bodyCode)
            {
                _lastTypeCode.bodyCode = bodyCode;
                // 设置身体
                yield return LoadAssetAsync(bodyCode,
                    bodySprites => Body.Display(bodySprites, _spriteAssetsTransformInfoMap[bodyCode].Data.TransformInfoData));
            }

            if (_lastTypeCode.faceCode == faceCode)
            {
                yield break;
            }

            _lastTypeCode.faceCode = faceCode;

            // 设置脸部
            yield return LoadAssetAsync(faceCode,
                facesSprites => Faces.Display(facesSprites, _spriteAssetsTransformInfoMap[faceCode].Data.TransformInfoData));
        }

        /// <summary>
        /// 将引用的资源(图集)加载到内存中
        /// </summary>
        /// <param name="typeCode">图集代码</param>
        /// <param name="onCompletion">在加载完所有图集时</param>
        /// <returns>此方法需要配合协程使用</returns>
        public IEnumerator LoadAssetAsync(string typeCode, Action<Sprite[]> onCompletion)
        {
            if (string.IsNullOrEmpty(typeCode) || string.IsNullOrWhiteSpace(typeCode))
            {
                Debug.LogError("空的\"typeCode\"");
                yield break;
            }

            if (!_spriteAssetsTransformInfoMap.TryGetValue(typeCode, out var spriteAsset))
            {
                Debug.LogError($"不存在的资源名称\"{typeCode}\"");
                yield break;
            }

            var transformInfoData = spriteAsset.Data.TransformInfoData;
            var sprites = new Sprite[transformInfoData.Length];

            // 这里获取的就是这个"body(faces) code"对应的精灵立绘信息
            for (var i = 0; i < sprites.Length; i++)
            {
                var resource = _refAssetsMap[transformInfoData[i].Path];
                var index = i;
                yield return resource.GetAsync(sprite => sprites[index] = sprite);
            }

            onCompletion?.Invoke(sprites);
        }

        /// <summary>
        /// 将引用的资源(图集)加载到内存中
        /// </summary>
        /// <param name="typeCode">图集代码</param>
        /// <param name="onItemCompletion">在加载完成一个图集时</param>
        /// <returns>此方法需要配合协程使用</returns>
        public IEnumerator LoadAssetAsync(string typeCode, Action<Sprite> onItemCompletion)
        {
            if (string.IsNullOrEmpty(typeCode) || string.IsNullOrWhiteSpace(typeCode))
            {
                Debug.LogError("空的\"typeCode\"");
                yield break;
            }

            if (!_spriteAssetsTransformInfoMap.TryGetValue(typeCode, out var spriteAsset))
            {
                Debug.LogError($"不存在的资源名称\"{typeCode}\"");
                yield break;
            }

            // 这里获取的就是这个"body(faces) code"对应的精灵立绘信息
            foreach (var data in spriteAsset.Data.TransformInfoData)
            {
                var resource = _refAssetsMap[data.Path];
                yield return resource.GetAsync(sprite => onItemCompletion?.Invoke(sprite));
            }
        }
        


        /// <summary>
        /// 创建一个角色立绘控制器
        /// </summary>
        /// <param name="parent">立绘父节点 (*需要是 RectTransform)</param>
        /// <param name="roleTye">立绘控制器类型(名称) (*强烈建议您将这个控制器需要加载的标签名称作为参数名称)</param>
        /// <param name="transformInfoDataJson">立绘图集初始化参数信息 JSON</param>
        /// <returns></returns>
        public static RoleContentRoot CreateRole(Transform parent, string roleTye, JObject transformInfoDataJson)
        {
            var template = PreformScriptableObject.Table["Role Content Root Template"].Preform;
            template.name = $"RoleContentRoot#{roleTye}#";
            return Instantiate(template, parent: parent).GetComponent<RoleContentRoot>().Init(roleTye, transformInfoDataJson);
        }


        private RoleContentRoot Init(string roleTye, JObject transformInfoDataJson)
        {
            Type = roleTye;
            Body.Init(nameof(Body), 1);
            Faces.Init(nameof(Faces), 4);

            _spriteAssetsTransformInfoMap = RoleTransformInfo.Load(transformInfoDataJson);
            return this;
        }
        

    }
}