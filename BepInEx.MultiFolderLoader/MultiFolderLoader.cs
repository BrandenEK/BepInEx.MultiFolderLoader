﻿using System.Collections.Generic;
using BepInEx.Logging;
using Mono.Cecil;

namespace BepInEx.MultiFolderLoader
{
    public static class MultiFolderLoader
    {
        public static readonly ManualLogSource Logger = Logging.Logger.CreateLogSource(nameof(MultiFolderLoader));

        // Add dummy property to fulfil the preloader patcher contract
        public static IEnumerable<string> TargetDLls => new string[0];

        public static void Initialize()
        {
            // Not used, exists so that this works as preloader patch
        }

        public static void Finish()
        {
            // Hook chainloader only after preloader to not cause resolving on UnityEngine too soon
            ChainloaderHandler.Init();
        }

        public static void Patch(AssemblyDefinition ass)
        {
            // Not used, exists so that this works as preloader patch
        }
    }
}