using System.Collections;

namespace HarmonyLoaderAotenjo.ModLoading;

public class HarmonyModLoader : MonoBehaviour
{
    public IEnumerator DelayedLoadMods()
    {
        yield return new WaitUntil(() => ModManager.Instance != null && ModManager.Instance.isInitialized);
        Main.Instance.LoadMods();
    }
}