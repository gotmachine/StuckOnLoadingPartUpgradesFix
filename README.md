# StuckOnLoadingPartUpgradesFix

This is a fix for the KSP 1.12.2 bug that cause the game to be stuck on `Loading Part Upgrades` on a modded install.

This bug happens when you have a duplicated `*.dll` file in you `GameData` folder/subfolders. In KSP 1.12, the code that is supposed to be handling that (perfectly normal, mod authors aren't doing anything wrong) situation is crashing, causing the whole loading process to stop.

If that is happening to you, you will see the following error near the beginning of your `KSP.log` file :
```
ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection.
Parameter name: index
	System.ThrowHelper.ThrowArgumentOutOfRangeException (System.ExceptionArgument argument, System.ExceptionResource resource)
	System.ThrowHelper.ThrowArgumentOutOfRangeException ()
	System.Collections.Generic.List`1[T].RemoveAt (System.Int32 index)
	AssemblyLoader.FlagDuplicatedPlugins ()
	GameDatabase+<LoadObjects>d__90.MoveNext ()
	UnityEngine.SetupCoroutine.InvokeMoveNext (System.Collections.IEnumerator enumerator, System.IntPtr returnValueAddress)
	UnityEngine.MonoBehaviour:StartCoroutine(IEnumerator)
	<CreateDatabase>d__71:MoveNext()
	UnityEngine.MonoBehaviour:StartCoroutine(IEnumerator)
	GameDatabase:StartLoad()
	<LoadSystems>d__11:MoveNext()
	UnityEngine.MonoBehaviour:StartCoroutine(IEnumerator)
	LoadingScreen:Start()
```

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

However, this can have some (unlikely) side issues, and is quite unpractical, so while we wait for a proper fix in an hypothetical KSP 1.12.3 release, I've attempted to to provide an easier solution. 

Fixing this bug can't be done from a KSP plugin, since it cause the whole KSP plugin loader to crash before any plugin has a chance to execute. 

I've initially tried to make a `Mono.Cecil` based patcher for the KSP `Assembly-CSharp.dll` assembly, however `Mono.Cecil` fails to rewrite the KSP assembly correctly due to obfuscation. 

So the fix is implemented through [BepInEx](https://github.com/BepInEx/BepInEx), a popular Unity/XNA runtime patcher and plugin framework used for many games (Valheim, Outwards, WorldBox...).

### Licensing

The provided download contains :
- BepInEx 5.4.17 : **`LGPL-2.1`** license
- A BepInEx plugin ([source](https://github.com/gotmachine/StuckOnLoadingPartUpgradesFix)) named `AssemblyLoaderFixBepInEx.dll` that patch the faulty KSP 1.12.2 method : **`MIT`** license
