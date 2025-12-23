using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Miop.Patches;
using ModIO;
using Newtonsoft.Json;
using PluginUtilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Miop
{
    [BepInPlugin(Guid, Name, Version)]
    [BepInDependency(SetInjectionFlag.Guid)]
    [BepInDependency(RPCPlugin.RPCPlugin.Guid)]
    public class ModIOPrivatePatch : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "MOP (Mod IO Private Patch)";
        public const string Guid = "org.HolloFox.mop.patch";
        public const string Version = "0.0.0.0";

        internal static ManualLogSource InternalLogger;

        // Configuration
        private ConfigEntry<string> WhiteList { get; set; }      // Sample configuration for triggering a plugin via keyboard
        private ConfigEntry<string> BlackList { get; set; }     // Sample configuration for triggering a plugin via keyboard
        
        internal static HashSet<string> SubscribedAuthors;
        internal static HashSet<string> IgnoredAuthors;

        internal static string LocalHidden { get; set; }

        /// <summary>
        /// Method triggered when the plugin loads
        /// </summary>
        public void Awake()
        {
            InternalLogger = Logger;
            Logger.LogInfo($"In Awake for {Name}");
            Harmony harmony = new Harmony(Guid);
            DoConfig(Config);
            harmony.PatchAll();
        }

        /// <summary>
        /// Establishes the config for the plugin,
        /// </summary>
        /// <param name="config">Configfile regarding the specific plugin</param>
        public void DoConfig(ConfigFile config)
        {
            WhiteList = config.Bind("Author Filtering", "Auto-Subscribe", string.Empty);
            BlackList = config.Bind("Author Filtering", "Ignore", string.Empty);
            SubscribedAuthors = WhiteList.Value.Split(",").ToHashSet();
            IgnoredAuthors = BlackList.Value.Split(",").ToHashSet();

            LocalHidden = Path.Join(Path.GetDirectoryName(Info.Location),".hidden");

            // Start importing all other hidden/private minis
            string[] files = Directory.GetFiles(Path.GetDirectoryName(Info.Location), "*.hidden");
            if (files.Any())
            {
                foreach (string file in files.Where(f => f != LocalHidden)) {
                    foreach (KeyValuePair<long, ModProfile> profile in JsonConvert.DeserializeObject<Dictionary<long, ModProfile>>(File.ReadAllText(file)))
                    {
                        ModIOUnityAsyncGetCurrentUserCreationsPatch.importing[profile.Key] = profile.Value;
                    }
                } 
            }

            // Cache my hidden files
            if (File.Exists(LocalHidden))
            {
                ModIOUnityAsyncGetCurrentUserCreationsPatch.myPrivateMods = JsonConvert.DeserializeObject<Dictionary<long, ModProfile>>(File.ReadAllText(LocalHidden));
            }
        }

        
    }
}
