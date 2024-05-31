using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using WorkSpace.GameFunction.RoleComponent;

namespace WorkSpace.Global.CharacterIllustration
{
    public static class SpriteResourceManager
    {
        private static readonly Dictionary<string, SpriteAssetLoader> TypeToSpriteAssetLoaders = new();

        public static IEnumerator CreateAssetAsync(string type, JObject roleTransformInfoJObject)
        {
            yield return RoleTransformInfo.LoadAsync(roleTransformInfoJObject,
                spriteAssetMap =>
                {
                    var spriteAssetLoader = new SpriteAssetLoader(spriteAssetMap);
                    if (!TypeToSpriteAssetLoaders.TryAdd(type, spriteAssetLoader))
                    {
                        Debug.LogError($"可能包含重复的key\"{type}\"!");
                    }
                });
        }

        public static SpriteAssetLoader Get(string type) => TypeToSpriteAssetLoaders[type];

        public static bool TryGet(string type, out SpriteAssetLoader spriteAssetLoader) => TypeToSpriteAssetLoaders.TryGetValue(type, out spriteAssetLoader);
    }
}