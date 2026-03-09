using EbonianMod.Content.Items.Consumables.BossItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Content.Items.Misc;
public class RoSTest : ModItem
{
    public override string Texture => Helper.Placeholder;
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = 1;
        Item.value = 1000;
        Item.rare = ItemRarityID.Blue;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.noUseGraphic = true;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.consumable = false;
        Item.useTurn = false;
    }
    public override bool? UseItem(Player player)
    {
        if (player.whoAmI == Main.myPlayer)
        {
            if (Main.mouseRight)
                Star.starfallBoost = 4;
            else
                Star.starfallBoost = 0;
            NetMessage.SendData(MessageID.WorldData);
        }
        return base.UseItem(player);
    }
}
