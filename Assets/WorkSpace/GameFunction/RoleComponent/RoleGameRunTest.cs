using System.Collections;
using UnityEngine;


namespace WorkSpace.GameFunction.RoleComponent
{
    public class RoleGameRunTest : MonoBehaviour
    {
        [SerializeField] [Range(0F, 1F)] private float value;

        private IEnumerator Start()
        {
            yield return null;
        }
    }
}