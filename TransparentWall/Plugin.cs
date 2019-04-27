using System;
using System.IO;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Utilities;
using IPALogger = IPA.Logging.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TransparentWall
{
    class Plugin : IBeatSaberPlugin
    {
        public static string PluginName = "TransparentWall";

        internal static Ref<PluginConfig> config;
        internal static IConfigProvider configProvider;

        private GameScenesManager _scenesManager;

        public const string KeyTranparentWall = "TransparentWall";
        public const string KeyHMD = "HMD";
        public const string KeyLIV = "LIVCamera";

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

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
            throw new NotImplementedException();
        }

        public void OnSceneUnloaded(Scene scene)
        {
            throw new NotImplementedException();
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            throw new NotImplementedException();
        }

        public void OnLevelWasLoaded(int level)
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
            if ("".Equals(ModPrefs.GetString(Plugin.PluginName, Plugin.KeyLIV, "")))
            {
                ModPrefs.SetBool(Plugin.PluginName, Plugin.KeyLIV, true);
            }
        }

        public void Init(IPALogger logger, [Config.Prefer("json")] IConfigProvider cfgProvider)
        {
            Logger.log = logger;
            Logger.log.Debug("Logger prepared");

            configProvider = cfgProvider;
            config = cfgProvider.MakeLink<PluginConfig>((p, v) =>
            {
                if (v.Value == null || v.Value.RegenerateConfig || v.Value == null && v.Value.RegenerateConfig)
                {
                    p.Store(v.Value = new PluginConfig() { RegenerateConfig = false });
                }
                config = v;
            });
        }
    }
}
