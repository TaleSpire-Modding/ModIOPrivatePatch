using HarmonyLib;
using Miop.Consumer.Messages;
using RPCPlugin.RPC;
using System.Reflection;
using TaleSpire.Modding;
using UnityEngine;

namespace Miop.Patches
{
    [HarmonyPatch(typeof(CommunityModsBrowserOwnedItem), "OnClicked")]
    internal class CommunityModsBrowserOwnedItemOnClickedPatch
    {
        public static void Prefix(ref ModInfo ____modInfo)
        {
            if (____modInfo.Kind != ModKind.Creature) return;

            RPCInstance.SendMessage(new RespondModInfo()
            {
                ModInfo = ____modInfo
            });
        }
    }

    [HarmonyPatch(typeof(CommunityModsBrowserOwnedItem), "DeliverData")]
    internal class CommunityModsBrowserOwnedItemDeliverDataPatch
    {
        // We want to hide the Edit button for mod infos that are not ours
        public static void Postfix(ref ModInfo ____modInfo, ref GameObject ____editButton)
        {
            if (____modInfo.Kind != ModKind.Creature || string.IsNullOrWhiteSpace(ModIOUnityAsyncGetCurrentUserCreationsPatch.MyRepoUserName)) return;
            if (____modInfo.RepoUserName != ModIOUnityAsyncGetCurrentUserCreationsPatch.MyRepoUserName)
            {
                ____editButton.SetActive(false);
            }
        }
    }

    public static class SysReflec
    {
        // Simple sys reflec to call any methods on any object
        public static object CallMethod(this object o, string methodName, params object[] args)
        {
            MethodInfo mi = o.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            return mi?.Invoke(mi.IsStatic ? null : o, args);
        }
    }

}
