using System;
using System.Collections.Generic;
using System.Linq;
using BS_Utils.Gameplay;
using UnityEngine;

namespace TransparentWall
{
    public class TransparentWall : MonoBehaviour
    {
        public static int WallLayer = 25;
        public static int MoveBackLayer = 27;
        public static string LIVCam_Name = "MainCamera";
        private static List<string> _excludedCams = Plugin.ExcludedCams;
        public static List<int> LayersToMask = new List<int> { WallLayer, MoveBackLayer };
        public static List<string> livNames = new List<string> { "MenuMainCamera", "MainCamera", "LIV Camera" };

        private BeatmapObjectSpawnController _beatmapObjectSpawnController;

        private void Start()
        {
            if (!Plugin.IsTranparentWall)
                return;
            try
            {
                if (Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().Count() > 0)
                {
                    this._beatmapObjectSpawnController = Resources.FindObjectsOfTypeAll<BeatmapObjectSpawnController>().First();
                }

                if (Resources.FindObjectsOfTypeAll<MoveBackWall>().Count() > 0)
                {
                    MoveBackLayer = Resources.FindObjectsOfTypeAll<MoveBackWall>().First().gameObject.layer;
                }

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
            _excludedCams = Plugin.ExcludedCams;
            StartCoroutine(setupCamerasCoroutine());
        }

        private IEnumerator<WaitForEndOfFrame> setupCamerasCoroutine()
        {
            yield return new WaitForEndOfFrame();

            LevelData levelSetup = new LevelData();

            Camera mainCamera = Camera.main;

            if (Plugin.IsHMDOn)
            {
                ScoreSubmission.DisableSubmission("TransparentWall");
                mainCamera.cullingMask &= ~(1 << WallLayer);
            }
            else
            {
                mainCamera.cullingMask |= (1 << WallLayer);
            }

            try
            {
                LIV.SDK.Unity.LIV.FindObjectsOfType<LIV.SDK.Unity.LIV>().Where(x => livNames.Contains(x.name)).ToList().ForEach(l =>
                {
                    if (Plugin.IsLIVCameraOn)
                    {
                        LayersToMask.ForEach(i => { l.SpectatorLayerMask &= ~(1 << i); });
                    }
                });
                GameObject.FindObjectsOfType<Camera>().Where(c => (c.name.ToLower().EndsWith(".cfg"))).ToList().ForEach(c =>
                {
                    if (_excludedCams.Contains(c.name.ToLower()))
                    {
                        LayersToMask.ForEach(i => { c.cullingMask |= (1 << i); });
                    }
                    else
                    {
                        if (Plugin.IsCameraPlusOn)
                        {
                            LayersToMask.ForEach(i => { c.cullingMask &= ~(1 << i); });
                        }
                        else
                        {
                            LayersToMask.ForEach(i => { c.cullingMask |= (1 << i); });
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TransparentWall] {ex.Message}\n{ex.StackTrace}");
            }
        }

        public virtual void HandleObstacleDiStartMovementEvent(BeatmapObjectSpawnController obstacleSpawnController, ObstacleController obstacleController)
        {
            try
            {
                StretchableObstacle _stretchableObstacle = ReflectionUtil.GetPrivateField<StretchableObstacle>(obstacleController, "_stretchableObstacle");
                StretchableCube _stretchableCore = ReflectionUtil.GetPrivateField<StretchableCube>(_stretchableObstacle, "_stretchableCore");
                //MeshRenderer _meshRenderer = ReflectionUtil.GetPrivateField<MeshRenderer>(_stretchableCore, "_mesh");
                _stretchableCore.gameObject.layer = WallLayer;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
