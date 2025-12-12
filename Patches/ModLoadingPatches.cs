namespace HarmonyLoaderAotenjo.Patches;

[HarmonyPatch]
internal class ModLoadingPatches
{
    [HarmonyPatch(typeof(ModLocalizationLoader), nameof(ModLocalizationLoader.LoadModLocalizations), [])]
    [HarmonyPostfix]
    public static void ModLocalizationLoader_LoadModLocalizations_Postfix()
    {
        Main.Instance.LoadMods();
    }
}