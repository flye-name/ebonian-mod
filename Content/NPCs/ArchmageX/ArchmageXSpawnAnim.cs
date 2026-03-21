using System.Collections.Generic;
using EbonianMod.Content.Projectiles.ArchmageX;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Core.Systems.Boss;
using EbonianMod.Core.Systems.Cinematic;

namespace EbonianMod.Content.NPCs.ArchmageX;

public class ArchmageXSpawnAnim : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetStaticDefaults()
    {
        FinalDrawSystem.ProjectileTypeList.Add(Type);
    }
    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(2, 2);
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 400;
    }
    Rectangle GetArenaRect()
    {
        Vector2 sCenter = Projectile.Center;
        float LLen = Helper.Raycast(sCenter, -Vector2.UnitX, 29f * 16).RayLength;
        float RLen = Helper.Raycast(sCenter, Vector2.UnitX, 29f * 16).RayLength;
        Vector2 U = Helper.Raycast(sCenter, -Vector2.UnitY, 380).Point;
        Vector2 D = Helper.Raycast(Projectile.Center, Vector2.UnitY, 380).Point;
        sCenter.Y = U.Y + Helper.FromAToB(U, D, false).Y * 0.5f;
        Vector2 L = sCenter;
        Vector2 R = sCenter;
        if (LLen > RLen)
        {
            R = Helper.Raycast(sCenter, Vector2.UnitX, 29f * 16).Point;
            L = Helper.Raycast(R - new Vector2(16, 0), -Vector2.UnitX, 34.5f * 32).Point;
        }
        else
        {
            R = Helper.Raycast(L + new Vector2(16, 0), Vector2.UnitX, 34.5f * 32).Point;
            L = Helper.Raycast(sCenter, -Vector2.UnitX, 29f * 16).Point;
        }
        Vector2 TopLeft = new Vector2(L.X, U.Y);
        Vector2 BottomRight = new Vector2(R.X, D.Y);
        Rectangle rect = new Rectangle((int)L.X, (int)U.Y, (int)Helper.FromAToB(TopLeft, BottomRight, false).X, (int)Helper.FromAToB(TopLeft, BottomRight, false).Y);
        return rect;
    }
    public override void OnKill(int timeLeft)
    {
        XareusSystem.xareusFightCooldown = 500;
        for (int i = 0; i < 15; i++)
            MPUtils.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(10, 20), ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));

        NPC.NewNPCDirect(null, Projectile.Center, NPCType<ArchmageX>());
    }
    public override bool PreDraw(ref Color lightColor)
    {
        XareusRendering.DrawCache.Add(() =>
        {
            Texture2D tex3 = Assets.Extras.crosslight.Value;
            for (int i = 0; i < 4; i++)
            {
                Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * (glareAlpha), 0, tex3.Size() / 2, (glareAlpha) * .6f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * (glareAlpha * 0.5f), 0, tex3.Size() / 2, (glareAlpha) * 1.6f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * (glareAlpha * 0.1f) * 10, 0, tex3.Size() / 2, (glareAlpha) * 2f, SpriteEffects.None, 0);
            }
        });
        return false;
    }
    float glareAlpha;
    private List<int> PlayersFighting = new List<int>();
    public override void AI()
    {
        if (Projectile.timeLeft > 20)
            glareAlpha = MathHelper.SmoothStep(0, 1, Utils.GetLerpValue(400, 0, Projectile.timeLeft));
        else
            glareAlpha = MathHelper.Lerp(glareAlpha, 0, 0.05f);

        foreach (Player player in Main.ActivePlayers)
        {
            if (GetArenaRect().Size().Length() > 100)
            {
                float dist = player.Distance(GetArenaRect().Center());
                if (dist > 1200 && PlayersFighting.Contains(player.whoAmI))
                {
                    Helper.TPNoDust(GetArenaRect().Center(), player);
                }
                else if (dist <= 550) 
                {
                    if (!PlayersFighting.Contains(player.whoAmI))
                        PlayersFighting.Add(player.whoAmI);

                    while (player.Center.X < GetArenaRect().X)
                        player.Center += Vector2.UnitX * 2;

                    while (player.Center.X > GetArenaRect().X + GetArenaRect().Width)
                        player.Center -= Vector2.UnitX * 2;

                    while (player.Center.Y < GetArenaRect().Y)
                        player.Center += Vector2.UnitY * 2;
                }
            }
        }
        
        Projectile.scale = MathHelper.Lerp(Projectile.scale, 4, 0.01f);
        int fac = 12;
        if (Projectile.timeLeft < 140)
            fac = 1;
        else if (Projectile.timeLeft < 205)
            fac = 3;
        else if (Projectile.timeLeft < 275)
            fac = 5;
        else if (Projectile.timeLeft < 300)
            fac = 7;
        else if (Projectile.timeLeft < 320)
            fac = 8;
        else if (Projectile.timeLeft < 360)
            fac = 9;


        if (Projectile.timeLeft % fac == 0 && Projectile.timeLeft > 35)
        {
            MPUtils.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 2) * Projectile.scale, ProjectileType<XCloudVFXExtra>(), 0, 0);
        }


        if (fac < 9 && Projectile.timeLeft % fac == 0 && Projectile.timeLeft > 5)
            for (int i = 0; i < 2; i++)
            {
                Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, MathHelper.Clamp(10 - fac, 3, 10)) * (Projectile.scale * 0.5f);
                MPUtils.NewProjectile(null, Projectile.Center - vel * 2, vel, ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));
            }


        if (Projectile.timeLeft < 300)
        {
            if (Projectile.timeLeft % 50 == 0)
            {
                Vector2 pos = Projectile.Center + new Vector2(Main.rand.NextFloat(-280, 280), 0);
                float off = Main.rand.NextFloat(-1, 1);
                //Projectile.NewProjectile(null, pos, Vector2.UnitY.RotatedBy(off), ProjectileType<XLightningBolt>(), 0, 0, ai1: 1);
                //Projectile.NewProjectile(null, pos, -Vector2.UnitY.RotatedBy(off), ProjectileType<XLightningBolt>(), 0, 0, ai1: 1);
            }
        }
        if (Projectile.timeLeft == 399)
        {
            SoundEngine.PlaySound(Sounds.buildup, Projectile.Center);
            //CameraSystem.ChangeCameraPos(Projectile.Center, 460, null, 2, InOutQuint);
        }
        if (Projectile.timeLeft == 299)
            CameraSystem.ChangeZoom(120, new ZoomInfo(1.4f, 1.1f, InOutCirc, Rigid1, true));
        if (Projectile.timeLeft == 199)
            CameraSystem.ChangeZoom(220, new ZoomInfo(2, 1.1f, InOutCirc, Rigid1, true, true));
        if (Projectile.timeLeft == 130)
            SoundEngine.PlaySound(Sounds.BeamWindUp.WithPitchOffset(-0.5f), Projectile.Center);

        if (Projectile.timeLeft == 40)
        {
            XareusSystem.xareusFightCooldown = 0;
            MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<ArchmageChargeUp>(), 0, 0);
        }
        if (Projectile.timeLeft == 25)
            CameraSystem.ChangeZoom(40, new ZoomInfo(2, 1.1f, Rigid1, InOutSine, true));
    }
}
public class SpawnAnimMusic : ModBiome
{
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/ambience");
    public override bool IsBiomeActive(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<ArchmageXSpawnAnim>()] > 0;
    }
}
