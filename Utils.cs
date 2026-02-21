using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace elisShenanigans
{
    public class Utils
    {
        public GameObject FindRecursive(string name, Transform root)
        {
            Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>(true);
            GameObject gameObject = null;
            foreach (Transform transform in componentsInChildren)
            {
                if (transform.gameObject.name == name)
                {
                    gameObject = transform.gameObject;
                    break;
                }
            }
            return gameObject;
        }

        public static List<string> GetAllSceneNames()
        {
            List<string> sceneNames = new List<string>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                string path = SceneUtility.GetScenePathByBuildIndex(i);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);

                sceneNames.Add(sceneName);
            }
            sceneNames.Sort();
            return sceneNames;
        }

        public static async Task WaitUntilAsync(System.Func<bool> condition, int checkIntervalMs = 50)
        {
            while (!condition())
            {
                await Task.Delay(checkIntervalMs);
            }
        }
    }
}
