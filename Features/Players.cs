using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace elisShenanigans.Features
{
    public class Players
    {
        private static MethodInfo _setObjectInHandServerMethod;
        private static MethodInfo _bumpPlayerServerMethod;
        public static void InfHealth(PlayerCache player)
        {
            player.PlayerHealth.RemoveHealth(-float.MaxValue);
        }

        public static void Kill(PlayerCache player)
        {
            player.PlayerHealth.RemoveHealth(10);
            GameManager.Instance.PlayerDied(player.ClientInstance.PlayerId);
        }
        public static void SetFreeze(PlayerCache player, bool freeze)
        { 
            player.PlayerHealth.TaserEnemy(player.PlayerHealth, freeze ? float.MaxValue : 0);
        }
        public static void Kick(PlayerCache player)
        {
            if (Cache.Instance.LocalPlayer == null)
            {
                Menu.Instance.Log("LocalPlayer must exist");
                return;
            }


            Exploits.Destroy(player.GameObject);

        }

        public static void Fling(PlayerCache player)
        {
            if (!Cache.Instance.LocalPlayer.GameObject.activeInHierarchy)
            {
                Menu.Instance.Log("LocalPlayer must be alive to fling");
                return;
            }

            if (_bumpPlayerServerMethod == null)
            {
                _bumpPlayerServerMethod = typeof(PlayerPickup).GetMethod(
                    "BumpPlayerServer",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

                if (_bumpPlayerServerMethod == null)
                {
                    Menu.Instance.Log("Failed to find private method SetObjectInHandServer");
                    return;
                }
            }
            try
            {
                _bumpPlayerServerMethod.Invoke(Cache.Instance.LocalPlayer.PlayerPickup, [new Vector3(10, 10, 10), 99999999, player.PlayerHealth]);
            }
            catch (Exception ex)
            {
                Menu.Instance.Log($"Error calling BumpPlayerServer: {ex.Message}");
            }
        }

        public static void BreakPlayer(PlayerCache player)
        {
            PlayerPickup playerPickup = Cache.Instance.LocalPlayer.PlayerPickup;

            if (playerPickup == null)
            {
                Menu.Instance.Log("PlayerPickup not found on LocalPlayer");
                return;
            }

            if (_setObjectInHandServerMethod == null)
            {
                _setObjectInHandServerMethod = typeof(PlayerPickup).GetMethod(
                    "SetObjectInHandServer",
                    BindingFlags.Instance | BindingFlags.NonPublic
                );

                if (_setObjectInHandServerMethod == null)
                {
                    Menu.Instance.Log("Failed to find private method SetObjectInHandServer");
                    return;
                }
            }
            try
            {
                _setObjectInHandServerMethod.Invoke(playerPickup, [player.GameObject, Vector3.zero, new Quaternion(), player.GameObject, false]);
            }
            catch
            {
                //Menu.Instance.Log($"Error calling SetObjectInHandServer: {ex.Message}");
                Menu.Instance.Log($"Broke {player.PlayerName}'s character");
            }
        }
    }
}
