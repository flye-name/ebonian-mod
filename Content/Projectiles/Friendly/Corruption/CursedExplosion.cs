using System;

namespace EbonianMod.Content.Projectiles.Friendly.Corruption;

public class CursedExplosion : ModProjectile
{
    public override string Texture => Helper.Empty;

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(144, 144);
        Projectile.timeLeft = 10;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        for (int i = 0; i < 50; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Main.rand.NextVector2CircularEdge(8, 8) * Main.rand.NextFloat(0.9f, 1), Scale: 3).noGravity = true;
        }

        int count = Main.rand.Next(3, 7);
        for (int i = 0; i < count; i++)
        {
            Vector2 direction = (i * 2 * (Pi / count) + Main.rand.NextFloat(-Pi, Pi) / count).ToRotationVector2() * Main.rand.NextFloat(0.8f, 1);
            for (float j = 0; j <= 12; j += 0.5f)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, direction * j, Scale: (j + 1) / 2).noGravity = true;
            }
        }
    }
}