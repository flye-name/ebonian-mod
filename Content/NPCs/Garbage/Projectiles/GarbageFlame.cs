using System;
using EbonianMod.Common.Graphics;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageFlame : ModProjectile
{
    public override string Texture => Helper.AssetPath+"Extras/Extras2/fire_01";
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 25;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override bool PreKill(int timeLeft)
    {
        return true;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        GarbageFlameRendering.DrawCache.Add(() =>
        {
            var fadeMult = Helper.SafeDivision(1f / Projectile.oldPos.Length);
            Texture2D texture = Assets.Extras.Extras2.fire_01.Value;
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                float Y = MathHelper.Lerp(60, 0, (float)(MathHelper.Clamp(Projectile.velocity.Length(), -10, 10) + 10) / 20);
                Vector2 oldpos = Vector2.SmoothStep(Projectile.oldPos[i], Projectile.oldPos[i] - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 5 + Main.windSpeedCurrent * 2, Y), (float)i / Projectile.oldPos.Length);
                Vector2 olderpos = Vector2.SmoothStep(Projectile.oldPos[i - 1], Projectile.oldPos[i - 1] - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 5 + Main.windSpeedCurrent * 2, Y), (float)i / Projectile.oldPos.Length);
                if (oldpos == Vector2.Zero || oldpos == Projectile.position) continue;
                float mult = (1f - fadeMult * i);
                Color color = Color.Lerp(Color.Red, Color.Orange, mult * 0.35f) * mult * 0.35f;
                color *= MathHelper.Clamp(alpha * (1 + i / (float)Projectile.oldPos.Length), 0, 1f);
                
                if (i > Projectile.oldPos.Length * alpha)
                    continue;
                
                for (float j = 0; j < 3; j++)
                {
                    Vector2 pos = Vector2.Lerp(oldpos, olderpos, (float)(j / 5)) + new Vector2(0, Main.rand.NextFloat(-16f, 16f) * MathF.Pow(1f - mult, 0.75f)).RotatedBy(Projectile.oldRot[i]);
                    Main.spriteBatch.Draw(texture, pos + new Vector2(5, 10 / Main.GameZoomTarget) + Projectile.Size / 2 - Main.screenPosition, null, color, Projectile.oldRot[i] + MathHelper.PiOver2 + Main.rand.NextFloat(-0.04f, 0.04f), texture.Size() / 2, 0.06f * MathF.Pow(mult, 0.5f), SpriteEffects.None, 0);
                }
            }
        });
        return false;
    }
    public override void PostDraw(Color lightColor)
    {
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        //Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.Yellow, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.035f, SpriteEffects.None, 0);
        //Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, 0, TextureAssets.Projectile[Type].Value.Size() / 2, 0.03f, SpriteEffects.None, 0);
        Main.spriteBatch.ApplySaved(sbParams);
    }

    public override void SetDefaults()
    {
        Projectile.width = 5;
        Projectile.height = 5;
        Projectile.aiStyle = 14;
        AIType = ProjectileID.StickyGlowstick;
        Projectile.friendly = false;
        Projectile.tileCollide = true;
        Projectile.hostile = true;
        Projectile.timeLeft = 240;
    }
    float savedP, alpha = 1f;
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        if (Projectile.Center.Y >= savedP - 100)
            fallThrough = false;
        return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
    }
    public override void AI()
    {
        Projectile.timeLeft--;

        Projectile.rotation = Projectile.velocity.ToRotation();
        
        Lighting.AddLight(Projectile.Center, TorchID.Torch);
        if (savedP == 0)
            savedP = Main.player[Projectile.owner].Center.Y;
        if (Projectile.velocity.Y > 2.8f && Projectile.ai[0] == 0)
        {
            Projectile.velocity *= 0.87f;
        }

        if (Projectile.timeLeft < 30)
        {
            alpha = MathHelper.SmoothStep(1, 0, 1f - Projectile.timeLeft / 30f);
        }
    }
}
