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

        public static bool IsAnythingOn
        {
            get
            {
                return (Plugin.IsHMDOn || Plugin.IsLIVCameraOn);
            }
        }

        public static bool IsHMDOn
        {
            get
            {
                return config.Value.HMD;
            }
            set
            {
                config.Value.HMD = value;
            }
        }

        public static bool IsLIVCameraOn
        {
            get
            {
                return config.Value.IsLIVCameraOn;
            }
            set
            {
                config.Value.IsLIVCameraOn = value;
            }
        }

        public void OnApplicationStart()
        {
            Logger.log.Debug("OnApplicationStart");

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");

            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            if (_scenesManager != null)
            {
                _scenesManager.transitionDidFinishEvent -= SceneTransitionDidFinish;
            }
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            if (_scenesManager == null)
            {
                _scenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();

                if (_scenesManager != null)
                {
                    _scenesManager.transitionDidFinishEvent += SceneTransitionDidFinish;
                }
            }
        }

        private void SceneTransitionDidFinish()
        {
            if (SceneManager.GetActiveScene().name == "GameCore")
            {
                new GameObject("TransparentWall").AddComponent<TransparentWall>();
            }
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {
        }

        public void OnSceneUnloaded(Scene scene)
        {
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
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
            Logger.log.Debug("Configuration loaded");
        }
    }
}
