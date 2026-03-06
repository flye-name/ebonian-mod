using EbonianMod.Common.Graphics.Trails;
using System.Collections.Generic;

namespace EbonianMod.Content.Projectiles.VFXProjectiles;

public class AnimeSlash : ModProjectile
{
    public override string Texture => Helper.Placeholder;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 60;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        EbonianMod.projectileFinalDrawList.Add(Type);
    }
    public override void SetDefaults()
    {
        Projectile.Size = Vector2.One;
        Projectile.aiStyle = -1;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 2;
        Projectile.scale = 0.15f;
    }
    public override bool? CanDamage() => false;
    public List<Vector2> oldPosSaved = new List<Vector2>(30);
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.laser2.Value;
        float s = 0f;
        List<VertexPositionColorTexture> vertices = new();
        List<VertexPositionColorTexture> vertices2 = new();

        if (oldPosSaved.Count > 1)
        {
            for (int i = 1; i < oldPosSaved.Count; i++)
            {
                if (i < oldPosSaved.Count / 2)
                    s = MathHelper.SmoothStep(0, 1, (float)(i) / (oldPosSaved.Count / 2));
                else
                    s = MathHelper.SmoothStep(1, 0, (float)(i - (oldPosSaved.Count / 2)) / (oldPosSaved.Count / 2));

                float alpha = Projectile.scale * s;

                Vector2 start = oldPosSaved[i] - Main.screenPosition;
                Vector2 end = oldPosSaved[i - 1] - Main.screenPosition;
                float rot = Helper.FromAToB(start, end).ToRotation();
                if (oldPosSaved.Count < 5)
                    rot = Projectile.velocity.ToRotation();
                float y = MathHelper.Lerp(-5, 0, s);
                vertices.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count + 5), y).RotatedBy(rot + MathHelper.PiOver2), new Vector2(s * 0.5f, 0), Color.White * alpha * 3));
                vertices.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count + 5), y).RotatedBy(rot - MathHelper.PiOver2), new Vector2(s * 0.5f, 1), Color.White * alpha * 3));

                vertices2.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count * 1.5f + 5) * 3.5f, y).RotatedBy(rot + MathHelper.PiOver2), new Vector2(s * 0.5f, 0), Color.White * alpha * 1.75f));
                vertices2.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count * 1.5f + 5) * 3.5f, y).RotatedBy(rot - MathHelper.PiOver2), new Vector2(s * 0.5f, 1), Color.White * alpha * 1.75f));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count >= 3)
        {
            for (int j = 0; j < 3; j++)
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, tex);

            Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, tex);
        }
        Main.spriteBatch.ApplySaved(sbParams);
        return false;
    }
    float velLen, randValue;
    Vector2 baseVel, visualOffset, visualVelocity;
    public override void OnSpawn(IEntitySource source)
    {
        //randValue = Main.rand.NextFloat(0, 2f);
        baseVel = Projectile.velocity;
        velLen = Projectile.velocity.Length();
    }
    public override void AI()
    {
        Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
        Projectile.timeLeft = 10;
        if (Projectile.ai[0] <= 20 + velLen)
        {
            Projectile.scale = MathHelper.SmoothStep(0.15f, (Projectile.ai[2] == 0 ? 0.45f : Projectile.ai[2]), Projectile.ai[0] / (20 + velLen));
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1]);
            oldPosSaved.Clear();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    break;
                oldPosSaved.Add(Projectile.oldPos[i]);
            }
            //visualVelocity = Vector2.SmoothStep(Vector2.Zero, baseVel * Projectile.scale * (0.25f + Projectile.ai[1]) * randValue * Projectile.direction, Projectile.ai[0] / (20 + velLen));
        }
        //visualOffset += visualVelocity;
        if (Projectile.ai[0]++ > 20 + velLen)
        {
            //visualVelocity *= 0.95f;
            Projectile.velocity = Vector2.Zero;
            Projectile.scale -= MathHelper.Clamp(0.008f - 0.005f / (Projectile.ai[2] == 0 ? 0.45f : Projectile.ai[2]), 0.0035f, 0.01f);

            if (Projectile.scale < (Projectile.ai[2] == 0 ? 0.45f : Projectile.ai[2]) * 0.75f && Projectile.ai[0] % 2 == 0 && oldPosSaved.Count > 3)
                oldPosSaved.RemoveAt(oldPosSaved.Count - 1);

            if (Projectile.scale <= 0f)
                Projectile.Kill();
        }
    }
}

