using EbonianMod.Content.Projectiles.Bases;

namespace EbonianMod.Content.Items.Weapons.Ranged
{
    public class SalvagedThruster : ModItem
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/SalvagedThruster";

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 4;
            Item.useTime = 100;
            Item.crit = 0;
            Item.shootSpeed = 1;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileType<SalvagedThrusterProjectile>();
            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
            Item.useAmmo = AmmoID.Gel;
        }

        public override bool CanConsumeAmmo(Item ammo, Player player) => false;

        public override bool? CanAutoReuseItem(Player player) => false;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }
    }
    public class SalvagedThrusterProjectile : HeldProjectileGun
    {
        bool _isAmmoLeft, _isReady;

        float _charge
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        float _rayLength
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/SalvagedThrusterProjectile";

        public override bool? CanDamage() => _isAmmoLeft;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.Size = new Vector2(44, 30);
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ArmorPenetration = 100;

            ItemType = ItemType<SalvagedThruster>();
            HoldOffset = new Vector2(20, 4);
        }

        public override void OnSpawn(IEntitySource source)
        {
            CalculateAttackSpeedParameters(100);

            RotationSpeed = AttackSpeedMultiplier / 40;
            Projectile.rotation = (Main.MouseWorld - Main.player[Projectile.owner].Center).ToRotation();
            Projectile.scale = 0;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float number = 0;
            Vector2 origin = Projectile.Center + new Vector2(22, -4 * Main.player[Projectile.owner].direction).RotatedBy(Projectile.rotation);
            return _isReady ? Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), origin, origin + Projectile.rotation.ToRotationVector2() * (_charge / 216) * _rayLength, 0, ref number) : false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            player.heldProj = Projectile.whoAmI;

            if (_isReady)
            {
                Vector2 origin = Projectile.Center + new Vector2(22, -4 * player.direction).RotatedBy(Projectile.rotation);
                if (Projectile.timeLeft % 10 == 0)
                {
                    _isAmmoLeft = Projectile.UseAmmo(AmmoID.Gel);
                    if (_isAmmoLeft)
                    {
                        SoundEngine.PlaySound(SoundID.Item34 with { Volume = _charge / 250 }, Projectile.Center);
                    }
                    else
                    {
                        Dust.NewDustPerfect(origin, DustID.Smoke, Projectile.rotation.ToRotationVector2().RotatedByRandom(PiOver4 / 3) * Main.rand.NextFloat(0.1f, 3));
                        Dust.NewDustPerfect(origin, DustID.Smoke, Projectile.rotation.ToRotationVector2().RotatedByRandom(PiOver4 / 3) * Main.rand.NextFloat(0.1f, 3));
                        Projectile.frame = 0;
                    }
                }
                if (_isAmmoLeft)
                {
                    _charge = Lerp(_charge, 216, Projectile.velocity.Length() / 20f);
                    _rayLength = Helper.Raycast(origin, Projectile.rotation.ToRotationVector2(), _charge).RayLength;

                    Dust.NewDustPerfect(origin, DustID.Smoke, Projectile.rotation.ToRotationVector2().RotatedByRandom(Pi / 4) * Main.rand.NextFloat(0.1f, 2), Scale: Main.rand.NextFloat(1.1f, 2));

                    for (int i = 0; i < _charge / 12; i++)
                    {
                        for (int j = 0; j <= i / 4; j++)
                        {
                            Dust.NewDustPerfect(origin + Projectile.rotation.ToRotationVector2() * (i / 17f) * _rayLength, DustID.Torch, Main.rand.NextVector2CircularEdge(1, 1) * Main.rand.NextFloat(1.2f, 2) * Clamp(i * _charge / 1800, 0.5f, 5), Scale: Main.rand.NextFloat(2, 2.5f)).noGravity = true;
                        }
                    }

                    Projectile.frame = 1;
                }
                else _charge = 0;
            }
            else
            {
                Projectile.scale = Lerp(Projectile.scale, 1, 0.3f);
                if (Projectile.scale >= 0.95f)
                {
                    _isReady = true;
                    Projectile.scale = 1;
                }
            }

            if (!player.channel) Projectile.Kill();

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 240);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center + player.GFX() + Main.rand.NextVector2Circular(_charge, _charge) / 100f - Main.screenPosition, new Rectangle(0, Projectile.height * Projectile.frame, Projectile.width, Projectile.height), lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);

            return false;
        }
    }
}