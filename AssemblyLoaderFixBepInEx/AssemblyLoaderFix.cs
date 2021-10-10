using System.Collections.Generic;
using BepInEx;
using HarmonyLib;

namespace AssemblyLoaderFix
{
    [BepInPlugin("AssemblyLoaderFix", "AssemblyLoaderFix", "1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Harmony harmony = new Harmony("AssemblyLoaderFix");
            harmony.PatchAll();
            Destroy(this);
        }
    }

    [HarmonyPatch(typeof(AssemblyLoader))]
    [HarmonyPatch(nameof(AssemblyLoader.FlagDuplicatedPlugins))]
    class FlagDuplicatedPlugins
    {
        static bool Prefix(List<AssemblyInfo> ___availableAssemblies)
        {
            for (int i = ___availableAssemblies.Count - 1; i >= 0; i--)
            {
                AssemblyInfo test = ___availableAssemblies[i];
                for (int j = i - 1; j >= 0; j--)
                {
                    AssemblyInfo other = ___availableAssemblies[j];
                    if (!string.IsNullOrEmpty(test.name) && !string.IsNullOrEmpty(other.name) && test.name == other.name)
                    {
                        if (test.assemblyVersion > other.assemblyVersion)
                        {
                            test.isDuplicate = true;
                            ___availableAssemblies.RemoveAt(j);
                        }
                        else
                        {
                            other.isDuplicate = true;
                            ___availableAssemblies.RemoveAt(i);
                        }
                    }
                }
            }

            UnityEngine.Debug.Log("FlagDuplicatedPlugins patch ran successfully");
            return false;
        }
    }
}

