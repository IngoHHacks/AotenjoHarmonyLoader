namespace HarmonyLoaderAotenjo.Patches;

[HarmonyPatch]
internal class ModLoadingPatches
{
    private static bool _modsLoaded = false;
    
    [HarmonyPatch(typeof(UIArtifactCollectionView), nameof(UIArtifactCollectionView.RefreshContent))]
    [HarmonyPostfix]
    public static void UIArtifactCollectionView_RefreshContent_Postfix()
    {
        if (_modsLoaded) return;
        Main.Instance.LoadMods();
        _modsLoaded = true;
    }
}