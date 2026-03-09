using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;

namespace EbonianMod.Content.Skies;
public class RiverOfStarlightSky : CustomSky
{
    public bool isActive;
    public float Intensity;
    public Star[] stars;
    public struct Star
    {
        public Asset<Texture2D> texture;
        public Vector2 pos;
        public float depth;
    }
    public override void Activate(Vector2 position, params object[] args)
    {
        isActive = true;
        stars = new Star[100];
        for (int i = 0; i < stars.Length; i++)
        {
            int variant = Main.rand.Next(2);
            if (Main.rand.NextBool(50))
                variant = 2;
            stars[i].texture = variant switch
            {
                0 => Assets.ExtraSprites.star0,
                1 => Assets.ExtraSprites.star1,
                _ => Assets.ExtraSprites.star2,
            };
            stars[i].pos = new Vector2(Main.rand.NextFloat(Main.screenWidth), Main.rand.NextFloat(Main.screenHeight * 0.25f));
            if (variant != 2)
                stars[i].depth = Main.rand.NextFloat(0.1f, 0.5f);
            else
                stars[i].depth = Main.rand.NextFloat(0.5f, 0.7f);
        }
    }
    public override void Deactivate(params object[] args)
    {
        isActive = false;
    }
    public override void Reset()
    {
        isActive = false;
    }
    public override void Update(GameTime gameTime)
    {
        if (isActive)
        {
            Intensity = Math.Min(1f, 0.01f + Intensity);
        }
        else
        {
            Intensity = Math.Max(0f, Intensity - 0.01f);
        }

    }
    public override bool IsActive()
    {
        return Intensity > 0;
    }


    public override float GetCloudAlpha()
    {
        return Lerp(1, 0.1f, Intensity);
    }
    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        if (maxDepth >= 3.40282347E+38f && minDepth < 3.40282347E+38f)
        {
            spriteBatch.Snapshot(out var sbParams);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, sbParams.depthStencilState, sbParams.rasterizerState, null, sbParams.matrix);
            Texture2D Tex = Assets.Extras.gradation3.Value;
            Texture2D Tex2 = Assets.Extras.swirlyNoise.Value;
            Texture2D Tex3 = Assets.Extras.gradation2.Value;
            Vector2 Pos = new(Main.screenWidth / 2, Main.screenHeight / 2);

            for (int i = 0; i < stars.Length; i++)
            {
                stars[i].pos.X += stars[i].depth * 3;
                if (stars[i].pos.X > Main.screenWidth + 100)
                {
                    int variant = Main.rand.Next(2);
                    if (Main.rand.NextBool(50))
                        variant = 2;
                    stars[i].texture = variant switch
                    {
                        0 => Assets.ExtraSprites.star0,
                        1 => Assets.ExtraSprites.star1,
                        _ => Assets.ExtraSprites.star2,
                    };
                    if (variant != 2)
                        stars[i].depth = Main.rand.NextFloat(0.1f, 0.5f);
                    else
                        stars[i].depth = Main.rand.NextFloat(0.5f, 0.7f);
                    stars[i].pos.X = -100;
                    stars[i].pos.Y = Main.rand.NextFloat(Main.screenHeight * 0.25f);
                }
                spriteBatch.Draw(stars[i].texture.Value, stars[i].pos, null, Color.White * Intensity * 2 * stars[i].depth, Main.GameUpdateCount * 0.01f * stars[i].depth, stars[i].texture.Value.Size() / 2, stars[i].depth, SpriteEffects.None, 0);

            }
            int yOff = (int)Lerp(20, -70, Clamp(Main.LocalPlayer.Center.Y / 6000f, 0, 1));

            spriteBatch.Draw(Tex, new Rectangle(0, 0 - (int)Main.screenPosition.Y, Main.screenWidth, 3500), null, Color.DodgerBlue * Intensity * 0.35f, 0, Vector2.Zero, SpriteEffects.None, 0);

            spriteBatch.Draw(Tex3, new Rectangle(0, -50, Main.screenWidth, Main.screenHeight + 200), null, Color.DodgerBlue * Intensity * 0.8f, 0, Vector2.Zero, SpriteEffects.None, 0);

            Effects.starlightRiver.Value.CurrentTechnique.Passes[0].Apply();
            Effects.starlightRiver.Value.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            Effects.starlightRiver.Value.Parameters["colMult"].SetValue(2 * Intensity);
            spriteBatch.Draw(Tex2, new Rectangle(0, -50 + yOff, Main.screenWidth, Main.screenHeight + 300), null, Color.DodgerBlue * Intensity * 0.4f, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
            spriteBatch.End();
            spriteBatch.ApplySaved(sbParams);
        }
    }

    public override Color OnTileColor(Color inColor)
    {
        Vector4 value = inColor.ToVector4();
        return new Color(Vector4.Lerp(value, Vector4.One, Intensity * 0.2f));
    }
}
