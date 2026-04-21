using System;
using System.IO;

namespace EbonianMod.Content.Projectiles.Friendly.Generic
{
    public class Baseball : ModProjectile
    {
        public override string Texture => Helper.AssetPath + "Projectiles/Friendly/Generic/" + Name;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(20);
            Projectile.timeLeft = 400;
            Projectile.penetrate = 2;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
        }

        float _baseDamage
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void OnSpawn(IEntitySource source)
        {
            _baseDamage = Projectile.damage;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Length() > 3)
            {
                Vector2 normal = Vector2.UnitY;
                if (Projectile.velocity.X != oldVelocity.X)
                    normal = Vector2.UnitX;

                if (normal.X == 0)
                {
                    Projectile.velocity.Y = oldVelocity.Y * -0.5f;
                    Projectile.velocity.X = oldVelocity.X * 0.9f;
                }
                else
                {
                    Projectile.velocity.Y = oldVelocity.Y * 0.9f;
                    Projectile.velocity.X = oldVelocity.X * -0.6f;
                }
            }
            else Projectile.velocity *= 0.94f;
            if (Projectile.timeLeft > 20) Projectile.timeLeft -= 8;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
        {
            Projectile.velocity *= 0.945f;
            if (Projectile.penetrate == 1)
                for (int i = 0; i < 25; i++)
                    Dust.NewDustPerfect(Projectile.Center, DustID.Sand, Main.rand.NextVector2Circular(5, 5), Scale: Main.rand.NextFloat(1.2f, 1.9f)).noGravity = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.damage = (int)(_baseDamage * MathF.Max(1, MathF.Pow(Projectile.velocity.Length(), 2) / 220f));

            if (Projectile.damage > 75)
            {
                if (Projectile.ai[0] > 0) Projectile.ai[0]--;
                Projectile.CritChance = 100;
            }
            else
            {
                Projectile.ai[0] = 10;
                Projectile.CritChance = 0;
            }

            if (Projectile.timeLeft > 365)
            {
                Projectile.velocity.X = player.velocity.X;
                Projectile.Center = new Vector2(player.MountedCenter.X, Projectile.Center.Y);
            }

            if(MathF.Abs(Projectile.velocity.X) > 1)
            {
                Projectile.ai[2] = Projectile.velocity.X > 1 ? 1 : -1;
            }
            float rotationMultiplier = MathF.Abs(Projectile.velocity.X) < 1 ? (player.direction * 2) : (Projectile.velocity.X / 2);
            Projectile.rotation += ToRadians(Projectile.velocity.Length() * rotationMultiplier);
            Projectile.velocity.Y += 0.45f;

            if (Projectile.timeLeft < 20)
                Projectile.Opacity *= 0.8f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.damage > 75)
                for (int i = 0; i < Projectile.oldPos.Length - Projectile.ai[0]; i++)
                    Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, lightColor * (1 - i / 8f), Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None);

            return true;
        }
    }
}