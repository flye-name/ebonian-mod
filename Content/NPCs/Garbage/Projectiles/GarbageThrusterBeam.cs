using System;
using System.Collections.Generic;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageThrusterBeam : ModProjectile
{
	public override string Texture => Helper.Empty;
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1500;
	}
	public override void SetDefaults()
	{
		Projectile.Size = Vector2.One;
		Projectile.tileCollide = false;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 30;
	}
	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		float a = 0f;
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity.ToRotation().ToRotationVector2() * Projectile.ai[1], 30, ref a) && Projectile.scale > 0.5f;
	}

	public override void AI()
	{
		Projectile.ai[0] = Utils.GetLerpValue(30, 0, Projectile.timeLeft, true);
		Projectile.ai[1] = MathHelper.SmoothStep(0, 400, Projectile.ai[0]);
	}

	public override bool PreDraw(ref Color lightColor)
	{	
		GarbageFlameRendering.DrawCache.Add(() =>
		{
			float length = Clamp(Projectile.ai[1], 10, 400);
			List<VertexPositionColorTexture>[] vertices = [new(), new() ];

			for (int k = 0; k < 2; k++)
			for (float i = 0; i < 1f; i += 0.1f)
			{
				Vector2 position = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * length * i - Main.screenPosition;
				Color color = Color.Lerp(Color.White * (1f - i) * 0.7f, Color.OrangeRed, k) * MathF.Pow(MathF.Sin(i * MathF.PI) * MathF.Sin(Projectile.ai[0] * MathF.PI), 2);
				for (int j = -1; j < 2; j += 2)
				{
					Vector2 vPosition = position + new Vector2(Projectile.ai[0] * 20 + 40 * i * k, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2 * j);
					vertices[k].Add(Helper.AsVertex(vPosition, color, new Vector2(i - Main.GlobalTimeWrappedHourly * 3, j < 0 ? 0 : 1)));
				}
			}
			
			Helper.DrawTexturedPrimitives(vertices[1].ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.trail_04);
			Helper.DrawTexturedPrimitives(vertices[1].ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.FlamesSeamless);
			Helper.DrawTexturedPrimitives(vertices[0].ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.Tentacle);
		});
		
		return false;
	}
}