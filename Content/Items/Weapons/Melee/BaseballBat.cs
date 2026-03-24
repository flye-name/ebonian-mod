using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Generic;
using System;

namespace EbonianMod.Content.Items.Weapons.Melee;

public class BaseballBat : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/BaseballBat";
    public override void SetDefaults()
    {
        Item.knockBack = 10f;
        Item.Size = new Vector2(48);
        Item.crit = 0;
        Item.damage = 18;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.noUseGraphic = true;
        Item.autoReuse = false;
        Item.noMelee = true;
        Item.value = Item.buyPrice(0, 3, 0, 0);
        Item.channel = true;
        Item.DamageType = DamageClass.Melee;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.rare = ItemRarityID.Green;
        Item.shoot = ProjectileType<BaseballBatProjectile>();
        Item.shootSpeed = 1;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.SandBlock, 10).AddIngredient(ItemID.Silk, 10).AddRecipeGroup(RecipeGroupID.IronBar, 20).AddTile(TileID.Anvils).Register();
    }
    public override bool AltFunctionUse(Player player) => true;
    public override bool CanUseItem(Player player)
    {
        Item.shoot = player.altFunctionUse == 2 ? ProjectileType<Baseball>() : ProjectileType<BaseballBatProjectile>();
        return Item.shoot == ProjectileType<BaseballBatProjectile>() || (Item.shoot == ProjectileType<Baseball>() && player.ownedProjectileCounts[ProjectileType<Baseball>()] < 4);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (player.altFunctionUse == 2)
        {
            velocity = new Vector2(player.velocity.X, player.velocity.Y - 3);
        }

        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1);
        return false;
    }
}
public class BaseballBatProjectile : HeldProjectile
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/BaseballBat";
    public override bool? CanDamage() => Projectile.ai[2] < 1;
    public override void SetDefaults()
    {
        base.SetDefaults();
        ItemType = ItemType<BaseballBat>();
        Projectile.Size = new Vector2(48);
        HoldOffset = new Vector2(42, 0);
        PlayerFacesCursor = false;
        Projectile.penetrate = -1;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.rotation = Projectile.velocity.ToRotation();
        Projectile.ai[0] = Projectile.rotation - Pi * Main.player[Projectile.owner].direction * 0.9f;
        Projectile.ai[2] = 1;
    }
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        player.heldProj = Projectile.whoAmI;

        float attackSpeedMultiplier = Main.LocalPlayer.GetAttackSpeed(DamageClass.Melee);
        if (Projectile.ai[1] < 20)
        {
            Projectile.ai[1] += attackSpeedMultiplier;
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.ai[0], Min(0.25f * attackSpeedMultiplier, 1));
        }
        else if(!player.channel)
        {
            Projectile.rotation += Projectile.ai[2] * attackSpeedMultiplier * Projectile.direction;
            Projectile.ai[2] *= MathF.Pow(0.86f, attackSpeedMultiplier);
            if (Projectile.ai[2] < 0.4f)
            {
                Projectile.ai[2] -= 0.025f * attackSpeedMultiplier;
                Projectile.Opacity *= MathF.Pow(0.8f, attackSpeedMultiplier);
                if (Projectile.ai[2] < 0) Projectile.Kill();
            }
            else if(Projectile.ai[2] > 0.5f && Projectile.ai[1] != 100)
            {
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.type == ProjectileType<Baseball>() && Vector2.Distance(Projectile.Center, projectile.Center) < 50)
                    {
                        SoundEngine.PlaySound(SoundID.Item126.WithPitchOffset(Main.rand.NextFloat(-0.9f, -0.7f)), Projectile.Center);
                        SoundEngine.PlaySound(SoundID.Item10.WithPitchOffset(Main.rand.NextFloat(1f, 2f)), Projectile.Center);
                        projectile.velocity = Projectile.velocity * Clamp(MathF.Pow(projectile.velocity.Length(), 2) / 9, 14, 34);
                        projectile.timeLeft = 360;
                        projectile.Opacity = 1;
                        if (player.whoAmI == Main.myPlayer && Distance(Main.MouseWorld.X, player.Center.X) < 8) projectile.velocity.X = player.velocity.X;
                        projectile.SyncProjectile();
                        projectile.netUpdate = true;
                        Projectile.ai[1] = 99;
                    }
                }
                if (Projectile.ai[1] == 99) Projectile.ai[1] = 100;
            }
        }

        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);

        base.AI();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor * Projectile.Opacity, Projectile.rotation + Pi / 4, Projectile.Size / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
}
