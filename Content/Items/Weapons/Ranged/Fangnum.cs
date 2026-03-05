using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Crimson;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class Fangnum : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/Fangnum";
    public override void SetDefaults()
    {
        Item.knockBack = 10f;
        Item.width = 42;
        Item.height = 24;
        Item.crit = 10;
        Item.damage = 23;
        Item.useTime = 50;
        Item.useAnimation = 30;
        Item.DamageType = DamageClass.Ranged;
        Item.rare = ItemRarityID.LightRed;
        Item.shoot = ProjectileType<FangnumProjectile>();
        Item.useStyle = 1;
        Item.value = Item.buyPrice(0, 4, 0, 0);
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.noMelee = true;
        Item.useAmmo = AmmoID.Bullet;
    }
    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = ProjectileType<FangnumProjectile>();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI);
        return false;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.TheUndertaker).AddIngredient(ItemID.TissueSample, 20).AddIngredient(ItemID.Vertebrae, 15).AddTile(TileID.DemonAltar).Register();
    }
    public override bool CanConsumeAmmo(Item ammo, Player player) => false;
}
public class FangnumProjectile : HeldProjectileGun
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/Fangnum";
    public override bool? CanDamage() => false;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.Size = new(42, 24);
        ItemType = ItemType<Fangnum>();
        HoldOffset.Y = -4;
        AimingOffset = 11;
        RotationSpeed = 0.23f;
        RecoilRecoverySpeed = 0.3f;
    }
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(50);
        Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
    }
    public override void AI()
    {
        base.AI();

        Player player = Main.player[Projectile.owner];

        player.heldProj = Projectile.whoAmI;

        HoldOffset.X = Lerp(HoldOffset.X, 27, 0.2f);

        if (Projectile.ai[0]++ >= 50 * AttackDelayMultiplier)
        {
            HoldOffset.X -= 21 * RecoilMultiplier;
            RecoilRotation = -0.3f * player.direction;

            Vector2 shotPoint = Projectile.Center + new Vector2(21, -4 * player.direction).RotatedBy(Projectile.rotation);
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), shotPoint, Projectile.rotation.ToRotationVector2() * 20, ProjectileType<Tooth>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            for (int i = 0; i < 7; i++)
                Dust.NewDustPerfect(shotPoint, DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(-PiOver4, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(2, 8), Scale: 1.5f).noGravity = true;

            SoundEngine.PlaySound(SoundID.Item38.WithPitchOffset(Main.rand.NextFloat(0.5f, 1f)), player.Center);

            Projectile.UseAmmo(AmmoID.Bullet);
            Projectile.ai[0] = 0;
        }

        if (!player.channel) Projectile.Kill();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}