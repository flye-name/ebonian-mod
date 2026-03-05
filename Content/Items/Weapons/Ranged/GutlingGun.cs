using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Cecitior;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class GutlingGun : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/GutlingGun";
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 21;
        Item.useTime = 1;
        Item.useAnimation = 10;
        Item.reuseDelay = 25;
        Item.shoot = ProjectileType<GutlingGunP>();
        Item.shootSpeed = 8f;
        Item.rare = ItemRarityID.LightRed;
        Item.useStyle = 5;
        Item.useAmmo = AmmoID.Bullet;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
    }
    public override bool? CanAutoReuseItem(Player player) => false;
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-10, 0);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.IllegalGunParts).AddIngredient(ItemID.Ichor, 5).AddIngredient(ItemID.SoulofFright, 15).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        type = ProjectileType<GutlingGunP>();
        velocity.Normalize();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 35);
        return false;
    }
}
public class GutlingGunP : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/GutlingGun";
    float holdOffset = 20;
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.Size = new Vector2(52, 30);
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
    }
    public override bool? CanDamage() => false;
    Vector2 Scale = Vector2.Zero;
    public override void AI()
    {
        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.33f);
        Player player = Main.player[Projectile.owner];
        if (!player.active || player.dead || player.CCed || player.noItems || !player.channel || !player.channel)
        {
            Projectile.Kill();
            return;
        }
        player.itemTime = 2;
        player.itemAnimation = 2;
        if (player.HeldItem.type != ItemType<GutlingGun>()) { Projectile.Kill(); }
        Projectile.timeLeft = 10;
        Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
        Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
        player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
        player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
        pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
        Projectile.Center = pos;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (player.gravDir != -1)
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - PiOver2);
        if (Main.myPlayer == Projectile.owner)
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.MouseWorld - player.Center, 0.15f - Lerp(0.09f, 0f, Projectile.ai[0] / 4)).SafeNormalize(Vector2.UnitX);
        if (Projectile.ai[0] > 2.7f)
            Projectile.ai[0] -= .6f;
        if (++Projectile.ai[1] > Projectile.ai[0])
        {
            bool success = false;
            for (int j = 0; j < 58; j++)
            {
                if (player.inventory[j].ammo == player.HeldItem.useAmmo && player.inventory[j].stack > 0)
                {
                    if (player.inventory[j].maxStack > 1)
                        player.inventory[j].stack--;
                    success = true;
                    break;
                }
            }
            if (!success)
            {
                Projectile.Kill();
                return;
            }
            SoundEngine.PlaySound(SoundID.Item11.WithPitchOffset(Main.rand.NextFloat(-1f, 0.2f)), player.Center);
            SoundEngine.PlaySound(SoundID.NPCHit9.WithPitchOffset(Main.rand.NextFloat(1f, 0.4f)), player.Center);
            Scale = new Vector2(0.8f, 1.3f);
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Projectile.velocity * 10, Projectile.velocity.RotatedBy(Main.rand.NextFloat(-(MathHelper.Pi / 16), MathHelper.Pi / 16)) * Main.rand.NextFloat(20, 25), ProjectileType<CecitiorTeethFriendly>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            Projectile.ai[1] = 0;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, Scale, Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}
