using EbonianMod.Content.Projectiles.VFXProjectiles;
using System;
using System.IO;
using System.Linq;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class Pipebomb : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Garbage/" + Name;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 25;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.aiStyle = 2;
        AIType = ProjectileID.Shuriken;
        Projectile.width = 18;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 100;
        Projectile.height = 36;
    }
    float savedP;
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        if (Projectile.Center.Y >= savedP - 100)
            fallThrough = false;
        return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        lightColor = Color.White;
        Texture2D tex = Helper.GetTexture(Texture + "_Bloom").Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        var fadeMult = 1f / Projectile.oldPos.Count();
        for (int i = 0; i < Projectile.oldPos.Count(); i++)
        {
            float mult = (1 - i * fadeMult);
            Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Maroon * (mult * 0.8f), Projectile.oldRot[i], tex.Size() / 2, Projectile.scale * .9f, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return true;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.netUpdate = true;
        Projectile.velocity = Vector2.Zero;
        savedP = int.MaxValue;
        Projectile.aiStyle = -1;
        return false;
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
    bool shouldDie = false;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(savedP);
        writer.Write(shouldDie);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        savedP = reader.ReadSingle();
        shouldDie = reader.ReadBoolean();
    }
    public override void AI()
    {
        if (savedP == 0)
        {
            Projectile.netUpdate = true;
            savedP = Main.player[Projectile.owner].Center.Y;
            Projectile.netUpdate = true; // TEST
        }
        Projectile.tileCollide = Projectile.Center.Y > savedP - 20;
        if (savedP == int.MaxValue)
        {
            Projectile.netUpdate = true;
            Projectile.aiStyle = -1;
            Projectile.velocity = Vector2.Zero;
        }
        Dust.NewDustPerfect(Projectile.Center - new Vector2(-8, 15).RotatedBy(Projectile.rotation), DustID.Torch);
        if (Projectile.timeLeft < 40 && !shouldDie)
        {
            Projectile.netUpdate = true;
            MPUtils.NewProjectile(NPC.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<ChargeUp>(), 0, 0, ai2: 0.4f);
            Projectile.velocity = Vector2.Zero;
            shouldDie = true;
        }
    }
    public override void OnKill(int timeLeft)
    {
        Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSpriteHostile>(), Projectile.damage, 0, Projectile.owner);
    }
}
