using EbonianMod.Content.Dusts;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageMissile : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Garbage/" + Name;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 5;
    }
    public override void SetDefaults()
    {
        Projectile.width = 10;
        Projectile.height = 10;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 350;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.frame = Main.rand.Next(5);
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithPitchOffset(1f), Projectile.Center);
        Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
        
        for (int i = 0; i < 15; i++) 
        {
            Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDustAkaFireDustButNoGlow2>(), newColor: Color.Gray).customData = 0.5f;
            Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Unit() * Main.rand.NextFloat(3, 15), newColor: Color.Gray, Scale: 0.1f);
        }
    }
    public override bool? CanDamage() => Projectile.timeLeft < 300;
    public override Color? GetAlpha(Color lightColor) => Color.White;
    public override void AI()
    {
        Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        if (Projectile.timeLeft > 300)
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]) * 0.99f;
            Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDustAkaFireDustButNoGlow2>(), newColor: Color.Gray).customData = 0.1f;
        }
        else
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0] * -2f);
            
            Projectile.tileCollide = true;
            for (int i = 0; i < 3; i++)
                Dust.NewDustPerfect(Projectile.Center, DustType<SmokeDustAkaFireDustButNoGlow2>(), newColor: Color.Gray).customData = 0.3f;
            if (Projectile.velocity.Length() < 25)
                Projectile.velocity *= 1.025f;
        }
        if (Projectile.timeLeft == 300)
            SoundEngine.PlaySound(Sounds.firework, Projectile.Center);
    }
}
