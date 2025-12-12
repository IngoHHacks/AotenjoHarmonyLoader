function init()
    local local_mods_path = CS.System.IO.Path.Combine(CS.UnityEngine.Application.streamingAssetsPath, "mods")
    if CS.System.IO.Directory.Exists(local_mods_path) then
        local dirs = CS.System.IO.Directory.GetDirectories(local_mods_path)
        for i = 0, dirs.Length - 1 do
            local dir = dirs[i]
            if CS.System.IO.Directory.Exists(CS.System.IO.Path.Combine(dir, ".harmonyloader")) then
                load_harmony(CS.System.IO.Path.Combine(dir, ".harmonyloader"))
                return
            end
        end
    end
    if CS.SteamIntegration.Instance ~= null and CS.SteamIntegration.Instance.Connected then
        local mods_path = CS.SteamWorkshopIntegration.ModDirectories
        if mods_path ~= nil and mods_path.Count > 0 then
            for i = 0, mods_path.Count - 1 do
                local mod_path = mods_path[i].Item1
                if CS.System.IO.Directory.Exists(CS.System.IO.Path.Combine(mod_path, ".harmonyloader")) then
                    load_harmony(CS.System.IO.Path.Combine(mod_path, ".harmonyloader"))
                    return
                end
            end
        end
    end
    CS.Aotenjo.Logger.Log("Could not find Harmony Loader in any mod directories.")
end

function load_harmony(mod_path)
    CS.Aotenjo.Logger.Log("Loading Harmony from: " .. mod_path)
    local libs_path = CS.System.IO.Path.Combine(mod_path, ".libs")
    for i = 0, CS.System.IO.Directory.GetFiles(libs_path).Length - 1 do
        local file = CS.System.IO.Directory.GetFiles(libs_path)[i]
        CS.System.Reflection.Assembly.LoadFrom(file)
        CS.Aotenjo.Logger.Log("Loaded assembly from " .. file)
    end
    local harmony_path = CS.System.IO.Path.Combine(mod_path, "HarmonyLoaderAotenjo.dll")
    local assembly = CS.System.Reflection.Assembly.LoadFrom(harmony_path)
    local harmony_type = assembly:GetType("HarmonyLoaderAotenjo.Main")
    local harmony_main = CS.System.Activator.CreateInstance(harmony_type)
    harmony_main:Init()
end