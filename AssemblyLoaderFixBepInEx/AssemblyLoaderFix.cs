using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace AssemblyLoaderFix
{
    [BepInPlugin("AssemblyLoaderFix", "AssemblyLoaderFix", "1.1")]
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

            UnityEngine.Debug.Log("AssemblyLoader: FlagDuplicatedPlugins patch ran successfully");
            return false;
        }
    }

    [HarmonyPatch(typeof(AssemblyLoader))]
    [HarmonyPatch(nameof(AssemblyLoader.LoadPluginInfo))]
    class LoadPluginInfo
    {
        static bool Prefix(FileInfo file, List<AssemblyInfo> ___availableAssemblies)
        {
            Version fileVersion = null;
            string fileVersionString = FileVersionInfo.GetVersionInfo(file.FullName).FileVersion;
            if (!string.IsNullOrEmpty(fileVersionString))
                Version.TryParse(fileVersionString, out fileVersion);

            Version assemblyVersion = null;
            try
            {
                AssemblyName assemblyName = AssemblyName.GetAssemblyName(file.FullName);
                assemblyVersion = assemblyName.Version;
            }
            catch
            {
                return false;
            }

            Version bestVersion;
            if (fileVersion == null && assemblyVersion == null)
                bestVersion = new Version(0, 0, 0, 0);
            else if (fileVersion == null)
                bestVersion = assemblyVersion;
            else if (assemblyVersion == null)
                bestVersion = fileVersion;
            else
                bestVersion = assemblyVersion > fileVersion ? assemblyVersion : fileVersion;

            AssemblyInfo item = new AssemblyInfo(Path.GetFileNameWithoutExtension(file.FullName), file.FullName, bestVersion);
            ___availableAssemblies.Add(item);
            return false;
        }
    }

    [HarmonyPatch(typeof(AssemblyLoader))]
    [HarmonyPatch(nameof(AssemblyLoader.LoadPlugin))]
    class LoadPlugin
    {
        static bool Prefix(FileInfo file)
        {
            try
            {
                AssemblyName.GetAssemblyName(file.FullName);
            }
            catch (Exception e)
            {
                if (e is BadImageFormatException)
                    UnityEngine.Debug.LogWarning($"AssemblyLoader: skipping \"{file.FullName}\", this isn't a valid managed assembly (BadImageFormatException)");
                else
                    UnityEngine.Debug.LogWarning($"AssemblyLoader: skipping \"{file.FullName}\", {e.GetType()}");

                return false;
            }

            return true;
        }
    }
}

