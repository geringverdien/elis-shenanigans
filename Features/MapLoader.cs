using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BepInEx;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

namespace elisShenanigans.Features
{
    public class MapLoader
    {
        public static string[] GetMapFiles()
        {
            string pluginPath = Path.Combine(Paths.PluginPath, $"eli-{MyPluginInfo.PLUGIN_GUID}");
            if (!Directory.Exists(pluginPath))
            {
                Menu.Instance.Log($"Plugin path {pluginPath} does not exist");
                return [];
            }

            string mapsFolder = Path.Combine(pluginPath, "maps");
            if (!Directory.Exists(mapsFolder))
            {
                Directory.CreateDirectory(mapsFolder);
                Menu.Instance.Log($"Maps folder {mapsFolder} does not exist (created folder)");
                return [];
            }

            string[] files = Directory.GetFiles(mapsFolder, "*.json");
            return files;
        }

        public static async void LoadMap(string filePath)
        {
            DeserializedMap mapData = GetMapData(filePath);

            if (mapData.type != "VictoryScene" && !ObjectSpawner.IsMapSupported())
            {
                Menu.Instance.Log($"Map type {mapData.type} requires vending machine or mine item on current map");
                return;
            }

            if (SceneManager.GetActiveScene().name != "EndGame" && mapData.type == "VictoryScene") await PrepareScene();

            ItemDispenser[] originalDispensers = GameObject.FindObjectsOfType<ItemDispenser>();

            GameObject itemDispenserObj = null;

            foreach (ItemDispenser dispenser in Resources.FindObjectsOfTypeAll<ItemDispenser>())
            {
                GameObject dispenserObject = dispenser.gameObject;
                if (dispenserObject && dispenserObject.scene.IsValid()) continue;
                itemDispenserObj = dispenserObject;
                break;
            }

            await ObjectSpawner.SpawnObject(itemDispenserObj, new Vector3(0, float.MaxValue-5, 0), Quaternion.identity);

            Menu.Instance.Log("Created starter dispenser for setup");

            await Task.Delay(250);

            itemDispenserObj = GameObject.FindObjectOfType<ItemDispenser>().gameObject;

            Menu.Instance.Log($"loading {mapData.objs.Count} objects...");
            foreach (MapObject obj in mapData.objs)
            {
                Vector3 position = new Vector3((float) obj.pos[0], (float) obj.pos[1], (float) obj.pos[2]);
                Quaternion rotation = Quaternion.Euler((float) obj.rot[0], (float) obj.rot[1], (float) obj.rot[2]);
                await ObjectSpawner.SpawnObject(itemDispenserObj, position, rotation);
                await Task.Delay(50);
            }

            await Task.Delay(50);

            foreach (ItemDispenser dispenser in originalDispensers) 
            {
                Exploits.Destroy(dispenser.gameObject);
                await Task.Delay(50);
            }

            Menu.Instance.Log($"Loaded custom map {Path.GetFileNameWithoutExtension(filePath)} :3");
        }

        private static async Task PrepareScene()
        { 
            Exploits.ForceLoadScene("EndGame");
            await Utils.WaitUntilAsync(() => SceneManager.GetActiveScene().name == "EndGame");
            Menu.Instance.Log("Loaded into EndGame scene");
            await Task.Delay(800);

            return;
        }

        public static DeserializedMap GetMapData(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Menu.Instance.Log($"{filePath} not found");
                return null;
            }

            string json = File.ReadAllText(filePath, Encoding.UTF8);
            DeserializedMap objectList = JsonConvert.DeserializeObject<DeserializedMap>(json);
            return objectList;
        }
    }

    public class MapObject
    {
        public List<double> pos { get; set; }
        public List<double> rot { get; set; }
    }

    public class DeserializedMap
    {
        public string type { get; set; }
        public List<MapObject> objs { get; set; }
    }


}
