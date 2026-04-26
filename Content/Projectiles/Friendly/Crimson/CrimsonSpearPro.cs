using System;

namespace EbonianMod.Content.Projectiles.Friendly.Crimson;

// there will not be any effort put into cleaning this, screw you!
public class CrimsonSpearPro : ModProjectile
{
    public override string Texture => $"{Helper.AssetPath}Projectiles/Friendly/Crimson/{Name}";

    #region Fields

    private Player Owner => Main.player[Projectile.owner];

    private enum AIState
    {
        SlashDown,
        SlashUp,
        Thrust1,
        Thrust2,
        Thrust3,
        Thrust4
    }

    private AIState State => (AIState)Projectile.ai[0];

    private float MaxTimeLeft
    {
        get;
        set;
    }

    private float Progress => 1f - (Projectile.timeLeft / MaxTimeLeft);

    private ref float Offset => ref Projectile.ai[1];

    private ref float SwingDirection => ref Projectile.ai[2];

    #endregion Fields

    #region Initialization

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
        ProjectileID.Sets.AllowsContactDamageFromJellyfish[Type] = true;
        ProjectileID.Sets.NoMeleeSpeedVelocityScaling[Type] = true;
    }

    public override void SetDefaults()
    {
        Projectile.penetrate = 5;
        Projectile.DamageType = DamageClass.Melee;
        Projectile.friendly = true;
        Projectile.hostile = false;

        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.ownerHitCheck = true;

        Projectile.Size = new(118);
        Projectile.scale = 1f;
        Projectile.hide = true;

        Projectile.usesOwnerMeleeHitCD = true;
        Projectile.stopsDealingDamageAfterPenetrateHits = true;

        Projectile.MaxUpdates = 3;
        Projectile.aiStyle = -1;
        AIType = -1;
    }

    public override void OnSpawn(IEntitySource source)
    {
        MaxTimeLeft = Owner.HeldItem.useTime * Projectile.MaxUpdates;

        switch (State)
        {
            case AIState.SlashDown:
                SwingDirection = -Owner.direction;
                Offset = 30;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One);
                break;
            case AIState.SlashUp:
                SwingDirection = Owner.direction;
                Offset = 30;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One);
                break;
            case AIState.Thrust1:
                InitThrust();
                break;
            case AIState.Thrust2:
                InitThrust();
                break;
            case AIState.Thrust3:
                InitThrust();
                break;
            case AIState.Thrust4:
                InitThrust();
                break;
        }

        Projectile.timeLeft = (int)MaxTimeLeft;

        AI();
    }

    private void InitThrust()
    {
        SwingDirection = -Owner.direction;
        Offset = -20;
        MaxTimeLeft *= 0.5f;
        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.One).RotatedByRandom(0.3f);

        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 10, ModContent.ProjectileType<MeleeGoldenShower>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
    }

    #endregion Initialization

    #region Behavior

    public override bool ShouldUpdatePosition() => false;

    public override void AI()
    {
        switch (State)
        {
            case AIState.SlashDown:
                DoSlash();
                break;
            case AIState.SlashUp:
                DoSlash();
                break;
            case AIState.Thrust1:
                DoThrust();
                break;
            case AIState.Thrust2:
                DoThrust();
                break;
            case AIState.Thrust3:
                DoThrust();
                break;
            case AIState.Thrust4:
                DoThrust();
                break;
        }

        Owner.heldProj = Projectile.whoAmI;
        Owner.SetDummyItemTime(2);
    }

    private void DoSlash()
    {
        float rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp(MathHelper.PiOver4 * SwingDirection, -MathHelper.PiOver4 * SwingDirection, EaseOut(Progress)) - MathHelper.PiOver2;

        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

        if (Progress <= 0.4f)
            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(20, 80), 0).RotatedBy(Projectile.rotation), DustID.Ichor, Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * 3 * -SwingDirection, 0, default, 1f).noGravity = true;
        else
            Offset = MathHelper.Lerp(30, -20, EaseIn(1f - (Projectile.timeLeft / (MaxTimeLeft * 0.5f))));

        Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Lerp((MathHelper.PiOver2 + MathHelper.PiOver4) * SwingDirection, -(MathHelper.PiOver2 + MathHelper.PiOver4) * SwingDirection, EaseOut(Progress));

        Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

        Projectile.Center = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, rotation) + Projectile.rotation.ToRotationVector2() * Offset + new Vector2(0, Owner.gfxOffY);

        Owner.heldProj = Projectile.whoAmI;
    }

    private void DoThrust()
    {
        Projectile.Center = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2) + Projectile.velocity * Offset + new Vector2(0, Owner.gfxOffY);
        Projectile.rotation = Projectile.velocity.ToRotation();

        float minOffset = -20f;
        float maxOffset = 70f;

        Offset = MathHelper.Lerp(minOffset, maxOffset, EaseOut(Progress < 0.5f ? Progress * 2f : (1f - Progress) * 2f));

        float t = MathHelper.Clamp((Offset - minOffset) / (maxOffset - minOffset), 0f, 1f);

        var stretch = (Player.CompositeArmStretchAmount)(int)Math.Round(t * 3f);

        Owner.SetCompositeArmFront(true, stretch, Projectile.rotation - MathHelper.PiOver2);
    }

    private float EaseIn(float progress) => MathF.Pow(progress, 4f);

    private float EaseOut(float progress) => 1f - EaseIn(1f - progress);

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Ichor, 360);
        Projectile.damage = (int)(Projectile.damage * 0.7f);
    }

    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float p = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.BottomLeft.RotatedBy(Projectile.rotation + MathHelper.PiOver4, Projectile.Center), Projectile.TopRight.RotatedBy(Projectile.rotation + MathHelper.PiOver4, Projectile.Center), 32, ref p);
    }

    #endregion Behavior

    #region Drawing

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
        Texture2D glowTexture = ModContent.Request<Texture2D>($"{Texture}_Glow").Value;

        var position = Projectile.Center - Main.screenPosition;
        float rotation = Projectile.rotation + MathHelper.PiOver4;
        var spriteEffect = SpriteEffects.None;

        bool drawFlipped = SwingDirection == 1;

        if (drawFlipped)
        {
            spriteEffect = SpriteEffects.FlipHorizontally;
            rotation += MathHelper.PiOver2;
        }

        Main.spriteBatch.Draw(texture, position, texture.Bounds, Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2f, Projectile.scale, spriteEffect, 0f);
        Main.spriteBatch.Draw(glowTexture, position, glowTexture.Bounds, Projectile.GetAlpha(Color.White), rotation, glowTexture.Size() / 2f, Projectile.scale, spriteEffect, 0f);

        return false;
    }

    #endregion Drawing
}

public class MeleeGoldenShower : ModProjectile
{
    public override string Texture => Helper.Empty;

    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.GoldenShowerFriendly);
        AIType = ProjectileID.GoldenShowerFriendly;

        Projectile.DamageType = DamageClass.Melee;
    }

    public override void OnSpawn(IEntitySource source)
    {
        SoundEngine.PlaySound(SoundID.Item17, Projectile.Center);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.Ichor, 360);
        Projectile.damage = (int)(Projectile.damage * 0.7f);
    }
}