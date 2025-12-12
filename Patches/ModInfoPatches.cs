namespace HarmonyLoaderAotenjo.Patches;

[HarmonyPatch]
internal class ModInfoPatches
{
    [HarmonyPatch(typeof(UIMainMenuModManager), nameof(UIMainMenuModManager.ToggleDisplayModInfo))]
    [HarmonyPostfix]
    public static void UIMainMenuModManager_ToggleDisplayModInfo_Postfix(UIMainMenuModManager __instance, Mod mod)
    {
        if (mod.name == "Harmony Loader")
        {
            if (Main.Instance.HasLoadErrors)
            {
                __instance.descriptionText.text = "Allows mods to use Harmony patches to modify game behavior. Does not add any content by itself.\nMod loaded with errors. Check the log for details.";
                return;
            }
            __instance.descriptionText.text = "Allows mods to use Harmony patches to modify game behavior. Does not add any content by itself.\nMod loaded successfully!";
        }
    }
}