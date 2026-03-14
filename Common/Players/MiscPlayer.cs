using EbonianMod.Content.Items.Weapons.Melee;
using EbonianMod.Content.NPCs.Crimson.Fleshformator;

namespace EbonianMod.Common.Players;

public class MiscPlayer : ModPlayer
{
    public int fleshformators, consistentTimer;
    public override void ResetEffects()
    {
        if (!NPC.AnyNPCs(NPCType<Fleshformator>()))
            fleshformators = 0;
    }
    public override void PreUpdateMovement()
    {
        if (fleshformators > 0)
            Player.gravDir = 1f;
    }

    public override bool PreItemCheck()
    {
        if (fleshformators > 0)
        {
            Player.controlUseItem = false;
            Player.controlUseTile = false;
            Player.controlThrow = false;
            return false;
        }

        return base.PreItemCheck();
    }

    public override void PostUpdateMiscEffects()
    {
        consistentTimer++;
    }
}