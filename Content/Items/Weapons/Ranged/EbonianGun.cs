using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Corruption;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class EbonianGun : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/EbonianGun";
    public override void SetDefaults()
    {
        Item.knockBack = 10f;
        Item.Size = new Vector2(72, 24);
        Item.crit = 6;
        Item.damage = 44;
        Item.useTime = 32;
        Item.useAnimation = 30;
        Item.DamageType = DamageClass.Ranged;
        Item.useStyle = 1;
        Item.rare = ItemRarityID.LightRed;
        Item.shoot = ProjectileType<EbonianGunProjectile>();
        Item.value = Item.buyPrice(0, 10, 0, 0);
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.noMelee = true;
        Item.useAmmo = AmmoID.Bullet;
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = ProjectileType<EbonianGunProjectile>();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        velocity = Vector2.Zero;
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.ShadowScale, 15).AddIngredient(ItemID.RottenChunk, 20).AddIngredient(ItemID.Musket).AddTile(TileID.DemonAltar).Register();
    }
}
public class EbonianGunProjectile : HeldProjectileGun
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/EbonianGun";
    public override bool? CanDamage() => false;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.Size = new(72, 24);
        ItemType = ItemType<EbonianGun>();
        RotationSpeed = 0.25f;
        HoldOffset.Y = -6;
        AimingOffset = 10;
    }
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(32);
        Projectile.rotation = (Main.MouseWorld - Main.player[Projectile.owner].Center).ToRotation();
    }
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        HoldOffset.X = Lerp(HoldOffset.X, 26, 0.2f);

        player.heldProj = Projectile.whoAmI;

        Projectile.ai[0]++;
        if (Projectile.ai[0] >= 32 * AttackDelayMultiplier)
        {
            Projectile.UseAmmo(AmmoID.Bullet);
            RecoilRotation = -0.14f * player.direction;
            SoundEngine.PlaySound(SoundID.Item11.WithPitchOffset(Main.rand.NextFloat(-1f, -0.5f)), player.Center);
            SoundEngine.PlaySound(SoundID.Item17.WithPitchOffset(Main.rand.NextFloat(-0.5f, -0.2f)), player.Center);
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center + new Vector2(26, -4f * player.direction).RotatedBy(Projectile.rotation), Projectile.rotation.ToRotationVector2() * 10, ProjectileType<CorruptionHitscan>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            HoldOffset.X = 11;
            Projectile.ai[0] = 0;
        }

        base.AI();

        if (!player.channel) Projectile.Kill();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}
