namespace EbonianMod.Content.Dusts;

public class XGoopDust : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        //if (dust.scale <= 1f && dust.scale >= 0.8f)
        //  dust.scale = 0.25f;
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale -= 0.04f;
        dust.velocity *= 0.95f;
        dust.rotation += MathHelper.ToRadians(MathHelper.Clamp(dust.scale * 2, 0, 2));
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        XareusRendering.DrawCache.Add(() =>
        {
            Texture2D tex = Assets.Extras.Extras2.fire_01.Value;
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, (d.customData is null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 0.5f), d.rotation, tex.Size() / 2, d.scale * 0.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, (d.customData is null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 0.5f), -d.rotation, tex.Size() / 2, d.scale * 0.2f, SpriteEffects.None, 0);
        });
        return false;
    }
}
public class XGoopDust2 : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        //if (dust.scale <= 1f && dust.scale >= 0.8f)
        //  dust.scale = 0.25f;
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale -= 0.01f;
        dust.velocity *= 0.95f;
        dust.rotation += MathHelper.ToRadians(MathHelper.Clamp(dust.scale * 2, 0, 2));
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }
    
    public override bool PreDraw(Dust d)
    {
        XareusRendering.DrawCache.Add(() =>
        {
            Texture2D tex = Assets.Extras.Extras2.fire_01.Value;
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, (d.customData is null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 0.5f), d.rotation, tex.Size() / 2, d.scale * 0.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, (d.customData is null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 0.5f), -d.rotation, tex.Size() / 2, d.scale * 0.2f, SpriteEffects.None, 0);
        });
        return false;
    }
}
public class XGoopDustDark : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        //if (dust.scale <= 1f && dust.scale >= 0.8f)
        //  dust.scale = 0.25f;
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
    public static void DrawAll(SpriteBatch sb)
    {
        foreach (Dust d in Main.dust)
        {
            if (d.type == DustType<XGoopDustDark>() && d.active)
            {
                Texture2D tex = Assets.Extras.Spotlight.Value;
                sb.Draw(tex, d.position - Main.screenPosition, null, (d.customData is null ? Color.White : d.color) * MathHelper.Clamp(d.scale * 2, 0, 1), 0, tex.Size() / 2, d.scale, SpriteEffects.None, 0);
            }
        }
    }
}
