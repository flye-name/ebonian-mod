using EbonianMod.Common.Players;
using EbonianMod.Content.Items.BossTreasure;
using EbonianMod.Content.Items.Materials;

namespace EbonianMod.Content.Items.Armor.Cecitoma;
[AutoloadEquip(EquipType.Head)]
public class Terrorhead : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Armor/Cecitoma/Terrorhead";
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.defense = 8;
        Item.rare = ItemRarityID.LightRed;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<CorruptEyeMaterial>(), 12).AddIngredient(ItemType<TerrortomaMaterial>(), 1).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemType<Terrorbody>() && legs.type == ItemType<Terrorlegs>();
    }
    public const float DmgIncrease = 0.06f;
    public const float CritIncrease = 0.025f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DmgIncrease * 100, CritIncrease * 100);
    public override void UpdateEquip(Player player)
    {
        player.magicDamage += DmgIncrease;
        player.magicCrit += CritIncrease;
    }
    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = this.GetLocalization("SetBonus").Value;
        player.GetModPlayer<SetBonusPlayer>().terrortoma = true;
    }
}
[AutoloadEquip(EquipType.Body)]
public class Terrorbody : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Armor/Cecitoma/Terrorbody";
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.defense = 16;
        Item.rare = ItemRarityID.LightRed;
    }
    public const float DmgIncrease = 0.07f;
    public const int ManaIncrease = 100;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DmgIncrease * 100, ManaIncrease);
    public override void UpdateEquip(Player player)
    {
        player.magicDamage += DmgIncrease;
        player.statManaMax2 += ManaIncrease;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<CorruptEyeMaterial>(), 16).AddIngredient(ItemType<TerrortomaMaterial>(), 2).AddTile(TileID.MythrilAnvil).Register();
    }
}
[AutoloadEquip(EquipType.Legs)]
public class Terrorlegs : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Armor/Cecitoma/Terrorlegs";
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.defense = 14;
        Item.rare = ItemRarityID.LightRed;
    }
    public const float DmgIncrease = 0.03f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DmgIncrease * 100);
    public override void UpdateEquip(Player player)
    {
        player.magicDamage += DmgIncrease;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<CorruptEyeMaterial>(), 12).AddIngredient(ItemType<TerrortomaMaterial>(), 1).AddTile(TileID.MythrilAnvil).Register();
    }
}
