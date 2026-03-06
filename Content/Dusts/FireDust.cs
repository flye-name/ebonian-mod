using EbonianMod.Common.Graphics;

namespace EbonianMod.Content.Dusts;

public class FireDust : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        dust.customData = Main.rand.Next(1, 3);
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale -= 0.01f;
        dust.velocity *= 0.95f;
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        Texture2D tex = Request<Texture2D>(Helper.AssetPath+"Extras/Extras2/fire_0" + d.customData).Value;
        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, Color.White with { A = 0 }, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, Color.OrangeRed with { A = 0 }, 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
        return false;
    }
}
public class GarbageFlameDust : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        if (dust.scale > 0.25f)
            dust.scale = 0.25f;
        dust.customData = Main.rand.Next(1, 3);
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale -= 0.003f;
        dust.velocity *= 0.95f;
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        Texture2D tex = Assets.Extras.Extras2.fire_01.Value;

        GarbageFlameRendering.DrawCache.Add(() =>
        {
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, d.color * (d.scale * 5), d.rotation, tex.Size() / 2, d.scale * 0.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, Color.White * (d.scale * .1f), d.rotation, tex.Size() / 2, d.scale * 0.6f, SpriteEffects.None, 0);
        });
        return false;
    }
}

public class ColoredFlameDust : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        if (dust.scale > 0.25f)
            dust.scale = 0.25f;
        dust.customData = Main.rand.Next(1, 3);
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale -= 0.003f;
        dust.velocity *= 0.95f;
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        Texture2D tex = Assets.Extras.Extras2.fire_01.Value;

        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, d.color with { A = 0 } * (d.scale * 5), d.rotation, tex.Size() / 2, d.scale * 0.6f, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, Color.White with { A = 0 } * (d.scale * .1f), d.rotation, tex.Size() / 2, d.scale * 0.6f, SpriteEffects.None, 0);
        return false;
    }
}

public class SmokeDustAkaFireDustButNoGlow : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        //if (dust.scale > 0.35f). t
        dust.rotation = Main.rand.NextFloat(MathHelper.Pi * 2);
        dust.scale = 0;
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale += 0.01f;
        dust.velocity *= 0.95f;

        if (dust.customData is not null && dust.customData.GetType() == typeof(Vector2))
        {
            dust.velocity = Vector2.Lerp(dust.velocity, Helper.FromAToB(dust.position, (Vector2)dust.customData, false) / 25, 0.05f + dust.scale * new UnifiedRandom(dust.dustIndex).NextFloat(0.9f, 1.2f));
            if (dust.position.Distance((Vector2)dust.customData) < 100)
                dust.scale += 0.01f;
        }

        if (dust.scale >= Main.rand.NextFloat(0.3f, 0.5f))
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        float alpha = MathHelper.Lerp(1, 0, d.scale * 2.857142857142857f);
        Texture2D tex = d.dustIndex % 2 == 0 ? Assets.Extras.Extras2.fire_01.Value : Assets.Extras.Extras2.fire_02.Value;
        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, d.color with { A = 0 } * alpha, d.rotation, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
        return false;
    }
}
public class SmokeDustAkaFireDustButNoGlow2 : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        //if (dust.scale > 0.35f). t
        dust.rotation = Main.rand.NextFloat(MathHelper.Pi * 2);
        dust.scale = 0;
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale += 0.01f;
        dust.velocity *= 0.95f;
        dust.rotation += new UnifiedRandom(dust.dustIndex).NextFloat(ToRadians(-10), ToRadians(10));

        if (dust.customData is not null && dust.customData.GetType() == typeof(Vector2))
        {
            dust.velocity = Vector2.Lerp(dust.velocity, Helper.FromAToB(dust.position, (Vector2)dust.customData, false) / 25, 0.05f + dust.scale * new UnifiedRandom(dust.dustIndex).NextFloat(0.9f, 1.2f));
            if (dust.position.Distance((Vector2)dust.customData) < 100)
                dust.scale += 0.01f;
        }

        if (dust.scale >= Main.rand.NextFloat(0.3f, 0.5f))
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        float alpha = MathHelper.Lerp(1, 0, d.scale * 2.857142857142857f);
        Texture2D tex = d.dustIndex % 2 == 0 ? Assets.Extras.Extras2.fire_01.Value : Assets.Extras.Extras2.fire_02.Value;
        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, d.color with { A = 0 } * alpha, d.rotation, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
        return false;
    }
}
