using Aotenjo.Console;
using System.Reflection;

namespace HarmonyLoaderAotenjo.Patches;

[HarmonyPatch]
internal class DebugCommandPatches
{
    [HarmonyPatch(typeof(AotenjoCommandSystem), nameof(AotenjoCommandSystem.ExecuteCommand))]
    [HarmonyPrefix]
    public static bool AotenjoCommandSystem_ExecuteCommand_Prefix(ref string __result, MethodInfo method, MethodInfo[] parsers, ParameterInfo[] parameters, string[] args)
    {
        if (method.DeclaringType == typeof(Player))
        {
            return true; // Build-in Player commands are handled normally
        }

        var player = GameManager.Instance?.player;
        if (player == null)
        {
            __result = "Error: Player instance not available";
            return false;
        }

        var numArgs = args.Length - 1;
        var minArgs = parameters.Count(p => !p.HasDefaultValue);
        var maxArgs = parameters.Length;
        if (numArgs < minArgs || numArgs > maxArgs)
        {
            __result = $"Error: Expected {minArgs}-{maxArgs} arguments, got {numArgs}\n" + "Usage: " + args[0] + " " + AotenjoCommandSystem.GetUsageString(parameters);
            return false;
        }
        object[] array = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            if (i < numArgs)
            {
                try
                {
                    array[i] = parsers[i].Invoke(null, [
                        player,
                        args[i + 1]
                    ]);
                }
                catch (Exception ex)
                {
                    __result = $"Error: Failed to parse argument {i + 1} ('{args[i + 1]}'): {ex.Message}";
                    return false;
                }
            }
            else
            {
                array[i] = Type.Missing;
            }
        }
        object obj = method.Invoke(method.IsStatic ? null : GetInstance(method.DeclaringType), array);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Sync();
        }
        string text = string.Join(", ", from arg in args.Skip(1)
            select "'" + arg + "'");
        string text2 = "-- Executed " + method.Name + "(" + text + ")";
        if (method.ReturnType != typeof(void) && obj != null)
        {
            text2 += $"\nResult: {obj}";
        }
        __result = text2;
        return false;
    }
    
    private static object GetInstance(Type type)
    {
        var instanceProperty = type.GetProperty("Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) 
                               ?? type.GetProperty("instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                               ?? type.GetProperty("_instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (instanceProperty != null)
        {
            return instanceProperty.GetValue(null);
        }
        var instanceField = type.GetField("Instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static) 
                            ?? type.GetField("instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                            ?? type.GetField("_instance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (instanceField != null)
        {
            return instanceField.GetValue(null);
        }
        throw new Exception($"No static instance property/field found for type {type.FullName}");
    }
}