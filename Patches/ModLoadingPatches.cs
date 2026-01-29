namespace HarmonyLoaderAotenjo.Patches;

[HarmonyPatch]
internal class ModLoadingPatches
{
    private static bool _modsLoaded = false;
    
    [HarmonyPatch(typeof(Logger), nameof(Logger.Log))]
    [HarmonyPostfix]
    public static void Logger_Log(string message)
    {
        if (_modsLoaded) return;
        if (!message.ToLower().Contains("finished loading ") || !message.ToLower().Contains(" mods")) return;
        _modsLoaded = true; // Needs to be set before calling LoadMods to avoid recursion since Log (with "finished loading mods") is called again inside LoadMods
        Main.Instance.LoadMods();
    }
}