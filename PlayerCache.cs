using UnityEngine;

namespace elisShenanigans
{
    public class PlayerCache
    {
        public ClientInstance ClientInstance { get; private set; }
        public GameObject GameObject { get; private set; }
        public PlayerHealth PlayerHealth { get; private set; }
        public PlayerPickup PlayerPickup { get; private set; }
        public PlayerManager PlayerManager { get; private set; }
        public FirstPersonController FirstPersonController { get; private set; }
        public Collider Collider { get; private set; }
        public ulong SteamID { get; private set; } = 0;
        public string PlayerName { get; private set; } = "Unknown";
        public bool IsAlive { get; private set; } = true;

        public PlayerCache(ClientInstance clientInstance)
        {


            ClientInstance = clientInstance;
            PlayerManager = ClientInstance.PlayerSpawner;
            SteamID = ClientInstance.PlayerSteamID;
            PlayerName = ClientInstance.PlayerName;

            foreach (GameObject playerRig in GameObject.FindGameObjectsWithTag("Player"))
            {
                PlayerValues playerValues = playerRig.GetComponent<PlayerValues>();
                if (playerValues.playerClient != clientInstance) continue;

                this.GameObject = playerRig;
                break;
            }

            if (!GameObject)
            {
                IsAlive = false;
                return;
            }

            FirstPersonController = GameObject.GetComponent<FirstPersonController>();
            PlayerHealth = GameObject.GetComponent<PlayerHealth>();
            PlayerPickup = GameObject.GetComponent<PlayerPickup>();
            Collider = GameObject.GetComponent<Collider>();
        }
    }
}