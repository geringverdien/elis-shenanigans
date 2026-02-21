using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;
using FishNet.Object;
using elisShenanigans.Features;
using System.IO;

namespace elisShenanigans
{
    public class Menu : MonoBehaviour
    {
        //private Cache Cache => Cache.Instance;
        private bool _menuOpen = false;
        private bool _changedScene = true;
        private Rect _windowRect = new(100, 100, 1075, 480);
        private string currentMenu = "Home";
        private readonly List<string> _sceneNames = Utils.GetAllSceneNames();
        public static Menu Instance { get; private set; }


        private Vector2 _debugScrollPosition = Vector2.zero;
        private List<string> _debugLogs = new List<string>();
        private const int MAX_LOGS = 50;

        private GUIStyle _windowStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _toggleStyle;
        private GUIStyle _logStyle;
        private GUIStyle _panelStyle;
        private GUIStyle _tabStyle;
        private GUIStyle _activeTabStyle;
        private GUIStyle _toggleButtonStyle;
        private GUIStyle _toggleButtonActiveStyle;
        private GUIStyle _customToggleStyle;
        private GUIStyle _customToggleActiveStyle;
        private GUIStyle _warningStyle;

        //private Color _accentColor = new Color(0.2f, 0.6f, 1f);
        private Color _backgroundColor = new Color(0.12f, 0.12f, 0.12f, 0.95f);
        private Color _headerColor = new Color(0.15f, 0.15f, 0.15f, 1f);
        private Color _buttonColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        private Color _buttonHoverColor = new Color(0.25f, 0.25f, 0.25f, 1f);
        private Color _toggleActiveColor = new Color(0.2f, 0.6f, 1f, 1f);
        private Color _warningColor = new Color(1f, 0.5f, 0f, 1f);

