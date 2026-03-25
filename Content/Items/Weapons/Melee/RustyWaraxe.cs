using EbonianMod.Content.Buffs;
using EbonianMod.Content.Projectiles.Bases;
using System;

namespace EbonianMod.Content.Items.Weapons.Melee;
public class RustyWaraxe : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/RustyWaraxe";
    public override void SetDefaults()
    {
        Item.knockBack = 10f;
        Item.width = 54;
        Item.height = 54;
        Item.crit = 10;
        Item.damage = 15;
        Item.useAnimation = 32;
        Item.useTime = 32;
        Item.noUseGraphic = true;
        Item.autoReuse = false;
        Item.noMelee = true;
        Item.channel = true;
        Item.DamageType = DamageClass.Melee;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.rare = ItemRarityID.Green;
        Item.shootSpeed = 1f;
        Item.shoot = ModContent.ProjectileType<RustyWaraxeP>();

        Item.value = Item.buyPrice(0, 1, 50, 0);
    }
    int dir = 1;
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        dir = -dir;
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0, dir);
        return false;
    }
}
public class RustyWaraxeP : HeldSword
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/RustyWaraxe";
    public override void SetExtraDefaults()
    {
        swingTime = 35 * 2;
        holdOffset = 38;
        Projectile.Size = new(54, 54);
        Projectile.extraUpdates = 1;
    }
    public override float Ease(float x)
    {
        return MathF.Pow(x, 5 + 2 * x) * MathF.Pow(1 + MathF.Sin(x * Pi), 3);
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (hit.Crit)
        {
            SoundEngine.PlaySound(SoundID.Item171, Projectile.Center);
            for (int i = 0; i < 40; i++)
                Dust.NewDust(target.position, target.width, target.height, DustID.Blood, Helper.FromAToB(Projectile.Center, target.Center).X * Main.rand.NextFloat(-10, 10), Helper.FromAToB(Projectile.Center, target.Center).Y * Main.rand.NextFloat(-10, 10), newColor: Color.Brown);
            target.AddBuff(ModContent.BuffType<RustyCut>(), 120);
        }
    }
    public override void OnSpawn(IEntitySource source)
    {
        SoundEngine.PlaySound(SoundID.Item1);
    }
    public override void ExtraAI()
    {
        int direction = (int)Projectile.ai[1];
        float swingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft));
        float nextSwingProgress = Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft - 1));

        float defRot = Projectile.velocity.ToRotation();
        float start = defRot - (PiOver2 + PiOver4);
        float end = defRot + (PiOver2 + PiOver4);
        float rotation = direction == 1 ? start + Pi * 3 / 2 * swingProgress : end - Pi * 3 / 2 * swingProgress;
        Vector2 position = Main.player[Projectile.owner].GetFrontHandPosition(stretch, rotation - PiOver2) +
            rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);

        float nextRotation = direction == 1 ? start + Pi * 3 / 2 * nextSwingProgress : end - Pi * 3 / 2 * nextSwingProgress;
        Vector2 nextPosition = Main.player[Projectile.owner].GetFrontHandPosition(stretch, rotation - PiOver2) +
            nextRotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);

        if (swingProgress is < 0.8f and > 0.2f)
        {
            for (float i = -1; i < 1; i += 0.05f)
            {
                Vector2 _pos = position + Main.player[Projectile.owner].DirectionTo(position) * 20 + rotation.ToRotationVector2() * 1.3f * (i + Main.rand.NextFloat(-0.025f, 0.025f));
                Vector2 _pos2 = nextPosition + Main.player[Projectile.owner].DirectionTo(nextPosition) * 20 + rotation.ToRotationVector2() * 1.2f * ((1f - i) + Main.rand.NextFloat(-0.025f, 0.025f));
                Dust.NewDustPerfect(_pos, DustID.Poop, _pos.FromAToB(_pos2) * Main.rand.NextFloat(5), Scale: 0.75f).noGravity = true;
            }
        }
    }
    public override void PostDraw(Color lightColor)
    {
        base.PostDraw(lightColor);
    }
}
