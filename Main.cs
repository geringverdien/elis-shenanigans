using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace elisShenanigans
{
    public class Main
    {
        private static GameObject _holderObject;
        public static void Init()
        {
            Plugin.Logger.LogInfo("the silly menu is real!! :3");

            _holderObject = new GameObject("silly menu holder");
            _holderObject.AddComponent<Cache>();
            _holderObject.AddComponent<Menu>();
            UnityEngine.Object.DontDestroyOnLoad(_holderObject);

            Plugin.Logger.LogInfo("created holder object and added components.");
            Plugin.Logger.LogInfo(_holderObject.scene);
        }
    }
}
