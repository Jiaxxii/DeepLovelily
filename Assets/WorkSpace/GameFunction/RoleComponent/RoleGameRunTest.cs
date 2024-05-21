using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace WorkSpace.GameFunction.RoleComponent
{
    public class RoleGameRunTest : MonoBehaviour
    {

        private IEnumerator Start()
        {
            var initJson = JObject.Parse(Resources.Load<TextAsset>("Transform/yuk faces test").text);

            var controlHandel = RoleContentRoot.CreateRole(transform, "yuk", initJson);
            // yield return controlHandel.LoadRefAssetsAsync();

            yield return controlHandel.Display("yuk_a_0000", "yuk_a_0010");
        }
    }
}