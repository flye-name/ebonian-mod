using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Corruption;

namespace EbonianMod.Content.Items.Weapons.Magic
{
    public class CursedDoll : ModItem
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/CursedDoll";

        public override void SetDefaults()
        {
            Item.damage = 270;
            Item.useTime = 75;
            Item.mana = 25;
            Item.crit = 20;

            Item.value = Item.sellPrice(gold: 13);
            Item.shoot = ProjectileType<CursedDollProjectile>();
            Item.DamageType = DamageClass.Magic;
            Item.rare = ItemRarityID.Green;
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.autoReuse = false;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<TerrortomaMaterial>(4).AddIngredient(ItemType<CorruptEyeMaterial>(), 10).AddTile(TileID.MythrilAnvil).Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false;
        }
    }
    public class CursedDollProjectile : HeldProjectileGun
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/CursedDoll";

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.Size = new Vector2(28, 40);
            Projectile.penetrate = -1;

            ItemType = ItemType<CursedDoll>();
            HoldOffset = new Vector2(16, -10);
            RotationSpeed = 0.08f;
        }

        Vector2 _scale = new Vector2(0, 0);

        public override void OnSpawn(IEntitySource source)
        {
            CalculateAttackSpeedParameters(75);

            Player player = Main.player[Projectile.owner];
            player.CheckMana(-player.HeldItem.mana, true);

            Projectile.rotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation();
            Projectile.frame = 5;

            Projectile.netUpdate = true;
        }

        public override void AI()
        {
            base.AI();

            Player player = Main.player[Projectile.owner];

            player.heldProj = Projectile.whoAmI;

            if (!Main.player[Projectile.owner].channel || !player.CheckMana(player.HeldItem.mana))
                Projectile.Kill();

            _scale = Vector2.Lerp(_scale, new Vector2(1, 1), 0.14f);

            if (Projectile.ai[0] >= 75 * AttackDelayMultiplier)
            {
                _scale = new Vector2(0.65f, 1.6f);
                player.CheckMana(player.HeldItem.mana, true, true);

                if (Main.myPlayer == player.whoAmI)
                {
                    Vector2 mousePosition = Main.MouseWorld;

                    for (int i = 0; i < 25; i++)
                    {
                        Dust.NewDustPerfect(mousePosition, DustID.CursedTorch, Main.rand.NextVector2Circular(5, 5), Scale: Main.rand.NextFloat(1.2f, 1.9f)).noGravity = true;
                        Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Main.rand.NextVector2Circular(2, 2), Scale: 1.5f).noGravity = true;
                    }
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), mousePosition, Vector2.Zero, ProjectileType<CursedDollCast>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.ai[0] = 0;
                }

                SoundEngine.PlaySound(SoundID.NPCHit28.WithPitchOffset(Main.rand.NextFloat(0.2f, 0.5f)) with { Volume = 0.1f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.NPCHit4.WithPitchOffset(Main.rand.NextFloat(-0.5f, -0.3f)) with { Volume = 0.12f }, Projectile.Center);
            }
            Projectile.ai[0]++;

            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];
            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, _scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);

            return false;
        }
    }

}