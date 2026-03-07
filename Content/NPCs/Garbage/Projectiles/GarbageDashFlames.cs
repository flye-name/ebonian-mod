using System;
using EbonianMod.Common.Graphics;
using EbonianMod.Content.Dusts;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageDashFlames : ModProjectile
{
	public override string Texture => Helper.Empty;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 100;
	}

	public override bool ShouldUpdatePosition() => false;

	public override void AI()
	{
		Collision.StepDown(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
		Vector2 bottomLeft = Helper.Raycast(Projectile.BottomLeft, Vector2.UnitY, 32).Point;
		Vector2 bottomRight = Helper.Raycast(Projectile.BottomRight, Vector2.UnitY, 32).Point;
		
		float lowestY = MathF.Max(bottomLeft.Y, bottomRight.Y);

		int attempts = 0;
		while (lowestY > Projectile.Bottom.Y && attempts++ < 100)
			Projectile.Bottom = new Vector2(Projectile.Bottom.X, lowestY);

		attempts = 0;
		while (!Helper.Raycast(Projectile.Center - new Vector2(0, 10), Vector2.UnitY, 2).Success && attempts++ < 100)
			Projectile.Center += Vector2.UnitY;
		
		if (Projectile.ai[2] == 0)
			Projectile.ai[2] = Main.rand.NextFloat(0.2f, 0.4f);

		if (Projectile.ai[1] == 0)
			Projectile.ai[1] = Main.rand.NextFloat(0.7f, 1.2f);
		Projectile.ai[0] = MathHelper.Lerp(Projectile.ai[0], 1, 0.1f);
		if (Main.rand.NextBool(4))
			Dust.NewDustPerfect(Projectile.Top + new Vector2(Main.rand.NextFloat(-25, 25f), Main.rand.NextFloat(-8f, -2f) * Projectile.scale), ModContent.DustType<LineDustFollowPoint>(), Projectile.scale * new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-3, -1)), Scale: Main.rand.NextFloat(0.03f, 0.1f), newColor: Color.OrangeRed);
		Projectile.scale = MathHelper.Clamp(MathHelper.SmoothStep(0, 2, Projectile.timeLeft / 100f), 0.1f, 2);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = Assets.Extras.starshowerNoise.Value;
		Texture2D tex2 = Assets.Extras.swirlyNoise.Value;
		Texture2D tex3 = Assets.Extras.gradation2.Value;
		Texture2D tex4 = Assets.Extras.explosion.Value;
		
		GarbageFlameRendering.DrawCache.Add(() =>
		{
			Main.spriteBatch.End(out var ss);
			Main.spriteBatch.Begin(ss with { sortMode = SpriteSortMode.Immediate, blendState = BlendState.Additive, effect = Effects.flameGround.Value, samplerState = SamplerState.PointWrap });
		
			Effects.flameGround.Value.Parameters["uTime"].SetValue(MathF.Floor((-Main.GlobalTimeWrappedHourly * Projectile.ai[1] * 1.5f + Projectile.whoAmI * 80) * 11f) / 11f);
			Effects.flameGround.Value.Parameters["uIntensity"].SetValue(0.08f * Projectile.scale);
			Effects.flameGround.Value.Parameters["uScale"].SetValue(1.3f);
			Effects.flameGround.Value.Parameters["uColorOverride"].SetValue(new Vector4(0.85f, 0.3f, 0.1f, 1));
			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, MathF.PI, new Vector2(tex.Width / 2f, 8), new Vector2(1, Projectile.ai[0]) * Projectile.ai[2] * 0.15f, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(tex2, Projectile.Center - Main.screenPosition, null, Color.White, MathF.PI, new Vector2(tex2.Width / 2f, 8), new Vector2(1, Projectile.ai[0]) * Projectile.ai[2] * 0.15f, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(ss with { samplerState = SamplerState.PointClamp });
			
			Main.EntitySpriteDraw(tex3, Projectile.Center + Projectile.GFX() - Main.screenPosition, new Rectangle(0, 0, tex3.Width, tex3.Height), Color.OrangeRed * 0.1f * Projectile.scale, MathF.PI, new Vector2(tex3.Width / 2f, 8), new Vector2(0.5f, Projectile.ai[0]) * Projectile.ai[2] * .4f, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(tex4, Projectile.Center + Projectile.GFX() - Main.screenPosition, null, Color.OrangeRed * 0.2f * Projectile.scale, 0, new Vector2(tex4.Width / 2f, tex4.Height / 2f), new Vector2(1.5f, 0.3f * Projectile.ai[0]) * Projectile.ai[2] * .4f, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(ss);
		});
		return false;
	}
}