using System.IO;

namespace EbonianMod.Content.Projectiles.Friendly.Crimson;

public class GoryJaw : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Friendly/Crimson/GoryJaw";

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(34, 46);
        Projectile.penetrate = -1;
        Projectile.timeLeft = 3000;
        Projectile.extraUpdates = 10;
        Projectile.frame = 2;
        Projectile.aiStyle = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.hostile = false;
    }

    Vector2 _positionOffset;
    Vector2 _scale = Vector2.One;

    int _targetIndex
    {
        get => (int)Projectile.ai[1];
        set => Projectile.ai[1] = value;
    }

    public override bool? CanDamage() => _targetIndex == -1;

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;
        _targetIndex = -1;
    }

    public override void OnKill(int timeLeft)
    {
        if (Main.dedServ) 
            return;

        for (int i = 0; i < 4; i++)
        {
            Gore.NewGore(null, Projectile.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/GoryJaw" + i).Type, 1);
        }
        for (int i = 0; i < 20; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Blood, Main.rand.NextFloat(-Pi, Pi).ToRotationVector2() * Main.rand.NextFloat(2, 5), Scale: 1.5f);
        }

        SoundEngine.PlaySound(Sounds.chomp1.WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.2f)), Projectile.Center);
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (_targetIndex == -1)
        {
            _positionOffset = Projectile.Center - target.Center;
            _targetIndex = target.whoAmI;

            Projectile.frame = 0;
            Projectile.tileCollide = false;
            Projectile.netUpdate = true;
        }
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (_targetIndex != -1)
        {
            if (_targetIndex >= Main.npc.Length - 1)
            {
                _targetIndex = -1;
                Projectile.tileCollide = true;
                return;
            }

            NPC target = Main.npc[_targetIndex];

            if(!target.active)
            {
                _targetIndex = -1;
                Projectile.tileCollide = true;
                return;
            }

            Projectile.Center = target.Center + _positionOffset;

            Projectile.frameCounter++;
            if (Projectile.frameCounter == 60)
            {
                Projectile.frame++;
                if (Projectile.frame > 2)
                {
                    target.StrikeNPC(Projectile.damage, 0, target.direction);
                    float rotationWithOffset = Projectile.rotation - Pi / 2;
                    for (int i = 0; i < 8; i++)
                    {
                        Dust.NewDustPerfect(Projectile.Center + rotationWithOffset.ToRotationVector2() * 10, DustID.Blood, (rotationWithOffset + Main.rand.NextFloat(-Pi / 3f, Pi / 3f)).ToRotationVector2() * Main.rand.NextFloat(2, 6), Scale: 1.5f).noGravity = true;
                    }
                    Projectile.frame = 0;
                }
                Projectile.frameCounter = 0;
            }
        }
        else
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + Pi / 2;
            Projectile.velocity.Y += 0.002f;
        }
        float scaleModifier = 3000 - Projectile.timeLeft;
        _scale = Vector2.Lerp(_scale, new Vector2(Main.rand.NextFloat(2f - scaleModifier / 1700, scaleModifier / 1700), Main.rand.NextFloat(2f - scaleModifier / 1700, scaleModifier / 1700)), scaleModifier / 6000);

        Projectile.netUpdate = true;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), lightColor, Projectile.rotation, Projectile.Size / 2f, Vector2.Clamp(_scale, Vector2.One * 0.75f, Vector2.One * 1.25f), Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

        return false;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Projectile.tileCollide);
        writer.WriteVector2(_positionOffset);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Projectile.tileCollide = reader.ReadBoolean();
        _positionOffset = reader.ReadVector2();
    }
}