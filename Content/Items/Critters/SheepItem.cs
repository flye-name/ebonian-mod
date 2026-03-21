using EbonianMod.Content.NPCs.Overworld.Critters;

namespace EbonianMod.Content.Items.Critters;

public class SheepItem : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Critters/SheepItem";
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Bunny);
        Item.makeNPC = NPCType<Sheep>();
        Item.value = Item.buyPrice(0, 0, 10, 0);
    }
}
public class SheepItemNaked : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Critters/SheepItemNaked";
    public override void SetDefaults()
    {
        Item.CloneDefaults(ItemID.Bunny);
        Item.makeNPC = 0;
        Item.value = Item.buyPrice(0, 0, 10, 0);
    }
    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer && player.IsInTileInteractionRange(Main.MouseWorld.ToTileCoordinates().X, Main.MouseWorld.ToTileCoordinates().Y, TileReachCheckSettings.Simple))
        {
            MPUtils.NewNPC(Main.MouseWorld, NPCType<Sheep>(), ai3: 10);
            return true;
        }

        return false;
    }
}
