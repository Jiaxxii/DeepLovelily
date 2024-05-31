using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorkSpace.StructData;

namespace WorkSpace.GameFunction.RoleComponent
{
    public class RoleUnit : MonoBehaviour
    {
        public Image SpriteContent { get; private set; }

        public bool Active
        {
            get => SpriteContent.gameObject.activeSelf;
            set => SpriteContent.gameObject.SetActive(value);
        }

        private RoleUnit Init()
        {
            SpriteContent = GetComponent<Image>();
            return this;
        }
        
        
        public void Display(Sprite sprite, TransformInfo transformInfo)
        {
            SetStyle(sprite, transformInfo.Size, transformInfo.Position);
            Active = true;
        }

        public void Hide(bool isClear = false)
        {
            Active = false;
            if (isClear)
            {
                Clear();
            }
        }

        public void Clear() => SetStyle(null, Vector2.zero, Vector2.zero);

        private void SetStyle(Sprite sprite, Vector2 size, Vector2 position)
        {
            SpriteContent.sprite = sprite;
            SpriteContent.rectTransform.sizeDelta = size;
            SpriteContent.rectTransform.anchoredPosition = position;
        }

        internal static IEnumerable<RoleUnit> CreateRole(Transform parent, int reserveNumber = 4)
        {
            var template = RoleContentRoot.PreformScriptableObject.Table["Role Unit Template"].Preform;

            var roleUnitArray = new RoleUnit[reserveNumber];
            for (var i = 0; i < reserveNumber; i++)
            {
                var roleUnitRect = Instantiate(template, parent: parent)
                    .GetComponent<RectTransform>();

                roleUnitRect.name = $"RoleUnit#{i}#";

                roleUnitRect.pivot = new Vector2(.5f, .5f);
                roleUnitRect.localScale = Vector3.one;

                roleUnitArray[i] = roleUnitRect.GetComponent<RoleUnit>().Init();
            }

            return roleUnitArray;
        }
    }
}