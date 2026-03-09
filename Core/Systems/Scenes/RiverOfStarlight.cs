using EbonianMod.Content.Projectiles.AsteroidShower;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Effects;

namespace EbonianMod.Core.Systems.Scenes;
public class RiverOfStarlight : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Meteor");
    public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
    public static bool ActiveConditions => !Main.dayTime && Star.starfallBoost > 3.2f && !Main.bloodMoon && Main.invasionType == 0;
    public override bool IsSceneEffectActive(Player player)
    {
        return ActiveConditions && (player.ZoneOverworldHeight || player.ZoneSkyHeight);
    }
    public override void SpecialVisuals(Player player, bool isActive)
    {
        if (Main.dedServ) return;
        if (IsSceneEffectActive(player))
        {
            if (!SkyManager.Instance["Asteroid"].IsActive())
            {
                SkyManager.Instance.Activate("Asteroid");
            }
            if (player.ZoneOverworldHeight || player.ZoneSkyHeight)
            {
                Filters.Scene["Asteroid"].GetShader().UseColor(Color.Blue).UseOpacity(0.1f);
            }
            player.ManageSpecialBiomeVisuals("Asteroid", isActive);
        }
        else
        {
            if (SkyManager.Instance["Asteroid"].IsActive())
            {
                SkyManager.Instance.Deactivate("Asteroid");
            }
            player.ManageSpecialBiomeVisuals("Asteroid", false);
        }
    }
}

public class RiverOfStarlightPlayer : ModPlayer
{
    public override void PostUpdate()
    {
        if (RiverOfStarlight.ActiveConditions ? Main.rand.NextBool(12000) : false)
        {
            MPUtils.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(1920 * Main.rand.NextFloat() - 960, -3000), new Vector2(Main.rand.NextFloat(-1, 1), 20f), ModContent.ProjectileType<FallingStarBig>(), 2000, 0);
        }
        if (RiverOfStarlight.ActiveConditions ? Main.rand.NextBool(600) : false)
        {
            MPUtils.NewProjectile(Player.GetSource_FromThis(), Player.Center + new Vector2(1920 * Main.rand.NextFloat() - 960, -2500), new Vector2(Main.rand.NextFloat(-10, 10), 20f), ModContent.ProjectileType<FallingStarTiny>(), 10, 0);
        }
    }
}