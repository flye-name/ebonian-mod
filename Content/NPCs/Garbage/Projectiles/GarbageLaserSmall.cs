using EbonianMod.Content.Dusts;
using EbonianMod.Content.NPCs.Garbage;
using System;
using System.Collections.Generic;
using EbonianMod.Common.Graphics;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageLaserSmall1 : ModProjectile
{
    public override string Texture => Helper.Empty;
    const int maxTime = 200;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
    }
    public override void SetDefaults()
    {
        Projectile.Size = Vector2.One;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 300;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float a = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 600, 30, ref a) && Projectile.scale > 0.5f;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool? CanDamage() => false;
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.netUpdate = true; // TEST
    }
    public override void AI()
    {
        NPC npc = Main.npc[(int)Projectile.ai[0]];
        if (npc is not null)
            if (npc.active && npc.type == NPCType<HotGarbage>())
            {
                Projectile.Center = npc.Center + new Vector2(-7 * npc.direction, npc.height * 0.4f);
                Projectile.velocity = -npc.rotation.ToRotationVector2();
                if ((int)npc.ai[0] == (int)HotGarbage.State.Death)
                    Projectile.Kill();
            }
        float progress = Utils.GetLerpValue(0, 300, Projectile.timeLeft);
        Projectile.scale = MathHelper.Clamp(MathF.Sin(progress * MathHelper.Pi) * 5, 0, 1);

        SoundStyle style = SoundID.Item13;
        SoundStyle style2 = SoundID.Item34;
        style.MaxInstances = 0;
        style2.MaxInstances = 0;
        if (Projectile.timeLeft % 10 == 0)
        {
            SoundEngine.PlaySound(style, Projectile.Center);
            SoundEngine.PlaySound(style2.WithPitchOffset(-0.5f).WithVolumeScale(0.5f), Projectile.Center);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        GarbageFlameRendering.DrawCache.Add(() =>
        {
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            Texture2D texture = Assets.Extras.trail_04.Value;
            Texture2D texture2 = Assets.Extras.laser2.Value;
            float progress = Utils.GetLerpValue(0, 300, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, progress) * 50, 0, 1);
            Vector2 start = Projectile.Center - Main.screenPosition;
            float factor = MathHelper.Lerp(MathF.Sin(Main.GlobalTimeWrappedHourly * 2), MathF.Cos(Main.GlobalTimeWrappedHourly * 2), (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) * 0.5f);
            Vector2 off = Projectile.velocity.ToRotation().ToRotationVector2() * 800;
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            for (float i = 0; i < 1; i += 0.0025f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                float __off = -Main.GlobalTimeWrappedHourly * 3;

                float _off = __off + i;

                Color col = Color.OrangeRed * (s * s * (1 + (1f - progress) * 2));
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(10 + MathHelper.SmoothStep(0, 50 - (1f-progress) * 25, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i + new Vector2(10 + MathHelper.SmoothStep(0, 50 - (1f-progress) * 25, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.Lerp(Color.OrangeRed, Color.Lerp(Color.OrangeRed, Color.White, MathF.Pow(1f - progress, 2) * 0.5f), i) * (s * s * (1 + (1f - progress) * 2));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(10 + MathHelper.SmoothStep(0, 250 - (1f-progress) * 220, i), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i + new Vector2(10 + MathHelper.SmoothStep(0, 250 - (1f-progress) * 220, i), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));
            }

            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }
            }
        });
        return false;
    }
}
public class GarbageLaserSmall2 : ModProjectile
{
    public override string Texture => Helper.Empty;
    const int maxTime = 160;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
    }
    public override void SetDefaults()
    {
        Projectile.Size = Vector2.One;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = maxTime;
    }

    public override void OnSpawn(IEntitySource source)
    {
        Projectile.netUpdate = true; // TEST
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float a = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 800, 50, ref a) && Projectile.scale > 0.5f;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool? CanDamage() => false;
    public override void AI()
    {
        NPC npc = Main.npc[(int)Projectile.ai[0]];
        if (npc is not null)
            if (npc.active && npc.type == NPCType<HotGarbage>())
            {
                Projectile.Center = npc.Center + new Vector2(-7 * npc.direction, npc.height * 0.4f);
                if ((int)npc.ai[0] == (int)HotGarbage.State.Death)
                    Projectile.Kill();
            }
        float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
        Projectile.scale = MathHelper.Clamp(MathF.Sin(progress * MathHelper.Pi) * 5, 0, 1);

        SoundStyle style = SoundID.Item13;
        SoundStyle style2 = SoundID.Item34;
        style.MaxInstances = 0;
        style2.MaxInstances = 0;
        if (Projectile.timeLeft % 10 == 0)
        {
            SoundEngine.PlaySound(style.WithPitchOffset(0.3f).WithVolumeScale(0.5f), Projectile.Center);
            SoundEngine.PlaySound(style2.WithPitchOffset(-0.2f).WithVolumeScale(0.5f), Projectile.Center);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        GarbageFlameRendering.DrawCache.Add(() =>
        {
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            Texture2D texture = Assets.Extras.trail_04.Value;
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, progress) * 50, 0, 1);
            Vector2 start = Projectile.Center - Main.screenPosition;
            float factor = MathHelper.Lerp(MathF.Sin(Main.GlobalTimeWrappedHourly * 2), MathF.Cos(Main.GlobalTimeWrappedHourly * 2), (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) * 0.5f);
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * (800 + (factor * 30)));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            for (float i = 0; i < 1; i += 0.005f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                float __off = Main.GlobalTimeWrappedHourly * -3;


                float _off = (__off + i);

                Color col = Color.Lerp(Color.Red, Color.OrangeRed, i) * (s * s * 2);
                vertices.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(MathHelper.SmoothStep(0, 70, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(MathHelper.SmoothStep(0, 70, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.Lerp(Color.OrangeRed, Color.Orange, i) * (s * s * 2);
                vertices2.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(MathHelper.Lerp(0, 200, i), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(MathHelper.Lerp(0, 200, i), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));
            }

            //Main.graphics.GraphicsDevice.Textures[0] = texture;
            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                }
            }
        });
        return false;
    }
}
public class GarbageLaserSmall3 : ModProjectile
{
    public override string Texture => Helper.Empty;
    const int maxTime = 140;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
    }
    public override void SetDefaults()
    {
        Projectile.Size = Vector2.One;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = maxTime;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float a = 0f;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * 1000, 100, ref a) && Projectile.scale > 0.5f;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.netUpdate = true; // TEST
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool? CanDamage() => false;
    public override void AI()
    {
        NPC npc = Main.npc[(int)Projectile.ai[0]];
        if (npc is not null)
            if (npc.active && npc.type == NPCType<HotGarbage>())
            {
                Projectile.Center = npc.Center + new Vector2(-7 * npc.direction, npc.height * 0.4f);
                if ((int)npc.ai[0] == (int)HotGarbage.State.Death)
                    Projectile.Kill();
            }
        float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
        Projectile.scale = MathHelper.Clamp(MathF.Sin(progress * MathHelper.Pi) * 5, 0, 1);

        SoundStyle style = SoundID.Item13;
        SoundStyle style2 = SoundID.Item34;
        style.MaxInstances = 0;
        style2.MaxInstances = 0;
        if (Projectile.timeLeft % 10 == 0)
        {
            SoundEngine.PlaySound(style.WithPitchOffset(0.6f).WithVolumeScale(0.5f), Projectile.Center);
            SoundEngine.PlaySound(style2.WithPitchOffset(0.2f).WithVolumeScale(0.5f), Projectile.Center);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {


        GarbageFlameRendering.DrawCache.Add(() =>
        {
            List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
            List<VertexPositionColorTexture> vertices3 = new List<VertexPositionColorTexture>();
            Texture2D texture = Assets.Extras.trail_04.Value;
            float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
            float i_progress = MathHelper.Clamp(MathHelper.SmoothStep(1, 0, progress) * 50, 0, 1);
            Vector2 start = Projectile.Center - Main.screenPosition;
            float factor = MathHelper.Lerp(MathF.Sin(Main.GlobalTimeWrappedHourly * 2), MathF.Cos(Main.GlobalTimeWrappedHourly * 2), (MathF.Sin(Main.GlobalTimeWrappedHourly) + 1) * 0.5f);
            Vector2 off = (Projectile.velocity.ToRotation().ToRotationVector2() * (1000 + (factor * 30)));
            Vector2 end = start + off;
            float rot = Helper.FromAToB(start, end).ToRotation();

            float s = 0f;
            for (float i = 0; i < 1; i += 0.005f)
            {
                if (i < 0.5f)
                    s = MathHelper.Clamp(i * 3.5f, 0, 0.5f);
                else
                    s = MathHelper.Clamp((-i + 1) * 2, 0, 0.5f);

                float __off = Main.GlobalTimeWrappedHourly * -3;

                float _off = (__off + i);

                Color col = Color.Lerp(Color.DarkRed, Color.Orange, i) * (s * s * 4);
                vertices.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(50 + MathHelper.SmoothStep(0, 50, i * 3), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(50 + MathHelper.SmoothStep(0, 50, i * 3), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

                col = Color.Lerp(Color.DarkRed, Color.Orange, i) * (s * s * 2);
                vertices2.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(50 + MathHelper.SmoothStep(0, 100, i), 0).RotatedBy(rot + MathHelper.PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));
                vertices2.Add(Helper.AsVertex(start + off * i * i_progress + new Vector2(50 + MathHelper.SmoothStep(0, 100, i), 0).RotatedBy(rot - MathHelper.PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));
            }

            //Main.graphics.GraphicsDevice.Textures[0] = texture;
            if (vertices.Count >= 3 && vertices2.Count >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                }
            }
        });
        return false;
    }
}
