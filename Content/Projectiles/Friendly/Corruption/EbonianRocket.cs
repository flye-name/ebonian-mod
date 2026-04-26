using EbonianMod.Content.Projectiles.VFXProjectiles;

namespace EbonianMod.Content.Projectiles.Friendly.Corruption;

public class EbonianRocket : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Friendly/Corruption/" + Name;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 30;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(36, 30);
        Projectile.timeLeft = 200;
        Projectile.localNPCHitCooldown = 0;
        Projectile.penetrate = 1;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.usesLocalNPCImmunity = true;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Player player = Main.player[Projectile.owner];
        Projectile.ai[1] = Projectile.velocity.Length() / 4;
    }

    public override void OnKill(int timeLeft)
    {
        if (Projectile.owner == Main.myPlayer)
        {
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<CursedExplosion>(), Projectile.damage, 0, Projectile.owner);
        }
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return Projectile.ai[0] > 40;
    }

    public override void AI()
    {
        if (Projectile.ai[0] > 40)
        {
            Vector2 direction = Projectile.rotation.ToRotationVector2();
            if ((int)Projectile.ai[0] == 41)
            {
                Projectile.velocity = direction;
            }

            float speed = Projectile.velocity.Length();
            Vector2 spawnPosition = Projectile.Center - direction * 20;

            for (int i = 0; i < 2; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);
                Dust.NewDustPerfect(spawnPosition, DustID.Smoke, Main.rand.NextVector2Circular(2, 2), 150, Scale: 3).noGravity = true;
                Dust.NewDustPerfect(spawnPosition, DustID.CursedTorch, velocity / 2 - direction * speed * Main.rand.NextFloat(0.4f, 0.6f), 150, Scale: 2.4f).noGravity = true;
                Dust.NewDustPerfect(spawnPosition, DustID.CursedTorch, velocity, 150, Scale: 3).noGravity = true;
            }

            if (Projectile.ai[0] < 80 && speed < 20 * Projectile.ai[1])
            {
                Projectile.velocity += direction * Projectile.ai[1];
            }
            else Projectile.velocity = direction * speed;
        }
        else
        {
            Projectile.velocity *= 0.86f;
            if (Main.myPlayer == Projectile.owner)
                Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
        }
        Projectile.ai[0]++;

        Projectile.netUpdate = true;
    }
}