        private Texture2D MakeTexture(int width, int height, Color color)
        {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = color;

            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private void InitializeStyles()
        {
            _windowStyle = new GUIStyle(GUI.skin.window);
            _windowStyle.normal.background = MakeTexture(1, 1, _backgroundColor);
            _windowStyle.onNormal.background = MakeTexture(1, 1, _backgroundColor);
            _windowStyle.hover.background = MakeTexture(1, 1, _backgroundColor);
            _windowStyle.active.background = MakeTexture(1, 1, _backgroundColor);
            _windowStyle.focused.background = MakeTexture(1, 1, _backgroundColor);
            _windowStyle.onFocused.background = MakeTexture(1, 1, _backgroundColor);
            _windowStyle.normal.textColor = Color.white;
            _windowStyle.fontSize = 14;
            _windowStyle.fontStyle = FontStyle.Bold;
            _windowStyle.padding = new RectOffset(10, 10, 10, 10);

            _headerStyle = new GUIStyle(GUI.skin.label);
            _headerStyle.normal.background = MakeTexture(2, 2, _headerColor);
            _headerStyle.normal.textColor = Color.white;
            _headerStyle.fontSize = 16;
            _headerStyle.fontStyle = FontStyle.Bold;
            _headerStyle.alignment = TextAnchor.MiddleLeft;
            _headerStyle.padding = new RectOffset(10, 10, 5, 5);

            _buttonStyle = new GUIStyle(GUI.skin.button);
            _buttonStyle.normal.background = MakeTexture(2, 2, _buttonColor);
            _buttonStyle.hover.background = MakeTexture(2, 2, _buttonHoverColor);
            _buttonStyle.normal.textColor = Color.white;
            _buttonStyle.fontSize = 12;
            _buttonStyle.padding = new RectOffset(10, 10, 5, 5);

            _toggleStyle = new GUIStyle(GUI.skin.toggle);
            _toggleStyle.normal.textColor = Color.white;
            _toggleStyle.hover.textColor = Color.white;
            _toggleStyle.fontSize = 12;
            _toggleStyle.padding = new RectOffset(5, 5, 5, 5);

            _logStyle = new GUIStyle(GUI.skin.label);
            _logStyle.normal.textColor = Color.white;
            _logStyle.fontSize = 11;
            _logStyle.wordWrap = true;
            _logStyle.padding = new RectOffset(5, 5, 2, 2);

            _panelStyle = new GUIStyle(GUI.skin.box);
            _panelStyle.normal.background = MakeTexture(2, 2, _backgroundColor);
            _panelStyle.padding = new RectOffset(10, 10, 10, 10);

            _tabStyle = new GUIStyle(GUI.skin.button);
            _tabStyle.normal.background = MakeTexture(2, 2, _buttonColor);
            _tabStyle.hover.background = MakeTexture(2, 2, _buttonHoverColor);
            _tabStyle.normal.textColor = Color.white;
            _tabStyle.fontSize = 12;
            _tabStyle.padding = new RectOffset(10, 10, 5, 5);
            _tabStyle.margin = new RectOffset(2, 2, 2, 2);

            _activeTabStyle = new GUIStyle(_tabStyle);
            _activeTabStyle.normal.background = MakeTexture(2, 2, _toggleActiveColor);
            _activeTabStyle.hover.background = MakeTexture(2, 2, _toggleActiveColor);
            _activeTabStyle.normal.textColor = Color.white;

            _toggleButtonStyle = new GUIStyle(GUI.skin.button);
            _toggleButtonStyle.normal.background = MakeTexture(2, 2, _buttonColor);
            _toggleButtonStyle.hover.background = MakeTexture(2, 2, _buttonHoverColor);
            _toggleButtonStyle.normal.textColor = Color.white;
            _toggleButtonStyle.fontSize = 12;
            _toggleButtonStyle.padding = new RectOffset(10, 10, 5, 5);
            _toggleButtonStyle.margin = new RectOffset(2, 2, 2, 2);

            _toggleButtonActiveStyle = new GUIStyle(_toggleButtonStyle);
            _toggleButtonActiveStyle.normal.background = MakeTexture(2, 2, _toggleActiveColor);
            _toggleButtonActiveStyle.hover.background = MakeTexture(2, 2, _toggleActiveColor);

            _customToggleStyle = new GUIStyle(GUI.skin.button);
            _customToggleStyle.normal.background = MakeTexture(2, 2, _buttonColor);
            _customToggleStyle.hover.background = MakeTexture(2, 2, _buttonHoverColor);
            _customToggleStyle.active.background = MakeTexture(2, 2, _buttonHoverColor);
            _customToggleStyle.normal.textColor = Color.white;
            _customToggleStyle.fontSize = 12;
            _customToggleStyle.padding = new RectOffset(10, 10, 5, 5);
            _customToggleStyle.margin = new RectOffset(2, 2, 2, 2);

            _customToggleActiveStyle = new GUIStyle(_customToggleStyle);
            _customToggleActiveStyle.normal.background = MakeTexture(2, 2, _toggleActiveColor);
            _customToggleActiveStyle.hover.background = MakeTexture(2, 2, _toggleActiveColor);
            _customToggleActiveStyle.active.background = MakeTexture(2, 2, _toggleActiveColor);
            _customToggleActiveStyle.normal.textColor = Color.white;

            _warningStyle = new GUIStyle(GUI.skin.label);
            _warningStyle.normal.textColor = _warningColor;
            _warningStyle.fontSize = 12;
            _warningStyle.fontStyle = FontStyle.Bold;
            _warningStyle.padding = new RectOffset(5, 5, 0, 0);
        }


        public void Draw()
        {
            if (_changedScene)
            {
                _changedScene = false;
                InitializeStyles();
            }

            int menuWidth = 200;
            int contentWidth = 400;

            GUILayout.BeginHorizontal(_windowStyle);

            // Tabs
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(menuWidth));
            DrawTab("Home", "Home");
            DrawTab("LocalPlayer", "LocalPlayer");
            DrawTab("Players", "Players");
            DrawTab("Exploits >:3", "Exploits");
            DrawTab("Scenes", "Scenes");
            //DrawTab("Destroy", "Destroy");
            DrawTab("Spawner", "Spawner");
            DrawTab("Maps", "Maps");
            DrawTab("Testing", "Testing");

            GUILayout.EndVertical();

            // Main
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(contentWidth));
            GUILayout.Label(currentMenu, _headerStyle);
            GUILayout.Space(10);
            DrawMenuContent();
            GUILayout.EndVertical();

