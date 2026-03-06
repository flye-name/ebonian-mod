using Terraria.Graphics.Effects;

namespace EbonianMod.Common.Graphics.RenderTargets;

public class GlobalRenderTargetRendering : ModSystem
{
	public delegate void Render();
	public static event Render PopulateTargets;
	public static event Render DrawTargets;
	
	public override void Load()
	{
		if (!Main.dedServ)
			Main.QueueMainThreadAction(() =>
			{
				Main.graphics.GraphicsDevice.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
				Main.graphics.ApplyChanges();
			});
		
		On_Main.DrawPlayers_AfterProjectiles += (orig, self) => 
		{
			orig(self);
			
			if (Main.gameMenu) return;

			RenderTargetBinding[] old = Main.instance.GraphicsDevice.GetRenderTargets();
			foreach (RenderTargetBinding target in old)
			{
				if (target.RenderTarget is RenderTarget2D rt)
					rt.RenderTargetUsage = RenderTargetUsage.PreserveContents;
			}
			
			PopulateTargets?.Invoke();
			Main.instance.GraphicsDevice.SetRenderTargets(old);
			DrawTargets?.Invoke();
		};
	}
}