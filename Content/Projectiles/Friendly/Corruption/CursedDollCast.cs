
using System.IO;

namespace EbonianMod.Content.Projectiles.Friendly.Corruption
{
    public class CursedDollCast : ModProjectile
    {
        public override string Texture => Helper.AssetPath + "Projectiles/Friendly/Corruption/" + Name;

        public override void SetDefaults()
        {
            Projectile.Size = new Vector2(42, 14);
            Projectile.timeLeft = 3;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        float _rotationOffset;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0] = -1;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0;
        }

        public override bool? CanDamage() => Projectile.ai[0] == -1;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == -1)
            {
                Projectile.ai[0] = target.whoAmI;
                Projectile.ai[2] = _rotationOffset = Clamp(target.Size.Length() / 15, 10, 1000);
                Projectile.Center = target.Center;
                Projectile.timeLeft = 3600;
            }
        }

        public override void AI()
        {
            if (Projectile.ai[0] == -1) return;

            NPC target = Projectile.ai[0] >= Main.npc.Length - 1 ? null : Main.npc[(int)Projectile.ai[0]];
            bool isTargetActive = target != null && target.active;

            if (isTargetActive) Projectile.Center = target.Center;

            if (_rotationOffset == 0)
            {
                Projectile.ai[2] *= 1.5f;
                if(Projectile.ai[1] < -15f)
                {
                    if (isTargetActive)
                    {
                        target.StrikeNPC(Projectile.damage, 0, target.direction);
                        target.AddBuff(BuffID.CursedInferno, 360);
                    }
                    Projectile.Kill();
                }
            }
            else
            {
                Projectile.ai[2] *= 0.9f;
                _rotationOffset = Lerp(_rotationOffset, 0, 0.15f);
                Projectile.rotation = _rotationOffset + Pi / 4;
                if (_rotationOffset < 0.02f)
                {
                    Projectile.ai[2] = -1;
                    _rotationOffset = 0;
                }
            }

            Projectile.ai[1] += Projectile.ai[2];
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.ai[0] == -1) return;

            for (int i = 0; i < 50; i++)
                Dust.NewDustPerfect(Projectile.Center, DustID.CursedTorch, Main.rand.NextFloat(0, Pi * 2).ToRotationVector2() * Main.rand.NextFloat(1, 12), Scale: Main.rand.NextFloat(1.2f, 3f)).noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] == -1) return false;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            for (float i = 0; i < Pi * 2; i += Pi / 2)
            {
                Main.EntitySpriteDraw(texture, Projectile.Center - new Vector2(Projectile.ai[1], 0).RotatedBy(Projectile.rotation + i) - Main.screenPosition, null, lightColor, Projectile.rotation + i, Projectile.Size / 2, Projectile.scale, SpriteEffects.None);
            }

            return false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(_rotationOffset);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            _rotationOffset = reader.ReadSingle();
        }
    }
}