            // Debug
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(contentWidth));
            GUILayout.Label("Debug Log", _headerStyle);
            _debugScrollPosition = GUILayout.BeginScrollView(_debugScrollPosition);
            foreach (string log in _debugLogs)
            {
                GUILayout.Label(log, _logStyle);
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("Clear Logs", _buttonStyle))
            {
                _debugLogs.Clear();
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        private void DrawTab(string name, string menu)
        {
            GUIStyle style = currentMenu == menu ? _activeTabStyle : _tabStyle;
            if (GUILayout.Button(name, style))
            {
                currentMenu = menu;
            }
        }

        private void DrawToggleButton(string label, ref bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{label}:", _logStyle);
            GUIStyle style = value ? _customToggleActiveStyle : _customToggleStyle;
            if (GUILayout.Button(value ? "✓" : "", style, GUILayout.Width(20), GUILayout.Height(20)))
            {
                value = !value;
                Log($"{label}: {value}");
            }
            GUILayout.EndHorizontal();
        }

        
        private void DrawMenuContent()
        {
            switch (currentMenu)
            {
                case "Home":
                    DrawHomeContent();
                    break;
                case "Testing":
                    DrawTestingContent();
                    break;
                case "LocalPlayer":
                    DrawLocalPlayerContent();
                    break;
                case "Players":
                    DrawPlayersContent();
                    break;
                case "Exploits":
                    DrawExploitsContent();
                    break;
                case "Scenes":
                    DrawSceneLoaderContent();
                    break;
                case "Spawner":
                    DrawSpawnerContent();
                    break;
                case "Maps":
                    DrawMapsContent();
                    break;
                //case "Destroy":
                //    DrawDestroyTab();
                //    break;
            }
        }

        private void DrawHomeContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("hiiiiii :3", _headerStyle);
            if (Cache.Instance.InLobby)
            { 
                GUILayout.Label($"Lobby: {SteamLobby.Instance.LobbyNameText.text}", _headerStyle);
                GUILayout.Label($"Host: {SteamFriends.GetFriendPersonaName(Cache.Instance.LobbyOwnerID)}", _headerStyle);
            }
            GUILayout.EndVertical();
        }



