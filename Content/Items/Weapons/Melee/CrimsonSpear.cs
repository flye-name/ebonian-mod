using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Friendly.Crimson;

namespace EbonianMod.Content.Items.Weapons.Melee;

public class CrimsonSpear : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/CrimsonSpear";

    public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

    public override void AddRecipes() => CreateRecipe().AddIngredient(ItemID.TheRottedFork).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();

    public override void SetStaticDefaults()
    {

    }

    public override void SetDefaults()
    {
        Item.Size = new(72);
        Item.scale = 1f;

        Item.DamageType = DamageClass.Melee;
        Item.noMelee = true;
        Item.damage = 75;
        Item.knockBack = 8f;

        Item.shoot = ProjectileType<CrimsonSpearPro>();
        Item.shootSpeed = 1;

        Item.autoReuse = true;
        Item.noUseGraphic = true;
        Item.useTime = Item.useAnimation = 28;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = new SoundStyle("EbonianMod/Assets/Sounds/Swing", 5)
        {
            PitchVariance = 0.1f,
            MaxInstances = 5
        };

        Item.value = Item.buyPrice(0, 12, 0, 0);
        Item.rare = ItemRarityID.LightRed;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        var p = player.GetModPlayer<CrimsonSpearPlayer>();

        Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, p.Combo);

        if (p.Combo++ >= 5)
            p.Combo = 0;

        return false;
    }
}

public class CrimsonSpearPlayer : ModPlayer
{
    public int Combo
    {
        get;
        set;
    }

    public override void Initialize()
    {
        Combo = 0;
    }
}