using EbonianMod.Content.Projectiles.VFXProjectiles;

namespace EbonianMod.Content.Projectiles.Friendly.Underworld
{
    public class CorebreakerHitscan : ModProjectile
    {
        public override string Texture => Helper.Empty;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(20);
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 60;
            Projectile.timeLeft = 350;
            Projectile.aiStyle = -1;

            Projectile.tileCollide = true;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            float angle = Projectile.velocity.ToRotation();

            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (angle - Pi / 2).ToRotationVector2() * Main.rand.NextFloat(0.7f, 2f), Scale: (float)Projectile.timeLeft / 70).noGravity = true;
            Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (angle + Pi / 2).ToRotationVector2() * Main.rand.NextFloat(0.7f, 2f), Scale: (float)Projectile.timeLeft / 70).noGravity = true;

            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == ProjectileType<CoreProjectile>() && Projectile.timeLeft < 345)
                {
                    if (Projectile.Distance(projectile.Center) < 45)
                    {
                        if (Main.myPlayer == projectile.owner)
                        {
                            Projectile explosion = Projectile.NewProjectileDirect(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), Projectile.damage * 5, 0);
                            explosion.scale *= 1.6f;
                            explosion.CritChance = 100;
                            explosion.friendly = true;
                            explosion.hostile = false;
                            explosion.SyncProjectile();

                            for (int i = 0; i < 35; i++)
                            {
                                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (Projectile.velocity + Main.rand.NextVector2Circular(5, 5)) * Main.rand.NextFloat(0.2f, 1.2f), Scale: Main.rand.NextFloat(1.8f, 2.7f));
                            }

                            projectile.Kill();
                            Projectile.Kill();
                        }
                        Projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 35; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (Projectile.velocity + Main.rand.NextVector2Circular(5, 5)) * Main.rand.NextFloat(0.2f, 1.2f), Scale: Main.rand.NextFloat(1.8f, 2.7f));
            }

            return true;
        }
    }
}