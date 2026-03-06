/*namespace EbonianMod.Common.Graphics.RenderTargets;

public class BlurRendering : BaseCachedActionRenderTarget<BlurRendering>
{
	public override int TargetAmount => 2;
	public override void PopulateTarget()
	{
		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[0]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);

		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawCache.InvokeAllAndClear();
		Main.spriteBatch.End();
	}

	public override void DrawTarget()
	{
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		Effects.Test2.Value.CurrentTechnique.Passes[0].Apply();
		Effects.Test2.Value.Parameters["tex0"].SetValue(Targets[0]);
		Effects.Test2.Value.Parameters["i"].SetValue(0.02f);
		Main.spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
		Main.spriteBatch.End();
	}
}*/ // Unused