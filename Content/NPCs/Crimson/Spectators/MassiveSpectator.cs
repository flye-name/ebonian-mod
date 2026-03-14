using EbonianMod.Core.Systems.Verlets;
using EbonianMod.Content.Items.Pets;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using System.IO;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Crimson.Spectators;

public class MassiveSpectator : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Crimson/Spectators/"+Name;
    public override void SetStaticDefaults()
    {

        var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            CustomTexturePath = Helper.AssetPath+"NPCs/Crimson/Spectators/MassiveSpectator_Bestiary",
            Position = new Vector2(20f, 40f),
            PortraitPositionXOverride = 0f,
            PortraitPositionYOverride = 32f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return (spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneOverworldHeight && Main.hardMode && !NPC.AnyNPCs(Type) && !NPC.AnyNPCs(NPCType<Cecitior.Cecitior>())) ? (GetInstance<DownedBossSystem>().downedCecitior ? 0.09f : 0.5f) : 0;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        int associatedNPCType = NPCType<Cecitior.Cecitior>();
        bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);


        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Organ"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.MassiveSpectator.Bestiary"),
        });
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<Panopticon>(), 40));
        npcLoot.Add(ItemDropRule.Common(ItemID.Lens, 2));
    }
    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        NPC.lifeMax = 1000;
        NPC.damage = 0;
        NPC.noTileCollide = true;
        NPC.defense = 3;
        NPC.knockBackResist = 0;
        NPC.width = 46;
        NPC.height = 44;
        NPC.lavaImmune = true;
        NPC.noGravity = true;
        NPC.buffImmune[24] = true;
        SoundStyle hit = Sounds.fleshHit;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = hit;
        NPC.netAlways = true;
        NPC.hide = true;
        NPC.chaseable = false;
    }
    Verlet verlet;
    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
    }
    public override void OnSpawn(IEntitySource source)
    {
        NPC.ai[1] = Main.rand.NextFloat(20, 100);
        NPC.ai[2] = Main.rand.NextFloat(30, 100);
        NPC.ai[3] = Main.rand.NextFloatDirection();
        NPC.netUpdate = true;
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (verlet is null)
            verlet = new Verlet(NPC.Center, 16, 20, -0.25f, true, true, 30);
        else
            verlet.Draw(spriteBatch, new VerletDrawData(new VerletTextureData(Texture + "_Vein", null, Texture + "_VeinBase"), _endRot: NPC.rotation + MathHelper.PiOver2));
        Texture2D texture = Request<Texture2D>(Texture).Value;
        spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        Texture2D glow = Assets.NPCs.Crimson.Spectators.MassiveSpectator_Glow.Value;
        spriteBatch.Draw(glow, NPC.Center - Main.screenPosition, null, Color.White, NPC.rotation, texture.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        return false;
    }
    Vector2 stalkBase;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(stalkBase);
        writer.Write(found);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        stalkBase = reader.ReadVector2();
        found = reader.ReadBoolean();
    }
    public override bool CheckDead()
    {
        NPC.NewNPCDirect(null, NPC.Center + new Vector2(0, -500), NPCType<Cecitior.Cecitior>());
        return base.CheckDead();
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (!NPC.active)
        {
            MPUtils.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0, 0);
            if (Main.dedServ)
                return;
            SoundEngine.PlaySound(Sounds.cecitiorDie, NPC.Center);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/Gnasher0").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk3").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk1").Type, NPC.scale);
            for (int i = 0; i < 5; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
            for (int i = 0; i < 3; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
            for (int i = 0; i < verlet.points.Count; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), verlet.points[i].position, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/CrimorrhageChain").Type, NPC.scale);
            }
        }
    }
    bool found = false;
    public override void AI()
    {
        NPC.dontTakeDamage = true;
        foreach (Player _player in Main.ActivePlayers)
        {
            if (_player.Distance(NPC.Center) < 200)
            {
                if (_player.whoAmI == Main.myPlayer && Main.MouseWorld.Distance(NPC.Center) < NPC.Size.Length() * 1.5f)
                {
                    NPC.dontTakeDamage = false;
                }
            }
        }
        NPC.timeLeft = 10;
        NPC.despawnEncouraged = false;
        NPC.chaseable = false;
        if (NPC.AnyNPCs(NPCType<Cecitior.Cecitior>())) NPC.active = false;
        Player player = Main.player[NPC.target];
        NPC.TargetClosest(false);
        if (!found && (stalkBase == Vector2.Zero || Helper.Raycast(Helper.Raycast(NPC.Center, Vector2.UnitY, 400).Point, Vector2.UnitY, 32, false).RayLength > 16))
        {
            NPC.netUpdate = true;
            Vector2 direction = Vector2.UnitY.RotatedBy(MathHelper.PiOver4 + MathHelper.PiOver4 * 0.25f);
            int attempts = 0;
            while (Helper.Raycast(NPC.Center, direction, 400).RayLength >= 399 && attempts++ <= 300)
            {
                NPC.Center += Vector2.UnitY * 4;
                NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-10, 10);
                direction = Vector2.UnitY;
            }
            if (Helper.Raycast(Helper.Raycast(NPC.Center, direction, 400).Point, Vector2.UnitY, 32).RayLength < 16)
            {
                stalkBase = Helper.Raycast(NPC.Center, direction, 400).Point + new Vector2(0, 40);
                found = true;
            }
            return;
        }
        NPC.rotation = NPC.Center.FromAToB(player.Center).ToRotation() + MathHelper.Pi;
        NPC.velocity = NPC.Center.FromAToB(player.Center - new Vector2(NPC.ai[1] * NPC.ai[3], NPC.ai[2]), false) * 0.005f;
        NPC.Center = Vector2.Clamp(NPC.Center, stalkBase - new Vector2(200), stalkBase + new Vector2(200));
        if (NPC.Center.Distance(stalkBase) > 200)
        {
            NPC.netUpdate = true;
            NPC.ai[1] = Main.rand.NextFloat(20, 100);
            NPC.ai[2] = Main.rand.NextFloat(30, 100);
            NPC.ai[3] = Main.rand.NextFloatDirection();
        }
        if (verlet is not null)
            verlet.Update(stalkBase, NPC.Center + new Vector2(13, 0).RotatedBy(NPC.rotation));
    }
}
