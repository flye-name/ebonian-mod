using EbonianMod.Content.Items.Consumables.Food;
using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Generic;
using System;
using System.IO;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class SpudCannon : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/SpudCannon";

    public override void SetDefaults()
    {
        Item.Size = new Vector2(58, 24);
        Item.knockBack = 5;
        Item.damage = 13;
        Item.crit = 7;
        Item.useTime = 100;

        Item.value = Item.sellPrice(gold: 2);
        Item.shoot = ProjectileType<PotatoProjectile>();
        Item.useAmmo = ItemType<Potato>();
        Item.DamageType = DamageClass.Ranged;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemRarityID.Blue;

        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        type = ProjectileType<SpudCannonProjectile>();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

        return false;
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) => false;

    public class SpudCannonProjectile : HeldProjectileGun
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/SpudCannon";

        public override bool? CanDamage() => false;

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.Size = new Vector2(58, 24);

            ItemType = ItemType<SpudCannon>();
            HoldOffset = new Vector2(25, -2);
            RotationSpeed = 0.2f;
            AimingOffset = -5;
        }

        Vector2 _scale = new Vector2(1, 1);
        float _charge;

        public override void OnSpawn(IEntitySource source)
        {
            CalculateAttackSpeedParameters(100);

            Player player = Main.player[Projectile.owner];
            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation() + player.direction * Pi * 2;
            Projectile.ai[2] = 1;
        }

        public override void AI()
        {
            base.AI();

            Player player = Main.player[Projectile.owner];

            player.heldProj = Projectile.whoAmI;

            _scale = Vector2.Lerp(_scale, Vector2.One, Clamp(0.21f * AttackSpeedMultiplier, 0.2f, 0.45f));

            if (Projectile.timeLeft % 15 == 0)
                Projectile.netUpdate = true;

            if (player.channel || Projectile.ai[0] < 14)
            {
                if (Projectile.ai[0] < 70)
                {
                    Projectile.ai[0] += AttackSpeedMultiplier;
                    _charge = Projectile.ai[0] / 35;
                }
                else if (Projectile.ai[0] != 1000)
                {
                    _charge = 2;
                    Projectile.ai[0] = 1000;
                    Projectile.ai[1] = 1f;
                    SoundEngine.PlaySound(SoundID.MaxMana.WithPitchOffset(-0.3f), Projectile.Center);
                }

                if (Projectile.timeLeft % 2 == 0)
                    SoundEngine.PlaySound(SoundID.Item98.WithPitchOffset(Clamp(Main.rand.NextFloat(_charge - 4, _charge - 3), -1, 1)) with { Volume = _charge / 10 }, Projectile.Center);
            }
            else
            {
                if (Projectile.ai[0] != 2000)
                {
                    Projectile.UseAmmo(ItemType<Potato>());

                    Vector2 shootingPoint = Projectile.Center + new Vector2(29, 5 * player.direction).RotatedBy(Projectile.rotation);

                    for (int i = 0; i < Clamp(10 * _charge, 8, 100); i++)
                        Dust.NewDustPerfect(shootingPoint, DustID.Smoke, (Projectile.rotation + Main.rand.NextFloat(-Pi / (_charge * 6), Pi / (_charge * 6))).ToRotationVector2() * Main.rand.NextFloat(0, 8) * _charge, Scale: 1.5f).noGravity = true;

                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile projectile = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), shootingPoint, Projectile.rotation.ToRotationVector2() * _charge * 8, ProjectileType<PotatoProjectile>(), (int)(Projectile.damage * MathF.Sqrt(_charge * 4)), Projectile.knockBack * _charge, Projectile.owner);
                        if (_charge == 2)
                        {
                            for (int i = 0; i < 16; i++)
                            {
                                Dust.NewDustPerfect(shootingPoint, DustID.Torch, (Projectile.rotation + Main.rand.NextFloat(-Pi / (_charge * 6), Pi / (_charge * 6))).ToRotationVector2() * Main.rand.NextFloat(0, 5) * _charge, Scale: Main.rand.NextFloat(0.4f, 3)).noGravity = true;
                            }
                            projectile.CritChance = 100;

                            SoundEngine.PlaySound(SoundID.Item40.WithPitchOffset(Main.rand.NextFloat(0.5f, 1)) with { Volume = 0.8f }, Projectile.Center);
                        }
                        projectile.netUpdate = true;
                    }
                    _scale = new Vector2(1 - _charge / 5, 1 + _charge * 0.4f);

                    SoundEngine.PlaySound(SoundID.Item42.WithPitchOffset(Main.rand.NextFloat(_charge - 3, _charge - 2.6f)) with { Volume = Clamp(_charge - 0.3f, 0.2f, 3) }, Projectile.Center);
                    SoundEngine.PlaySound(SoundID.Item89.WithPitchOffset(Main.rand.NextFloat(_charge - 2.1f, _charge - 1.8f)) with { Volume = _charge - 0.3f }, Projectile.Center);

                    Projectile.ai[0] = 2000;
                }
                if (_scale.Length() < 1.416f)
                {
                    player.itemTime = 0;
                    player.itemAnimation = 0;
                    Projectile.Kill();
                }
            }
            if (Projectile.ai[1] > 0)
            {
                Projectile.ai[1] -= 0.04f;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(_charge);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _charge = reader.ReadSingle();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            Vector2 shake = Projectile.ai[0] == 2000 ? Vector2.Zero : new Vector2(Main.rand.NextFloat(-_charge, _charge), Main.rand.NextFloat(-_charge, _charge));
            Vector2 position = Projectile.Center - Main.screenPosition + player.GFX() + shake;
            SpriteEffects flip = player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;

            Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, position, null, lightColor, Projectile.rotation, Projectile.Size / 2, _scale, flip);
            Main.EntitySpriteDraw(Assets.Items.Weapons.Ranged.SpudCannonFlash.Value, position, null, Color.White * Projectile.ai[1], Projectile.rotation, Projectile.Size / 2, _scale, flip);

            return false;
        }
    }
}