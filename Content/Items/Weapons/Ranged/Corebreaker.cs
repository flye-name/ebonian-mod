using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Underworld;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class Corebreaker : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/Corebreaker";
    public override void SetDefaults()
    {
        Item.damage = 21;
        Item.crit = 10;
        Item.useTime = 120;
        Item.knockBack = 10;

        Item.value = Item.sellPrice(gold: 10);
        Item.shoot = ProjectileType<CorebreakerProjectile>();
        Item.DamageType = DamageClass.Ranged;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemRarityID.Yellow;

        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

        return false;
    }
}
public class CorebreakerProjectile : HeldProjectileGun
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/Corebreaker";

    public override void SetDefaults()
    {
        base.SetDefaults();

        Projectile.Size = new Vector2(62, 38);

        ItemType = ItemType<Corebreaker>();
        HoldOffset.Y = -2;
        AimingOffset = 2;
        RotationSpeed = 0.25f;
    }

    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(120);

        Projectile.rotation = (Main.MouseWorld - Main.player[Projectile.owner].Center).ToRotation();
        Projectile.ai[1] = 40;
        Projectile.ai[0] = 50;

        Projectile.netUpdate = true;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        HoldOffset.X = Lerp(HoldOffset.X, 24, 0.13f);

        if (Projectile.ai[0] >= 120 * AttackDelayMultiplier)
        {
            HoldOffset.X = 0;
            RecoilRotation = -0.4f * player.direction;

            Vector2 shootingPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * 25;

            for (int i = 0; i < 30; i++)
                Dust.NewDustPerfect(shootingPoint, DustID.Torch, (Projectile.rotation + Main.rand.NextFloat(-PiOver4, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(0.3f, 8), Scale: Main.rand.NextFloat(1f, 4f)).noGravity = true;

            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), shootingPoint, Projectile.rotation.ToRotationVector2() * (Main.MouseWorld - Projectile.Center).Length() / 32, ProjectileType<CoreProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundEngine.PlaySound(SoundID.Item38.WithPitchOffset(Main.rand.NextFloat(-0.8f, -0.4f)), player.Center);
            SoundEngine.PlaySound(SoundID.Item40.WithPitchOffset(Main.rand.NextFloat(-1f, -0.5f)), player.Center);

            Projectile.ai[0] = 0;
        }
        if (Main.myPlayer == Projectile.owner && Main.mouseRight && Projectile.ai[1] >= 80 * AttackDelayMultiplier)
        {
            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation();

            HoldOffset.X = 0;

            Vector2 shootingPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * 25;

            for (int i = 0; i < 30; i++)
            {
                Dust.NewDustPerfect(shootingPoint, DustID.Smoke, (Projectile.rotation + Main.rand.NextFloat(PiOver2, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(1, 8), Scale: Main.rand.NextFloat(0.5f, 3f)).noGravity = true;
                Dust.NewDustPerfect(shootingPoint, DustID.Smoke, (Projectile.rotation + Main.rand.NextFloat(-PiOver2, -PiOver4)).ToRotationVector2() * Main.rand.NextFloat(1, 8), Scale: Main.rand.NextFloat(0.5f, 3f)).noGravity = true;
            }

            Projectile.NewProjectile(Projectile.InheritSource(Projectile), shootingPoint, Projectile.rotation.ToRotationVector2() * 4, ProjectileType<CorebreakerHitscan>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            SoundEngine.PlaySound(SoundID.Item62.WithPitchOffset(Main.rand.NextFloat(0.5f, 1.2f)), player.Center);
            SoundEngine.PlaySound(SoundID.Item98.WithPitchOffset(Main.rand.NextFloat(-1f, -0.5f)), player.Center);

            Projectile.ai[0] = 0;
            Projectile.ai[1] = 0;
        }
        Projectile.ai[0]++;
        Projectile.ai[1]++;

        base.AI();

        if (!player.channel) 
            Projectile.Kill();

        Projectile.netUpdate = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, 1, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);

        return false;
    }
}
