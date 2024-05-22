using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using WorkSpace.GameFunction.Lerp;


namespace WorkSpace.GameFunction.RoleComponent
{
    public class RoleGameRunTest : MonoBehaviour
    {
        [SerializeField] [Range(0F, 1F)] private float value;

        private IEnumerator Start()
        {
            // var initJson = JObject.Parse(Resources.Load<TextAsset>("Transform/yuk faces test").text);
            //
            // var controlHandel = RoleContentRoot.CreateRole(transform, "yuk", initJson);
            // // yield return controlHandel.LoadRefAssetsAsync();
            // 
            // yield return controlHandel.Display("yuk_a_0000", "yuk_a_0010");

            var image = GetComponent<Image>();

            var property = new[]
            {
                image.rectTransform.GalgameAnchoredPosition(new Vector2(100, 0)),
                image.transform.GalgameEulerAngles(new Vector3(0, 0, 360)),
                image.GalgameColorA(new Color(1, 0, 0, 0))
            };

            while (true)
            {
                property.GalgameSetValue(value);
                yield return null;
            }
        }
    }
}