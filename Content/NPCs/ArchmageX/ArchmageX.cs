using EbonianMod.Content.Bossbars;
using EbonianMod.Content.Buffs;
using EbonianMod.Common.Misc;
using EbonianMod.Common.UI.Dialogue;

using EbonianMod.Content.Dusts;
using EbonianMod.Content.Items.Vanity;
using EbonianMod.Content.Items.BossTreasure;
using EbonianMod.Content.Items.Misc;
using EbonianMod.Content.Items.Pets;
using EbonianMod.Content.Items.Tiles.Trophies;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.Items.Weapons.Melee;
using EbonianMod.Content.Items.Weapons.Ranged;
using EbonianMod.Content.Items.Weapons.Summoner;
using EbonianMod.Content.Projectiles.ArchmageX;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Core.Systems.Boss;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Content.NPCs.ArchmageX;

public class PebbleDropRule : IItemDropRuleCondition, IProvideItemConditionDescription
{
    public bool CanDrop(DropAttemptInfo info) => info.player.HasBuff<Sheepened>();

    public bool CanShowItemDropInUI() => true;

    public string GetConditionDescription() => (string) null;
}

[AutoloadBossHead]
public class ArchmageX : CommonNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/ArchmageX/"+Name;
    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Position = new Vector2(0, 30)
        });
    }
    public override void SetDefaults()
    {
        NPC.Size = new Vector2(50, 78);
        NPC.lifeMax = 9500;
        if (Main.expertMode)
            NPC.lifeMax = 11000;
        if (Main.masterMode)
            NPC.lifeMax = 13500;
        NPC.defense = 6;
        NPC.damage = 0;
        NPC.boss = true;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0;
        NPC.lavaImmune = true;
        NPC.dontTakeDamage = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.BossBar = GetInstance<XBar>();
        NPC.netAlways = true;
        NPC.value = Item.buyPrice(0, 30);
        Music = 0;
    }
    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.lifeMax = (int)(NPC.lifeMax * balance * bossAdjustment);
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Xareus"),
            new FlavorTextBestiaryInfoElement(NPC.BestiaryKey()),
        });
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        LeadingConditionRule pebble = new LeadingConditionRule(new PebbleDropRule());
        pebble.OnSuccess(new CommonDrop(ItemType<TheRock>(), 1));
        npcLoot.Add(pebble);
        
        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
        notExpertRule.OnSuccess(new CommonDrop(ItemType<PhantasmalGreatsword>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<XareusPotion>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<StaffofXWeapon>(), 4));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<ArchmageXTome>(), 4));
        npcLoot.Add(notExpertRule);

        npcLoot.Add(ItemDropRule.Common(ItemType<XTrophy>(), 4));
        npcLoot.Add(ItemDropRule.Common(ItemType<XMask>(), 4));
        npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<SheepPet>(), 4));
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<XRelic>()));

        npcLoot.Add(ItemDropRule.BossBag(ItemType<ArchmageBag>()));
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy)
        {
            AITimer++;
            headYOff = Lerp(headYOff, MathF.Sin((AITimer + headOffIncrementOffset) * 0.025f) * 2, 0.2f);
        }
        Texture2D tex = TextureAssets.Npc[Type].Value;
        Texture2D singularArm = Assets.ExtraSprites.ArchmageX.ArchmageX_Arm.Value;
        Texture2D head = Assets.ExtraSprites.ArchmageX.ArchmageX_Head.Value;
        Texture2D headGlow = Assets.ExtraSprites.ArchmageX.ArchmageX_HeadGlow.Value;
        Texture2D manaPot = TextureAssets.Projectile[ProjectileType<XManaPotion>()].Value;
        Texture2D staff = Assets.ExtraSprites.ArchmageX.StaffOfXItem.Value;
        Texture2D bigStaff = TextureAssets.Item[ItemType<StaffOfX>()].Value;
        Texture2D heli = Assets.ExtraSprites.ArchmageX.ArchmageXHeli.Value;
        Texture2D heliGlow = Assets.ExtraSprites.ArchmageX.ArchmageXHeli_Glow.Value;

        Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 0.25f;
        if (NPC.direction == 1)
            staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -10) + (rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction)).ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -.25f;
        float staffRot = rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);


        Vector2 heliP = NPC.Center - new Vector2(singularArm.Width + (NPC.direction == -1 ? 36 : 46), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * .87f - MathHelper.PiOver4 * 0.1f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4) + MathHelper.PiOver4 * 0.5f)) - new Vector2(0, NPC.direction == 1 ? 18 : 12);
        float heliR = Helper.FromAToB(NPC.Center, heliP).ToRotation() + MathHelper.PiOver2 * (NPC.direction == -1 ? 1.1f : 0.8f);

        spriteBatch.Draw(staff, staffP + NPC.GFX() - screenPos, null, Color.White * staffAlpha, staffRot, new Vector2(0, staff.Height), NPC.scale, SpriteEffects.None, 0f);

        spriteBatch.Draw(bigStaff, staffP + NPC.GFX() - screenPos, null, Color.White * bigStaffAlpha, staffRot, new Vector2(0, bigStaff.Height), NPC.scale, SpriteEffects.None, 0f);


        Vector2 scale = new Vector2(1f, 0.25f);
        Main.spriteBatch.Reload(BlendState.Additive);

        if (bigStaffBloomAlpha > 0)
            for (int i = 0; i < 6; i++)
                spriteBatch.Draw(bigStaff, staffP + NPC.GFX() - screenPos, null, Color.White * bigStaffBloomAlpha, staffRot, new Vector2(0, bigStaff.Height), NPC.scale, SpriteEffects.None, 0f);

        Vector4 col = (Color.White * heliAlpha).ToVector4();
        if (heliAlpha > 0)
        {
            Main.spriteBatch.Reload(Effects.SpriteRotation.Value);
            Effects.SpriteRotation.Value.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 1200));
            Effects.SpriteRotation.Value.Parameters["scale"].SetValue(scale);
            col.W = heliAlpha * 0.25f;
            Effects.SpriteRotation.Value.Parameters["uColor"].SetValue(col);


            for (int i = 12; i > 0; i--)
            {
                Vector2 pos = heliP + new Vector2(i * 0.2f, 0).RotatedBy(heliR + MathHelper.PiOver2);
                Main.spriteBatch.Draw(heliGlow, pos + NPC.GFX() - Main.screenPosition, null, Color.White * heliAlpha, heliR, heli.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);
        }
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        if (heliAlpha > 0)
        {
            Main.spriteBatch.Reload(Effects.SpriteRotation.Value);
            Effects.SpriteRotation.Value.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 1200));
            Effects.SpriteRotation.Value.Parameters["scale"].SetValue(scale);
            col.W = heliAlpha;
            Effects.SpriteRotation.Value.Parameters["uColor"].SetValue(col);
            for (int i = 12; i > 0; i--)
            {
                Vector2 pos = heliP + new Vector2(i * 0.2f, 0).RotatedBy(heliR + MathHelper.PiOver2);
                Main.spriteBatch.Draw(heli, pos + NPC.GFX() - Main.screenPosition, null, Color.White * heliAlpha, heliR, heli.Size() / 2, 1f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Reload(effect: null);
        }


        spriteBatch.Draw(manaPot, NPC.Center + NPC.GFX() - new Vector2(singularArm.Width + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18) - screenPos, null, Color.White * 0.9f * manaPotAlpha, 0, manaPot.Size() / 2, NPC.scale, SpriteEffects.None, 0f);

        spriteBatch.Draw(singularArm, NPC.Center + NPC.GFX() - new Vector2(NPC.direction == -1 ? -14 : -6, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, leftArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * -NPC.direction), new Vector2(NPC.direction == 1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        spriteBatch.Draw(singularArm, NPC.Center + NPC.GFX() - new Vector2(singularArm.Width - 2 + (NPC.direction == -1 ? 4 : 0), 0) - new Vector2(NPC.direction == 1 ? -42 : -24, 18).RotatedBy(NPC.rotation) - screenPos, null, drawColor, rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) - (NPC.direction == -1 ? MathHelper.PiOver4 * 0.5f : 0), new Vector2(NPC.direction == -1 ? singularArm.Width : 0, 0), NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        spriteBatch.Draw(tex, NPC.Center + NPC.GFX() - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        spriteBatch.Draw(head, NPC.Center + NPC.GFX() + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, drawColor, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        spriteBatch.Draw(headGlow, NPC.Center + NPC.GFX() + new Vector2(NPC.direction == -1 ? 6 : 12, -38 + headYOff * 0.5f).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
        spriteBatch.Draw(headGlow, NPC.Center + NPC.GFX() + new Vector2(NPC.direction == -1 ? 6 : 12, -38).RotatedBy(NPC.rotation) - screenPos, headFrame, Color.White, headRotation, new Vector2(36, 42) / 2, NPC.scale, NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);

        return false;
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        float arenaLength = GetArenaRect().Size().Length();
        if (arenaLength is > 1100 or < 900) return;
        arenaAlpha = MathHelper.Lerp(arenaAlpha, 1, 0.1f);

        arenaVFXOffset += 0.005f;
        if (arenaVFXOffset >= 1)
            arenaVFXOffset = 0;
        arenaVFXOffset = MathHelper.Clamp(arenaVFXOffset, float.Epsilon, 1 - float.Epsilon);
        List<VertexPositionColorTexture> verticesR = new List<VertexPositionColorTexture>();
        Texture2D texture = Assets.Extras.wavyLaser2.Value;
        Vector2 startR = GetArenaRect().BottomRight() + Vector2.UnitY * 16 - Main.screenPosition;
        Vector2 offVert = (Helper.FromAToB(GetArenaRect().BottomRight() + Vector2.UnitY * 16, GetArenaRect().BottomRight() - Vector2.UnitY * 16 * 6, false));
        float rotVert = Helper.FromAToB(startR, startR + offVert).ToRotation();
        float s = 0f;
        float sLin = 0f;
        for (float i = 0; i < 1; i += 0.001f)
        {
            if (i < 0.5f)
                s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
            else
                s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

            if (i < 0.5f)
                sLin = MathHelper.Clamp(i, 0, 0.5f);
            else
                sLin = MathHelper.Clamp((-i + 1), 0, 0.5f);

            float cA = MathHelper.Lerp(s, sLin, i);
            float vertSize = MathHelper.SmoothStep(5, 18, s);

            Color col = new Color(60, 2, 113) * (arenaAlpha * s);
            verticesR.Add(Helper.AsVertex(startR - new Vector2(-vertSize * 0.5f, vertSize) + offVert * i + new Vector2(vertSize, 0).RotatedBy(rotVert + MathHelper.PiOver2), new Vector2(arenaVFXOffset, 0), col * 2));
            verticesR.Add(Helper.AsVertex(startR - new Vector2(-vertSize * 0.5f, vertSize) + offVert * i + new Vector2(vertSize, 0).RotatedBy(rotVert - MathHelper.PiOver2), new Vector2(arenaVFXOffset, 1), col * 2));
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (verticesR.Count >= 3)
        {
            Helper.DrawTexturedPrimitives(verticesR.ToArray(), PrimitiveType.TriangleStrip, texture, false);
        }
        Rectangle rect = new Rectangle(GetArenaRect().X - (int)Main.screenPosition.X, GetArenaRect().Y - (int)Main.screenPosition.Y, GetArenaRect().Width, GetArenaRect().Height);

        for (int i = 0; i < 5; i++)
            Main.spriteBatch.Draw(Assets.Extras.pixel.Value, rect, Color.Indigo * arenaFlash);
        Main.spriteBatch.ApplySaved(sbParams);

    }
    float leftArmRot, rightArmRot;
    float headRotation, headYOff, headOffIncrementOffset;
    float manaPotAlpha, staffAlpha = 1f, bigStaffAlpha, bigStaffBloomAlpha, heliAlpha, arenaVFXOffset, arenaAlpha;
    Rectangle headFrame = new Rectangle(0, 0, 36, 42);
    private List<int> PlayersFighting = new List<int>();
    public void FacePlayer()
    {
        Player player = Main.player[NPC.target];
        NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
        NPC.spriteDirection = NPC.direction;
    }
    public override void OnSpawn(IEntitySource source)
    {
        sCenter = NPC.Center - new Vector2(0, 130);
    }

    const int
        NeutralFace = 0,
        ShockedFace = 1 * 42,
        SadFace = 2 * 42,
        DisappointedFace = 3 * 42,
        AngryFace = 4 * 42,
        LightSmirkFace = 5 * 42,
        SmirkFace = 6 * 42,
        VeryShockedFace = 7 * 42,
        BlinkingFace = 8 * 42,
        AssholeFace = 9 * 42;
    const int Phase2Transition = -5, Taunt = -4, Despawn = -3, Death = -2, Idle = -1, Spawn = 0,
        PhantasmalSpirit = 1, ShadowflamePuddles = 2, SpectralOrbs = 3, MagnificentFireballs = 4, SineLaser = 5, AmethystCloseIn = 6,
        HelicopterBlades = 7, GiantAmethyst = 8, Micolash = 9, TheSheepening = 10, ManaPotion = 11, PhantasmalBlast = 12, ShadowflameRift = 13,
        AmethystBulletHell = 14, AmethystStorm = 15, BullshitRain = 16, BONK = 17;
    public List<int> Phase1AttackPool = new List<int>()
    {
        ManaPotion,
        PhantasmalSpirit,
        AmethystCloseIn,
        TheSheepening,
        SpectralOrbs,
        SineLaser,
        AmethystBulletHell,
        Micolash,
        ShadowflamePuddles,
        MagnificentFireballs,
        AmethystStorm,
        //adding weight to the rng
        SineLaser,
        PhantasmalSpirit,
        ShadowflamePuddles,
        SpectralOrbs,
        MagnificentFireballs
    };
    public List<int> Phase2AttackPool = new List<int>()
    {
        ManaPotion,
        PhantasmalSpirit,
        AmethystCloseIn,
        GiantAmethyst,
        TheSheepening,
        SpectralOrbs,
        AmethystBulletHell,
        Micolash,
        ShadowflameRift,
        SineLaser,
        ShadowflamePuddles,
        MagnificentFireballs,
        HelicopterBlades,
        PhantasmalBlast,
        AmethystStorm,

    };
    public List<int> Phase3AttackPool = new List<int>()
    {
        ManaPotion,
        PhantasmalSpirit,
        AmethystCloseIn,
        GiantAmethyst,
        SineLaser,
        TheSheepening,
        SpectralOrbs,
        AmethystBulletHell,
        Micolash,
        ShadowflameRift,
        ShadowflamePuddles,
        MagnificentFireballs,
        HelicopterBlades,
        PhantasmalBlast,
        AmethystStorm,
        BONK,
        BullshitRain
    };
    public List<float> MeleeAttacks = new List<float>()
    {
        ManaPotion,
        PhantasmalSpirit,
        MagnificentFireballs,
        GiantAmethyst,
        AmethystStorm,
        HelicopterBlades,
        ShadowflamePuddles,
        SpectralOrbs,
        Micolash,
        AmethystBulletHell,
        AmethystCloseIn,

    };
    float Next = 1;
    float arenaFlash;
    int phaseMult;
    float oldAttack = Spawn;
    bool doneAttacksBefore;
    int blinkInterval;
    int frameBeforeBlink;
    SlotId helicopterSlot;
    int seedCounter;
    float swingProgress, lerpProg = 1;
    FloatingDialogueBox currentDialogue;
    public override void SendExtraAI(BinaryWriter writer)
    {
        if ((int)AIState == Spawn)
            writer.WriteVector2(sCenter);
        writer.Write((byte)Next);
        writer.Write((byte)phaseMult);
        writer.Write((byte)oldAttack);
        writer.Write(doneAttacksBefore);
        for (int i = 0; i < disposablePos.Length; i++)
            writer.WriteVector2(disposablePos[i]);

        writer.Write(seedCounter);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        if ((int)AIState == Spawn)
            sCenter = reader.ReadVector2();
        Next = reader.ReadByte();
        phaseMult = reader.ReadByte();
        oldAttack = reader.ReadByte();
        doneAttacksBefore = reader.ReadBoolean();
        for (int i = 0; i < disposablePos.Length; i++)
            disposablePos[i] = reader.ReadVector2();

        seedCounter = reader.ReadInt32();
    }
    void IdleAnimation()
    {
        if (NPC.collideY || NPC.velocity.Y.InRange(0))
        {
            rightArmRot = Utils.AngleLerp(rightArmRot, 0, 0.3f);
            leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
        }
        else
        {
            rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-NPC.velocity.Y * .5f), 0.1f);
            leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction + MathHelper.ToRadians(NPC.velocity.Y * .5f), 0.1f);
        }
    }
    void PickAttack()
    {
        bool phase2 = phaseMult >= 2;
        if (AIState != Taunt)
            oldAttack = AIState;
        if (doneAttacksBefore)
        {
            int attempts = 0;
            if (!phase2)
            {
                Next = Main.rand.Next(Phase1AttackPool);
                while (attempts++ < 10 && Next == AmethystStorm && oldAttack == Next)
                    Next = Main.rand.Next(Phase1AttackPool);
            }
            else
            {
                if (phaseMult == 3)
                {
                    Next = Main.rand.Next(Phase3AttackPool);
                    while (attempts++ < 10 && oldAttack == Next)
                        Next = Main.rand.Next(Phase3AttackPool);
                }
                else
                {
                    Next = Main.rand.Next(Phase2AttackPool);
                    while (attempts++ < 20 && oldAttack == Next)
                        Next = Main.rand.Next(Phase2AttackPool);
                }
            }
        }
        else
        {
            Next = oldAttack + 1 * (phaseMult == 3 ? -1 : 1);
            if (phaseMult == 3)
            {
                if (Next < PhantasmalSpirit)
                {
                    doneAttacksBefore = true;
                    PickAttack();
                    return;
                }
            }
            else
            {
                if (Next > (phase2 ? 15 : 11))
                {
                    doneAttacksBefore = true;
                    PickAttack();
                    return;
                }
            }
        }
        AIState = Idle;
        AITimer = Main.rand.Next(-40, 20);
        if (Main.rand.NextBool((int)(6 + phaseMult * 3)) && oldAttack != Spawn && phaseMult != 3)
        {
            AIState = Taunt;
            AITimer = 0;
        }
    }
    public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (ProjectileID.Sets.CultistIsResistantTo[projectile.type])
            modifiers.FinalDamage *= .85f;
    }
    public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
    {
        modifiers.CritDamage -= 0.4f;
        modifiers.FinalDamage *= (phaseMult == 3 ? .7f : .85f);
    }
    public override bool CheckDead()
    {
        MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ArchmageDeath>(), 0, 0);
        if (!GetInstance<XareusSystem>().downedXareus)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.type == NPCType<ArchmageStaffNPC>())
                {
                    npc.ai[2] = 2400;
                    npc.netUpdate = true;
                }
            }
            XareusSystem.xareusFightCooldown = 300;
            GetInstance<XareusSystem>().downedXareus = true;
        }
        else
        {
            XareusSystem.xareusFightCooldown = (int)GetInstance<EbonianServerConfig>().XareusDelay;
        }
        if (!Main.dedServ)
            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
            {
                sound.Stop();
            }
        return true;
    }
    public override void AI()
    {
        if (seedCounter == 0 && MPUtils.NotMPClient)
        {
            seedCounter = Main.rand.Next(200, 10000);
            NPC.netUpdate = true;
        }
        seedCounter++;
        NPC.dontTakeDamage = AIState == Spawn || AIState == Phase2Transition;
        bool phase2 = phaseMult >= 2;
        if (currentDialogue is not null)
        {
            currentDialogue.Center = NPC.Center - new Vector2(0, 80);
            currentDialogue.VisibleCenter = NPC.Center - new Vector2(0, 80);
        }
        arenaFlash = MathHelper.Lerp(arenaFlash, 0, 0.05f);
        if (arenaFlash.InRange(0, 0.01f))
            arenaFlash = 0;
        if (NPC.life < NPC.lifeMax / 3 + 1000)
        {
            if (phaseMult != 3 && AIState == Idle)
            {
                NPC.netUpdate = true;
                doneAttacksBefore = false;
                Next = BONK;
                NPC.defense += 7;
                phaseMult = 3;
            }
        }
        else if (NPC.life < NPC.lifeMax / 2 + 1600)
        {
            if (phaseMult < 2 && AIState != Taunt)
            {
                NPC.netUpdate = true;
                Next = 1;
                oldAttack = 0;
                Reset();
                NPC.defense += 8;
                AIState = Phase2Transition;
                doneAttacksBefore = false;
                phaseMult = 2;
            }
        }
        else if (NPC.life < NPC.lifeMax * 0.75f)
            phaseMult = 1;
        else
            phaseMult = 0;
        XareusSystem.xareusFightCooldown = 500;
        blinkInterval++;
        if (blinkInterval >= 170 && blinkInterval < 175)
        {
            if (headFrame.Y != BlinkingFace)
            {
                frameBeforeBlink = headFrame.Y;
            }
            if (frameBeforeBlink == DisappointedFace || frameBeforeBlink == NeutralFace)
                headFrame.Y = BlinkingFace;
        }
        if (blinkInterval == 176)
        {
            if (frameBeforeBlink == DisappointedFace || frameBeforeBlink == NeutralFace)
                headFrame.Y = frameBeforeBlink;
            blinkInterval = Main.rand.Next(-250, 10);
        }
        Player player = Main.player[NPC.target];
        if (!player.active || player.dead)
        {
            NPC.TargetClosest(false);
            player = Main.player[NPC.target];
            if (!player.active || player.dead)
            {
                if (AIState != Despawn)
                    Reset();
                AIState = Despawn;
            }
        }
        if (GetInstance<XareusSystem>().timesDiedToXareus <= 0)
        {
            GetInstance<XareusSystem>().timesDiedToXareus = 1;
        }
        if (GetArenaRect().Size().Length() > 100 && GetArenaRect().TopLeft().Distance(Vector2.Zero) > 300)
        {
            foreach (Player _pla in Main.ActivePlayers)
            {
                float dist = _pla.Distance(GetArenaRect().Center());
                if (dist < 1400)
                {
                    if (dist > 550 && PlayersFighting.Contains(_pla.whoAmI))
                    {
                        MPUtils.NewProjectile(null, _pla.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                        MPUtils.NewProjectile(null, GetArenaRect().Center(), Vector2.Zero, ProjectileType<XExplosion>(),
                            0, 0);

                        if (Main.zenithWorld)
                        {
                            _pla.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 200, 1);

                            _pla.Male = !_pla.Male;
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(4, -1, -1, null, _pla.whoAmI);
                            }
                        }

                        Helper.TPNoDust(GetArenaRect().Center(), _pla);
                    }
                    else if (dist <= 550)
                    {
                        if (!PlayersFighting.Contains(_pla.whoAmI))
                            PlayersFighting.Add(_pla.whoAmI);
                        
                        while (_pla.Center.X < GetArenaRect().X)
                            _pla.Center += Vector2.UnitX * 2;

                        while (_pla.Center.X > GetArenaRect().X + GetArenaRect().Width)
                            _pla.Center -= Vector2.UnitX * 2;

                        while (_pla.Center.Y < GetArenaRect().Y)
                            _pla.Center += Vector2.UnitY * 2;
                    }
                }
            }

            if (NPC.Distance(GetArenaRect().Center()) > 1200)
            {
                NPC.Center = GetArenaRect().Center();
            }
            else
            {
                while (NPC.Center.X < GetArenaRect().X)
                    NPC.Center += Vector2.UnitX * 2;

                while (NPC.Center.X > GetArenaRect().X + GetArenaRect().Width)
                    NPC.Center -= Vector2.UnitX * 2;

                while (NPC.Center.Y < GetArenaRect().Y)
                    NPC.Center += Vector2.UnitY * 2;
            }
        }

        if (NPC.velocity.Y.InRange(0) && !NPC.velocity.X.InRange(0, 0.1f) && !NPC.noGravity)
        {
            int interval = 6;
            float len = NPC.velocity.Length();
            if (len > 10)
                interval = 1;
            else if (len > 8)
                interval = 2;
            else if (len > 6)
                interval = 3;
            else if (len > 4)
                interval = 4;
            else if (len > 2)
                interval = 2;
            if (AITimer % interval == 0)
                Dust.NewDust(Helper.Raycast(NPC.BottomLeft, Vector2.UnitY, 700, true).Point, NPC.width, 1, DustType<SparkleDust>(), newColor: Color.Indigo * 0.75f, Scale: 0.1f);
        }

        if (NPC.direction != NPC.oldDirection)
            rightArmRot = 0;
        float rightHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver4 : MathHelper.PiOver2);
        float leftHandOffsetRot = MathHelper.Pi - (NPC.direction == 1 ? MathHelper.PiOver2 : MathHelper.PiOver4);
        /*Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 6;
        if (NPC.direction == 1)
            staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -2) + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -6;

        float staffRot = rightArmRot + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);*/

        Vector2 staffP = NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.Pi - MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * 0.25f;
        if (NPC.direction == 1)
            staffP = NPC.Center + new Vector2(NPC.width / 2 - 4, -10) + (rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction)).ToRotationVector2().RotatedBy(MathHelper.Pi + MathHelper.PiOver4 * 0.8f + (MathHelper.ToRadians((headYOff + 2) * 4) * NPC.direction)) * -.25f;
        float staffRot = rightArmRot + (MathHelper.ToRadians((headYOff + 2) * 5) * NPC.direction) + MathHelper.Pi * (NPC.direction == 1 ? .5f : AIState == HelicopterBlades ? .8f : 1f);

        Vector2 staffTip = staffP + staffRot.ToRotationVector2().RotatedBy(-MathHelper.PiOver4) * 58;

        Lighting.AddLight(staffTip, TorchID.Purple);
        AITimer++;
        headYOff = Lerp(headYOff, MathF.Sin((AITimer + headOffIncrementOffset) * 0.05f) * 2 * Lerp(2, 0, (float)NPC.life / NPC.lifeMax), 0.2f);
        bigStaffBloomAlpha = Lerp(bigStaffBloomAlpha, 0, 0.1f);
        if (bigStaffBloomAlpha.InRange(0, 0.05f)) bigStaffBloomAlpha = 0;
        switch (AIState)
        {
            case Despawn:
                {
                    rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-65f), 0.1f);
                    leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction + MathHelper.ToRadians(65f), 0.1f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    headFrame.Y = (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace);
                    if (AITimer == 40)
                    {
                        WeightedRandom<string> chat = new WeightedRandom<string>(seedCounter);
                        if (!phase2)
                        {
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn1").Value);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn2").Value);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn3").Value);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn4").Value);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn5").Value);
                            if (NPC.life > NPC.lifeMax - 2000)
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn6").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn7").Value);
                            }
                        }
                        else
                        {
                            if (phaseMult == 3)
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn8").Value);
                            else
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XDespawn9").Value);
                        }
                        currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer >= 100)
                    {
                        for (int i = 0; i < 15; i++)
                            MPUtils.NewProjectile(null, NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                        MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                        NPC.active = false;
                    }
                }
                break;
            case Spawn:
                {
                    FacePlayer();
                    if (AITimer == 1)
                    {
                        if (!Main.dedServ)
                            helicopterSlot = SoundEngine.PlaySound(Sounds.helicopter.WithVolumeScale(0.01f));
                        NPC.dontTakeDamage = true;
                        if (XareusSystem.heardXareusIntroMonologue || GetInstance<XareusSystem>().downedXareus)
                            AITimer = 339;
                    }

                    if (!Main.dedServ)
                        if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
                        {
                            sound.Volume = 0;
                        }
                    if (AITimer < 50 || AITimer > 350)
                    {
                        rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-20f) * NPC.direction, 0.15f);
                        leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction + MathHelper.ToRadians(20f) * NPC.direction, 0.15f);
                    }
                    else IdleAnimation();

                    if (AITimer == 1)
                    {
                        MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                        MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                        SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
                    }
                    if (AITimer == 50)
                    {
                        headFrame.Y = AngryFace;
                        currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro1").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer == 150)
                    {
                        headFrame.Y = SadFace;
                        currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro2").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer == 350)
                    {
                        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/xareus");
                        headFrame.Y = AngryFace;
                        if (!XareusSystem.heardXareusIntroMonologue && !GetInstance<XareusSystem>().downedXareus)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro3").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>(seedCounter);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro4").Value);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro5").Value);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro6").Value);
                            if (GetInstance<XareusSystem>().downedXareus)
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro7").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro8").Value);
                            }
                            else
                            {
                                if (!Main.dayTime)
                                {
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro9").Value);
                                }
                                else
                                {
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XIntro10").Value);
                                }
                            }
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                    }
                    if (AITimer >= 500)
                    {
                        NPC.dontTakeDamage = false;
                        XareusSystem.heardXareusIntroMonologue = true;
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case Phase2Transition:
                {
                    if (AITimer == 1)
                    {
                        disposablePos[0] = GetArenaRect().Center();
                    }
                    if (AITimer < 340 || AITimer > 551)
                    {
                        if (AITimer < 1000)
                            FacePlayer();
                        NPC.dontTakeDamage = true;
                        if (AITimer < 340)
                            headFrame.Y = AngryFace;
                        else if (AITimer < 1100)
                            headFrame.Y = DisappointedFace;
                        else
                            headFrame.Y = BlinkingFace;
                        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, disposablePos[0], false).X * .03f;
                        rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-30f) * NPC.direction, 0.15f);
                        leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction + MathHelper.ToRadians(30f) * NPC.direction, 0.15f);
                    }
                    else
                    {
                        NPC.velocity *= 0.5f;
                        headFrame.Y = DisappointedFace;
                        if (AITimer >= 400 && AITimer < 551)
                        {
                            FacePlayer();
                            rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        }
                        else
                            IdleAnimation();
                    }
                    if (AITimer == 40)
                    {
                        MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XPhase2").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer == 100)
                    {
                        //arenaFlash = 1;
                        float off = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int i = 0; i < 4; i++)
                        {
                            float angle = Helper.CircleDividedEqually(i, 4) + off;
                            Vector2 vel = angle.ToRotationVector2();
                            MPUtils.NewProjectile(null, NPC.Center - new Vector2(0, 130), vel, ProjectileType<XRift>(), 15, 0, ai1: 1);
                        }
                    }
                    if (AITimer == 150)
                    {
                        foreach (Player _player in Main.ActivePlayers)
                        {
                            _player.AddBuff(BuffType<Sheepened>(), 1150);
                            for (float i = 0; i < 1; i += 0.01f)
                            {
                                Dust.NewDustPerfect(Vector2.Lerp(NPC.Center - new Vector2(0, 130), _player.Center, i), DustType<XGoopDust2>(), Helper.FromAToB(NPC.Center - new Vector2(0, 130), _player.Center).RotatedByRandom(PiOver2) * Main.rand.NextFloat(2));
                            }
                            MPUtils.NewProjectile(null, _player.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                            MPUtils.NewProjectile(null, _player.Center, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        }
                    }
                    if (AITimer == 300)
                    {
                        SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                        for (int i = 0; i < 5; i++)
                        {
                            if (i == 2) continue;
                            Vector2 _pos = GetArenaRect().Left();
                            Vector2 __pos = GetArenaRect().Right();
                            Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 4);
                            MPUtils.NewProjectile(null, Helper.Raycast(pos, Vector2.UnitY, GetArenaRect().Height + 10).Point, -Vector2.UnitY, ProjectileType<XRift>(), 15, 0);
                        }
                    }
                    if (AITimer >= 480 && AITimer < 541 && AITimer % 15 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), staffTip);
                        Vector2 vel = Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.2f);
                        NPC.damage = 30;
                        MPUtils.NewProjectile(null, staffTip + vel * 15f, vel * 7f, ProjectileType<XBolt>(), 15, 0);
                    }
                    else NPC.damage = 0;
                    if (AITimer == 570)
                    {
                        SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                        NPC.netUpdate = true;
                        for (int i = 1; i < 3; i++)
                        {
                            Vector2 pos = Main.rand.NextVector2FromRectangle(OffsetBoundaries(Vector2.One * 200));
                            if (MPUtils.NotMPClient)
                                disposablePos[i] = pos;
                            MPUtils.NewProjectile(null, pos, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                            MPUtils.NewProjectile(null, pos, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                        }

                    }
                    if (AITimer == 600)
                    {
                        SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                        MPUtils.NewProjectile(null, disposablePos[1], Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        float off = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int j = 0; j < 4; j++)
                        {
                            float angle = Helper.CircleDividedEqually(j, 4) + off;
                            MPUtils.NewProjectile(null, disposablePos[1], angle.ToRotationVector2() * (1 + j * 0.5f), ProjectileType<XBolt>(), 20, 0);
                        }
                    }
                    if (AITimer == 630)
                    {
                        SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                        MPUtils.NewProjectile(null, disposablePos[2], Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        float off = Main.rand.NextFloat(MathHelper.TwoPi);
                        for (int j = 0; j < 8; j++)
                        {
                            float angle = Helper.CircleDividedEqually(j, 8) + off;
                            MPUtils.NewProjectile(null, disposablePos[2], angle.ToRotationVector2() * (3 + j * 0.5f), ProjectileType<XBolt>(), 20, 0);
                        }
                    }
                    if (AITimer == 660)
                    {
                        MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer);
                        MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer);
                        MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(0, -150), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer);
                    }
                    if (AITimer > 800 && AITimer < 1000 && AITimer % 50 == 0)
                    {
                        MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(250, -100), Helper.FromAToB(GetArenaRect().Center() + new Vector2(250, -100), player.Center).RotatedByRandom(PiOver4), ProjectileType<XBolt>(), 0, 0);
                        MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(-250, -100), Helper.FromAToB(GetArenaRect().Center() + new Vector2(-250, -100), player.Center).RotatedByRandom(PiOver4), ProjectileType<XBolt>(), 0, 0);
                        MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(0, -150), Helper.FromAToB(GetArenaRect().Center() + new Vector2(0, -150), player.Center).RotatedByRandom(PiOver4), ProjectileType<XBolt>(), 0, 0);
                    }
                    if (AITimer == 1030)
                    {
                        MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<SheepeningOrb>(), 1, 0,-1, player.whoAmI);
                    }
                    if (AITimer == 1110)
                        currentDialogue = DialogueSystem.NewDialogueBox(160, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XPhase2End").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 2.3f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    if (AITimer >= 1260)
                    {
                        Reset();
                        NPC.dontTakeDamage = false;
                        oldAttack = 0;
                        Next = 1;
                        AIState = 1;
                    }
                }
                break;
            case Taunt:
                {
                    if (AITimer < 80 + (NPC.life * 0.01f))
                    {
                        headFrame.Y = phase2 ? (NPC.life < NPC.lifeMax * 0.1f ? AngryFace : SmirkFace) : LightSmirkFace;
                        rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-20f) * NPC.direction, 0.15f);
                        leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction + MathHelper.ToRadians(20f) * NPC.direction, 0.15f);
                    }
                    else
                    {
                        headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                        IdleAnimation();
                    }
                    FacePlayer();
                    AITimer2--;

                    NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    if (AITimer == 40)
                    {
                        WeightedRandom<string> chat = new WeightedRandom<string>(seedCounter);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt1").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt2").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt3").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt4").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt5").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt6").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt7").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt8").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt9").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt10").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt11").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt12").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt13").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt14").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt15").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt16").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt17").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt18").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt19").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt20").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt21").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt22").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt23").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt24").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt25").Value);
                        if (NPC.HasBuff(BuffID.OnFire) || NPC.HasBuff(BuffID.OnFire3))
                        {
                            if (phase2)
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt26").Value, 1.4f);

                            }
                            else
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt27").Value, 1.4f);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt28").Value, 1.4f);
                            }
                        }
                        if (phase2)
                        {
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt29").Value);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt30").Value);
                        }
                        if (player.HeldItem is not null)
                        {
                            if (player.HeldItem.DamageType == DamageClass.Magic)
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt31").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt32").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt33").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt34").Value, 1.5);
                                if (Item.staff[player.HeldItem.type])
                                {
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt35").Value, 1.5);
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt36").Value, 1.5);
                                }
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt37").Value, 1.5);
                            }
                            if (player.HeldItem.DamageType == DamageClass.Melee)
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt38").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt39").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt40").Value, 1.5);
                                if (phase2)
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt41").Value, 1.5);
                            }
                            if (player.HeldItem.DamageType == DamageClass.Ranged)
                            {
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt42").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt43").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt44").Value, 1.5);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt45").Value, 1.5);
                                if (player.HeldItem.useAmmo == AmmoID.Bullet)
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt46").Value, 1.5);
                                if (player.HeldItem.useAmmo == AmmoID.Arrow)
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt47").Value, 1.5);
                                if (player.HeldItem.useAmmo == AmmoID.Rocket && player.ChooseAmmo(player.HeldItem).type == ItemID.MiniNukeI)
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt48").Value, 3);
                                if (phase2)
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt49").Value, 1.5);
                            }
                            if (player.HeldItem.DamageType == DamageClass.SummonMeleeSpeed)
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt50").Value, 1.5);
                        }
                        if (player.numMinions > 0)
                        {
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt51").Value, 1.5);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt52").Value, 1.5);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt53").Value, 1.5);
                            chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt54").Value, 1.5);
                            if (player.ownedProjectileCounts[ProjectileID.BabySlime] > 1)
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt55").Value, 1.5);

                            if (player.ownedProjectileCounts[ProjectileID.BabyBird] > 1)
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XTaunt56").Value, 1.5);
                        }
                        currentDialogue = DialogueSystem.NewDialogueBox(120, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer >= 80 + (NPC.life * 0.01f))
                    {
                        Reset();
                        AIState = Next;
                    }
                }
                break;
            case Idle:
                {
                    IdleAnimation();
                    //
                    if (MeleeAttacks.Contains(Next) && !(Next == HelicopterBlades && phaseMult == 3))
                    {
                        FacePlayer();

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 150, false).X * (phaseMult == 3 ? 0.09f : .02f), (phaseMult == 3 ? 0.15f : 0.1f));
                    }
                    else
                    {
                        if (AITimer == 1)
                        {
                            NPC.netUpdate = true;
                            if (MPUtils.NotMPClient)
                                disposablePos[0] = Main.rand.NextVector2FromRectangle(GetArenaRect());
                        }
                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[0].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                        if (NPC.Distance(pos) < 70)
                            AITimer++;

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * (phaseMult == 3 ? 0.07f : .01f);
                        FacePlayer();
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    }
                    AITimer2--;

                    NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);

                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    if (AITimer >= 35 + (NPC.life * 0.0035f))
                    {
                        Reset();
                        AIState = Next;
                    }
                }
                break;
            case PhantasmalSpirit:
                {
                    FacePlayer();
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 40)
                    {
                        if (oldAttack != AIState)
                        {
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack1.Angry").Value :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack1.Default").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack1.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }



                    if (AITimer < 135) rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                    else rightArmRot = Utils.AngleLerp(rightArmRot, 0, 0.2f);

                    if ((AITimer < 65 && AITimer > 50) || (AITimer < 125 && AITimer > 100))
                    {
                        headFrame.Y = ShockedFace;
                        Vector2 pos = staffTip + Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                        Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(staffTip, player.Center).RotatedByRandom(PiOver4) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                    }
                    NPC.velocity.X += -NPC.direction * .05f;
                    NPC.velocity.X *= 0.95f;
                    if (phase2 ? (AITimer >= 75 && AITimer % 20 == 0 && (phaseMult == 3 ? true : AITimer != 105)) : (AITimer == 75 || AITimer == 95 || AITimer == 135 || (phaseMult == 1 ? AITimer == 160 : false)))
                    {
                        NPC.velocity.X += -NPC.direction * 5;
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                        SoundEngine.PlaySound(Sounds.xSpirit, staffTip);
                        NPC.damage = 30;
                        Helper.AddCameraModifier(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 pos = NPC.Center + Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            if (i % 2 == 0)
                                Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(staffTip, player.Center).RotatedByRandom(PiOver4) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            else
                                Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(staffTip, player.Center).RotatedByRandom(PiOver4) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        if (!phase2)
                            MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * (phaseMult == 3 ? 4 : 5f), ProjectileType<XSpirit>(), 15, 0, Main.myPlayer, player.whoAmI);
                        else
                            MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.25f) * 6f, ProjectileType<XSpirit>(), 15, 0, Main.myPlayer, player.whoAmI);
                    }
                    else
                        NPC.damage = 0;

                    if (phaseMult == 3 && AITimer % 40 == 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            if (i == 0) continue;
                            MPUtils.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center).RotatedBy(i * 0.3f), ProjectileType<XBolt>(), 15, 0);
                        }
                    }
                    if (AITimer >= 150)
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case ShadowflamePuddles:
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;

                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 1)
                        AITimer2 = Main.rand.Next(4);
                    if (AITimer == 40)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack2.Angry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack2.Default").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack2.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    if (AITimer < 180 && AITimer > 1)
                    {
                        rightArmRot = Utils.AngleLerp(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                    }
                    else
                    {
                        FacePlayer();
                        rightArmRot = Utils.AngleLerp(rightArmRot, 0, 0.2f);
                    }


                    if (AITimer2 > 0 && phaseMult != 3)
                    {
                        if (AITimer >= 60 && AITimer <= 180 && AITimer % (phase2 ? 8 : (phaseMult == 1 ? 20 : 25)) == 0)
                        {
                            for (int i = 0; i < 5; i++)
                                Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                            Vector2 pos = new Vector2(MathHelper.Clamp(GetArenaRect().X + GetArenaRect().Width * Main.rand.NextFloat(), GetArenaRect().X + 32, GetArenaRect().X + GetArenaRect().Width - 32), NPC.Center.Y);
                            MPUtils.NewProjectile(null, pos, AITimer % (phase2 ? (phaseMult == 3 ? 20 : 30) : 50) == 0 ? Vector2.UnitY : -Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0);
                        }
                    }
                    else // phase 2
                    {
                        if (AITimer == 100)
                        {
                            for (int i = 0; i < 5; i++)
                                Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                            for (int i = 0; i < 7; i++)
                            {
                                Vector2 _pos = GetArenaRect().Left() + new Vector2(32, 0);
                                Vector2 __pos = GetArenaRect().Right() + new Vector2(-32, 0);
                                Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 6);
                                if (Helper.Raycast(pos, -Vector2.UnitY, GetArenaRect().Height + 10).RayLength <= GetArenaRect().Height)
                                {
                                    MPUtils.NewProjectile(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-50, 50) * Vector2.UnitX : Vector2.Zero), Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0, ai0: 1, ai1: 2);
                                }
                                MPUtils.NewProjectile(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-49, 49) * Vector2.UnitX : Vector2.Zero), -Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0, ai0: 1, ai1: 2);

                            }
                        }
                        if (AITimer == 200 && phaseMult == 3)
                        {
                            for (int i = 0; i < 5; i++)
                                Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                            for (int i = 0; i < 7; i++)
                            {
                                Vector2 _pos = GetArenaRect().Left();
                                Vector2 __pos = GetArenaRect().Right();
                                Vector2 pos = Vector2.Lerp(_pos + new Vector2(40, 0), __pos - new Vector2(16, 0), (float)i / 6);
                                if (Helper.Raycast(pos, -Vector2.UnitY, GetArenaRect().Height + 10).RayLength <= GetArenaRect().Height)
                                {
                                    MPUtils.NewProjectile(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-50, 50) * Vector2.UnitX : Vector2.Zero), Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0, ai0: 1, ai1: 2);

                                }
                                MPUtils.NewProjectile(null, pos + (i > 1 && i < 6 && phaseMult == 3 ? Main.rand.NextFloat(-49, 49) * Vector2.UnitX : Vector2.Zero), -Vector2.UnitY, ProjectileType<XShadowflame>(), 15, 0, ai0: 1, ai1: 2);

                            }
                        }
                    }
                    if (AITimer >= (phaseMult == 3 ? 160 : 260))
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case SpectralOrbs:
                {
                    FacePlayer();
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f); rightArmRot = Utils.AngleLerp(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                    if (AITimer == 30)
                        if (oldAttack != AIState && oldAttack != AmethystBulletHell)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack3.Angry").Value :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack3.Default").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                    if (AITimer == 50)
                    {
                        SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                        float off = Main.rand.NextFloat(MathHelper.Pi * 2);
                        for (int i = 0; i < 3 + (AITimer / 60); i++)
                        {
                            float angle = Helper.CircleDividedEqually(i, 3 + (AITimer / 60)) + off;
                            Projectile a = MPUtils.NewProjectile(null, staffTip, Vector2.UnitX.RotatedBy(angle), ProjectileType<XAmethyst>(), 15, 0);
                            if (a is not null)
                            {
                                a.timeLeft = 100;
                                a.SyncProjectile();
                            }
                        }
                    }
                    if (AITimer >= (phaseMult == 3 ? 100 : 70) && AITimer <= (Main.expertMode ? 140 : 110) && AITimer % (phase2 ? (phaseMult == 3 ? 15 : 20) : (phaseMult == 1 ? 25 : 30)) == 0)
                    {
                        SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                        float off = Main.rand.NextFloat(MathHelper.Pi * 2);
                        for (int i = 0; i < 3 + (AITimer / 60); i++)
                        {
                            float angle = Helper.CircleDividedEqually(i, 3 + (AITimer / 60)) + off;
                            Projectile a = MPUtils.NewProjectile(null, staffTip, Vector2.UnitX.RotatedBy(angle), ProjectileType<XAmethyst>(), 15, 0);
                            if (a is not null)
                            {
                                a.timeLeft = 100;
                                a.SyncProjectile();
                            }
                        }
                    }

                    if (AITimer >= 150)
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case MagnificentFireballs:
                {
                    if (phaseMult == 3)
                    {
                        AITimer++;
                        headFrame.Y = AngryFace;
                    }
                    if (AITimer < 3)
                    {
                        if (AITimer == 2)
                            NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                    }
                    if (AITimer < 50)
                    {
                        if (AITimer == 1 || AITimer == 49) NPC.netUpdate = true;
                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[0].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        NPC.direction = disposablePos[0].X > NPC.Center.X ? -1 : 1;
                        NPC.spriteDirection = NPC.direction;
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    }
                    else
                        NPC.velocity.X *= 0.9f;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 30)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? (doneAttacksBefore ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack4.Angry").Value :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack4.FirstTimeAngry").Value) :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack4.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AgainLoud").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    if (AITimer == 80)
                    {
                        MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                    }
                    if (AITimer > 55)
                    {
                        FacePlayer();
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                    }
                    if (AITimer == 150 || AITimer == 220)
                    {
                        int attempts = 0;
                        MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        if (MPUtils.NotMPClient)
                        {
                            while (++attempts < 100 && (disposablePos[2].Distance(NPC.Center) < 250 || disposablePos[2].Distance(player.Center) < 350 || !Collision.CanHit(NPC.position, NPC.width, NPC.height, disposablePos[2] - NPC.Size / 2, NPC.width, NPC.height)))
                            {
                                Rectangle rect = GetArenaRect();
                                rect.Width -= 120;
                                rect.X += 60;
                                rect.Height -= 120;
                                rect.Y += 60;
                                disposablePos[2] = Main.rand.NextVector2FromRectangle(rect);
                            }
                            disposablePos[2] = Helper.Raycast(disposablePos[2], Vector2.UnitY, 1000, true).Point - new Vector2(0, NPC.height / 2 + 8);
                        }
                        MPUtils.NewProjectile(null, disposablePos[2], Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);

                    }
                    else if (AITimer is (> 150 and < 153) or (> 220 and < 223))
                    {
                        if (disposablePos[2].Distance(NPC.Center) < 3000)
                        {
                            NPC.Center = disposablePos[2];
                            NPC.netOffset *= 0;
                            NPC.netUpdate = true;
                        }
                    }

                    if ((AITimer <= 120 && AITimer >= 110) || AITimer == 160 || (AITimer <= 190 && AITimer >= 170) || (AITimer <= 250 && AITimer >= 240))
                    {
                        if (AITimer % (AITimer > 229 ? (phase2 ? (phaseMult == 3 ? 4 : 5) : (phaseMult == 1 ? 5 : 10)) : (phase2 ? (phaseMult == 3 ? 4 : 6) : 10)) == 0)
                        {
                            NPC.netUpdate = true;
                            for (int i = 0; i < 5; i++)
                                Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));
                            SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), staffTip);
                            Vector2 vel = Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4 * MathHelper.Lerp(1.25f + phaseMult * 0.05f, 0.65f, AITimer / 300) * (AITimer == 110 ? 0 : (AITimer > 229 ? 1.2f : 0.65f)));
                            MPUtils.NewProjectile(null, staffTip, vel * 7f, ProjectileType<XBolt>(), 15, 0);
                        }
                        NPC.damage = 30;
                    }
                    else
                        NPC.damage = 0;
                    if (AITimer == 330 && phaseMult == 3 && !doneAttacksBefore)
                    {
                        currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetTextValue("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack3.Angry"), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer > 350 && AITimer < 500 && phaseMult == 3)
                    {
                        rightArmRot = Utils.AngleLerp(rightArmRot, -2.9f * NPC.direction, 0.1f);
                        if (AITimer % 6 == 0)
                        {
                            MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0, 0);
                            MPUtils.NewProjectile(null, staffTip, Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.1f, 0.6f), ProjectileType<XBolt>(), 15, 0, 0);
                        }
                    }
                    if (AITimer >= (330 + (phaseMult == 3 ? 200 : 0)))
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case SineLaser:
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = SmirkFace;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 30)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack5.Angry").Value :
                                (phase2 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack5.Helix").Value :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack5.Wave").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mod.EbonianMod.Dialogue.ArchmageXDialogue.AgainLoud").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    if (AITimer < 50)
                    {
                        if (AITimer == 1 || AITimer == 49)
                            NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                        FacePlayer();
                    }
                    rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot + (AITimer >= 145 ? MathF.Sin(AITimer * 6) * 0.1f : 0), 0.2f);
                    Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                    if (AITimer == 53)
                    {
                        SoundEngine.PlaySound(Sounds.cursedToyCharge, NPC.Center);

                        for (int i = 0; i < 5; i++)
                            Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));

                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: (phaseMult == 3 ? 55 : 70));
                        if (phase2)
                            MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: -(phaseMult == 3 ? 55 : 70));
                        if (phaseMult == 3)
                            MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0);
                    }
                    if (AITimer == 92)
                    {
                        SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);

                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: (phaseMult == 3 ? 55 : 70));
                        if (phase2)
                            MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: -(phaseMult == 3 ? 55 : 70));
                        if (phaseMult == 3)
                            MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0);
                    }
                    if (AITimer > 110 && AITimer < 130)
                    {
                        if (AITimer == 111 || AITimer == 129)
                            NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                    }
                    if (AITimer == 135 && phaseMult == 3)
                    {
                        FacePlayer();
                        SoundEngine.PlaySound(Sounds.cursedToyCharge, NPC.Center);

                        for (int i = 0; i < 5; i++)
                            Dust.NewDustPerfect(staffTip, DustType<XGoopDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.White, Main.rand.NextFloat(.1f, .6f));


                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: 70);
                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: -70);
                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 0, 0);
                    }
                    if (AITimer == 172 && phaseMult == 3)
                    {
                        SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);

                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: 70);
                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: -70);
                        MPUtils.NewProjectile(null, staffTip, vel, ProjectileType<XSineLaser>(), 15, 0);
                    }
                    if (AITimer >= 130 + (phaseMult == 3 ? 80 : 0))
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case AmethystCloseIn:
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = SmirkFace;
                    rightArmRot = Utils.AngleLerp(rightArmRot, MathHelper.ToRadians(AITimer * 8f), 1);
                    NPC.direction = NPC.spriteDirection = ((NPC.Center + rightArmRot.ToRotationVector2().RotatedBy(MathHelper.PiOver4 * 0.5f) * 20).X > NPC.Center.X ? -1 : 1);
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 40)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? (doneAttacksBefore ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack6.Angry").Value :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack6.FirstTimeAngry").Value) : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack6.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack6.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                    if (AITimer % (phase2 ? (phaseMult == 3 ? 10 : 15) : (phaseMult == 1 ? 20 : 25)) == 0 && AITimer > 29 && AITimer < 91)
                    {
                        float off = (phase2 ? AITimer * 0.18f : Main.rand.NextFloat(MathHelper.TwoPi));
                        for (int i = 0; i < 5; i++)
                        {
                            float angle = Helper.CircleDividedEqually(i, 5) + off;
                            Vector2 pos = NPC.Center + new Vector2(phase2 ? 225 * phaseMult : 300, 0).RotatedBy(angle);
                            MPUtils.NewProjectile(null, pos, Helper.FromAToB(pos, NPC.Center) * 0.1f, ProjectileType<XAmethystCloseIn>(), 15, 0, -1, NPC.Center.X, NPC.Center.Y);
                        }
                        MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                    }

                    if (AITimer >= 130)
                    {
                        AITimer3 = 0;
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case AmethystBulletHell: //phase 2
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                    rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-45f) * NPC.direction, 0.15f);
                    if (AITimer == 1) disposablePos[0] = player.Center;
                    if (AITimer < 50)
                    {
                        AITimer3 = Main.rand.Next(short.MaxValue);
                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[0].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        FacePlayer();
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    }
                    else
                        NPC.velocity.X *= 0.9f;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    rightArmRot = Utils.AngleLerp(rightArmRot, 0, 0.2f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 40)
                        if (oldAttack != AIState && oldAttack != SpectralOrbs)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack7.Angry").Value :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack7.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                    if (AITimer > 60 && AITimer < 170)
                    {
                        if (AITimer % 15 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);
                            for (int i = 0; i < 15; i++)
                            {
                                UnifiedRandom rand = new UnifiedRandom((int)AITimer3);
                                Vector2 pos = rand.NextVector2FromRectangle(GetArenaRect());

                                int atts = 0;
                                while (++atts < 100 && pos.Distance(player.Center) < 70)
                                    pos = rand.NextVector2FromRectangle(GetArenaRect());
                                for (int j = 0; j < 5; j++)
                                {
                                    Dust.NewDustPerfect(pos, DustType<SparkleDust>(), rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));

                                    Dust.NewDustPerfect(staffTip, DustType<SparkleDust>(), rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                    Dust.NewDustPerfect(staffTip, DustType<LineDustFollowPoint>(), rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = pos;
                                }
                                break;
                            }
                        }
                    }
                    if (AITimer > 60 && AITimer % 20 == 0)
                    {
                        AITimer3++;
                        for (int i = 0; i < 15; i++)
                        {
                            UnifiedRandom rand = new UnifiedRandom((int)AITimer3);
                            Vector2 pos = rand.NextVector2FromRectangle(GetArenaRect());

                            int atts = 0;
                            while (++atts < 100 && pos.Distance(player.Center) < 70)
                                pos = rand.NextVector2FromRectangle(GetArenaRect());

                            MPUtils.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + rand.NextVector2Circular(150, 150)) * 0.1f, ProjectileType<XAmethystCloseIn>(), 15, 0);

                            if (phaseMult == 3)
                            {
                                MPUtils.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center) * 0.05f, ProjectileType<XAmethystCloseIn>(), 15, 0);
                            }
                        }
                    }
                    if (AITimer >= 200)
                    {
                        AITimer3 = 0;
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case GiantAmethyst:
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                    if (AITimer == 1)
                    {
                        NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                    }
                    if (AITimer < 50)
                    {
                        if (phaseMult == 1)
                        {
                            Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                            if (disposablePos[0].X < NPC.Center.X)
                                pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                                NPC.velocity.Y = -9.5f;
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                            NPC.direction = disposablePos[0].X > NPC.Center.X ? -1 : 1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                        }
                        else if (phaseMult == 2)
                        {
                            FacePlayer();
                            disposablePos[0] = GetArenaRect().Center.ToVector2() - new Vector2(0, 16);
                            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                            NPC.velocity.X = Helper.FromAToB(NPC.Center, disposablePos[0], false).X * .03f;
                            rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-30f) * NPC.direction, 0.15f);
                            leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction + MathHelper.ToRadians(30f) * NPC.direction, 0.15f);
                        }
                        else
                        {
                            rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);

                            headFrame.Y = AngryFace;
                            NPC.velocity *= 0.9f;
                            NPC.noGravity = false;
                            heliAlpha = MathHelper.Lerp(heliAlpha, 0, 0.25f);
                            Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(200, 200), DustType<LineDustFollowPoint>(), Helper.FromAToB(Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.Center) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.06f, .2f));
                            d.noGravity = true;
                            d.customData = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 20;
                        }
                    }
                    else
                        NPC.velocity.X *= 0.9f;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    rightArmRot = Utils.AngleLerp(rightArmRot, -Vector2.UnitY.ToRotation() - (NPC.direction == -1 ? (MathHelper.PiOver2 + MathHelper.PiOver4) : 0), 0.2f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 40)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack8.Angry").Value :
                                Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack8.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack8.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    if (phaseMult < 3)
                    {
                        if (AITimer >= 60 && phase2 && (int)AITimer % 35 == 0)
                        {
                            float off = Main.rand.NextFloat(Pi);
                            for (int i = 0; i < 3; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 3) + off;
                                MPUtils.NewProjectile(null, staffTip, angle.ToRotationVector2(), ProjectileType<XLargeAmethystAlt>(), 15, 0);
                            }
                        }
                        if (!phase2 && AITimer >= 60 && (int)AITimer % 25 == 0)
                        {
                            MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XLargeAmethyst>(), 15, 0, Main.myPlayer, player.whoAmI);

                            MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                        }

                    }
                    else
                    {
                        if (AITimer > 90 && AITimer % 25 == 0)
                        {
                            SoundEngine.PlaySound(SoundID.Shatter, staffTip);
                            for (int h = 0; h < 6; h++)
                            {
                                MPUtils.NewProjectile(null, staffTip, Main.rand.NextVector2Circular(7 + AITimer * 0.02f, 7 + AITimer * 0.02f), ProjectileType<XAmethystShard>(), 15, 0);
                            }

                            MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                            float off = Main.rand.NextFloat(Pi);
                            for (int i = 0; i < 5; i++)
                            {
                                Projectile a = MPUtils.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center).RotatedBy(Helper.CircleDividedEqually(i, 5) + off) * 0.25f, ProjectileType<XBolt>(), 15, 0);
                                if (a is not null)
                                {
                                    a.tileCollide = false;
                                    a.SyncProjectile();
                                }
                            }
                        }
                    }
                    if (AITimer >= (phase2 ? (phaseMult == 3 ? 200 : 151) : (phaseMult == 1 ? 100 : 76)))
                    {
                        NPC.ai[3] = 0;
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case Micolash:
                {
                    rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                    //if (AITimer == 40)
                    //  if (oldAttack != AIState)
                    //    currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), "Think fast!", Color.Violet, -1, 0.Color.Indigo* 0.5f, default, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)),3);

                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    if (AITimer == 40)
                        if (oldAttack != AIState && AITimer3 == 0)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack9.Angry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack9.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack9.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                    if (phaseMult == 3 && AITimer3 > 0 && AITimer == 121)
                    {
                        string dialogue = Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack9.Angry2").Value;
                        if (AITimer3 == 2)
                            dialogue = Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack9.Angry3").Value;
                        currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), dialogue, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer < 122)
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = AssholeFace;
                        FacePlayer();
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.02f, 0.1f);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center - Helper.FromAToB(NPC.Center, player.Center) * 120, false).X * 0.07f, 0.1f);
                    }
                    else
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        NPC.velocity.X *= 0.7f;
                    }
                    if (AITimer == 130)
                    {
                        SoundEngine.PlaySound(Sounds.xSpirit.WithPitchOffset(-0.5f), NPC.Center);
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustPerfect(staffTip, DustType<SparkleDust>(), Main.rand.NextVector2Circular(5, 5), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            Dust.NewDustPerfect(staffTip, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                        }
                    }
                    if (AITimer == 150)
                    {
                        for (int i = -1; i < 2; i++)
                            MPUtils.NewProjectile(null, NPC.Center + Helper.FromAToB(NPC.Center, disposablePos[0]) * 60, Helper.FromAToB(NPC.Center, disposablePos[0]).RotatedBy(i), ProjectileType<XRiftSmall>(), 0, 0);

                        SoundEngine.PlaySound(Sounds.xSpirit.WithPitchOffset(-0.5f), NPC.Center);
                        for (int i = 0; i < 20; i++)
                        {
                            Dust.NewDustPerfect(staffTip, DustType<SparkleDust>(), Main.rand.NextVector2Circular(15, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            Dust.NewDustPerfect(staffTip, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                        }
                    }
                    if (AITimer < 130)
                    {
                        if (AITimer == 1 || AITimer == 129)
                            NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                    }
                    if (AITimer > 170 && AITimer < 202 && AITimer % 4 == 0)
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = VeryShockedFace;
                    }

                    if (AITimer >= (phaseMult == 3 ? 220 : 250))
                    {
                        Reset();
                        AITimer3++;
                        if (AITimer3 >= (phase2 ? (phaseMult) : 0))
                        {
                            AITimer3 = 0;
                            PickAttack();
                        }
                        else
                            AITimer = (phaseMult == 3 ? 120 : 80);
                    }
                }
                break;
            case TheSheepening:
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = SmirkFace;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    if (AITimer == 1) disposablePos[0] = player.Center;
                    if (AITimer < 50)
                    {

                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[0].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                        NPC.direction = disposablePos[0].X < NPC.Center.X ? -1 : 1;
                        NPC.spriteDirection = NPC.direction;
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                    }
                    else
                        NPC.velocity.X *= 0.9f;
                    if (AITimer < (phase2 ? 160 : 70) && AITimer > 50)
                    {
                        FacePlayer();
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                    }
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 40)
                    {
                        WeightedRandom<string> chat = new WeightedRandom<string>(seedCounter);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack10.Default1").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack10.Default2").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack10.Default3").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack10.Default4").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack10.Default5").Value);
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack10.Angry").Value : chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack10.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }

                    if (AITimer == 50)
                    {

                        for (int i = 0; i < 15; i++)
                        {
                            Dust.NewDustPerfect(NPC.Center, DustType<SparkleDust>(), Main.rand.NextVector2Circular(17, 17), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                        }
                    }
                    bool shouldAttack = AITimer == 70;
                    if (phase2)
                        shouldAttack = AITimer == 70 || (AITimer >= 120 && AITimer % 30 == 0 && AITimer <= 200);
                    if (shouldAttack)
                    {
                        SoundEngine.PlaySound(Sounds.cursedToyCharge, NPC.Center);
                        MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ProjectileType<SheepeningOrb>(), 1, 0, Main.myPlayer, player.whoAmI);

                        if (phaseMult == 3 && AITimer % 60 == 0)
                            for (int i = -1; i < 2; i++)
                            {
                                if (i == 0) continue;
                                Projectile a = MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity).RotatedBy(i * 0.75f), ProjectileType<SheepeningOrb>(), 1, 0, Main.myPlayer, -1);
                                if (a is not null)
                                {
                                    a.localAI[0] = 1;
                                    a.SyncProjectile();
                                }
                            }

                    }
                    if (phaseMult == 3 && AITimer == 260)
                    {
                        for (int i = -2; i < 4; i++)
                        {
                            Projectile a = MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity).RotatedBy(i * 0.35f), ProjectileType<SheepeningOrb>(), 1, 0, Main.myPlayer, -1);
                            if (a is not null)
                            {
                                a.localAI[0] = 1;
                                a.SyncProjectile();
                            }
                        }
                    }
                    if (AITimer >= (phase2 ? 260 + (phaseMult == 3 ? 50 : 0) : 160))
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case ManaPotion:
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = SmirkFace;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    headFrame.Y = DisappointedFace;
                    FacePlayer();

                    if (AITimer == 30 && phaseMult == 3)
                    {
                        if (NPC.ai[3] <= 0)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack11.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                        {
                            WeightedRandom<string> chat = new WeightedRandom<string>(seedCounter);
                            if (NPC.ai[3] == 1)
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack11.Continuation1").Value);
                            else
                            {
                                if (NPC.ai[3] % 2 == 0)
                                {
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack11.Continuation2").Value);
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack11.Continuation3").Value);
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack11.Continuation4").Value);
                                }
                                else
                                {
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack11.Continuation5").Value);
                                    chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack11.Continuation6").Value);
                                }
                            }
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                    }
                    if (AITimer < 40)
                    {
                        if (AITimer > 20)
                            manaPotAlpha = MathHelper.Lerp(0, 1, (AITimer - 20) / 20);
                        else
                            staffAlpha = MathHelper.Lerp(1, 0, AITimer / 19);
                        if (AITimer == 25 || AITimer == 5)
                        {
                            Vector2 pos = NPC.Center - new Vector2(26 + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18);
                            for (int i = 0; i < 10; i++)
                            {
                                Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                                Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                            }
                        }
                        AITimer2 = MathHelper.Lerp(AITimer2, MathHelper.PiOver4, 0.1f);
                        if (AITimer == 1 || AITimer == 39)
                            NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot - AITimer2 * NPC.direction, 0.2f);
                    }
                    if (AITimer == 60)
                    {
                        Vector2 pos = NPC.Center - new Vector2(26 + (NPC.direction == -1 ? 8 : 18), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(0, 18);
                        MPUtils.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + player.velocity) * Main.rand.NextFloat(12, 17), ProjectileType<XManaPotion>(), 15, 0);
                        if (phase2)
                            for (int i = 0; i < 2 + (phaseMult == 3 ? 2 : 0); i++)
                                MPUtils.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center + player.velocity).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4, 16), ProjectileType<XManaPotion>(), 15, 0);

                        if (phaseMult == 3)
                            MPUtils.NewProjectile(null, pos, new Vector2(Helper.FromAToB(pos, player.Center + player.velocity).X * Main.rand.NextFloat(12, 17), -10), ProjectileType<XManaPotion>(), 15, 0);
                    }
                    if (AITimer > 60)
                    {
                        AITimer2 = MathHelper.Lerp(AITimer2, -(MathHelper.PiOver2 * 0.75f), 0.25f);
                        if (AITimer > 75)
                            staffAlpha = MathHelper.Lerp(0, 1, 0.1f);
                        manaPotAlpha = 0;
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot - AITimer2 * NPC.direction, 0.2f);
                    }
                    if (AITimer >= 100)
                    {
                        Reset();
                        NPC.ai[3] = MathHelper.Clamp(NPC.ai[3] + 1, 0, 20);
                        if (NPC.ai[3] > 4 || Main.rand.Next((int)MathHelper.Clamp(phaseMult == 3 ? ((doneAttacksBefore ? 8 : 15) - (int)NPC.ai[3]) : 18, 1, 20)) < 4)
                        {
                            NPC.ai[3] = 0;
                            PickAttack();
                        }
                    }
                }
                break;
            case PhantasmalBlast: //phase 2
                {
                    if (AITimer == 1)
                    {
                        NPC.netUpdate = true;
                        disposablePos[1] = player.Center;
                    }
                    if (AITimer < 50)
                    {

                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[1].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        FacePlayer();
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    }
                    else
                    {
                        NPC.velocity.X *= 0.9f;
                    }
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                    if (AITimer == 30)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? ((player.HasBuff<Sheepened>() || player.HasBuff(BuffID.WolfMount) ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack12.It").Value : (player.Male ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack12.Him").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack12.Her").Value))) : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack12.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack12.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                    if (AITimer == 50)
                    {
                        AITimer2 = 0.5f;
                        MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosionInvis>(), 0, 0);
                    }

                    if (AITimer < 205 && AITimer > 50) rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, player.Center, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                    else rightArmRot = Utils.AngleLerp(rightArmRot, 0, 0.2f);

                    if (AITimer > 80 && AITimer < 140 && AITimer % 20 < 10)
                    {
                        headFrame.Y = AngryFace;
                        Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                        Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                    }
                    if (AITimer > 80 && AITimer < 170 && AITimer % (phaseMult == 3 ? 10 : 15) == 0)
                    {
                        FacePlayer();
                        AITimer2 += 0.1f;
                        headFrame.Y = ShockedFace;
                        SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                        Helper.AddCameraModifier(new PunchCameraModifier(NPC.Center, Main.rand.NextVector2Unit(), 6, 6, 30, 1000));
                        MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver2 * 0.7f * AITimer2) * Main.rand.NextFloat(7, 12) * (phaseMult == 3 ? 0.8f : 0.6f), ProjectileType<XSpiritNoHome>(), 15, 0, -1, 0.25f);
                    }
                    if (AITimer == 140)
                    {
                        NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                        Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                        if (phaseMult == 3)
                            MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ProjectileType<SheepeningOrb>(), 1, 0, Main.myPlayer, player.whoAmI);
                        else
                        {
                            MPUtils.NewProjectile(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: 7.5f);
                            MPUtils.NewProjectile(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 0, 0, ai1: -7.5f);
                        }
                    }
                    if (AITimer > 160 && AITimer < 200 && AITimer % 25 == 0 && phaseMult == 3)
                        MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center + player.velocity), ProjectileType<SheepeningOrb>(), 1, 0, Main.myPlayer, player.whoAmI);
                    if (AITimer == 170 && phaseMult != 3)
                    {
                        headFrame.Y = ShockedFace;
                        for (int i = 0; i < 20; i++)
                        {
                            Vector2 pos = NPC.Center + Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(10, 50);
                            if (i % 2 == 0)
                                Dust.NewDustPerfect(pos, DustType<SparkleDust>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            else
                                Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                        }
                        Vector2 vel = Helper.FromAToB(NPC.Center, disposablePos[0]);
                        SoundEngine.PlaySound(Sounds.cursedToyCharge, NPC.Center);
                        Projectile a = MPUtils.NewProjectile(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: 7.5f);
                        Projectile b = MPUtils.NewProjectile(null, NPC.Center + vel * 15, vel, ProjectileType<XSineLaser>(), 15, 0, ai1: -7.5f);

                        if (a is not null)
                        {
                            a.localAI[0] = 1;
                            a.SyncProjectile();
                        }
                        if (a is not null)
                        {
                            a.localAI[0] = 1;
                            a.SyncProjectile();
                        }
                    }

                    if (AITimer >= 240)
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case ShadowflameRift:
                {
                    if (phaseMult == 3)
                        headFrame.Y = AngryFace;
                    else
                        headFrame.Y = SmirkFace;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    //more portals in phase 2
                    if (AITimer == 80)
                    {
                        WeightedRandom<string> chat = new WeightedRandom<string>(seedCounter);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack13.Default1").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack13.Default2").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack13.Default3").Value);
                        chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack13.Default4").Value); //phase 2
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack13.Angry").Value : chat, Color.Violet, -1, 0.6f, Color.Indigo * 0.6f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack13.RepeatAngry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    }
                    if (AITimer == 1)
                    {
                        NPC.netUpdate = true;
                        disposablePos[2] = player.Center;
                    }
                    if (AITimer < 50)
                    {

                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[2].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        NPC.direction = disposablePos[2].X > NPC.Center.X ? -1 : 1;
                        NPC.spriteDirection = NPC.direction;
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    }
                    else
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        NPC.velocity.X *= 0.9f;
                    }
                    if (AITimer > 60 && AITimer < 160)
                    {
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        if (AITimer < 160)
                        {
                            if (AITimer == 1 || AITimer == 159)
                                NPC.netUpdate = true;
                            disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);
                        }

                        if (AITimer < 85)
                        {
                            if (AITimer == 1 || AITimer == 84)
                                NPC.netUpdate = true;
                            disposablePos[0] = NPC.Center + new Vector2(NPC.direction * 70, 0);
                        }
                        if (AITimer % 2 == 0)
                        {
                            Dust.NewDustPerfect(disposablePos[0], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                            Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];

                            Dust.NewDustPerfect(disposablePos[1], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            Dust.NewDustPerfect(disposablePos[1], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                        }
                    }
                    if (AITimer > 60 && AITimer < 270 && AITimer % 5 == 0)
                    {
                        Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                        Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];
                    }
                    if (AITimer == 85)
                    {
                        MPUtils.NewProjectile(null, disposablePos[0], Helper.FromAToB(disposablePos[0], NPC.Center), ProjectileType<XRift>(), 0, 0);
                    }

                    if (phaseMult < 3)
                    {
                        if (AITimer == 140)
                        {
                            MPUtils.NewProjectile(null, disposablePos[1] + Helper.FromAToB(disposablePos[1], player.Center) * 10, Helper.FromAToB(disposablePos[1], player.Center), ProjectileType<XRift>(), 15, 0);
                            if (phaseMult == 3)
                                MPUtils.NewProjectile(null, disposablePos[1], -Helper.FromAToB(disposablePos[1], player.Center), ProjectileType<XRift>(), 15, 0);
                        }

                        if (AITimer == 200 && phase2)
                        {
                            disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);
                            MPUtils.NewProjectile(null, disposablePos[1] + Helper.FromAToB(disposablePos[1], player.Center) * 10, Helper.FromAToB(disposablePos[1], player.Center), ProjectileType<XRift>(), 15, 0);
                        }
                    }
                    else
                    {
                        if (AITimer > 140 && AITimer < 241 && AITimer % 30 == 0)
                        {
                            disposablePos[1] = Helper.Raycast(Vector2.Clamp(player.Center, GetArenaRect().BottomLeft() + new Vector2(30, -30), GetArenaRect().TopRight() + new Vector2(-30, 30)), -Vector2.UnitY, 600).Point;
                            MPUtils.NewProjectile(null, disposablePos[1], Vector2.UnitY, ProjectileType<XRift>(), 15, 0);

                            disposablePos[1] = Helper.Raycast(Vector2.Clamp(player.Center, GetArenaRect().BottomLeft() + new Vector2(30, -30), GetArenaRect().TopRight() + new Vector2(-30, 30)), Vector2.UnitY, 600).Point;
                            MPUtils.NewProjectile(null, disposablePos[1], -Vector2.UnitY, ProjectileType<XRift>(), 15, 0);
                        }
                    }

                    if (AITimer >= (phaseMult == 3 ? 310 : 330))
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case HelicopterBlades: //phase 2
                {
                    if (phaseMult == 3 && AITimer == 1)
                    {
                        AITimer = 450;
                    }
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);
                    if (AITimer < 400)
                        FacePlayer();
                    if (AITimer < 405)
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                    if (AITimer == 40)
                    {
                        if (!Main.dedServ)
                            helicopterSlot = SoundEngine.PlaySound(Sounds.helicopter.WithVolumeScale(1));
                    }

                    if (AITimer >= 40 && AITimer <= 440)
                    {
                        if (!Main.dedServ)
                            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
                            {
                                sound.Volume = MathHelper.Lerp(sound.Volume, 1, 0.1f);
                            }
                        headFrame.Y = AssholeFace;
                        if (AITimer > 80 && AITimer % 10 == 0 && AITimer < 120 || AITimer == 170)
                        {
                            SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                            Vector2 vel = Helper.FromAToB(NPC.Center, player.Center).RotatedByRandom(MathHelper.PiOver4 * 0.5f);
                            if (AITimer == 170)
                                vel = Helper.FromAToB(NPC.Center, player.Center + player.velocity);
                            MPUtils.NewProjectile(null, NPC.Center - new Vector2(14 + (NPC.direction == -1 ? 10 : 20), 0).RotatedBy(NPC.direction == -1 ? (rightArmRot - MathHelper.PiOver2 * 1.11f) : (rightArmRot - (MathHelper.Pi - MathHelper.PiOver4))) - new Vector2(25 * -NPC.direction, 24), vel * 7f, ProjectileType<XKnife>(), 15, 0);
                        }
                        heliAlpha = MathHelper.Lerp(heliAlpha, 1, 0.1f);
                        NPC.noGravity = true;
                        if (AITimer > 40 && AITimer < 170)
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, Helper.Raycast(player.Center, Vector2.UnitY, 300, true).Point - new Vector2(0, 150), false) / 35, 0.025f);
                        else if (AITimer >= 170 && AITimer < 240)
                            NPC.velocity *= 0.9f;
                        else if (AITimer > 240 && AITimer < 400)
                        {
                            if (AITimer % (phaseMult == 3 ? 15 : 30) == 0)
                            {
                                SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                                MPUtils.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center + player.velocity).RotatedByRandom(MathHelper.PiOver4 * 0.5f) * 2, ProjectileType<XBolt>(), 15, 0);
                            }
                            NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, Helper.Raycast(player.Center, Vector2.UnitY, 300, true).Point - new Vector2(MathF.Sin(AITimer * 3) * 200, 150), false) / 25, 0.05f);
                        }
                        else
                            NPC.velocity *= 0.9f;
                    }
                    if (AITimer > 400 && AITimer < 490)
                    {
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, NPC.Center - Vector2.UnitY.RotatedBy(NPC.direction * 0.5f) * 100, reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = phase2 ? SmirkFace : LightSmirkFace;
                        if (!Main.dedServ)
                            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
                            {
                                sound.Volume = MathHelper.Lerp(sound.Volume, 0, 0.1f);
                            }
                        NPC.velocity *= 0.9f;
                        NPC.noGravity = false;
                        heliAlpha = MathHelper.Lerp(heliAlpha, 0, 0.25f);
                        Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(200, 200), DustType<LineDustFollowPoint>(), Helper.FromAToB(Main.rand.NextVector2FromRectangle(NPC.getRect()), NPC.Center) * Main.rand.NextFloat(3, 7), 0, Color.Indigo, Main.rand.NextFloat(0.06f, .2f));
                        d.noGravity = true;
                        d.customData = NPC.Center + Helper.FromAToB(NPC.Center, player.Center) * 20;
                    }
                    if (AITimer == 470)
                    {
                        NPC.netUpdate = true;
                        disposablePos[0] = Helper.FromAToB(NPC.Center, player.Center);
                        MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0, ai2: 1);
                    }
                    if (phaseMult < 3)
                    {
                        if (AITimer > 499 && AITimer <= 530)
                        {
                            if (AITimer % 10 == 0)
                            {
                                SoundEngine.PlaySound(SoundID.Item73.WithPitchOffset(-0.2f), NPC.Center);
                                MPUtils.NewProjectile(null, staffTip, Helper.FromAToB(staffTip, player.Center).RotatedByRandom(MathHelper.PiOver4) * 7f, ProjectileType<XKnife>(), 15, 0);
                            }
                        }
                    }
                    else
                    {
                        FacePlayer();
                        if (AITimer == 460)
                        {
                            if (oldAttack != AIState)
                                currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack14.Angry").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        }
                        if (AITimer == 520)
                        {
                            SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                            MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                            for (int i = 0; i < 5; i++)
                            {
                                MPUtils.NewProjectile(null, staffTip, staffTip.FromAToB(player).RotatedBy(.6f * (i - 2.5f)) * (5 - MathF.Abs((i - 2.5f))), ProjectileType<XSpiritNoHome>(), 15, 0);
                            }
                        }
                        if (AITimer == 580)
                        {
                            SoundEngine.PlaySound(Sounds.xSpirit, NPC.Center);
                            MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosionTiny>(), 0, 0);
                            for (int i = 0; i < 8; i++)
                            {
                                MPUtils.NewProjectile(null, staffTip, staffTip.FromAToB(player).RotatedBy(.56f * (i - 4f)) * (8 - MathF.Abs((i - 4f))), ProjectileType<XSpiritNoHome>(), 15, 0);
                            }
                        }
                    }
                    if (AITimer >= (phaseMult == 3 ? 620 : 550))
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case AmethystStorm:
                {
                    if (AITimer == 40)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), phaseMult == 3 ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack15.Angry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack15.Default").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), (phaseMult == 3 ? (player.HasBuff<Sheepened>() ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack15.RepeatAngry1").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack15.RepeatAngry2").Value) : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.AndAgain").Value), Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                    if (AITimer < 260 && AITimer > 50)
                    {
                        if (phaseMult == 3)
                            headFrame.Y = AngryFace;
                        else
                            headFrame.Y = phase2 ? SmirkFace : DisappointedFace;
                        rightArmRot = Utils.AngleLerp(rightArmRot, -NPC.direction + MathHelper.ToRadians(-40f) * NPC.direction, 0.15f);
                        leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction + MathHelper.ToRadians(40f) * NPC.direction, 0.15f);
                    }
                    else
                    {
                        headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
                        IdleAnimation();
                    }

                    if (AITimer == 1)
                    {
                        NPC.netUpdate = true;
                        disposablePos[0] = player.Center;
                    }
                    if (AITimer < 50)
                    {

                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[0].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);
                        FacePlayer();
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.05f, 0.1f);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                    }
                    else
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        NPC.velocity.X *= 0.9f;
                    }
                    if (AITimer == 70)
                    {
                        if (phase2)
                        {
                            MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer, ai1: (phaseMult == 3 ? 1 : 0), ai2: player.whoAmI);
                            MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(-250, -100), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer, -20, ai1: (phaseMult == 3 ? 1 : 0), ai2: player.whoAmI);
                            if (phaseMult == 3)
                            {
                                MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(0, -150), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer, -30, ai1: (phaseMult == 3 ? 1 : 0), ai2: player.whoAmI);

                                MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(420, -50), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer, -10, ai1: (phaseMult == 3 ? 1 : 0), ai2: player.whoAmI);
                                MPUtils.NewProjectile(null, GetArenaRect().Center() + new Vector2(-420, -50), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer, 5, ai1: (phaseMult == 3 ? 1 : 0), ai2: player.whoAmI);
                            }
                        }
                        else
                            MPUtils.NewProjectile(null, new Vector2(MathHelper.Clamp(NPC.Center.X, GetArenaRect().X + 40, GetArenaRect().X + GetArenaRect().Width - 40), MathHelper.Clamp(NPC.Center.Y - 120, GetArenaRect().Y + 70, GetArenaRect().Y + GetArenaRect().Width - 70)), Vector2.Zero, ProjectileType<XCloud>(), 0, 0, Main.myPlayer, ai2: player.whoAmI);
                    }

                    if (AITimer >= 280 + (phaseMult == 3 ? 100 : 0))
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case BullshitRain:
                {
                    if (AITimer == 40)
                        if (oldAttack != AIState)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack16.Angry").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack16.RepeatAngry").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                    headFrame.Y = AngryFace;
                    leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                    if (AITimer == 1)
                    {
                        NPC.netUpdate = true;
                        disposablePos[2] = player.Center;
                    }
                    if (AITimer < 50)
                    {

                        Vector2 pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220, NPC.Center.Y);
                        if (disposablePos[2].X < NPC.Center.X)
                            pos = new Vector2(OffsetBoundaries(NPC.Size * 1.5f).X + 220 + OffsetBoundaries(NPC.Size * 1.5f).Width - 220, NPC.Center.Y);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = Helper.FromAToB(NPC.Center, pos, false).X * .01f;
                        NPC.direction = disposablePos[2].X > NPC.Center.X ? -1 : 1;
                        NPC.spriteDirection = NPC.direction;
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.03f, 0.1f);
                    }
                    else
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        NPC.velocity.X *= 0.9f;
                    }
                    if (AITimer > 60 && AITimer < 160)
                    {
                        if (AITimer == 61 || AITimer == 159 || AITimer == 84)
                            NPC.netUpdate = true;
                        rightArmRot = Utils.AngleLerp(rightArmRot, Helper.FromAToB(NPC.Center, disposablePos[0], reverse: true).ToRotation() + rightHandOffsetRot, 0.2f);
                        if (AITimer < 160)
                            disposablePos[1] = player.Center - new Vector2(-player.velocity.X, 100).RotatedByRandom(MathHelper.TwoPi);

                        if (AITimer < 85)
                        {
                            disposablePos[0] = NPC.Center + new Vector2(NPC.direction * 70, 0);
                        }
                        if (AITimer % 2 == 0)
                        {
                            Dust.NewDustPerfect(disposablePos[0], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                            Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];

                            Dust.NewDustPerfect(disposablePos[1], DustType<SparkleDust>(), Main.rand.NextVector2Circular(7, 7), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.175f));
                            Dust.NewDustPerfect(disposablePos[1], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f));
                        }
                    }
                    if (AITimer > 60 && AITimer < 270 && AITimer % 5 == 0)
                    {
                        Dust.NewDustPerfect(disposablePos[0], DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = NPC.Center;
                        Dust.NewDustPerfect(NPC.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(10, 10), 0, Color.Indigo, Main.rand.NextFloat(0.05f, 0.24f)).customData = disposablePos[0];
                    }
                    if (AITimer == 85)
                    {
                        MPUtils.NewProjectile(null, disposablePos[0], Helper.FromAToB(disposablePos[0], NPC.Center), ProjectileType<XRift>(), 0, 0);
                    }

                    if (AITimer == 100)
                    {
                        for (float i = 0; i <= 1; i += 0.2f)
                        {
                            Vector2 pos = Helper.Raycast(new Vector2(GetArenaRect().X + 30 + (GetArenaRect().Width - 60) * i, GetArenaRect().Center.Y), -Vector2.UnitY, 700).Point;
                            MPUtils.NewProjectile(null, pos, Vector2.UnitY, ProjectileType<XRift>(), 0, 0);
                        }
                    }
                    if (AITimer > 170 && AITimer < 260 && AITimer % 10 == 0)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 pos = Helper.Raycast(new Vector2(GetArenaRect().X + 30 + (GetArenaRect().Width - 60) * Main.rand.NextFloat(), GetArenaRect().Center.Y), -Vector2.UnitY, 700).Point;

                            Vector2 vel = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 * 0.7f);
                            SoundEngine.PlaySound(Sounds.xSpirit, pos);
                            for (int j = 0; j < 10; j++)
                            {
                                if (j % 2 == 0)
                                    Dust.NewDustPerfect(pos, DustType<SparkleDust>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                                else
                                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                            }

                            WeightedRandom<int> projType = new();
                            projType.Add(ProjectileType<XAmethystCloseIn>());
                            projType.Add(ProjectileType<XBolt>());
                            projType.Add(ProjectileType<XKnife>());
                            projType.Add(ProjectileType<XManaPotion>());
                            int _projType = projType;
                            Vector2 _vel = Vector2.UnitY.RotatedByRandom(PiOver4 * 0.3f);
                            if (_projType == ProjectileType<XKnife>())
                                _vel *= 7;
                            else
                                MPUtils.NewProjectile(null, pos + new Vector2(0, 32), _vel, _projType, 20, 0);
                        }
                    }

                    if (AITimer >= 280)
                    {
                        Reset();
                        PickAttack();
                    }
                }
                break;
            case BONK:
                {
                    if (AITimer == 40)
                        if (oldAttack != AIState && AITimer3 == 0)
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), doneAttacksBefore ? Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack17.Angry").Value : Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack17.FirstTimeAngry").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);
                        else
                            currentDialogue = DialogueSystem.NewDialogueBox(100, NPC.Center - new Vector2(0, 80), Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack17.RepeatAngry").Value, Color.Violet, -1, 0.6f, Color.Indigo * 0.5f, 4f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_OgreRoar.WithPitchOffset(0.9f + (phaseMult == 3 ? 0.1f : 0)), 5);

                    if (AITimer < 110)
                    {
                        rightArmRot = Utils.AngleLerp(rightArmRot, 1.75f * -NPC.direction, 0.3f);
                        leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.3f);
                        headFrame.Y = AngryFace;
                        FacePlayer();
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, NPC.velocity.X * 0.02f, 0.1f);

                        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                        if (NPC.Center.Y - player.Center.Y > NPC.height * 1.4f && NPC.velocity.Y.InRange(0))
                            NPC.velocity.Y = -9.5f;
                        NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center - Helper.FromAToB(NPC.Center, player.Center) * 120, false).X * 0.07f, 0.1f);
                    }
                    else
                    {
                        NPC.rotation = MathHelper.Lerp(NPC.rotation, 0, 0.1f);
                        NPC.velocity.X *= 0.7f;
                    }
                    if (AITimer == 1 || AITimer == 109)
                        NPC.netUpdate = true;
                    if (AITimer < 110)
                        disposablePos[0] = player.Center;

                    if (AITimer == 80)
                    {
                        staffAlpha = 0;
                        for (int i = 0; i < 15; i++)
                            MPUtils.NewProjectile(null, staffTip, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                        MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                        bigStaffAlpha = 1;
                        bigStaffBloomAlpha = 1;
                        AITimer2 = 60;
                    }

                    if (AITimer > 130 && AITimer2 > 0)
                    {
                        AITimer2--;
                        int direction = -NPC.direction;
                        if (AITimer > 132)
                            leftArmRot = Utils.AngleLerp(leftArmRot, NPC.direction * 1.5f, 0.3f);
                        if (lerpProg > 0)
                            swingProgress = MathHelper.Lerp(swingProgress, BigStaffEasingFunction(Utils.GetLerpValue(0, 60, AITimer2)), lerpProg);
                        float defRot = (Vector2.UnitX * NPC.direction).ToRotation();
                        float start = defRot - (MathHelper.PiOver2 + MathHelper.PiOver4);
                        float end = defRot + (MathHelper.PiOver2 + MathHelper.PiOver4);
                        if (lerpProg >= 0)
                        {
                            rightArmRot = Utils.AngleLerp(rightArmRot, (direction == 1 ? start + MathHelper.Pi * 3 / 2 * swingProgress : end - MathHelper.Pi * 3 / 2 * swingProgress) + MathHelper.PiOver4 - rightHandOffsetRot, 0.3f * lerpProg);
                        }
                        Vector2 position = NPC.Center +
                            (rightArmRot - PiOver4 + Pi).ToRotationVector2() * 170 * BigStaffScaleFunction(swingProgress);

                        if (lerpProg > -1 && swingProgress > 0.1f && swingProgress < 0.9f && AITimer2 % 2 == 0)
                        {
                            MPUtils.NewProjectile(null, position - Helper.FromAToB(position, NPC.Center) * 20, Helper.FromAToB(staffTip + (rightArmRot - PiOver4).ToRotationVector2() * 50, disposablePos[0]) * .1f, ProjectileType<XBolt>(), 20, 0);
                        }

                        if (AITimer > 164 && Helper.Raycast(NPC.Center, Vector2.UnitY, NPC.height).RayLength < NPC.height - 2)
                            if (AITimer3 == 0 && Helper.Raycast(position, Vector2.UnitY, 100).RayLength < 30)
                            {
                                AITimer3 = 1;
                                AITimer2 = 15;
                                AITimer = 190;
                                Helper.AddCameraModifier(new PunchCameraModifier(position, Vector2.UnitY, 10, 15, 30));
                                lerpProg = -1;
                                SoundEngine.PlaySound(SoundID.Item70, position);
                                SoundEngine.PlaySound(Sounds.xSpirit, position);
                                WeightedRandom<string> chat = new();
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack17.Staff1").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack17.Staff2").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack17.Staff3").Value);
                                chat.Add(Language.GetText("Mods.EbonianMod.Dialogue.ArchmageXDialogue.XAttack17.Staff4").Value);
                                DialogueSystem.NewDialogueBox(40, position - new Vector2(-40 * NPC.direction, 70), chat, Color.White, -1, 0.6f, Color.Magenta * 0.6f, 8f, true, DialogueAnimationIDs.BopDown | DialogueAnimationIDs.ColorWhite, SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.9f), 2);
                                Projectile p = MPUtils.NewProjectile(null, Helper.Raycast(position - new Vector2(0, 20), Vector2.UnitY, 80).Point, Vector2.Zero, ProjectileType<XImpact>(), 20, 0);
                                if (p is not null)
                                {
                                    p.friendly = false;
                                    p.hostile = true;
                                    MPUtils.SyncProjectile(p);
                                }
                            }

                    }
                    if (lerpProg == -1 || AITimer > 190)
                        leftArmRot = Utils.AngleLerp(leftArmRot, 0, 0.1f);

                    if (AITimer == 240)
                    {
                        for (int i = 0; i < 15; i++)
                            MPUtils.NewProjectile(null, staffTip, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

                        MPUtils.NewProjectile(null, staffTip, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
                        bigStaffBloomAlpha = 1;
                        bigStaffAlpha = 0;
                        staffAlpha = 1;
                    }
                    if (AITimer >= 270)
                    {
                        Reset();
                        AITimer3 = 0;
                        PickAttack();
                    }
                }
                break;
        }
    }
    float BigStaffEasingFunction(float x)
    {
        return x == 0
? 0
: x == 1
? 1
: x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
: (2 - MathF.Pow(2, -20 * x + 10)) / 2;
    }
    float BigStaffScaleFunction(float progress)
    {
        return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
    }
    // Sounds like something
    Vector2 sCenter;
    Rectangle GetArenaRect()
    {
        float LLen = Helper.Raycast(sCenter, -Vector2.UnitX, 29f * 16).RayLength;
        float RLen = Helper.Raycast(sCenter, Vector2.UnitX, 29f * 16).RayLength;
        Vector2 U = Helper.Raycast(sCenter, -Vector2.UnitY, 380).Point;
        Vector2 D = Helper.Raycast(NPC.Center, Vector2.UnitY, 380).Point;
        sCenter.Y = U.Y + Helper.FromAToB(U, D, false).Y * 0.5f;
        Vector2 L = sCenter;
        Vector2 R = sCenter;
        if (LLen > 400)
        {
            if (LLen > RLen)
            {
                R = Helper.Raycast(sCenter, Vector2.UnitX, 29f * 16).Point;
                L = Helper.Raycast(R - new Vector2(16, 0), -Vector2.UnitX, 34.5f * 32).Point;
            }
            else
            {
                R = Helper.Raycast(L + new Vector2(16, 0), Vector2.UnitX, 34.5f * 32).Point;
                L = Helper.Raycast(sCenter, -Vector2.UnitX, 29f * 16).Point;
            }
        }
        else
        {
            R = sCenter + new Vector2(495, 0);
            L = sCenter - new Vector2(464, 0);
        }
        Vector2 TopLeft = new Vector2(L.X, U.Y);
        Vector2 BottomRight = new Vector2(R.X, D.Y);
        Rectangle rect = new Rectangle((int)L.X, (int)U.Y, (int)Helper.FromAToB(TopLeft, BottomRight, false).X, (int)Helper.FromAToB(TopLeft, BottomRight, false).Y);
        return rect;
    }
    Rectangle OffsetBoundaries(Vector2 offset)
    {
        Rectangle rect = GetArenaRect();
        rect.Width -= (int)offset.X;
        rect.Height -= (int)offset.Y;
        rect.X += (int)offset.X / 2;
        rect.Y += (int)offset.Y / 2;
        return rect;
    }
    Vector2[] disposablePos = new Vector2[3];
    public override bool? CanFallThroughPlatforms()
    {
        Player player = Main.player[NPC.target];
        return (player.Center.Y > NPC.Center.Y + 50 && (AIState == Idle || (AIState == BONK && AITimer < 130) || AIState == Spawn || (AIState == Micolash && AITimer < 130))) || NPC.noGravity;
    }
    public override void Reset()
    {
        bool phase2 = phaseMult >= 2;
        NPC.netUpdate = true;
        if (!Main.dedServ)
            if (SoundEngine.TryGetActiveSound(helicopterSlot, out var sound))
            {
                sound.Volume = 0;
            }
        headOffIncrementOffset = Main.rand.NextFloat(500);
        AITimer = 0;
        AITimer2 = 0;
        NPC.noGravity = false;
        manaPotAlpha = 0;
        staffAlpha = 1;
        heliAlpha = 0;
        NPC.velocity.X = 0;
        bigStaffAlpha = 0;
        bigStaffBloomAlpha = 0;
        headFrame.Y = phase2 ? DisappointedFace : NeutralFace;
        for (int i = 0; i < disposablePos.Length; i++) disposablePos[i] = Vector2.Zero;
        NPC.damage = 0;
        lerpProg = 1;
        swingProgress = 0;
    }
}
