using Terraria.Graphics.Shaders;

namespace EbonianMod.Core.Utilities;

public class RenderingUtils
{
	public static void CreateRender(ref RenderTarget2D target)
	{
		if (target is not null)
			if (!target.IsDisposed)
				target.Dispose();
		target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
		target.RenderTargetUsage = RenderTargetUsage.PlatformContents;
	}
	
	public static readonly BlendState Subtractive = new BlendState
	{
		ColorSourceBlend = Blend.SourceAlpha,
		ColorDestinationBlend = Blend.One,
		ColorBlendFunction = BlendFunction.ReverseSubtract,
		AlphaSourceBlend = Blend.SourceAlpha,
		AlphaDestinationBlend = Blend.One,
		AlphaBlendFunction = BlendFunction.ReverseSubtract
	};
	public static void DrawWithDye(SpriteBatch spriteBatch, DrawData data, int dye, Entity entity, bool Additive = false)
	{
		spriteBatch.End(out var sbParams);
		spriteBatch.Begin(SpriteSortMode.Immediate, Additive ? BlendState.Additive : BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
		GameShaders.Armor.GetShaderFromItemId(dye).Apply(null, data);
		data.Draw(Main.spriteBatch);
		spriteBatch.End();
		spriteBatch.Begin(sbParams);
	}
}