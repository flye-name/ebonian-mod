using EbonianMod.Content.Bossbars;
using EbonianMod.Content.Dusts;
using EbonianMod.Content.Items.Vanity;
using EbonianMod.Content.Items.BossTreasure;
using EbonianMod.Content.Items.Pets;
using EbonianMod.Content.Items.Tiles.Trophies;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.Items.Weapons.Melee;
using EbonianMod.Content.Items.Weapons.Ranged;
using EbonianMod.Content.Items.Weapons.Summoner;
using EbonianMod.Content.NPCs.ArchmageX;
using EbonianMod.Content.NPCs.Corruption;
using EbonianMod.Content.NPCs.Garbage.Projectiles;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Content.Tiles;
using EbonianMod.Core.Systems.Cinematic;
using ReLogic.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Content.NPCs.Garbage;

public partial class HotGarbage : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Garbage/"+Name;
    public enum State 
    {
        Death = -1, 
        Intro,
        Idle, 
        WarningForDash, 
        Dash,
        SlamPreperation,
        SlamSlamSlam,
        WarningForBigDash,
        BigDash,
        OpenLid,
        SpewFire,
        CloseLid,
        FallOver,
        SpewFire2, 
        BouncingBarrels,
        TrashBags,
        SodaMissiles,
        PipeBombAirstrike,
        SateliteLightning,
        MassiveLaser,
        MailBoxes,
        GiantFireball,
        SummonDrones,
    }

    public enum AnimationStyles
    {
        Intro = -1,
        Still,
        Idle,
        BoostWarning,
        Boost,
        Open,
        Close,
        Constipated
    }

    public AnimationStyles AnimationStyle;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 13;
        NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        NPCID.Sets.TrailingMode[NPC.type] = 0;
    }
    public override void SetDefaults()
    {
        NPC.width = 80;
        NPC.height = 74;
        NPC.damage = 30;
        NPC.defense = 8;
        NPC.lifeMax = 3200;
        NPC.value = Item.buyPrice(0, 10);
        NPC.knockBackResist = 0f;
        NPC.HitSound = SoundID.NPCHit4;
        NPC.buffImmune[BuffID.OnFire] = true;
        NPC.buffImmune[BuffID.OnFire3] = true;
        NPC.buffImmune[BuffID.Frostburn] = true;
        NPC.buffImmune[BuffID.Frostburn2] = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.aiStyle = -1;
        NPC.noGravity = false;
        NPC.waterMovementSpeed = 1f;
        NPC.BossBar = GetInstance<GarbageBar>();
        NPC.noTileCollide = false;
        NPC.boss = true;
        NPC.netAlways = true;
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Garbage");
        }
    }

    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        if (Main.expertMode)
            NPC.lifeMax = 4400;
        if (Main.masterMode)
            NPC.lifeMax = 5600;
    }

    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
    {
        npcHitbox.Height = NPC.height - 20;
        npcHitbox.Y += 24;

        npcHitbox.Width = NPC.width - 26;
        npcHitbox.X += 13;
        return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
        notExpertRule.OnSuccess(new CommonDrop(ItemType<Chainsword>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<DoomsdayRemote>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<MailboxStaff>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<SalvagedThruster>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<GarbageFlail>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<NastySnapper>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<PipebombI>(), 1, 20, 100));
        npcLoot.Add(notExpertRule);

        npcLoot.Add(ItemDropRule.Common(ItemType<GarbageMask>(), 4));
        npcLoot.Add(ItemDropRule.Common(ItemType<GarbageTrophy>(), 4));
        npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<GarbagePet>(), 4));
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<GarbageRelic>()));

        npcLoot.Add(ItemDropRule.BossBag(ItemType<GarbageBagI>()));
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.HotGarbage"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.HotGarbage.Bestiary"),
        });
    }
    public State AIState
    {
        get => (State)NPC.ai[0];
        set => NPC.ai[0] = (float)value;
    }
    public int AITimer
    {
        get => (int)NPC.ai[1];
        set => NPC.ai[1] = value;
    }
    public float AITimer2
    {
        get => NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    public float AITimer3
    {
        get => NPC.ai[3];
        set => NPC.ai[3] = value;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write((byte)NextAttack);
        writer.Write((byte)NextAttack2);
        writer.Write(StruckDead);
        writer.Write(PerformedFullMoveset);
        writer.WriteVector2(DisposablePosition);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        NextAttack = (State)reader.ReadByte();
        NextAttack2 = (State)reader.ReadByte();
        StruckDead = reader.ReadBoolean();
        PerformedFullMoveset = reader.ReadBoolean();
        DisposablePosition = reader.ReadVector2();
    }
    public override bool CheckDead()
    {
        if (NPC.life <= 0 && !StruckDead)
        {
            NPC.life = 1;
            AIState = State.Death;
            NPC.frameCounter = 0;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            CameraSystem.ScreenShakeAmount = 5;
            StruckDead = true;
            AITimer = -75;
            AITimer2 = -110;
            NPC.velocity = Vector2.Zero;
            NPC.frame.X = 160;
            NPC.frame.Y = 0;
            NPC.life = 1;
            if (!Main.dedServ)
                Music = 0;
            NetMessage.SendData(MessageID.SyncNPC, number: NPC.whoAmI);
            return false;
        }
        return true;
    }
    public override bool? CanFallThroughPlatforms()
    {
        return (NPC.Center.Y <= player.Center.Y - 100) || AIState == State.MassiveLaser;
    }
}