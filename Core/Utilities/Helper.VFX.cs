using System;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Core.Utilities;

public static partial class Helper
{
    public static Rectangle ScreenRect => new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
    public static Vector2 HalfScreen => new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
    public static void AddCameraModifier(ICameraModifier modifier)
    {
        if (!Main.dedServ)
            Main.instance.CameraModifiers.Add(modifier);
    }
    public static void SpawnDust(Vector2 position, Vector2 size, int type, Vector2 velocity = default, int amount = 1, Action<Dust> dustModification = null)
    {
        for (int i = 0; i < amount; i++)
        {
            var dust = Main.dust[Dust.NewDust(position, (int)size.X, (int)size.Y, type, velocity.X, velocity.Y)];
            dustModification?.Invoke(dust);
        }
    }
    public static void DustExplosion(Vector2 pos, Vector2 size = default, int type = 0, Color color = default, bool sound = true, bool smoke = true, float scaleFactor = 1, float increment = 0.125f, Vector2 _vel = default, float MinMulti = 1, float MaxMulti = 1)
    {
        int dustType = DustType<Content.Dusts.ColoredFlameDust>();
        switch (type)
        {
            case 0:
                dustType = DustType<Content.Dusts.ColoredFlameDust>();
                break;
            case 1:
                dustType = DustType<Content.Dusts.FireDust>();
                break;
            case 2:
                dustType = DustType<Content.Dusts.SmokeDustAkaFireDustButNoGlow>();
                break;
        }
        if (sound)
            SoundEngine.PlaySound(SoundID.Item62.WithPitchOffset(Main.rand.NextFloat(-0.2f, 0.4f)), pos);
        for (float num614 = 0f; num614 < 1f; num614 += increment)
        {
            Vector2 velocity = Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f) * Main.rand.NextFloat(MinMulti, MaxMulti);
            if (increment == 1 || type == 2)
                velocity = _vel;
            Dust dust = Dust.NewDustPerfect(pos, dustType, velocity, 150, color, scaleFactor);
            dust.noGravity = true;
            dust.color = color;

        }
        if (smoke)
            for (int num905 = 0; num905 < 10; num905++)
            {
                int num906 = Dust.NewDust(new Vector2(pos.X, pos.Y), (int)size.X, (int)size.Y, 31, 0f, 0f, 0, default(Color), 2.5f * scaleFactor);
                Main.dust[num906].position = pos + Vector2.UnitX.RotatedByRandom(3.1415927410125732) * size.X / 2f;
                Main.dust[num906].noGravity = true;
                Dust dust2 = Main.dust[num906];
                dust2.velocity *= 3f;
            }
        if (smoke)
            for (int num899 = 0; num899 < 4; num899++)
            {
                int num900 = Dust.NewDust(new Vector2(pos.X, pos.Y), (int)size.X, (int)size.Y, 31, 0f, 0f, 100, default(Color), 1.5f * scaleFactor);
                Main.dust[num900].position = pos + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * size.X / 2f;
            }
    }
    public static void QuickDustLine(Vector2 start, Vector2 end, float splits, Color color)
    {
        Dust.QuickDust(start, color).scale = 1f;
        Dust.QuickDust(end, color).scale = 1f;
        float num = 1f / splits;
        for (float amount = 0.0f; (double)amount < 1.0; amount += num)
            Dust.QuickDustSmall(Vector2.Lerp(start, end, amount), color).scale = 1f;
    }
    public static void QuickDustLine(this Dust dust, Vector2 start, Vector2 end, float splits, Color color1, Color color2)
    {
        Dust.QuickDust(start, color1).scale = 1f;
        Dust.QuickDust(end, color2).scale = 1f;
        float num = 1f / splits;
        for (float amount = 0.0f; (double)amount < 1.0; amount += num)
        {
            Color color = Color.Lerp(color1, color2, amount);
            Dust.QuickDustSmall(Vector2.Lerp(start, end, amount), color).scale = 1f;
        }
    }
}