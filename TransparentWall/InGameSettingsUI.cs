using CustomUI.Settings;
using CustomUI.Utilities;
using CustomUI.MenuButton;
using CustomUI.GameplaySettings;
using UnityEngine;

namespace TransparentWall
{
    public class InGameSettingsUI : MonoBehaviour
    {
        private static ToggleOption hmdToggle;
        private static BoolViewController hmdController;
        private static BoolViewController livCameraController;

        private static readonly Sprite optionIcon = UIUtilities.LoadSpriteFromResources(Plugin.PluginName + ".Properties.icetransparent.png");
        private static readonly string disclaimer = "Enabling '" + Plugin.PluginName + "' in the headset will deactivate ScoreSubmission until this option is turned off!";

        /// <summary>
        /// Adds an additional submenu in the "Settings" page
        /// </summary>
        public static void CreateSettingsMenu()
        {
            SubMenu subMenu = SettingsUI.CreateSubMenu(Plugin.PluginName);

            hmdController = subMenu.AddBool("Enable in headset", disclaimer);
            hmdController.GetValue += delegate { return Plugin.IsHMDOn; };
            hmdController.SetValue += delegate (bool value)
            {
                ChangeTransparentWallState(value);
                Logger.log.Debug($"'Enable in headset' (IsHMDOn) in the main settings is set to '{value}'");
            };

            livCameraController = subMenu.AddBool("Disable in LIVCamera");
            livCameraController.GetValue += delegate { return Plugin.IsDisableInLIVCamera; };
            livCameraController.SetValue += delegate (bool value)
            {
                Plugin.IsDisableInLIVCamera = value;
                Logger.log.Debug($"'Disable in LIVCamera' (IsDisableLIVCameraWall) in the main settings is set to '{value}'");
            };
        }

        /// <summary>
        /// Adds a toggle option to the in-game "Gameplay Setup" window. It can be found in the left panel of the Player Settings
        /// </summary>
        public static void CreateGameplaySetupMenu()
        {
            hmdToggle = GameplaySettingsUI.CreateToggleOption(GameplaySettingsPanels.PlayerSettingsLeft, Plugin.PluginName, disclaimer, optionIcon, 0);

            hmdToggle.GetValue = Plugin.IsHMDOn;
            hmdToggle.OnToggle += ((bool value) =>
            {
                Plugin.IsHMDOn = value;
                Logger.log.Debug($"Toggle is very '{(value ? "toggled" : "untoggled")}! Value is now '{value}'");
            });
        }

        /// <summary>
        /// Adds a button to the "Mods" settings menu (INCOMPLETE)
        /// </summary>
        public static void CreateModMenuButton()
        {
            //Currently there's no indication of which state is set when it's clicked. You can always check the toggle option in "Gameplay Setup" to be sure afterwards :P
            MenuButtonUI.AddButton($"{Plugin.PluginName} toggle", delegate
            {
                ChangeTransparentWallState(!Plugin.IsHMDOn);
                Logger.log.Debug($"Button was brutally smashed! Value should now be '{Plugin.IsHMDOn}'");
            });
        }

        private static void ChangeTransparentWallState(bool state)
        {
            hmdToggle.SetToggleState(state);
        }
    }
}
