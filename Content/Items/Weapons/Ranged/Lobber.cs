using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Corruption;
using EbonianMod.Content.Projectiles.Friendly.Crimson;

namespace EbonianMod.Content.Items.Weapons.Ranged
{
    public class Lobber : ModItem
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/Lobber";

        public override void SetDefaults()
        {
            Item.damage = 80;
            Item.crit = 25;
            Item.useTime = 20;
            Item.knockBack = 5;
            Item.shootSpeed = 16;

            Item.value = Item.sellPrice(gold: 20);
            Item.useAmmo = AmmoID.Rocket;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Red;

            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.noMelee = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            Item.shoot = player.altFunctionUse == 2 ? ProjectileType<LobberProjectile1>() : ProjectileType<LobberProjectile0>();

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.GrenadeLauncher).AddIngredient(ItemType<CecitiorMaterial>(), 10).AddIngredient(ItemType<CorruptEyeMaterial>(), 10).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => false;
    }


    public class LobberProjectile0 : HeldProjectileGun
    {
        Vector2 Scale = new Vector2(0, 0);

        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/LobberProjectile0";

        public override bool? CanDamage() => false;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.Size = new Vector2(56, 48);
            Projectile.frame = 1;

            ItemType = ItemType<Lobber>();
            HoldOffset = new Vector2(15, -10);
            RotationSpeed = 0.2f;
            AimingOffset = 25;
        }

        public override void OnSpawn(IEntitySource source)
        {
            CalculateAttackSpeedParameters(20);

            Player player = Main.player[Projectile.owner];

            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation() + player.direction * Pi / 2;
            Projectile.frameCounter = (int)(-4 * AttackDelayMultiplier);
        }

        public override void AI()
        {
            base.AI();

            Player player = Main.player[Projectile.owner];

            player.heldProj = Projectile.whoAmI;

            if (Projectile.frameCounter > (int)(5 * AttackDelayMultiplier))
            {
                Projectile.frame++;
                if (Projectile.frame > 3)
                {
                    Projectile.frame = 0;
                }

                if (Projectile.frame == 0)
                {
                    Scale = new Vector2(0.65f, 1.6f);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        RecoilRotation = -0.2f * player.direction;

                        Vector2 shootingPoint = Projectile.Center + new Vector2(21, -7 * player.direction).RotatedBy(Projectile.rotation);

                        for (int i = 0; i < 14; i++)
                            Dust.NewDustPerfect(shootingPoint, DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(-PiOver4, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(2, 8), Scale: 1.5f).noGravity = true;

                        if (player.whoAmI == Main.myPlayer)
                            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), shootingPoint, Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length(), ProjectileType<CrimsonBall>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }

                    SoundEngine.PlaySound(SoundID.NPCDeath13.WithPitchOffset(Main.rand.NextFloat(0, 0.3f)), player.Center);

                    Projectile.UseAmmo(AmmoID.Rocket);
                }
                Projectile.frameCounter = 0;
            }
            Projectile.frameCounter++;

            Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);

            if (!player.channel)
                Projectile.Kill();

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), lightColor, Projectile.rotation, Projectile.Size / 2, Scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);

            return false;
        }
    }

    public class LobberProjectile1 : HeldProjectileGun
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/LobberProjectile1";

        public override bool? CanDamage() => false;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.Size = new Vector2(58, 50);

            ItemType = ItemType<Lobber>();
            HoldOffset = new Vector2(15, -10);
            RotationSpeed = 0.13f;
        }

        Vector2 _scale = new Vector2(0, 0);

        float _flashOpacity
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void OnSpawn(IEntitySource source)
        {
            CalculateAttackSpeedParameters(20);

            Player player = Main.player[Projectile.owner];
            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation() + player.direction * PiOver2;
            Projectile.ai[2] = 1;
        }

        public override void AI()
        {
            base.AI();

            Player player = Main.player[Projectile.owner];

            player.heldProj = Projectile.whoAmI;

            if (Projectile.frame < 6)
            {
                if (Projectile.ai[0] <= 120)
                {
                    Projectile.ai[0] += AttackSpeedMultiplier;
                    if (Projectile.frameCounter > 20 * AttackDelayMultiplier)
                    {
                        Projectile.frame++;
                        Projectile.frameCounter = 0;
                    }
                    Projectile.frameCounter++;
                }
                else if (Projectile.ai[0] != 1000)
                {
                    _flashOpacity = 1;

                    SoundEngine.PlaySound(SoundID.MaxMana.WithPitchOffset(-0.3f), Projectile.Center);

                    Projectile.ai[0] = 1000;
                }

                float charge = Clamp(Projectile.ai[0], 5, 120);
                float scaleNoise = charge / 82;

                _scale = Vector2.Lerp(_scale, new Vector2(Main.rand.NextFloat(2f - scaleNoise, scaleNoise), Main.rand.NextFloat(2f - scaleNoise, scaleNoise)), scaleNoise / 10);

                if (player.whoAmI == Main.myPlayer && !Main.mouseRight)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame = 6;

                    RecoilRotation = -charge / 240f * player.direction;

                    Vector2 shootingPoint = Projectile.Center + new Vector2(21, -7 * player.direction).RotatedBy(Projectile.rotation);

                    for (int u = 0; u < 14; u++)
                        Dust.NewDustPerfect(shootingPoint, DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(-PiOver4, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(2, 8), Scale: 1.5f).noGravity = true;

                    for (int i = 0; i < charge / 15; i++)
                    {
                        _scale = new Vector2(0.55f, 1.7f);

                        if (player.whoAmI == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile),
                                shootingPoint,
                                Main.rand.NextFloat(Projectile.rotation - PiOver2 * 20 / charge * Projectile.velocity.Length() / 20,
                                Projectile.rotation + (PiOver2 * 20 / charge)).ToRotationVector2() * Main.rand.NextFloat(charge / 5, charge / 8),
                                ProjectileType<CorruptionBall>(),
                                Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        SoundEngine.PlaySound(SoundID.NPCDeath13, player.Center);

                        Projectile.UseAmmo(AmmoID.Rocket);
                    }
                }
            }
            else
            {
                if (Projectile.frameCounter >= 5)
                {
                    Projectile.frame++;

                    if(Projectile.frame > 8)
                        Projectile.Kill();

                    Projectile.frameCounter = 0;
                }
                Projectile.frameCounter++;
            }

            _scale = Vector2.Lerp(_scale, new Vector2(1, 1), 0.2f);
            if (_flashOpacity > 0)
            {
                _flashOpacity -= 0.04f;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 position = Projectile.Center - Main.screenPosition;
            SpriteEffects flip = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, position, new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), lightColor, Projectile.rotation, Projectile.Size / 2, _scale, flip);
            Main.EntitySpriteDraw(Assets.Items.Weapons.Ranged.LobberFlash.Value, position, null, Color.White * _flashOpacity, Projectile.rotation, Projectile.Size / 2, _scale, flip);

            return false;
        }
    }
}