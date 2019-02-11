using IllusionPlugin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TransparentWall
{
    class Plugin : IEnhancedPlugin, IPlugin
    {
        public static string PluginName = "TransparentWall";
        public const string VersionNum = "0.2.1";

        public string Name => PluginName;
        public string Version => VersionNum;
        public string[] Filter { get; }

        private GameScenesManager _scenesManager;

        public const string KeyTranparentWall = "TransparentWall";
        public const string KeyHMD = "HMD";
        public const string KeyCameraPlus = "CameraPlus";
        public const string KeyLIV = "LIVCamera";
        public const string KeyExcludedCams = "ExcludedCamPlusCams";

        public static bool IsTranparentWall
        {
            get
            {
                return ModPrefs.GetBool(Plugin.PluginName, KeyTranparentWall, false);
            }
            set
            {
                ModPrefs.SetBool(Plugin.PluginName, KeyTranparentWall, value);
            }
        }

        public static bool IsHMDOn
        {
            get
            {
                return ModPrefs.GetBool(Plugin.PluginName, KeyHMD, true);
            }
            set
            {
                ModPrefs.SetBool(Plugin.PluginName, KeyHMD, value);
            }
        }

        public static bool IsCameraPlusOn
        {
            get
            {
                return ModPrefs.GetBool(Plugin.PluginName, KeyCameraPlus, true);
            }
            set
            {
                ModPrefs.SetBool(Plugin.PluginName, KeyCameraPlus, value);
            }
        }

        public static bool IsLIVCameraOn
        {
            get
            {
                return ModPrefs.GetBool(Plugin.PluginName, KeyLIV, true);
            }
            set
            {
                ModPrefs.SetBool(Plugin.PluginName, KeyLIV, value);
            }
        }

        public static List<string> ExcludedCams
        {
            get
            {
                return ModPrefs.GetString(Plugin.PluginName, KeyExcludedCams, "").Split(',').ToList().Select(c => c.ToLower().Trim()).ToList();
            }
            set
            {
                ModPrefs.SetString(Plugin.PluginName, KeyExcludedCams, string.Join(",", value));
            }
        }


        public void OnApplicationStart()
        {
            CheckForUserDataFolder();
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            if (_scenesManager != null)
                _scenesManager.transitionDidFinishEvent -= SceneTransitionDidFinish;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (_scenesManager == null)
            {
                _scenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();

                if (_scenesManager != null)
                    _scenesManager.transitionDidFinishEvent += SceneTransitionDidFinish;
            }
        }

        private void SceneTransitionDidFinish()
        {
            if (SceneManager.GetActiveScene().name == "GameCore")
                new GameObject("TransparentWall").AddComponent<TransparentWall>();
        }

        public void OnLateUpdate()
        {
        }

        public void OnLevelWasLoaded(int level)
        {
        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        private void CheckForUserDataFolder()
        {
            string userDataPath = Environment.CurrentDirectory + "/UserData";
            if (!Directory.Exists(userDataPath))
            {
                Directory.CreateDirectory(userDataPath);
            }
            if ("".Equals(ModPrefs.GetString(Plugin.PluginName, Plugin.KeyTranparentWall, "")))
            {
                ModPrefs.SetBool(Plugin.PluginName, Plugin.KeyTranparentWall, true);
            }
            if ("".Equals(ModPrefs.GetString(Plugin.PluginName, Plugin.KeyHMD, "")))
            {
                ModPrefs.SetBool(Plugin.PluginName, Plugin.KeyHMD, true);
            }
            if ("".Equals(ModPrefs.GetString(Plugin.PluginName, Plugin.KeyCameraPlus, "")))
            {
                ModPrefs.SetBool(Plugin.PluginName, Plugin.KeyCameraPlus, true);
            }
            if ("".Equals(ModPrefs.GetString(Plugin.PluginName, Plugin.KeyLIV, "")))
            {
                ModPrefs.SetBool(Plugin.PluginName, Plugin.KeyLIV, true);
            }
            if ("".Equals(ModPrefs.GetString(Plugin.PluginName, Plugin.KeyExcludedCams, "")))
            {
                ModPrefs.SetString(Plugin.PluginName, Plugin.KeyExcludedCams, "");
            }
        }
    }
}
