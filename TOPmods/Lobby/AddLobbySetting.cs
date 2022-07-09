using System;
using HarmonyLib;
using UnityEngine;
using UnhollowerBaseLib;

namespace TownOfPlus
{
    class LobbySetting
    {
        [HarmonyPatch(typeof(GameSettingMenu), nameof(GameSettingMenu.InitializeOptions))]
        public static class GameSettingMenuPatch
        {
            public static void Prefix(GameSettingMenu __instance)
            {
                if (!main.RoomOption.Value || !main.ShowMapSelect.Value) return;
                // �I�����C�����[�h�ŕ����𗧂Ē����Ȃ��Ă��}�b�v��ύX�ł���悤�ɕύX
                __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Start))]
        public static class GameOptionsMenuPatch
        {
            public static void Postfix(GameOptionsMenu __instance)
            {
                if (!main.RoomOption.Value) return;
                foreach (var ob in __instance.Children)
                {
                    switch (ob.Title)
                    {
                        case StringNames.GameShortTasks:
                        case StringNames.GameLongTasks:
                        case StringNames.GameCommonTasks:
                        case StringNames.GameKillCooldown:
                            ob.Cast<NumberOption>().ValidRange = new FloatRange(0, 180);
                            break;

                        case StringNames.GameRecommendedSettings:
                            ob.enabled = false;
                            ob.gameObject.SetActive(false);
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Start))]
        public static class RolesSettingsMenuPatch
        {
            public static void Postfix(RolesSettingsMenu __instance)
            {
                if (!main.RoomOption.Value) return;
                foreach (var ob in __instance.Children)
                {
                    switch (ob.Title)
                    {
                        case StringNames.ScientistCooldown:
                        case StringNames.ScientistBatteryCharge:
                        case StringNames.EngineerCooldown:
                        case StringNames.EngineerInVentCooldown:
                        case StringNames.GuardianAngelCooldown:
                        case StringNames.GuardianAngelDuration:
                        case StringNames.ShapeshifterCooldown:
                        case StringNames.ShapeshifterDuration:
                            ob.Cast<NumberOption>().ValidRange = new FloatRange(0, 180);
                            if (main.AdvancedNum.Value) SetIncrement(ob, 1f, 5f);
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
        public static class AdvancedNum
        {
            public static void Postfix(GameOptionsMenu __instance)
            {
                if (!main.RoomOption.Value || !main.AdvancedNum.Value) return;
                foreach (var ob in __instance.Children)
                {
                    switch (ob.Title)
                    {
                        case StringNames.GameKillCooldown:
                            SetIncrement(ob, 0.1f, 2.5f);
                            break;

                        case StringNames.GameEmergencyCooldown:
                            SetIncrement(ob, 1, 5);
                            break;

                        case StringNames.GameDiscussTime:
                        case StringNames.GameVotingTime:
                            SetIncrement(ob, 1, 15);
                            break;

                        case StringNames.GamePlayerSpeed:
                        case StringNames.GameCrewLight:
                        case StringNames.GameImpostorLight:
                            SetIncrement(ob, 0.01f, 0.25f);
                            break;
                    }
                }
            }
        }

        [HarmonyPatch(typeof(RolesSettingsMenu), nameof(RolesSettingsMenu.Update))]
        public static class AdvancedRoleNum
        {
            public static void Postfix(RolesSettingsMenu __instance)
            {
                if (!main.RoomOption.Value || !main.AdvancedNum.Value) return;
                foreach (var ob in __instance.Children)
                {
                    switch (ob.Title)
                    {
                        case StringNames.ScientistCooldown:
                        case StringNames.ScientistBatteryCharge:
                        case StringNames.EngineerCooldown:
                        case StringNames.EngineerInVentCooldown:
                        case StringNames.GuardianAngelCooldown:
                        case StringNames.GuardianAngelDuration:
                        case StringNames.ShapeshifterCooldown:
                        case StringNames.ShapeshifterDuration:
                            SetIncrement(ob, 1f, 5f);
                            break;
                    }
                }
            }
        }
        static void SetIncrement(OptionBehaviour ob, float Setf, float Resetf)
        {
            var obn = ob.Cast<NumberOption>();
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Flag.Run(() =>
                {
                    obn.Increment = Setf;
                    Flag.NewFlag($"Reset{ob.Title}");
                }, $"Set{ob.Title}", true);

                //�덷���C��
                obn.Value = (float)Math.Round(obn.Value, 1);
            }
            else
            {
                Flag.Run(() =>
                {
                    obn.Increment = Resetf;
                    Flag.NewFlag($"Set{ob.Title}");
                }, $"Reset{ob.Title}");
            }
        }
    }
}
