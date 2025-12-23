using HarmonyLib;
using ModIO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Miop.Patches
{

    [HarmonyPatch(typeof(ModIOUnityAsync), nameof(ModIOUnityAsync.GetMods))]
    internal class ModIOUnityAsyncGetModsPatch
    {
        private static List<ModProfile> mods = new List<ModProfile>();
        private static HashSet<int> addedPages = new HashSet<int>();
        
        public static void Prefix(ref SearchFilter filter, ref Task<ResultAnd<ModPage>> __result)
        {
            // No point patching if we're not ignoring authors
            if (ModIOPrivatePatch.IgnoredAuthors.Count == 0) return;

            filter.SetPageSize(100);
        }

        public static void Postfix(ref SearchFilter filter, ref Task<ResultAnd<ModPage>> __result)
        {
            // No point patching if we're not ignoring authors
            if (ModIOPrivatePatch.IgnoredAuthors.Count == 0) return;

            int index = new Traverse(filter).Field("pageIndex").GetValue<int>();
            if (index == 0)
            {
                mods.Clear();
                addedPages.Clear();
            }

            Task<ResultAnd<ModPage>> copy = __result;
            __result = Task.Run(async () => {
                ResultAnd<ModPage> result = await copy;

                if (!addedPages.Contains(index))
                {
                    addedPages.Add(index);
                    mods.AddRange(result.value.modProfiles.Where(mod => !ModIOPrivatePatch.IgnoredAuthors.Contains(mod.creator.username)));
                }

                result.value.modProfiles = mods.Skip(60 * index).Take(60).ToArray();
                return result;
            });
        }
    }

    [HarmonyPatch(typeof(ModIOUnityAsync), nameof(ModIOUnityAsync.GetCurrentUserCreations))]
    internal class ModIOUnityAsyncGetCurrentUserCreationsPatch
    {
        internal static Dictionary<long,ModProfile> myPrivateMods = new Dictionary<long,ModProfile>();
        internal static Dictionary<long, ModProfile> importing = new Dictionary<long, ModProfile>();
        private static HashSet<int> addedPages = new HashSet<int>();

        internal static List<ModProfile> modsPaginationProxy = new List<ModProfile>();

        internal static string MyRepoUserName;

        private static long actualOwned = -1;

        public static void Postfix(ref SearchFilter filter, ref Task<ResultAnd<ModPage>> __result)
        {
            var traversableFilter = new Traverse(filter);

            // Don't patch if not creatures
            if (!traversableFilter.Field("tags").GetValue<List<string>>().Contains("Creature"))
                return;

            int index = traversableFilter.Field("pageIndex").GetValue<int>();
            int pageSize = traversableFilter.Field("pageSize").GetValue<int>();



            if (index == 0)
            {
                addedPages.Clear();
                modsPaginationProxy.Clear();
                modsPaginationProxy.AddRange(importing.Values);
            }

            Task<ResultAnd<ModPage>> copy = __result;
            __result = Task.Run(async () => {
                // alter result
                ResultAnd<ModPage> result = await copy;
                
                actualOwned = result.value.totalSearchResultsFound;

                // Record my Private Minis
                if (!addedPages.Contains(index))
                {
                    addedPages.Add(index);
                    foreach (ModProfile mod in result.value.modProfiles.Where(mod => !mod.visible))
                    {
                        myPrivateMods[mod.id] = mod;
                    }

                    string serialized = JsonConvert.SerializeObject(myPrivateMods);
                    File.WriteAllText(ModIOPrivatePatch.LocalHidden, serialized);

                    modsPaginationProxy.AddRange(result.value.modProfiles);
                }

                result.value.totalSearchResultsFound += importing.Count;

                // if first, add all other hidden minis first
                if (index == 0 && result.value.modProfiles.Length > 0)
                {
                    MyRepoUserName = result.value.modProfiles.First().creator.username;
                }

                // return the joined 
                result.value.modProfiles = modsPaginationProxy.Skip(60 * index).Take(60).ToArray();

                return result;
            });
        }
    }

}
