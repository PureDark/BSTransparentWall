using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TransparentWall
{
    public class TransparentWall : MonoBehaviour
    {
        public static int WallLayer = 25;

        private BeatmapObjectSpawnController _beatmapObjectSpawnController;

        private void Start()
        {
            if (!Plugin.IsTranparentWall)
                return;
            try
            {
                this._beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();

                if (_beatmapObjectSpawnController != null)
                {
                    _beatmapObjectSpawnController.obstacleDiStartMovementEvent += this.HandleObstacleDiStartMovementEvent;
                }

                setupCams();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void OnDestroy()
        {
            if (_beatmapObjectSpawnController != null)
            {
                _beatmapObjectSpawnController.obstacleDiStartMovementEvent -= this.HandleObstacleDiStartMovementEvent;
            }
        }
        private void setupCams()
        {
            if (Plugin.IsLIVCameraOn)
            {
                Camera[] cameras = FindObjectsOfType<Camera>();
                foreach (Camera camera in cameras)
                {
                    camera.cullingMask &= ~(1 << WallLayer);
                }
            }
            StartCoroutine(setupCamerasCoroutine());
        }

        private IEnumerator<WaitForEndOfFrame> setupCamerasCoroutine()
        {
            yield return new WaitForEndOfFrame();

            var mainGameSceneSetupData = Resources.FindObjectsOfTypeAll<MainGameSceneSetupData>().FirstOrDefault();

            Camera mainCamera = FindObjectsOfType<Camera>().FirstOrDefault(x => x.CompareTag("MainCamera"));
            if (Plugin.IsHMDOn && !mainGameSceneSetupData.gameplayOptions.validForScoreUse)
                mainCamera.cullingMask &= ~(1 << WallLayer);
            else
                mainCamera.cullingMask |= (1 << WallLayer);

            foreach (var plugin in IllusionInjector.PluginManager.Plugins)
            {
                if (plugin.Name == "CameraPlus" || plugin.Name == "CameraPlusOrbitEdition" || plugin.Name == "DynamicCamera")
                {
                    MonoBehaviour _cameraPlus = ReflectionUtil.GetPrivateField<MonoBehaviour>(plugin, "_cameraPlus");
                    while (_cameraPlus == null)
                    {
                        yield return new WaitForEndOfFrame();
                        _cameraPlus = ReflectionUtil.GetPrivateField<MonoBehaviour>(plugin, "_cameraPlus");
                    }
                    Camera cam = ReflectionUtil.GetPrivateField<Camera>(_cameraPlus, "_cam");
                    if (cam != null)
                    {
                        if (((plugin.Name == "CameraPlus" || plugin.Name == "CameraPlusOrbitEdition") && Plugin.IsCameraPlusOn) || (plugin.Name == "DynamicCamera" && Plugin.IsDynamicCameraOn))
                            cam.cullingMask &= ~(1 << WallLayer);
                        else
                            cam.cullingMask |= (1 << WallLayer);
                    }
                    Camera multi = ReflectionUtil.GetPrivateField<Camera>(_cameraPlus, "multi");
                    if (multi != null)
                    {
                        if (Plugin.IsMutiViewFirstPersonOn)
                            multi.cullingMask &= ~(1 << WallLayer);
                        else
                            multi.cullingMask |= (1 << WallLayer);
                    }
                    break;
                }
            }
        }

        public virtual void HandleObstacleDiStartMovementEvent(BeatmapObjectSpawnController obstacleSpawnController, ObstacleController obstacleController)
        {
            try
            {
                StretchableObstacle _stretchableObstacle = ReflectionUtil.GetPrivateField<StretchableObstacle>(obstacleController, "_stretchableObstacle");
                StretchableCube _stretchableCoreOutside = ReflectionUtil.GetPrivateField<StretchableCube>(_stretchableObstacle, "_stretchableCoreOutside");
                StretchableCube _stretchableCoreInside = ReflectionUtil.GetPrivateField<StretchableCube>(_stretchableObstacle, "_stretchableCoreInside");
                //MeshRenderer _meshRenderer = ReflectionUtil.GetPrivateField<MeshRenderer>(_stretchableCoreOutside, "_meshRenderer");
                //MeshRenderer _meshRenderer2 = ReflectionUtil.GetPrivateField<MeshRenderer>(_stretchableCoreInside, "_meshRenderer");
                _stretchableCoreOutside.gameObject.layer = WallLayer;
                _stretchableCoreInside.gameObject.layer = WallLayer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
