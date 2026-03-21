using EbonianMod.Content.Items.Weapons.Summoner;
using EbonianMod.Content.Projectiles.Enemy.Overworld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Drawing;

namespace EbonianMod.Content.Items.Weapons.Magic;
public class KodamaStaff : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/KodamaStaff";
    public override void SetStaticDefaults()
    {
        Item.staff[Item.type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<SudamaTrident>();
    }

    public override void SetDefaults()
    {
        Item.damage = 9;
        Item.width = 40;
        Item.height = 40;
        Item.mana = 5;
        Item.DamageType = DamageClass.Magic;
        Item.useTime = 10;
        Item.useAnimation = 50;
        Item.reuseDelay = 30;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 10;
        Item.rare = ItemRarityID.Green;
        Item.noMelee = true;
        Item.value = Item.buyPrice(0, 20, 0, 0);
        Item.autoReuse = true;
        Item.shoot = ProjectileType<KodamaF>();
        Item.shootSpeed = 5;
    }
    public override void HoldItem(Player player)
    {
        if (!player.ItemTimeIsZero)
            Lighting.AddLight(player.itemLocation, 197f / 255f, 226f / 255f, 105f / 255f);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.LivingWoodWand).AddIngredient(ItemID.Emerald, 15).AddTile(TileID.Anvils).Register();
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 mouse = Main.MouseWorld - player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true) - new Vector2(0, 1000);
        player.itemRotation = MathF.Atan2(mouse.Y * player.direction, mouse.X * player.direction);
        NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
        NetMessage.SendData(MessageID.ShotAnimationAndSound, number: player.whoAmI);

        Vector2 vel = new Vector2(velocity.X * 0.5f * Main.rand.NextFloat(1, 2), -Item.shootSpeed).RotatedByRandom(PiOver4);
        Projectile.NewProjectile(source, position + vel * 10, vel, type, damage, knockback, player.whoAmI);
        return false;
    }
}
