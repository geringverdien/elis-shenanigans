using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

namespace elisShenanigans
{
    public class Cache : MonoBehaviour
    {
        private readonly FieldInfo PlayerPickup_weaponInHand = typeof(PlayerPickup).GetField("weaponInHand", BindingFlags.NonPublic | BindingFlags.Instance);
        private readonly FieldInfo PlayerPickup_weaponInLeftHand = typeof(PlayerPickup).GetField("weaponInLeftHand", BindingFlags.NonPublic | BindingFlags.Instance);

        private float _lastTimeUpdated = 0;
        private float _updateInterval = 1;
        private List<PlayerCache> _players = [];

        public static Cache Instance { get; private set; }

        public bool InLobby { get; private set; }
        public bool InGame { get; private set; }
        public CSteamID LobbyOwnerID { get; private set; }
        public PlayerCache LocalPlayer { get; private set; }
        public Weapon LocalWeaponLeft { get; private set; }
        public Weapon LocalWeaponRight { get; private set; }
        public ClientInstance LocalClientInstance { get; private set; }
        public PlayerManager LocalPlayerManager { get; private set; }
        public FirstPersonController LocalController { get; private set; }
        public Camera MainCamera { get; private set; }
        public List<PlayerCache> Players => _players;

        private void UpdateCache()
        {
            LocalClientInstance = ClientInstance.Instance;
            if (!(LocalClientInstance != null))
            {
                InGame = false;
                InLobby = false;
                LocalPlayer = null;
                Menu.Instance.FreezeStates.Clear();
                return;
            }

            InLobby = true;
            InGame = SceneManager.GetActiveScene().name != "MainMenu";
            LobbyOwnerID = SteamMatchmaking.GetLobbyOwner((CSteamID)SteamLobby.Instance.CurrentLobbyID);


            LocalPlayer = new PlayerCache(ClientInstance.Instance);
            LocalPlayerManager = LocalClientInstance.PlayerSpawner;
            LocalController = LocalPlayerManager.player;



            if (LocalPlayer.IsAlive)
            {
                MainCamera = LocalController.playerCamera;

                LocalWeaponLeft = (Weapon)PlayerPickup_weaponInLeftHand.GetValue(LocalPlayer.PlayerPickup);
                LocalWeaponRight = (Weapon)PlayerPickup_weaponInHand.GetValue(LocalPlayer.PlayerPickup);
            }

            _players.Clear();

            foreach (GameObject clientInstanceHolder in GameObject.FindGameObjectsWithTag("ClientInstance"))
            {
                //if (clientInstance == LocalPlayer.GameObject)
                //    continue;
                ClientInstance playerClientInstance = clientInstanceHolder.GetComponent<ClientInstance>();

                if (playerClientInstance == null) continue;

                PlayerCache player = new PlayerCache(playerClientInstance);

                _players.Add(player);
            }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        public void Update()
        {
            if (Time.time - _lastTimeUpdated > _updateInterval)
            {
                _lastTimeUpdated = Time.time;
                UpdateCache();
            }
        }
    }
}