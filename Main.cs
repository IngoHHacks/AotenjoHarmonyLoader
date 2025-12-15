using Aotenjo.Console;
using HarmonyLoaderAotenjo.ModLoading;
using System.Reflection;
using Object = UnityEngine.Object;

namespace HarmonyLoaderAotenjo;

internal class Main
{
    public static Main Instance { get; private set; }
    public bool HasLoadErrors { get; private set; } = false;
    
    public void Init()
    {
        Logger.Log("Initializing HarmonyLoader...");
        Instance = this;
        Harmony harmony = new("ingoh.Aotenjo.HarmonyLoaderAotenjo");
        try
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        catch (Exception e)
        {
            HasLoadErrors = true;
            Logger.LogError($"Failed to apply Harmony patches: {e}");
        }
        var loader = new GameObject("HarmonyLoaderAotenjo");
        var ml = loader.AddComponent<HarmonyModLoader>();
        ml.StartCoroutine(ml.DelayedLoadMods());
        Object.DontDestroyOnLoad(loader);
    }
    
    internal void LoadMods()
    {
        Logger.Log("Loading mods...");
        var mods = ModManager.Instance.loadedMods;
        mods = DependencyResolver.SortModsByDependencies(mods);
        var numLibsLoaded = 0;
        var numModsLoaded = 0;
        foreach (var mod in mods)
        {
            var path = mod.modDir;
            var libPath = Path.Combine(path, "libs");
            if (Directory.Exists(libPath))
            {
                var libs = Directory.GetFiles(libPath, "*.dll", SearchOption.AllDirectories);
                foreach (var lib in libs)
                {
                    try
                    {
                        Logger.Log($"Loading library: {lib}");
                        Assembly.LoadFrom(lib);
                        numLibsLoaded++;
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to load library {lib}: {e}");
                    }
                }
            }
            var modPath = Path.Combine(path, "harmony");
            if (Directory.Exists(modPath))
            {
                foreach (var file in Directory.GetFiles(modPath, "*.dll", SearchOption.AllDirectories))
                {
                    try
                    {
                        Logger.Log($"Loading mod assembly: {file}");
                        var assembly = Assembly.LoadFrom(file);
                        var modClass = assembly.GetTypes()
                            .Where(t => t.IsSubclassOf(typeof(HarmonyMod)) && !t.IsAbstract).ToList();
                        if (modClass.Count == 0)
                        {
                            Logger.LogWarning($"No HarmonyMod subclass found in assembly {file}.");
                            continue;
                        }

                        if (modClass.Count > 1)
                        {
                            Logger.LogWarning(
                                $"Multiple HarmonyMod subclasses found in assembly {file}. They will all be initialized, but only the first one will be used for mod info. Consider splitting them into separate assemblies.");
                        }

                        bool assemblyInitialized = false;
                        foreach (var type in modClass)
                        {
                            try
                            {
                                var modInstance = (HarmonyMod)Activator.CreateInstance(type);
                                if (modInstance != null)
                                {
                                    Logger.Log(
                                        $"Initializing mod: {modInstance.ModName} by {modInstance.ModAuthor} (version {modInstance.ModVersion})");
                                    modInstance.Init();
                                    if (!assemblyInitialized)
                                    {
                                        InitializeAssembly(assembly, modInstance);
                                        assemblyInitialized = true;
                                    }

                                    numModsLoaded++;
                                }
                                else
                                {
                                    Logger.LogWarning(
                                        $"Failed to create instance of mod class {type.FullName} in assembly {file}.");
                                }
                            }
                            catch (Exception e)
                            {
                                Logger.LogError(
                                    $"Failed to initialize mod class {type.FullName} in assembly {file}: {e}");
                            }
                        }

                        Logger.Log($"Patched assembly: {file}");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Failed to load mod assembly {file}: {e}");
                    }
                }
            }
        }

        if (numModsLoaded + numLibsLoaded > 0)
        {
            Logger.Log(
                $"Finished loading mods. Loaded {numLibsLoaded} libraries and initialized {numModsLoaded} mods.");
        }
        else
        {
            Logger.Log("No mods or libraries found to load.");
        }
    }
    
    private void InitializeAssembly(Assembly assembly, HarmonyMod modInstance)
    {
        // Apply Harmony patches
        var harmony = new Harmony(modInstance.ModGuid);
        harmony.PatchAll(assembly);
        // Apply AotenjoCommand attribute commands
        foreach (var type in assembly.GetTypes())
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                                          BindingFlags.Instance);
            foreach (var method in methods)
            {
                var attributes = method.GetCustomAttributes(typeof(AotenjoCommandAttribute), false);
                foreach (AotenjoCommandAttribute attribute in attributes)
                {
                    try
                    {
                        AotenjoCommandSystem.Initialize();
                        var (func, commandMetadata) = AotenjoCommandSystem.CreateCommandFunction(method, attribute);
                        if (func != null && commandMetadata != null)
                        {
                            AotenjoCommandSystem.generatedCommands[attribute.CommandName] = func;
                            AotenjoCommandSystem.commandMetadata[attribute.CommandName] = commandMetadata;
                        }
                        Logger.Log(
                            $"Registered Aotenjo command '{attribute.CommandName}' from mod '{modInstance.ModName}'.");
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(
                            $"Failed to register Aotenjo command '{attribute.CommandName}' from mod '{modInstance.ModName}': {e}");
                    }
                }
            }
        }
    }
}
