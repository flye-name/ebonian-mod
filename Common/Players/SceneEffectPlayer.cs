using EbonianMod.Content.Items.Consumables.Food;
using EbonianMod.Content.NPCs.Aureus;

namespace EbonianMod.Common.Players;

public class SceneEffectPlayer : ModPlayer
{
    public override void PostUpdateMiscEffects()
    {
        Player.ManageSpecialBiomeVisuals("EbonianMod:Conglomerate", Player.HasBuff<ConglomerateEnergyBuff>()); // add npc here later when we add conglo
        Player.ManageSpecialBiomeVisuals("EbonianMod:Aureus", NPC.AnyNPCs(NPCType<Aureus>()));
    }
}