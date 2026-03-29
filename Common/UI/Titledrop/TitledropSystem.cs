using System.Collections.Generic;

namespace EbonianMod.Common.UI.Titledrop;

[Autoload(Side = ModSide.Client)]
public class TitledropSystem : ModSystem
{
	public static TitledropSystem Instance => ModContent.GetInstance<TitledropSystem>(); 
	
	public TitledropStyle Style;
	public bool Active;


	public static void SetStyle(TitledropStyle style)
	{
		Instance.Style = style;
		Instance.Style.Activate();
		
		Instance.Active = true;
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int layer = layers.FindIndex(layer => layer.Name.Equals("Vanilla: MP Player Names"));
		
		layers.Insert(layer, new LegacyGameInterfaceLayer("EbonianMod: Titledrop", () =>
		{
			if (Active)
				Style?.Draw();
			
			return true;
		}, InterfaceScaleType.UI));
	}

	public override void PostUpdateDusts()
	{
		if (Active)
			Style?.Update();
	}
	
	public override void OnWorldUnload() => Active = false;
}