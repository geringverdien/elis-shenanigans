using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using FishNet.Object;

namespace elisShenanigans.Features
{
    public class ObjectSpawner
    {
        private static MethodInfo _spawnWeaponMethod;
        private static WeaponHandSpawner _spawnerMine;

        public static bool IsMapSupported()
        {
            ItemDispenser dispenser = UnityEngine.Object.FindObjectOfType<ItemDispenser>();
            WeaponHandSpawner mine = UnityEngine.Object.FindObjectOfType<WeaponHandSpawner>();

            return ((dispenser && dispenser.gameObject.activeInHierarchy) || (mine && mine.gameObject.activeInHierarchy));
        }

        public static async Task SpawnObject(GameObject obj, Vector3 pos = new Vector3(), Quaternion rot = new Quaternion())
        {
            if (_spawnerMine == null || !_spawnerMine.gameObject.activeInHierarchy)
            {
                await CreateSpawnerItem();
            }

            if (obj.GetComponent<NetworkBehaviour>() == null)
            {
                Menu.Instance.Log("Can only spawn networked objects");
                return;
            }

            try
            {
                _spawnerMine.SpawnObject(obj, pos, rot);
            }
            catch 
            { 
            
            }
            Menu.Instance.Log($"Created {obj.name}");
        }

        private static async Task CreateSpawnerItem()
        {
            ItemDispenser dispenser = UnityEngine.Object.FindObjectOfType<ItemDispenser>();
            WeaponHandSpawner mine = UnityEngine.Object.FindObjectOfType<WeaponHandSpawner>();

            if (mine)
            {
                Menu.Instance.Log("Using existing mine to create spawner mine");
                await DoMineSpawn();
                return;
            }

            if (_spawnWeaponMethod == null)
            {
                _spawnWeaponMethod = typeof(ItemDispenser).GetMethod(
                    "SpawnWeapon",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

                if (_spawnWeaponMethod == null)
                {
                    Menu.Instance.Log("Failed to find private method SpawnWeapon");
                    return;
                }
            }
            try
            {
                WeaponHandSpawner unloadedMineObject = Resources.FindObjectsOfTypeAll<WeaponHandSpawner>()[0];
                try
                {
                    _spawnWeaponMethod.Invoke(dispenser, [unloadedMineObject.gameObject]);
                }
                catch (Exception ex)
                {
                    Menu.Instance.Log($"Error invoking SpawnWeapon: {ex.Message}");
                }
                Menu.Instance.Log($"Created Init mine");
                await Task.Delay(100);
                await DoMineSpawn();
            }
            catch (Exception ex)
            {
                Menu.Instance.Log($"Error finishing dispenser routine: {ex.Message}");
            }
        }

        private static async Task DoMineSpawn()
        { 
            WeaponHandSpawner tempMine = await GetAndClaimMineAsync();
            try
            {
                tempMine.SpawnObject(tempMine.gameObject, new Vector3(0, float.MaxValue, 0), new Quaternion());
            }
            catch
            {
                //Menu.Instance.Log($"Error spawning mine: {ex.Message}");
            }
            await Task.Delay(100);
            Exploits.Destroy(tempMine.gameObject);
            _spawnerMine = await GetAndClaimMineAsync();
            Menu.Instance.Log("Created spawner mine :3");
        }

        private static async Task<WeaponHandSpawner> GetAndClaimMineAsync()
        {
            WeaponHandSpawner highestMine = UnityEngine.Object.FindObjectOfType<WeaponHandSpawner>();

            foreach (WeaponHandSpawner spawner in UnityEngine.Object.FindObjectsOfType<WeaponHandSpawner>())
            {
                if (spawner.cam != null && spawner.cam != Cache.Instance)
                    continue;

                if (highestMine && spawner.transform.position.y <= highestMine.transform.position.y) continue;

                highestMine = spawner;
                
            }

            Exploits.Claim(highestMine.gameObject);

            await Utils.WaitUntilAsync(() => highestMine.IsOwner);

            return highestMine;
        }
    }
}
