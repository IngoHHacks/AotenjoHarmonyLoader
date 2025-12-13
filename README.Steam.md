[b]Harmony Loader for Aotenjo[/b]

[b]Introduction[/b]

Aotenjo's official mod loader uses Lua. While creating mods in Lua is straightforward, it can be limiting for developers who want to modify the game's core functionality or implement complex features. The interoperability between Lua and C# can also be cumbersome due to data type conversions, indexing differences, null handling, and additional overhead.  
Harmony Loader for Aotenjo is a mod loader that allows you to create mods using C# and the Harmony library. Harmony is a popular library for patching .NET applications at runtime, enabling you to modify existing methods, add new functionality, and change game behavior without overwriting the original code.  
Like the official mod loader, Harmony Loader supports loading multiple mods simultaneously. However, it is less stable and may break with game updates, so use it at your own risk. It is recommended to use the official modding API whenever possible for better compatibility and stability.

[b]How to Use[/b]

First, set up a mod in Aotenjo as you normally would using Lua as explained in the [url=https://github.com/XOCatGames/Aotenjo-Src/wiki/1.-Getting-Started-%E5%BF%AB%E9%80%9F%E4%B8%8A%E6%89%8B]official documentation[/url]. Once you have your mod structure ready, follow these steps to integrate Harmony Loader:

- Make sure you have the Harmony Loader mod installed in Aotenjo (through Steam Workshop or manually).
- Create a new C# Class Library project in your preferred IDE (e.g., Visual Studio, JetBrains Rider)
    - Target .NET Framework 4.7.2 is recommended to ensure compatibility.
- Add references to the Harmony library, this library, Aotenjo's assemblies, and any other dependencies your mod requires.
    - Any assemblies that your mod depends on besides the three mentioned above must be placed in the [i]libs[/i] folder of your mod directory.
    - Stripping and publicizing the Aotenjo assemblies is recommended. See References section for tools that can help with this.
    - AllowUnsafeBlocks option may need to be enabled in your project settings when accessing private members from stripped assemblies.
- Create a class that extends [i]HarmonyLoaderAotenjo.HarmonyMod[/i].
    - Override the [i]ModName[/i], [i]ModAuthor[/i], and [i]ModVersion[/i] properties to provide metadata about your mod.
    - The [i]Init()[/i] method is called when your mod is loaded.
    - Patches are applied automatically when the mod is loaded, so you don't need to call [i]harmony.PatchAll()[/i] manually.
- Implement your mod logic using Harmony patches and other C# features.
- Build your project to produce a DLL file.
- Place the compiled DLL into the [i]harmony[/i] folder within your mod directory.
- Launch Aotenjo, and your Harmony patches should be applied automatically when your mod is loaded.

[b]It is recommended to use the official modding API (namespace [i]Aotenjo[/i] in the game assemblies) whenever possible for better compatibility and stability as the API is designed to be stable across updates. Harmony patches should be used when the official API does not provide the necessary functionality.[/b]

[b]File Structure[/b]  
Your mod directory should look like this:

[code]
MyHarmonyMod/
├── scripts/                  # Folder for Lua scripts (if any)
│   └── my_script.lua         # Example Lua script
├── harmony/                  # Folder for Harmony DLLs
│   └── MyHarmonyMod.dll      # Your compiled Harmony mod DLL
├── libs/                     # Folder for additional dependencies (if any)
│   └── SomeDependency.dll    # Example dependency DLL
├── modinfo.json             # Mod metadata file
└── other_mod_files...        # Any other files/folders your mod requires (e.g. textures)
[/code]

[b]References[/b]

[u]Aotenjo Modding Documentation[/u]  
[url=https://github.com/XOCatGames/Aotenjo-Src/wiki/1.-Getting-Started-%E5%BF%AB%E9%80%9F%E4%B8%8A%E6%89%8B]GitHub Wiki[/url]

[u]Harmony Docs[/u]  
[url=https://harmony.pardeike.net/articles/intro.html]Harmony Intro[/url]

[u]Game Source Code[/u]  
[url=https://github.com/XOCatGames/Aotenjo-Src]GitHub[/url] 
The source code of Aotenjo is publicly available on GitHub. You can refer to it for understanding the game's architecture and for finding classes and methods to patch.  
Alternatively, you can use ILSpy, dnSpy, or other .NET decompilers to explore the game's assemblies.

[u]Publicizing and Stripping Tools[/u]  
[url=https://github.com/jacobEAdamson/publicize/releases/tag/v1.0.0]Publicize[/url] 
[url=https://github.com/BepInEx/BepInEx.AssemblyPublicizer]AssemblyPublicizer[/url] 
[url=https://github.com/bbepis/NStrip]NStrip[/url]

[b]Mahjong Trivia[/b]  
Did you know? Aotenjo (青天井) means "Blue Ceiling" in Japanese.  
This is a ruleset in Japanese Mahjong where scores do not have an upper limit, allowing for extremely high-scoring hands.  
The 'blue' symbolizes the sky and depicts the limitless nature of the sky.  
I'm sure you can guess why the game was named this way!

[b]License[/b]  
[url=https://creativecommons.org/publicdomain/zero/1.0/]https://creativecommons.org/publicdomain/zero/1.0/[/url] 
This work is licensed under the Creative Commons Zero v1.0 Universal License.  
You are free to copy, modify, distribute and perform the work, even for commercial purposes, all without asking permission.  
No warranty or liability is provided. If something goes wrong, that's a skill issue.  
For more information, please refer to the license text at the link above.