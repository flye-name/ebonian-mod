using System;
using Humanizer;
using ReLogic.Graphics;

namespace EbonianMod.Common.UI.Titledrop;

public class HotGarbageTitledrop : TitledropStyle, ILoadable
{
	public static HotGarbageTitledrop Instance;

	public void Load(Mod mod)
	{
		Instance = new HotGarbageTitledrop();
	}

	public void Unload()
	{
		Instance = null;
	}

	public LocalizedText Name => Language.GetText("Mods.EbonianMod.NPCs.HotGarbage.DisplayName"); 
	
	private const int MaxTime = 300;
	private int time;
	private float movementTimer;

	public override void Activate()
	{
		time = 0;
		movementTimer = 0;
	}

	public override void Update()
	{
		time++;

		movementTimer += 0.08f * movementTimer.SafeDivision();
		if (movementTimer > 2)
			movementTimer = 0;
		
		if (time >= 120)
			TitledropSystem.Instance.Active = false;
	}

	public override void Draw()
	{
		float progress = Utils.GetLerpValue(0, 120, time);
		float factor = MathF.Sin(progress * MathF.PI);
		float baseAlpha = MathHelper.Clamp(MathF.Pow(factor, 5) * 16, 0, 1);
		
		Texture2D hazard = Assets.Extras.hazardUnblurred.Value;
		Texture2D textGlow = Assets.Extras.textGlow.Value;
		Texture2D exclamation = Assets.Extras.exclamation.Value;

		Vector2 randOffset() => Main.rand.NextVector2Circular(15, 15) * factor;
		
        for (int k = 0;  k < 2; k++) 
        {
	        Main.spriteBatch.Draw(textGlow, new Vector2(Main.screenWidth / 2f, Main.screenHeight * 0.16f), null, Color.Black * baseAlpha, 0, textGlow.Size() / 2f, new Vector2(10, 6), SpriteEffects.None, 0);
	        Color color = k == 0 ? Color.Black : (Color.Maroon with { A = 0 });
        	
	        for (int i = -(int)(Main.screenWidth / hazard.Width); i < (int)(Main.screenWidth / hazard.Width); i++)
	        {
		        float hazardPositionX = Main.screenWidth / 2f + (i * hazard.Width);
		        float hazardOffsetX = hazard.Width * movementTimer;
		        float direction = progress > 0.5f ? 1 : -1;
		        
		        for (int j = 0; j < 2; j++)
		        {
			        float alpha = MathF.Pow(1 - MathF.Abs(i) / (Main.screenWidth / (float)hazard.Width), 3) * factor;
			        Main.spriteBatch.Draw(hazard, randOffset() + new Vector2(hazardPositionX + hazardOffsetX, Main.screenHeight * 0.11f), null, color * 2 * alpha, 0, hazard.Size() / 2f, 1, SpriteEffects.None, 0);
			        Main.spriteBatch.Draw(hazard, randOffset() + new Vector2(hazardPositionX - hazardOffsetX, Main.screenHeight * 0.25f), null, color * 2 * alpha, 0, hazard.Size() / 2f, 1, SpriteEffects.None, 0);
			        
			        if (k == 1) 
			        {
						Main.spriteBatch.Draw(hazard, randOffset() * 5 + new Vector2(hazardPositionX + hazardOffsetX, Main.screenHeight * 0.11f), null, color * 0.2f * alpha, 0, hazard.Size() / 2f, 3, SpriteEffects.None, 0);
						Main.spriteBatch.Draw(hazard, randOffset() * 5 + new Vector2(hazardPositionX - hazardOffsetX, Main.screenHeight * 0.25f), null, color * 0.2f * alpha, 0, hazard.Size() / 2f, 3, SpriteEffects.None, 0);
			        }
	
			        Vector2 exPos = randOffset() + new Vector2(Main.screenWidth / 2f + i * exclamation.Width + movementTimer * exclamation.Width * (i < 0 ? 1 : -1), Main.screenHeight * 0.178f);
			        Main.spriteBatch.Draw(exclamation, exPos, null, color * 2 * alpha * Lerp(0, 2, Clamp(MathF.Abs(exPos.X - Main.screenWidth / 2f) / 5000, 0, 1)), 0, exclamation.Size() / 2f, 0.1f, SpriteEffects.None, 0);
		        }
	        }
        	
	        DynamicSpriteFontExtensionMethods.DrawString(Main.spriteBatch, FontAssets.DeathText.Value, Name.Value, randOffset() + new Vector2(Main.screenWidth / 2f - FontAssets.DeathText.Value.MeasureString(Name.Value).X / 2, Main.screenHeight * 0.15f), color * 2 * MathHelper.Clamp(baseAlpha * 10, 0, 1));
        }
	}
}