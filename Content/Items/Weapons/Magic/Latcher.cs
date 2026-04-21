using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Crimson;

namespace EbonianMod.Content.Items.Weapons.Magic;

public class Latcher : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/Latcher";
    public override void SetDefaults()
    {
        Item.damage = 80;
        Item.useTime = 50;
        Item.knockBack = 20;
        Item.mana = 40;
        Item.shootSpeed = 27;

        Item.value = Item.sellPrice(gold: 8);
        Item.shoot = ProjectileType<LatcherProjectile>();
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.DamageType = DamageClass.Magic;
        Item.rare = ItemRarityID.Green;

        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Vertebrae, 20).AddIngredient(ItemID.Hook).AddTile(TileID.Anvils).Register();
    }

    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<LatcherTongue>()] == 0;
    }

    public override bool? CanAutoReuseItem(Player player) => false;

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

        return false;
    }
}
public class LatcherProjectile : HeldProjectileGun
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/Latcher";

    public override void SetDefaults()
    {
        base.SetDefaults();

        Projectile.Size = new Vector2(60, 38);

        ItemType = ItemType<Latcher>();
        HoldOffset = new Vector2(25, -4);
        AimingOffset = 2;
    }

    Vector2 _scale = new Vector2(0, 0);
    int _targetIndex
    {
        get => (int)Projectile.ai[2];
        set => Projectile.ai[2] = value;
    }

    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(50);

        Player player = Main.player[Projectile.owner];
        Projectile.rotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation() + player.direction * Pi;
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        _scale = Vector2.Lerp(_scale, new Vector2(1, 1), 0.14f);

        base.AI();

        if (Projectile.ai[1] == 1)
        {
            if (_targetIndex >= Main.projectile.Length - 1)
            {
                Projectile.Kill();
                return;
            }

            Projectile projectile = Main.projectile[_targetIndex];
            if (player.ownedProjectileCounts[ProjectileType<LatcherTongue>()] == 0 || !projectile.active)
            {
                Projectile.Kill();
                return;
            }

            Vector2 point = Projectile.Center + new Vector2(14, 4 * Projectile.direction).RotatedBy(Projectile.rotation);
            projectile.ai[0] = point.X;
            projectile.ai[1] = point.Y;

            RotationSpeed = 0.03f;
        }
        else
        {
            RotationSpeed = Min(0.1f * AttackSpeedMultiplier, 1);
            if (player.whoAmI == Main.myPlayer && !player.channel && Projectile.ai[0] > 45 * AttackDelayMultiplier)
            {
                Projectile.ai[1] = 1;
                _scale = new Vector2(0.65f, 1.2f);

                if (Helper.Raycast(player.Center, Projectile.rotation.ToRotationVector2(), 96).Success)
                {
                    Projectile.Kill();
                }
                else
                {
                    _targetIndex = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length(), ProjectileType<LatcherTongue>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai2: Projectile.rotation);
                }

                SoundEngine.PlaySound(SoundID.NPCHit8.WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), player.Center);
            }
        }
        Projectile.ai[0]++;
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 20; i++) 
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: 1.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, _scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);

        return false;
    }
}
