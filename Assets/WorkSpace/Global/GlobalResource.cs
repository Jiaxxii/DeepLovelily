using UnityEngine;

namespace WorkSpace.Global
{
    public static class GlobalResource
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void GameInit()
        {
        }
    }
}