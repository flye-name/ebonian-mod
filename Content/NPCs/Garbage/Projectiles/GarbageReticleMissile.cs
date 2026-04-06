using System;
using System.Collections.Generic;
using EbonianMod.Content.Dusts;
using EbonianMod.Content.Projectiles.VFXProjectiles;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageReticleMissile : ModProjectile
{
	public override string Texture => "Terraria/Images/Item_" + ItemID.MiniNukeII;

	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailingMode[Type] = 2;
		ProjectileID.Sets.TrailCacheLength[Type] = 5;

		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 6000;
	}

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 350;
	}
	
	public Vector2 Target => new Vector2(Projectile.ai[0], Projectile.ai[1]);

	public Vector2 ReticleScale;
	public float ReticleRotation, OldReticleRotation;
	
	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Texture2D reticle = Assets.Extras.crosshair.Value;
		Texture2D reticle2 = Assets.Extras.crosshair_transparent.Value;

		
		
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None);

		if (ReticleScale.Length() > 0f)
		{
			Main.EntitySpriteDraw(reticle2, Target  - Main.screenPosition, null, Color.Black, ReticleRotation, reticle.Size() / 2f, ReticleScale * 0.5f, SpriteEffects.None);
			Main.EntitySpriteDraw(reticle, Target - Main.screenPosition, null, Color.Lerp(Color.White, Color.Red, (Main.GameUpdateCount + Projectile.whoAmI) % 10 < 5 ? 1 : 0) with { A = 0 }, ReticleRotation, reticle.Size() / 2f, ReticleScale * 0.5f, SpriteEffects.None);
		}
		
		return false;
	}

	public bool ShouldBeep;
	public override void AI()
	{
		if (Projectile.ai[2] < -1)
			ShouldBeep = true;
		
		Projectile.rotation = Projectile.velocity.ToRotation();
		
		Dust.NewDustPerfect(Projectile.Center, DustType<GarbageFlameDust>(), Projectile.velocity * -0.2f, newColor: Color.Red * 0.4f, Scale: 0.15f);
		Projectile.ai[2]++;

		if (Projectile.ai[2] < 50 && Projectile.velocity.Length() < 25f)
			Projectile.velocity *= 1.05f;
		
		if (ShouldBeep && Projectile.ai[2] % (Projectile.ai[2] < 140 ? 10 : 5) == 0 && Projectile.ai[2] > 50)
			SoundEngine.PlaySound(Sounds.garbageBeep.WithVolumeScale(0.4f).WithPitchOffset(-0.2f), Target);
		
		if (Projectile.ai[2] > 80)
		{
			ReticleScale.X = MathHelper.Lerp(ReticleScale.X, 0.5f, 0.1f);
			if (ReticleScale.X < 0.4f)
				ReticleScale.Y = MathHelper.Lerp(ReticleScale.Y, 0.05f, 0.1f);
			else
				ReticleScale.Y = MathHelper.Lerp(ReticleScale.Y, 0.5f, 0.1f);

			if (ReticleScale.Y > 0.48f)
			{
				if (Projectile.ai[2] % 30 < 9)
					OldReticleRotation = ReticleRotation;
				else
					ReticleRotation = MathHelper.SmoothStep(OldReticleRotation, OldReticleRotation + MathHelper.PiOver4, (Projectile.ai[2] % 30 - 10) / 20f);
			}

			UnifiedRandom rand = new UnifiedRandom(Projectile.identity);
			float progress = MathF.Sin(Utils.GetLerpValue(80, 200, Projectile.ai[2], true) * MathF.PI);
			Projectile.ai[0] += rand.NextFloat(-15, 15) * progress;
			Projectile.ai[1] += rand.NextFloat(-1, 1) * progress * rand.NextBool().ToInt();
		}

		if ((int)Projectile.ai[2] == 185)
			SoundEngine.PlaySound(Sounds.firework, Projectile.Center);
		
		if (Projectile.ai[2] > 130)
		{
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Target - Projectile.Center).SafeNormalize(Vector2.UnitY) * 25, 0.1f);

			if (Target.Distance(Projectile.Center) < 40f)
			{
				MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FlameExplosionWSpriteHostile>(), 15, 0);
				Projectile.Kill();
			}
		}
	}
}