public class XAnimeSlash : ModProjectile
{
    public override string Texture => Helper.Placeholder;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 60;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        EbonianMod.projectileFinalDrawList.Add(Type);
    }
    public override void SetDefaults()
    {
        Projectile.Size = Vector2.One;
        Projectile.aiStyle = -1;
        Projectile.hostile = false;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 2;
        Projectile.scale = 0.15f;
    }
    public override bool? CanDamage() => false;
    public List<Vector2> oldPosSaved = new List<Vector2>(30);
    float animationOffset;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.wavyLaser.Value;
        float s = 0f;
        List<VertexPositionColorTexture> vertices = new();
        List<VertexPositionColorTexture> vertices2 = new();

        animationOffset -= 0.05f;
        if (animationOffset <= 0)
            animationOffset = 1;
        animationOffset = MathHelper.Clamp(animationOffset, float.Epsilon, 1 - float.Epsilon);
        if (oldPosSaved.Count > 1)
        {
            for (int i = 1; i < oldPosSaved.Count; i++)
            {
                if (i < oldPosSaved.Count / 2)
                    s = MathHelper.SmoothStep(0, 1, (float)(i) / (oldPosSaved.Count / 2));
                else
                    s = MathHelper.SmoothStep(1, 0, (float)(i - (oldPosSaved.Count / 2)) / (oldPosSaved.Count / 2));

                float alpha = Projectile.scale * s;

                Vector2 start = oldPosSaved[i] - Main.screenPosition;
                Vector2 end = oldPosSaved[i - 1] - Main.screenPosition;
                float rot = Helper.FromAToB(start, end).ToRotation();
                if (oldPosSaved.Count < 5)
                    rot = Projectile.velocity.ToRotation();
                float y = MathHelper.Lerp(-5, 0, s);

                float _off = animationOffset;
                if (_off > 1) _off = -_off + 1;
                float off = _off + (float)(i - 1) / oldPosSaved.Count;
                vertices.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count + 5), 0).RotatedBy(rot + MathHelper.PiOver2), Color.Indigo * alpha * 3, new Vector2(off, 0)));
                vertices.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count + 5), 0).RotatedBy(rot - MathHelper.PiOver2), Color.Indigo * alpha * 3, new Vector2(off, 1)));

                vertices2.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count * 1.5f + 5) * 3.5f, 0).RotatedBy(rot + MathHelper.PiOver2), Color.Indigo * alpha * 1.75f, new Vector2(off, 0)));
                vertices2.Add(Helper.AsVertex(start + new Vector2(2 + s * Projectile.scale * (oldPosSaved.Count * 1.5f + 5) * 3.5f, 0).RotatedBy(rot - MathHelper.PiOver2), Color.Indigo * alpha * 1.75f, new Vector2(off, 1)));
            }
        }
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        if (vertices.Count >= 3)
        {
            for (int j = 0; j < 3; j++)
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, tex, false);

            Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, tex, false);
        }

        Main.spriteBatch.ApplySaved(sbParams);
        return false;
    }
    float velLen, randValue;
    Vector2 baseVel, visualOffset, visualVelocity;
    public override void OnSpawn(IEntitySource source)
    {
        //randValue = Main.rand.NextFloat(0, 2f);
        baseVel = Projectile.velocity;
        velLen = Projectile.velocity.Length();
    }
    public override void AI()
    {
        Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
        Projectile.timeLeft = 10;
        if (Projectile.ai[0] <= 20 + velLen)
        {
            Projectile.scale = MathHelper.SmoothStep(0.15f, (Projectile.ai[2] == 0 ? 0.45f : Projectile.ai[2]), Projectile.ai[0] / (20 + velLen));
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[1]);
            oldPosSaved.Clear();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                    break;
                oldPosSaved.Add(Projectile.oldPos[i]);
            }
            //visualVelocity = Vector2.SmoothStep(Vector2.Zero, baseVel * Projectile.scale * (0.25f + Projectile.ai[1]) * randValue * Projectile.direction, Projectile.ai[0] / (20 + velLen));
        }
        //visualOffset += visualVelocity;
        if (Projectile.ai[0]++ > 20 + velLen)
        {
            //visualVelocity *= 0.95f;
            Projectile.velocity = Vector2.Zero;
            Projectile.scale -= MathHelper.Clamp(0.008f - 0.005f / (Projectile.ai[2] == 0 ? 0.45f : Projectile.ai[2]), 0.0035f, 0.01f);

            if (Projectile.scale < (Projectile.ai[2] == 0 ? 0.45f : Projectile.ai[2]) * 0.75f && Projectile.ai[0] % 2 == 0 && oldPosSaved.Count > 3)
                oldPosSaved.RemoveAt(oldPosSaved.Count - 1);

            if (Projectile.scale <= 0f)
                Projectile.Kill();
        }
    }
}
