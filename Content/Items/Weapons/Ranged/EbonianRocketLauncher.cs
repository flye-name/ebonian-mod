using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Corruption;

namespace EbonianMod.Content.Items.Weapons.Ranged
{
    public class EbonianRocketLauncher : ModItem
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/EbonianRocketLauncher";

        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override void SetDefaults()
        {
            Item.Size = new Vector2(50, 40);
            Item.crit = 20;
            Item.damage = 96;
            Item.knockBack = 10;
            Item.useTime = 110;
            Item.shootSpeed = 20;

            Item.value = Item.sellPrice(gold: 9);
            Item.shoot = ProjectileType<EbonianRocketLauncherProjectile>();
            Item.useAmmo = AmmoID.Rocket;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Green;

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

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.RocketLauncher).AddIngredient(ItemType<CorruptEyeMaterial>(), 20).AddTile(TileID.DemonAltar).Register();
        }
    }

    public class EbonianRocketLauncherProjectile : HeldProjectileGun
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/EbonianRocketLauncher";

        Vector2 _scale = new Vector2(0.2f, 0.4f);

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.Size = new(50, 40);

            ItemType = ItemType<EbonianRocketLauncher>();
            HoldOffset.Y = -12;
            RotationSpeed = 0.25f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            CalculateAttackSpeedParameters(110);

            Projectile.rotation = (Main.MouseWorld - Main.player[Projectile.owner].Center).ToRotation();
        }

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];

            _scale = Vector2.Lerp(_scale, new Vector2(1, 1), 0.14f);

            HoldOffset.X = Lerp(HoldOffset.X, 20, 0.1f);

            if (Projectile.ai[0] >= 110 * AttackDelayMultiplier)
            {
                Projectile.ai[0] = 0;
                Projectile.ai[1] = 4;
                Projectile.ai[2] = 0;
            }
            if (Projectile.ai[1] > 0)
            {
                if (Projectile.ai[2] >= 10 * AttackDelayMultiplier)
                {
                    Vector2 shootingPoint = Projectile.Center + new Vector2(17, 3 * player.direction).RotatedBy(Projectile.rotation);
                    float shootSpeed = Projectile.velocity.Length();

                    Shoot(shootingPoint, shootSpeed);
                    Projectile.ai[2] = 0;
                    Projectile.ai[1]--;
                }
                Projectile.ai[2]++;
            }
            Projectile.ai[0]++;

            if (!player.channel)
                Projectile.Kill();
        }

        void Shoot(Vector2 shootingPoint, float shootSpeed)
        {
            Player player = Main.player[Projectile.owner];

            _scale = new Vector2(1f, 1.8f);
            HoldOffset.X = 10;

            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(shootingPoint, DustID.CorruptGibs, (Projectile.rotation + Main.rand.NextFloat(PiOver4, -PiOver4)).ToRotationVector2() * Main.rand.NextFloat(2, 10), 150, Scale: Main.rand.NextFloat(1, 3)).noGravity = true;
            }

            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), shootingPoint, (Projectile.rotation + player.direction * Main.rand.NextFloat(-Pi, Pi) / 2).ToRotationVector2() * shootSpeed / 4f, ProjectileType<EbonianRocket>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }

            SoundEngine.PlaySound(SoundID.NPCDeath13.WithPitchOffset(Main.rand.NextFloat(-1, -0.5f)), Projectile.Center);

            Projectile.UseAmmo(AmmoID.Rocket);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, _scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);

            return false;
        }
    }
}