        private void DrawLocalPlayerContent()
        {
            if (!Cache.Instance.InGame)
            { 
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Gotta be ingame :(", _headerStyle);
                GUILayout.EndVertical();

                return;
            }
            
            GUILayout.BeginVertical(GUI.skin.box);
            if (Cache.Instance.LocalPlayer.IsAlive && GUILayout.Button("Inf health", _buttonStyle))
            {
                
                Players.InfHealth(Cache.Instance.LocalPlayer);
                Log("Got Inf health :3");
            }
            GUILayout.EndVertical();
        }



        public Dictionary<string, bool> FreezeStates = [];
        private Vector2 _playersScrollPosition;
        private void DrawPlayersContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            _playersScrollPosition = GUILayout.BeginScrollView(_playersScrollPosition);

            if (!Cache.Instance.InLobby)
            {
                GUILayout.Label("Gotta be in a lobby", _headerStyle);
                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                return;
            }

            foreach (PlayerCache player in Cache.Instance.Players) 
            {

                GUILayout.Label($"{player.PlayerName} {(player.SteamID == (ulong) Cache.Instance.LobbyOwnerID ? "[HOST]:" : ":")}", _headerStyle);

                if (GUILayout.Button("Kick", _buttonStyle))
                {
                    Players.Kick(player);
                    Log($"Kicked {player.PlayerName}");
                }

                if (!player.IsAlive || player.PlayerHealth == null)
                {
                    GUILayout.Label($"Player not alive", _warningStyle);
                    FreezeStates[player.SteamID.ToString()] = false;
                    continue;
                }

                if (GUILayout.Button("Inf health", _buttonStyle))
                {
                    Players.InfHealth(player);
                    Log($"Gave Inf health to {player.PlayerName}");
                }

                if (GUILayout.Button("Kill", _buttonStyle))
                {
                    Players.Kill(player);
                    Log($"Killed {player.PlayerName}");
                }

                if (GUILayout.Button("Fling", _buttonStyle))
                {
                    Players.Fling(player);
                    Log($"Flung {player.PlayerName}");
                }

                string key = player.SteamID.ToString();
                FreezeStates.TryGetValue(key, out bool isFrozen);

                bool newState = GUILayout.Toggle(isFrozen, isFrozen ? "Unfreeze" : "Freeze", _toggleButtonStyle);
                if (newState != isFrozen)
                {
                    Players.SetFreeze(player, newState);
                    FreezeStates[key] = newState;
                    Log($"{(newState ? "Froze" : "Unfroze")} {player.PlayerName}");
                }

                if (GUILayout.Button("Break player model", _buttonStyle))
                { 
                    Players.BreakPlayer(player);
                }


                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }



        private void DrawExploitsContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (!Cache.Instance.InLobby)
            {
                GUILayout.Label("Gotta be in a lobby", _headerStyle);
                GUILayout.EndVertical();

                return;
            }

            // Work in lobby

            if (GUILayout.Button("Softlock Lobby", _buttonStyle))
            {
                Exploits.Softlock();
                Log("Lobby members softlocked >:3");
            }

            if (GUILayout.Button("Rickroll Lobby", _buttonStyle))
            { 
                Exploits.RickRoll();
            }

            if (!Cache.Instance.InGame)
            {
                GUILayout.Label("Gotta be ingame for more exploits", _warningStyle);
                GUILayout.EndVertical();
                return;
            }

            // Only work ingame


            if (GUILayout.Button("Delete weapon spawners", _buttonStyle))
            {
                Exploits.DeleteSpawners();
            }

            if (GUILayout.Button("Nuke lobby", _buttonStyle))
            {
                Exploits.DeleteAll();
            }

            GUILayout.EndVertical();
        }


