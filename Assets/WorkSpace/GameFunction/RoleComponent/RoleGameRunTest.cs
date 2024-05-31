using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace WorkSpace.GameFunction.RoleComponent
{
    public class RoleGameRunTest : MonoBehaviour
    {
        [SerializeField] [Range(0F, 1F)] private float value;

        private IEnumerator Start()
        {
            yield return null;

            var roleContent = RoleContentRoot.CreateRole(transform, "ai",
                JObject.Parse(Resources.Load<TextAsset>("Transform/ai faces test").text));

            // 加载资源引用 (可选)
            yield return roleContent.LoadRefAssetsAsync();

            // 显示
            yield return roleContent.Display("ai_a_0000", "ai_a_0024");
            
        }
    }
}