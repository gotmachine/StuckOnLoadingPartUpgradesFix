# StuckOnLoadingPartUpgradesFix

This is a fix for the KSP 1.12.2 bug that cause the game to be stuck on `Loading Part Upgrades` on a modded install.

This bug happens when you have a duplicated `*.dll` file in you `GameData` folder/subfolders. In KSP 1.12, the code that is supposed to be handling that (perfectly normal, mod authors aren't doing anything wrong) situation is either :
- Crashing with an ArgumentOutOfRangeException, causing the whole loading process to stop.
- Silently removing random assemblies from the list of assemblies to be loaded, causing random issues.

If that is happening to you, you will see the following error near the beginning of your `KSP.log` file :
```
ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
	System.ThrowHelper.ThrowArgumentOutOfRangeException (System.ExceptionArgument argument, System.ExceptionResource resource)
	System.ThrowHelper.ThrowArgumentOutOfRangeException ()
	System.Collections.Generic.List`1[T].RemoveAt (System.Int32 index)
	AssemblyLoader.FlagDuplicatedPlugins ()
```

This also fix two other issues that have the same effect (but are less likely to happen, as most mod authors have already implemented workarounds)
- AssemblyLoader crashing on attempting to load a `*.dll` file with an absent or non-parseable `FileVersion` attribute : [bugtracker issue](https://bugs.kerbalspaceprogram.com/issues/28289)
- AssemblyLoader crashing when an unmanaged `*.dll` file exists : [bugtracker issue](https://bugs.kerbalspaceprogram.com/issues/28489)

### How to fix ?

- Download the `KSP1.12.2-StuckOnLoadingPartUpgradesFix.zip` file from the **[GitHub releases page](https://github.com/gotmachine/StuckOnLoadingPartUpgradesFix/releases)**.
- Extract that zip content **to the root KSP folder** (**NOT GameData**) :
  - On Windows, this is the folder containing the `KSP_x64.exe` file
  - On Linux, this is the folder containing the `KSP.x86_64` file
  - On MacOS, this is the folder containing the `KSP.app` file
- Done !

### Details

See corresponding [KSP bugtracker issue](https://bugs.kerbalspaceprogram.com/issues/28036).

Note that there is a manual workaround to this bug : search your `GameData` and subfolders contents for `*.dll` files with the exact same name, and rename the duplicates to something like `*.dll.duplicate` (or just delete it). 

However, this is quite unpractical, so while we wait for a proper fix in an hypothetical KSP 1.12.3 release, I've attempted to to provide an easier solution. Fixing this bug can't be done from a KSP plugin, since it cause the whole KSP plugin loader to crash before any plugin has a chance to execute. 

So the fix is implemented through [BepInEx](https://github.com/BepInEx/BepInEx), a popular Unity/XNA runtime patcher and plugin framework used for many games (Valheim, Outwards, WorldBox...).

### Note to modders : workaround

If you are redistributing a `*.dll` file that is likely to be also redistribtued by another mod, you can prevent the issue by renaming that `*.dll` to something unique. For example rename `SomeLibrary.dll` to `SomeLibrary_MyMod.dll`. Due to the fact that mono will only load a single instance of the same assembly by internal name, this is safe to do. 

The only caveat is that there will be multiple `LoadedAssembly` entries in `PluginLoader.loadedAssemblies` for the same assembly, so keep that in mind in the very unlikely case you are iterating on it.

Common cases :
- MiniAVC (depreciated and shouldn't be used)
- `KAS-API-v2.dll` (Kerbal Attachement System API) 
- `CLSInterfaces.dll` (Connected Living Spaces API)
- Unity Mono/.NET libraries missing from the KSP distribution, like `System.IO.Compression.dll`
- Any nuget package

### Changelog

- V1.1 : Added fixes for AssemblyLoader crashes on dlls containing no `FileVersion` attribute, or non-managed dlls.
- V1.0 : Initial release

### Licensing

The provided download contains :
- BepInEx 5.4.17 : **`LGPL-2.1`** license
- A BepInEx plugin ([source](https://github.com/gotmachine/StuckOnLoadingPartUpgradesFix)) named `AssemblyLoaderFixBepInEx.dll` that patch the faulty KSP 1.12.2 method : **`MIT`** license
