namespace EbonianMod.Content.Projectiles.Friendly.Crimson
{
    public class Fang : ModProjectile
    {
        public override string Texture => Helper.AssetPath + "Projectiles/Friendly/Crimson/Fang";

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(10, 20);
            Projectile.timeLeft = 600;
            Projectile.penetrate = 4;
            Projectile.aiStyle = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        }

        public override void AI()
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Main.rand.NextVector2Circular(3, 3), Scale: 1.5f).noGravity = true;

            Projectile.rotation = Projectile.velocity.ToRotation() + Pi / 2;
            Projectile.velocity.Y += 0.4f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCHit9.WithPitchOffset(Main.rand.NextFloat(-0.5f, 0f)), Projectile.Center);
            SoundEngine.PlaySound(Sounds.chomp1.WithPitchOffset(Main.rand.NextFloat(0f, 1f)), Projectile.Center);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Blood, Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1), Scale: 1.5f);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.NPCHit9.WithPitchOffset(Main.rand.NextFloat(-0.5f, 0f)), Projectile.Center);
        }
    }
}