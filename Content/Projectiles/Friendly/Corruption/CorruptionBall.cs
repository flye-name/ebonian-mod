
namespace EbonianMod.Content.Projectiles.Friendly.Corruption
{
    public class CorruptionBall : ModProjectile
    {
        public override string Texture => Helper.AssetPath + "Projectiles/Friendly/Corruption/CorruptionBall";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(32);
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<CursedExplosion>(), Projectile.damage, Projectile.knockBack);

            SoundEngine.PlaySound(Sounds.eggplosion, Projectile.Center);
        }

        public override void AI()
        {
            Projectile.frameCounter++;

            if (Projectile.frameCounter % 5 == 0)
            {
                if (Projectile.frame == 0) Projectile.frame++;
                else Projectile.frame = 0;
            }

            Projectile.velocity.Y += 0.25f;
        }
    }

}