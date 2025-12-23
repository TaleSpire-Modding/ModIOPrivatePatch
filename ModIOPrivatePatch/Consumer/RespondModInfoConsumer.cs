using Miop.Consumer.Messages;
using RPCPlugin.Interfaces;
using System.Collections.Generic;
using TaleSpire.ContentManagement;
using TaleSpire.Modding;

namespace Miop.Consumer
{
    [InitOnLoad]
    internal sealed class RespondModInfoConsumer : RpcConsumer<RespondModInfo>
    {
        private static readonly RespondModInfoConsumer instance = new RespondModInfoConsumer();
        static RespondModInfoConsumer() { } // Make sure it's truly lazy

        public static RespondModInfoConsumer Instance { get { return instance; } }

        // My constructor (If I wanted something to happen)
        private RespondModInfoConsumer() : base()
        {
        }

        // only want to try subscribe if first time receiving mod info
        private static readonly HashSet<ModInfo> alreadyLooked = new HashSet<ModInfo>();

        /// <summary>
        /// Event that's triggered once receiving the message that was sent
        /// </summary>
        public override void Handle(RespondModInfo message)
        {
            ModInfo modInfo = message.ModInfo;
            
            // already looking? skip instead of repeating search
            if (alreadyLooked.Contains(modInfo))
            {
                ModIOPrivatePatch.InternalLogger.LogMessage($"has already been searched, Skipping!");
                return;
            }
            alreadyLooked.Add(modInfo);
            
            AssetDb.TryFindSoleSpawnableContentWithLock(modInfo.PackId, out DbContentKind contentKind, out InternedContentAddress internedContentAddress);
            if (contentKind == DbContentKind.Creature)
            {
                return;
            }
            
            // Validate
            ModIOPrivatePatch.InternalLogger.LogMessage("Does not exist, lets try finding it");
            if (!modInfo.IsValid)
            {
                alreadyLooked.Remove(modInfo);
                return;
            }

            // It's valid, lets subscribe!!
            ModManager.Instance.MaybeAddSubscription(modInfo, out var sub);
            
        }
    }
}
