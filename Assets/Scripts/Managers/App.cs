using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Incandescent.Managers
{
    public static class App
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Debug.Log("App: Initialize()");
            
            GameObject app = Object.Instantiate(Resources.Load("obj_app")) as GameObject;
            if (app == null)
                throw new ApplicationException("Could not find App prefab in Resources folder.");

            Object.DontDestroyOnLoad(app);
        }
    }
}