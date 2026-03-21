using EbonianMod.Content.Buffs;
using EbonianMod.Common.Misc;
using EbonianMod.Content;
using EbonianMod.Content.Items.Accessories;
using EbonianMod.Content.Items.Pets.LilPilg;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.Projectiles.AsteroidShower;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EbonianMod.Core.Systems.Scenes;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.IO;
using Terraria.ModLoader.UI;

namespace EbonianMod.Content.NPCs.Overworld.Asteroid;
public class AsteroidHerder : CommonNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Asteroid/"+Name;
    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailCacheLength[Type] = 4;
        NPCID.Sets.TrailingMode[Type] = 1;
    }
    public override void SetDefaults()
    {
        NPC.Size = new Vector2(24, 30);
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.damage = 20;
        NPC.defense = 5;
        NPC.lifeMax = 450;
        NPC.value = Item.buyPrice(0, 1, 0, 0);
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.HitSound = SoundID.NPCHit42.WithPitchOffset(-0.5f);
        NPC.dontTakeDamage = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.buffImmune[BuffType<RustyCut>()] = true;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
        {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Construct"),
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
        });
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0 && ded)
        {
            SoundEngine.PlaySound(SoundID.DeerclopsRubbleAttack, NPC.Center);
            for (int i = 0; i < 8; i++)
            {
                Gore.NewGore(default, NPC.Center, Main.rand.NextVector2Circular(10, 10), i % 2 == 0 ? GoreType<HerderStarGore>() : GoreType<HerderStarGore2>());
            }
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            Color newColor7 = Color.CornflowerBlue;
            for (int num613 = 0; num613 < 7; num613++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default, 0.8f);
            }
            for (float num614 = 0f; num614 < 1f; num614 += 0.125f)
            {
                Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
            }
            for (float num615 = 0f; num615 < 1f; num615 += 0.25f)
            {
                Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
            Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
            if (NPC.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
            {
                for (int num616 = 0; num616 < 7; num616++)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * NPC.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
            NPC.SpawnGore("EbonianMod/Warden", vel: -Vector2.UnitY * 3);
        }
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return RiverOfStarlight.ActiveConditions && spawnInfo.Player.ZoneNormalSpace ? 0.02f : 0;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<LilPilgI>(), 40));
        npcLoot.Add(ItemDropRule.Common(ItemType<StarBit>(), 8));
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Vector2 starPosition = NPC.Center;
        for (int i = 0; i < NPC.oldPos.Length; i++)
        {
            if (NPC.oldPos[i] == Vector2.Zero) break;
            starPosition = NPC.oldPos[i] + NPC.Size / 2;
        }
        void DrawStars(bool after, float rotOff, float scaleMult = 1.25f)
        {
            Texture2D star = Assets.NPCs.Overworld.Asteroid.AsteroidHerder_Star.Value;
            Texture2D star2 = Assets.NPCs.Overworld.Asteroid.AsteroidHerder_StarSmall.Value;
            int max = 8;
            for (int i = after ? max : 0; after ? i > 0 : i < max; i += (after ? -1 : 1))
            {
                Texture2D chosenStar = i % 2 == 0 ? star : star2;
                float angle = Helper.CircleDividedEqually(i, max) + rotOff;
                float progresso = (MathF.Sin(rotOff * 0.1f) + 1) * 0.5f;
                float distscale = Lerp(0.5f, 0.65f, progresso) * scaleMult * starRadius;
                Vector2 angleVec = angle.ToRotationVector2() * 50;
                float perspectiveScale = Lerp(MathF.Sin(rotOff * .33f), starForcedPerspective, starPerspectiveNormalizationLerpFactor);
                Vector2 offset = new Vector2(angleVec.X, angleVec.Y * (0.25f + perspectiveScale)) * distscale;
                float scale = Lerp(Lerp(0.5f + MathF.Abs(perspectiveScale * 0.25f), 1, Clamp(MathF.Sin(angle), 0, 1f)), 1, starForcedPerspective.InRange(0.75f, 0.1f) ? starPerspectiveNormalizationLerpFactor : 0);
                bool shouldDraw = after ? scale > 0.73f : scale <= 0.73f;

                if (shouldDraw)
                    Main.spriteBatch.Draw(chosenStar, starPosition + offset.RotatedBy(starRotation + MathF.Sin(rotOff * 0.05f) * 3) - screenPos, null, Color.Lerp(Color.Transparent, Color.White, Clamp(scale * 2, 0, 1)), angle, chosenStar.Size() / 2, scale, SpriteEffects.None, 0f);
            }
        }
        Texture2D tex = TextureAssets.Npc[Type].Value;
        Texture2D glow = Assets.NPCs.Overworld.Asteroid.AsteroidHerder_Glow.Value;

        float rotation = Main.GlobalTimeWrappedHourly + NPC.whoAmI * 14 + ToRadians(additionalRotation);

        if (NPC.velocity.Length() > 0 && AIState == 0)
            starRotation = Utils.AngleLerp(starRotation, NPC.velocity.ToRotation() + PiOver2, 0.01f);
        else if (AIState == 1)
            starRotation = Utils.AngleLerp(starRotation, 0, 0.1f);
        else if (AIState == 3)
            starRotation = Utils.AngleLerp(starRotation, -MathF.Sin(rotation * 0.05f) * 3, 0.2f);
        DrawStars(false, rotation);
        Main.spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        Main.spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        DrawStars(true, rotation);
        return false;
    }
    public override bool CheckDead()
    {
        if (!ded)
        {
            SoundEngine.PlaySound(SoundID.Item88.WithPitchOffset(0.2f), NPC.Center);
            foreach (Projectile p in Main.ActiveProjectiles)
            {
                if (p.type == ProjectileType<WardenSigil>() && p.ai[2] > 0)
                    p.Kill();
            }
            savedPos = NPC.Center;
            NPC.life = NPC.lifeMax;
            NPC.dontTakeDamage = true;
            ded = true;
            SwitchState(-1);
            return false;
        }
        return true;
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(movementFreq);
        writer.WriteVector2(savedPos);
        writer.Write(ded);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        movementFreq = reader.ReadSingle();
        savedPos = reader.ReadVector2();
        ded = reader.ReadBoolean();
    }
    // local
    float starRadius = 0.5f, starPerspectiveNormalizationLerpFactor = 0f, starForcedPerspective = 0.75f, starRotation = 0f, additionalRotation = 0f;
    // should be synced
    public float movementFreq;
    public Vector2 savedPos;
    public bool ded;
    SlotId deathSound;
    public override void AI()
    {
        Lighting.AddLight(NPC.Center, new Vector3(195, 169, 13) / 255 * 0.5f);
        Player player = Main.player[NPC.target];
        NPC.spriteDirection = NPC.direction;
        movementFreq++;
        switch (AIState)
        {
            case -1:
                {
                    if (!Main.dedServ)
                    {
                        if (SoundEngine.TryGetActiveSound(deathSound, out var _activeSound))
                        {
                            _activeSound.Pitch = Lerp(0, 1.5f, InOutCirc.Invoke(AITimer / 95f));
                            _activeSound.Position = NPC.Center;
                        }
                        else
                        {
                            deathSound = SoundEngine.PlaySound(Sounds.herderDying.WithVolumeScale(2), NPC.Center, (_) => NPC.active && !Main.gameInactive);
                        }
                    }
                    AITimer++;
                    NPC.velocity *= 0.9f;
                    NPC.Center = savedPos + Main.rand.NextVector2Circular(Clamp(AITimer * 0.1f, 0, 10), Clamp(AITimer * 0.1f, 0, 10));
                    if (AITimer < 80)
                    {
                        if (AITimer % 10 == 0 && AITimer < 250)
                            MPUtils.NewProjectile(null, NPC.Center + new Vector2(4 * NPC.direction, 0), Vector2.Zero, ProjectileType<MagicChargeUp>(), 0, 0, ai1: Lerp(0, 2, AITimer / 80f), ai2: 2);
                        additionalRotation += Lerp(0, 10, Helper.Saturate(AITimer / 60f));
                        starForcedPerspective = Lerp(starForcedPerspective, MathF.Sin(AITimer * 0.5f), 0.2f);
                        NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                        starPerspectiveNormalizationLerpFactor = Lerp(starPerspectiveNormalizationLerpFactor, 1, 0.2f);
                        starRadius = Lerp(starRadius, 1 + AITimer * 0.03f + MathF.Sin(AITimer * 0.5f) * 0.4f, 0.2f);
                        if (AITimer == 78)
                            MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<MagicChargeUp>(), 0, 0);
                    }
                    else
                    {
                        if (AITimer == 80)
                        {
                            SoundEngine.PlaySound(Sounds.reiTP.WithPitchOffset(-0.5f), NPC.Center);
                            for (float i = 0; i < 2; i++)
                            {
                                MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<WardenSigil>(), 0, 0, ai1: i, ai2: 2);
                            }
                        }
                        if (starRadius > 0)
                            starRadius -= Lerp(0, .4f, (AITimer - 80) / 15f);
                        if (AITimer > 95)
                        {
                            NPC.dontTakeDamage = false;
                            NPC.StrikeInstantKill();
                        }
                    }
                }
                break;
            case 0:
                {
                    if (NPC.dontTakeDamage)
                    {
                        starRadius = SmoothStep(0.5f, 2, AITimer / 120f);
                        NPC.velocity.Y -= Lerp(0.05f, 0, Helper.Saturate(AITimer / 60f));
                        if (AITimer > 60)
                            NPC.velocity *= 0.9f;
                        if (AITimer > 120)
                            NPC.dontTakeDamage = false;
                    }
                    starForcedPerspective = 0.75f;
                    NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                    NPC.TargetClosest(false);
                    float dist = player.Center.Distance(NPC.Center);
                    if ((!Main.dayTime && dist < 1050) || NPC.dontTakeDamage)
                    {
                        if (dist < 400) AITimer++;

                        starPerspectiveNormalizationLerpFactor = Lerp(starPerspectiveNormalizationLerpFactor, 0, 0.1f);
                        if (!NPC.dontTakeDamage)
                        {
                            starRadius = Lerp(starRadius, 1, 0.1f);

                            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.5f, 0.03f);
                            NPC.velocity.Y = Lerp(NPC.velocity.Y, Helper.FromAToB(NPC.Center, Helper.Raycast(NPC.Center, Vector2.UnitY, 200).Point - new Vector2(0, 100 + MathF.Sin(movementFreq * 0.03f) * 25), false).Y * 0.05f, 0.1f);
                        }
                        if (AITimer > 400)
                        {
                            if (MPUtils.NotMPClient)
                                SwitchState(AITimer3 == 0 ? 1 : Main.rand.Next(1, 4));
                        }
                    }
                    else
                        NPC.velocity.Y -= 0.5f;
                }
                break;
            case 1:
                {
                    starForcedPerspective = 0.75f;
                    AITimer++;
                    if (AITimer == 140)
                    {
                        SoundStyle style = SoundID.AbigailSummon;
                        style.Volume = 0.5f;
                        SoundEngine.PlaySound(style, NPC.Center);
                        Projectile a = MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<WardenSigil>(), 0, 0, ai2: 1);
                        if (a is not null) a.timeLeft = 450;
                        a?.SyncProjectile();
                    }
                    if (AITimer < 495)
                    {
                        float scale = Lerp(0, 6, InOutQuart.Invoke(Clamp(AITimer / 250f, 0, 1)));
                        additionalRotation += scale;
                        starPerspectiveNormalizationLerpFactor = Lerp(0, 1, InOutElastic.Invoke(Clamp(AITimer / 250f, 0, 1)));
                        starRadius = Lerp(starRadius, 0.7f + scale * 0.25f + MathF.Sin(Main.GlobalTimeWrappedHourly * scale) * 0.5f, 0.05f);

                        if (AITimer > 160 && AITimer < 430)
                        {
                            int interval = 7 - (int)MathF.Floor((AITimer - 160) / 60f);
                            if (AITimer > 200 ? AITimer % interval == 0 : AITimer == 165)
                            {
                                Vector2 pos = NPC.Center - new Vector2(NPC.direction * -300 + Main.rand.NextFloat(-300, 300), 700);
                                Vector2 vel = Vector2.UnitY.RotatedBy(NPC.direction * -0.3f + Main.rand.NextFloat(-0.02f, 0.02f));
                                MPUtils.NewProjectile(null, pos, vel * Main.rand.NextFloat(8, 15), ProjectileType<FallingStarTinyHostile>(), 15, 0);
                                MPUtils.NewProjectile(null, pos, vel, ProjectileType<FallingStarTinyHostile>(), 15, 0)?.Kill();
                            }
                        }
                        else
                            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                    }
                    else
                    {
                        starPerspectiveNormalizationLerpFactor = Lerp(starPerspectiveNormalizationLerpFactor, 0, 0.1f);
                        starRadius = Lerp(starRadius, 1, 0.1f);
                    }
                    NPC.velocity *= 0.9f;

                    if (AITimer > 540)
                        SwitchState(AITimer3 == 0 ? 2 : 0);
                }
                break;
            case 2:
                {
                    starForcedPerspective = 0.75f;
                    AITimer++;
                    NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                    if (AITimer > 40)
                    {
                        float scale = Lerp(0, 6, InOutQuart.Invoke(Clamp((AITimer - 40) / 100f, 0, 1)));
                        additionalRotation += scale;
                        starPerspectiveNormalizationLerpFactor = Lerp(starPerspectiveNormalizationLerpFactor, 1, 0.15f);
                        if (AITimer < 205)
                            starRadius = Lerp(starRadius, 0.7f + scale * 0.75f, 0.15f);
                        else
                            starRadius = Lerp(starRadius, 2, 0.15f);

                        NPC.velocity *= 0.9f;
                        if (AITimer < 200 && AITimer % 5 == 0)
                        {
                            MPUtils.NewProjectile(null, NPC.Center + new Vector2(4 * NPC.direction, 0), Vector2.Zero, ProjectileType<MagicChargeUp>(), 0, 0, ai1: Lerp(0, 2, (AITimer - 40) / 60f), ai2: 2);
                        }
                        if (AITimer == 202)
                            SoundEngine.PlaySound(Sounds.reiTP.WithPitchOffset(-0.7f), NPC.Center);
                        if (AITimer >= 200 && AITimer < 220 && AITimer % 2 == 0)
                        {
                            MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<WardenSigil>(), 0, 0, ai1: Lerp(0f, 2, (AITimer - 200) / 20f), ai2: 2);
                            if (AITimer == 210)
                            {
                                Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(100, 0) - new Vector2(0, 700);
                                Vector2 vel = player.Center.Y > pos.Y + 200 ? Helper.FromAToB(pos, player.Center) : Vector2.UnitY.RotatedByRandom(PiOver4);
                                MPUtils.NewProjectile(null, pos, vel * Main.rand.NextFloat(25, 35), ProjectileType<FallingStarBigHostile>(), 15, 0);
                                MPUtils.NewProjectile(null, pos, vel * Main.rand.NextFloat(25, 35), ProjectileType<FallingStarTinyHostile>(), 15, 0)?.Kill();
                            }
                        }
                        if (AITimer > 240)
                        {
                            AITimer2++;
                            if (AITimer2 > 2)
                                SwitchState(AITimer3 == 0 ? 3 : 0);
                            else
                                AITimer = 100;
                        }
                    }
                    else
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 3, 0.01f);
                }
                break;
            case 3:
                {
                    NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
                    starForcedPerspective = 0;

                    AITimer++;
                    if (AITimer < 80)
                    {
                        if (AITimer < 40)
                        {
                            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 5f, 0.03f);
                            NPC.velocity.Y = Lerp(NPC.velocity.Y, Helper.FromAToB(NPC.Center, Helper.Raycast(NPC.Center, Vector2.UnitY, 200).Point - new Vector2(0, 100 + MathF.Sin(movementFreq * 0.03f) * 25), false).Y * 0.05f, 0.3f);
                        }
                        else
                            NPC.velocity *= 0.9f;

                        additionalRotation += Lerp(0, 5, AITimer / 80f);
                        starPerspectiveNormalizationLerpFactor = Lerp(starPerspectiveNormalizationLerpFactor, 1f, AITimer / 80f);
                        starRadius = Lerp(starRadius, 5f, InOutCirc.Invoke(AITimer / 80f));
                    }
                    else
                    {
                        if (AITimer < 130)
                        {
                            if (AITimer == 85)
                                SoundEngine.PlaySound(Sounds.reiTP.WithPitchOffset(-0.7f), NPC.Center);
                            savedPos = player.Center;
                            additionalRotation += Lerp(5, 0, (AITimer - 80) / 50f);
                            if (AITimer < 105 && AITimer % 2 == 0)
                            {
                                float t = (AITimer - 80) / 25f;
                                Projectile.NewProjectile(null, NPC.Center + new Vector2(0, Lerp(0, -100, t)), -Vector2.UnitY * Lerp(-1, 20, t), ProjectileType<WardenSigil>(), 0, 0, ai1: 1, ai2: AITimer == 80 ? 3 : 4);
                            }
                            starRadius = Lerp(starRadius, 1, InOutCirc.Invoke((AITimer - 80) / 50f));
                        }
                        else
                        {
                            if (AITimer == 130 || AITimer == 140)
                            {
                                for (int i = -3; i < 4; i++)
                                {
                                    if (AITimer == 130)
                                        MPUtils.NewProjectile(null, savedPos + new Vector2(i * 400, MathF.Abs(i) * -20), -Vector2.UnitY, ProjectileType<WardenSigil>(), 0, 0, ai1: 0.7f, ai2: 5);
                                    else
                                        MPUtils.NewProjectile(null, savedPos + new Vector2(i * 400, MathF.Abs(i) * -20 - 400), Vector2.UnitY * 5, ProjectileType<WardenSigil>(), 0, 0, ai1: 0.7f, ai2: 5);
                                }
                            }
                            if (AITimer > 160)
                                starPerspectiveNormalizationLerpFactor = Lerp(starPerspectiveNormalizationLerpFactor, 0, 0.1f);
                            if (AITimer > 160 && AITimer % 5 == 0 && AITimer < 230)
                            {
                                int side = 0;
                                if (AITimer % 20 == 0)
                                    side = 1;
                                if (AITimer % 20 == 5)
                                    side = 2;
                                if (AITimer % 20 == 10)
                                    side = 3;

                                if (side == 0)
                                {
                                    Vector2 pos = savedPos + new Vector2(0, -1000) + Main.rand.NextVector2Circular(30, 30);
                                    Vector2 vel = Vector2.UnitY.RotatedByRandom(0.05f) * 30;
                                    MPUtils.NewProjectile(null, pos, Vector2.Zero, ProjectileType<FallingStarTinyHostile>(), 15, 0)?.Kill();
                                    MPUtils.NewProjectile(null, pos, vel, ProjectileType<FallingStarBigHostile>(), 20, 0);
                                }
                                else
                                {
                                    for (int i = -1; i < (side == 0 ? 1 : 2); i++)
                                    {
                                        if (i == 0) continue;
                                        for (int j = 0; j < 2; j++)
                                        {
                                            Vector2 pos = savedPos + new Vector2(i * 400 * side, MathF.Abs(i * side) * -20 - 1000) + Main.rand.NextVector2Circular(30, 30);
                                            Vector2 vel = Vector2.UnitY.RotatedByRandom(0.05f) * Main.rand.NextFloat(10, 20);
                                            MPUtils.NewProjectile(null, pos, Vector2.Zero, ProjectileType<FallingStarTinyHostile>(), 15, 0)?.Kill();
                                            MPUtils.NewProjectile(null, pos, vel, ProjectileType<FallingStarTinyHostile>(), 20, 0);
                                        }
                                    }
                                }
                            }
                            if (AITimer > 250)
                            {
                                SwitchState(0);
                                AITimer3 = 1;
                            }
                        }
                    }
                }
                break;
        }
    }
    public override void Reset()
    {
        AITimer = 0;
        AITimer2 = 0;
        NPC.netUpdate = true;
    }
}
