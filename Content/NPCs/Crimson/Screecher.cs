using EbonianMod.Content.Items.Weapons.Ranged;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Crimson;

public class Screecher : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Crimson/"+Name;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 10;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.Screecher.Bestiary"),
        });
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Vertebrae, 2, 1, 4));
        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FangSlinger>(), 90));
    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        if (spawnInfo.Player.ZoneCrimson)
            return .07f;
        else
            return 0;
    }
    public override void SetDefaults()
    {
        NPC.width = 64;
        NPC.height = 70;
        NPC.damage = 10;
        NPC.defense = 8;
        NPC.lifeMax = 65;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.value = 60f;
        NPC.knockBackResist = 0.5f;
        NPC.aiStyle = -1;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
    }
    public override bool CheckDead()
    {
        if (Main.dedServ)
            return true;
        for (int i = 0; i < 5; i++)
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/WalkerGore" + i).Type, NPC.scale);
        return true;
    }

    private const int Idle = 0;
    private const int Walk = 1;
    private const int Attack = 2;
    public float AIState
    {
        get => NPC.ai[0];
        set => NPC.ai[0] = value;
    }
    public float AITimer
    {
        get => NPC.ai[1];
        set => NPC.ai[1] = value;
    }
    public float AITimer2
    {
        get => NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (AIState == Walk)
        {
            if (!NPC.velocity.X.InRange(0, 0.5f) && NPC.velocity.Y.InRange(0))
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 5 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 0;
                }
            }
            else
                NPC.frame.Y = 6 * frameHeight;
        }
        else
        {
            if (AITimer < 100)
            {
                if (NPC.frame.Y < 6 * frameHeight)
                    NPC.frame.Y = 6 * frameHeight;
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y < 9 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else
                        NPC.frame.Y = 8 * frameHeight;
                }
            }
            else
            {
                if (NPC.frameCounter % 5 == 0)
                {

                    if (NPC.frame.Y > 6 * frameHeight)
                        NPC.frame.Y -= frameHeight;
                }
            }
        }
    }
    public override void AI()
    {
        Player player = Main.player[NPC.target];
        if (AIState == Idle)
        {
            NPC.TargetClosest(true);
            if (NPC.HasValidTarget)
            {
                AIState = Walk;
                AITimer = 0;
            }
        }
        else if (AIState == Walk)
        {
            NPC.damage = 0;
            AITimer2++;
            AITimer = MathHelper.Clamp(AITimer, 0, 500);
            if (AITimer2 % 7 == 0)
                NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;

            NPC.spriteDirection = NPC.direction;

            if (player.Center.Distance(NPC.Center) > 100 && (player.Center.Y - NPC.Center.Y < 100 || player.Center.Y - NPC.Center.Y > -100))
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, NPC.direction * 3f, 0.1f);
            if (player.Center.Distance(NPC.Center) < 100)
            {
                NPC.netUpdate = true;
                AITimer += 4;
                NPC.velocity.X *= 0.9f;
                NPC.frame.Y = 6 * 72;
                NPC.frameCounter = 1;
            }
            if ((player.Center.Y - NPC.Center.Y > 100 || player.Center.Y - NPC.Center.Y < -100))
            {
                AITimer--;
            }

            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);

            if (NPC.collideY && NPC.collideX)
            {
                NPC.velocity.Y = -6;
            }


            if (AITimer >= 100)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = 0;
                AIState = Attack;
                AITimer = 0;
            }
        }
        else if (AIState == Attack)
        {
            AITimer++;
            if (AITimer == 10)
            {
                SoundEngine.PlaySound(Sounds.shriek, NPC.Center);
            }
            if (AITimer % (AITimer < 50 ? 10 : 5) == 0 && AITimer > 15 && AITimer < 90)
            {
                Projectile.NewProjectileDirect(null, NPC.Center - new Vector2(NPC.direction * -13, 4), new Vector2(NPC.direction * Main.rand.NextFloat(2, 6), 0).RotatedByRandom(MathHelper.PiOver4), ProjectileID.BloodNautilusShot, 13, 0).tileCollide = true;
            }
            if (player.Center.Distance(NPC.Center) > 100 && AITimer < 100)
                AITimer++;

            if (AITimer >= 120)
            {
                NPC.netUpdate = true;
                AIState = Walk;
                AITimer = 0;
            }
        }
    }
}
