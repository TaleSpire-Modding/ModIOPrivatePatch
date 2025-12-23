using Bounce.Unmanaged;
using RPCPlugin.Interfaces;
using System;
using TaleSpire.ContentManagement;
using TaleSpire.Modding;
using ZeroFormatter;

namespace Miop.Consumer.Messages
{
    [ZeroFormattable]
    public class RespondModInfo : RpcMessage
    {
        // Base Data
        [Index(0)] public virtual ushort ProviderIndex { get; set; }

        // Interned Pack ID Data
        [Index(1)] public virtual uint Ix { get; set; }
        [Index(2)] public virtual uint Iy { get; set; }
        [Index(3)] public virtual uint Iz { get; set; }
        [Index(4)] public virtual uint Iw { get; set; }

        // Interned Pack ID Data
        [Index(5)] public virtual uint Mx { get; set; }
        [Index(6)] public virtual uint My { get; set; }
        [Index(7)] public virtual uint Mz { get; set; }
        [Index(8)] public virtual uint Mw { get; set; }

        // Back to Base Data
        [Index(9)] public virtual string Name { get; set; }
        [Index(10)] public virtual string Summary { get; set; }
        [Index(11)] public virtual string Description { get; set; }
        [Index(12)] public virtual string ThumbnailUrl { get; set; }
        [Index(13)] public virtual string RepoUserName { get; set; }
        [Index(14)] public virtual string RepoModWebPageUrl { get; set; }
        [Index(15)] public virtual string ImageUrl { get; set; }
        [Index(16)] public virtual byte Kind { get; set; }
        [Index(17)] public virtual byte Status { get; set; }
        [Index(18)] public virtual ModVisibility ModVisibility { get; set; }

        // Archive Data
        [Index(19)] public virtual string ArchiveUrl { get; set;}
        [Index(20)] public virtual byte[] ArchiveMd5 { get; set;}
        [Index(21)] public virtual int ArchiveSize { get; set;}
        [Index(22)] public virtual DateTime UpdateDateTime { get; set;}

        // Votes Data
        [Index(23)] public virtual int UpVotes { get; set; }
        [Index(24)] public virtual int DownVotes { get; set; }
        [Index(25)] public virtual string CampaignGuid { get; set; }



        [IgnoreFormat]
        public ModInfo ModInfo { 
            get {
                
                return new ModInfo(
                    new ProviderIndex(ProviderIndex),
                    new InternedPackId(
                        InternedPackSourceReflectionFactory.Create(PackSourceKind.BrModIo),
                        new SourceLocalPackId(new Unity.Mathematics.uint4(Ix,Iy,Iz,Iw)),
                        new MD5(new Unity.Mathematics.uint4(Mx, My, Mz, Mw))),
                    Name,
                    (ModKind)Kind,
                    (ModStatus)Status,
                    Summary,
                    Description,
                    ThumbnailUrl,
                    ImageUrl,
                    ModVisibility,
                    new ModArchiveInfo(ArchiveUrl, new MD5(ArchiveMd5), ArchiveSize, UpdateDateTime),
                    true,
                    RepoUserName,
                    RepoModWebPageUrl,
                    new ModVotes(UpVotes, DownVotes),
                    new CampaignGuid(CampaignGuid),
                    null
                    );
            } 
            set {
                ProviderIndex = (ushort) value.ProviderIndex.AsIndex;
                Ix = value.PackId.SourceLocalPackId.GetRawData().x;
                Iy = value.PackId.SourceLocalPackId.GetRawData().y;
                Iz = value.PackId.SourceLocalPackId.GetRawData().z;
                Iw = value.PackId.SourceLocalPackId.GetRawData().w;
                Mx = value.PackId.Version.Data.x;
                My = value.PackId.Version.Data.y;
                Mz = value.PackId.Version.Data.z;
                Mw = value.PackId.Version.Data.w;
                Name = value.Name;
                Kind = (Byte) value.Kind;
                Status = (Byte) value.Status;
                Summary = value.Summary;
                Description = value.Description;
                ThumbnailUrl = value.ThumbnailUrl;
                ImageUrl = value.ImageUrl;
                ModVisibility = value.ModVisibility;
                ArchiveUrl = value.ArchiveInfo.ArchiveUrl;
                ArchiveMd5 = value.ArchiveInfo.ArchiveMd5.ToArray();
                ArchiveSize = value.ArchiveInfo.ArchiveSize;
                UpdateDateTime = value.ArchiveInfo.UpdateDateTime;
                RepoUserName = value.RepoUserName;
                RepoModWebPageUrl = value.RepoModWebPageUrl;
                UpVotes = value.RepoVotes.UpVotes;
                DownVotes = value.RepoVotes.DownVotes;
                CampaignGuid = value.CampaignId.ToString();
            } 
        }

        public RespondModInfo() { }

        // Serialize to Binary
        public override byte[] Value() {
            return ZeroFormatterSerializer.Serialize(this);
        }

        // Construct from Binary
        public RespondModInfo(byte[] data)
        {
            ModIOPrivatePatch.InternalLogger.LogInfo($"Received {data.Length} bytes.");
            RespondModInfo t = ZeroFormatterSerializer.Deserialize<RespondModInfo>(data);
            ProviderIndex = t.ProviderIndex;
            Iw = t.Iw;
            Ix = t.Ix;
            Iy = t.Iy;
            Iz = t.Iz;
            Mw = t.Mw;
            Mx = t.Mx;
            My = t.My;
            Mz = t.Mz;
            Name = t.Name;
            Summary = t.Summary;
            Description = t.Description;
            ThumbnailUrl = t.ThumbnailUrl;
            ImageUrl = t.ImageUrl;
            RepoUserName = t.RepoUserName;
            RepoModWebPageUrl = t.RepoModWebPageUrl;
            ImageUrl = t.ImageUrl;
            Kind = t.Kind;
            Status = t.Status;
            ModVisibility = t.ModVisibility;
            ArchiveUrl = t.ArchiveUrl;
            ArchiveMd5 = t.ArchiveMd5;
            ArchiveSize = t.ArchiveSize;
            UpdateDateTime = t.UpdateDateTime;
            UpVotes = t.UpVotes;
            DownVotes = t.DownVotes;
            CampaignGuid = t.CampaignGuid;
        }
    }
}