        private void DrawTestingContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);
            if (GUILayout.Button("Reload Menu Styles", _buttonStyle))
            {
                InitializeStyles();
                Log("Menu styles reloaded");
            }
            GUILayout.EndVertical();
        }



        private Vector2 _scenesScrollPosition;
        private string _sceneSearch = "Search...";

        private void DrawSceneLoaderContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (!Cache.Instance.InLobby)
            {
                GUILayout.Label("Gotta be in a lobby", _headerStyle);
                GUILayout.EndVertical();

                return;
            }

            GUILayout.Label("Note: Loading a scene forces exploration mode", _warningStyle);
            _sceneSearch = GUILayout.TextField(_sceneSearch, _customToggleActiveStyle);
            
            _scenesScrollPosition = GUILayout.BeginScrollView(_scenesScrollPosition);

            foreach (string sceneName in _sceneNames)
            {
                if ((_sceneSearch != "Search..." && _sceneSearch.Trim() != "") && !sceneName.Contains(_sceneSearch, StringComparison.OrdinalIgnoreCase)) continue;

                if (GUILayout.Button(sceneName, _buttonStyle))
                {
                    Exploits.ForceLoadScene(sceneName);
                    Log($"Force loaded into {sceneName}");
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }



        private Vector2 _spawnerScrollPosition;
        private string _spawnerSearch = "Search...";

        private void DrawSpawnerContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (!Cache.Instance.InLobby)
            {
                GUILayout.Label("Gotta be in a lobby", _headerStyle);
                GUILayout.EndVertical();

                return;
            }

            if (!ObjectSpawner.IsMapSupported()) {
                GUILayout.Label("Requires a vending machine or mine item to work", _warningStyle);
                GUILayout.EndVertical();
                return;
            }
            _spawnerSearch = GUILayout.TextField(_spawnerSearch, _customToggleActiveStyle);

            _spawnerScrollPosition = GUILayout.BeginScrollView(_spawnerScrollPosition);

            foreach (ItemBehaviour item in Resources.FindObjectsOfTypeAll<ItemBehaviour>())
            {
                GameObject itemObj = item.gameObject;

                if (itemObj && itemObj.scene.IsValid()) continue;

                string itemName = itemObj.name;
                if ((_spawnerSearch != "Search..." && _spawnerSearch.Trim() != "") && !itemName.Contains(_spawnerSearch, StringComparison.OrdinalIgnoreCase)) continue;

                if (GUILayout.Button(itemName, _buttonStyle))
                {
                    #pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    ObjectSpawner.SpawnObject(itemObj, Cache.Instance.LocalPlayer.GameObject.transform.position);
                    #pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }


        private Vector2 _mapsScrollPosition;
        private string _mapsSearch = "Search...";

        private void DrawMapsContent()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (!Cache.Instance.InGame)
            {
                GUILayout.Label("Gotta be ingame", _headerStyle);
                GUILayout.EndVertical();

                return;
            }

            _mapsSearch = GUILayout.TextField(_mapsSearch, _customToggleActiveStyle);

            _mapsScrollPosition = GUILayout.BeginScrollView(_mapsScrollPosition);

            if (MapLoader.GetMapFiles().Length == 0)
            {
                GUILayout.Label($"No .json files found in plugin/maps folder", _warningStyle);
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
                return;
            }

            foreach (string mapPath in MapLoader.GetMapFiles())
            {
                string mapName = Path.GetFileNameWithoutExtension(mapPath);
                if ((_mapsSearch != "Search..." && _mapsSearch.Trim() != "") && !mapName.Contains(_mapsSearch, StringComparison.OrdinalIgnoreCase)) continue;

                if (GUILayout.Button(mapName, _buttonStyle))
                {
                    DeserializedMap data = MapLoader.GetMapData(mapPath);

                    MapLoader.LoadMap(mapPath);
                    Log($"Loading {mapName} ({mapPath})");
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        /*
        private Vector2 _destroyScrollPosition;
        private string _destroySearch = "Search...";

        private void DrawDestroyTab()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (!Cache.Instance.InLobby)
            {
                GUILayout.Label("Gotta be in a lobby", _headerStyle);
                GUILayout.EndVertical();

                return;
            }

            _destroySearch = GUILayout.TextField(_destroySearch, _customToggleActiveStyle);

            _destroyScrollPosition = GUILayout.BeginScrollView(_destroyScrollPosition);

            foreach (NetworkBehaviour obj in Resources.FindObjectsOfTypeAll<NetworkBehaviour>())
            {
                if (obj.gameObject == null || !obj.gameObject.scene.IsValid()) continue;

                string objName = obj.gameObject.name;
                if ((_destroySearch != "Search..." && _destroySearch.Trim() != "") && !objName.Contains(_destroySearch, StringComparison.OrdinalIgnoreCase)) continue;

                if (GUILayout.Button(objName, _buttonStyle))
                {
                    Exploits.Destroy(obj.gameObject);
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        */


        private void Awake()
        {
            Plugin.Logger.LogInfo("Menu init");
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            SceneManager.activeSceneChanged += OnSceneChanged;
        }
        private void HandleMenu(int id)
        {
            Draw();
            GUI.DragWindow();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                _menuOpen = !_menuOpen;
            }

            if (_menuOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void OnGUI()
        {
            if (_menuOpen)
                _windowRect = GUI.Window(0, _windowRect, HandleMenu, "eli's silly menu :3");
        }

        private void OnSceneChanged(Scene oldScene, Scene newScene)
        {
            _changedScene = true;


        }

        public void Log(string message)
        {
            _debugLogs.Add($"[{DateTime.Now.ToString("HH:mm:ss")}] {message}");
            if (_debugLogs.Count > MAX_LOGS)
                _debugLogs.RemoveAt(0);
        }
    }
}

