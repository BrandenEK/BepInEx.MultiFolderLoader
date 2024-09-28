using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BepInEx.MultiFolderLoader
{
    public class Mod
    {
        public string PreloaderPatchesPath { get; set; }
        public string PluginsPath { get; set; }
        public string ModDir { get; set; }
    }


    public static class ModManager
    {
        private class ModDirSpec
        {
            public string baseDir;
            public HashSet<string> blockedMods, enabledMods;
        }

        private const string CONFIG_NAME = "doorstop_config.ini";
        public static readonly List<Mod> Mods = new List<Mod>();
        private static readonly List<ModDirSpec> ModDirs = new List<ModDirSpec>();

        public static void Init()
        {
            try
            {
                InitInternal();
            }
            catch (Exception e)
            {
                MultiFolderLoader.Logger.LogError($"Failed to index mods, no mods will be loaded: {e}");
            }
        }

        private static void InitInternal()
        {
            LoadFrom(new ModDirSpec
            {
                baseDir = "Modding/plugins"
            });
        }

        private static void LoadFrom(ModDirSpec modDir)
        {
            var modsBaseDirFull = Path.GetFullPath(modDir.baseDir);
            if (!Directory.Exists(modsBaseDirFull))
            {
                MultiFolderLoader.Logger.LogWarning("No mod folder found!");
                return;
            }

            AddMod(modsBaseDirFull);

            // Also resolve assemblies like bepin does
            AppDomain.CurrentDomain.AssemblyResolve += ResolveModDirectories;
        }

        private static Assembly ResolveModDirectories(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            foreach (var mod in Mods)
                if (Utility.TryResolveDllAssembly(name, mod.ModDir, out var ass))
                    return ass;

            return null;
        }

        public static IEnumerable<string> GetPreloaderPatchesDirs()
        {
            return Mods.Select(m => m.PreloaderPatchesPath).Where(s => s != null);
        }

        public static IEnumerable<string> GetPluginDirs()
        {
            return Mods.Select(m => m.PluginsPath).Where(s => s != null);
        }

        private static void AddMod(string dir)
        {
            Mods.Add(new Mod
            {
                PluginsPath = dir,
                PreloaderPatchesPath = null,
                ModDir = dir
            });
        }
    }
}