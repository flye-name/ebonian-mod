using EbonianMod.Content.Items.Consumables.Food;
using EbonianMod.Content.NPCs.Garbage;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using EbonianMod.Content.NPCs.Garbage.Projectiles;
using Terraria.Graphics.CameraModifiers;


namespace EbonianMod.Content.Items.Consumables.BossItems;

public class GarbageRemote : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Consumables/BossItems/GarbageRemote";
    public override void SetStaticDefaults()
    {
        ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
    }

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
        Item.shoot = ProjectileType<GarbageRemoteP>();
        Item.shootSpeed = 1;
        Item.consumable = false;
        Item.useTurn = false;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<Potato>(), 5).AddRecipeGroup(RecipeGroupSystem.SilverBars, 5).AddIngredient(ItemID.Glass, 10).AddTile(TileID.Anvils).Register();
    }

    public override bool CanUseItem(Player player)
    {
        bool proj = false;
        foreach (Projectile p in Main.ActiveProjectiles)
            if (p.type == ProjectileType<GarbageRemoteP>())
                proj = true;
        return !NPC.AnyNPCs(NPCType<HotGarbage>()) && !proj;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        position = Helper.Raycast(player.Center + new Vector2(40 * player.direction, -50), Vector2.UnitY, 1000, true).Point - new Vector2(0, 23);
        velocity = Vector2.Zero;

        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
    public override bool? UseItem(Player player)
    {
        if (Main.myPlayer != player.whoAmI)
            Projectile.NewProjectile(null, Helper.Raycast(player.Center + new Vector2(40 * player.direction, -50), Vector2.UnitY, 1000, true).Point - new Vector2(0, 23), Vector2.Zero, Item.shoot, 0, 0, player.whoAmI);

        SoundEngine.PlaySound(Sounds.garbageSignal.WithVolumeScale(3), player.position);
        return null;
    }
}
public class GarbageRemoteP : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Items/Consumables/BossItems/GarbageRemote";
    public override void SetDefaults()
    {
        Projectile.width = 36;
        Projectile.height = 46;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 430;


    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override void PostDraw(Color lightColor)
    {
        Texture2D tex = Request<Texture2D>(Texture + "_Overlay").Value;
        if (Projectile.timeLeft > 155)
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, 1, SpriteEffects.None, 0);
    }
    public override void OnKill(int timeLeft)
    {
        Helper.AddCameraModifier(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 10, 6, 30, 1000));
    }
    List<Vector2> points = new List<Vector2>();
    Vector2 end;
    Vector2 basePos;
    Vector2 pos;
    float rot;
    public override void AI()
    {

        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.active && npc.type == NPCType<HotGarbage>())
            {
                if (npc.Distance(Projectile.Center) < Projectile.Size.Length())
                    Projectile.Kill();
            }
        }
        Player player = Main.player[Projectile.owner];
        if (Projectile.ai[0] == 0)
        {
            Projectile.netUpdate = true;
            if (Projectile.owner == Main.myPlayer)
                basePos = Projectile.Center;

            if (Projectile.owner == Main.myPlayer)
                Projectile.netUpdate = true; // TEST
            Projectile.ai[0] = 1;
        }
        else
        {
            Projectile.netUpdate = true;
            if (Projectile.timeLeft > 155)
            {
                if (Projectile.timeLeft % 5 - (Projectile.timeLeft < 200 ? 2 : 0) == 0)
                {
                    rot = Main.rand.NextFloat(-0.5f, 0.5f);
                    if (Projectile.owner == Main.myPlayer)
                        pos = basePos + Main.rand.NextVector2Circular(5, 5);
                    if (Projectile.owner == Main.myPlayer)
                        Projectile.netUpdate = true; // TEST
                }
                if (Projectile.owner == Main.myPlayer)
                    if (pos != Vector2.Zero)
                        Projectile.Center = Vector2.Lerp(Projectile.Center, pos, 0.1f);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, rot, 0.5f);
            }
        }
        Projectile.ai[1]++;
        if (Projectile.ai[1] % 5 == 0 && Main.rand.NextBool(4) && Projectile.timeLeft > 155)
        {
            MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<YellowShockwave>(), 0, 0);
        }
        int n = 15;
        if (Projectile.timeLeft == 120)
        {
            if (Projectile.owner == Main.myPlayer)
                MPUtils.NewNPC(Projectile.Center + new Vector2(0, -1080), NPCType<HotGarbage>());
        }
        if (Projectile.timeLeft == 155)
        {
            Helper.AddCameraModifier(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 5, 6, 30, 1000));
            MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<BigGrayShockwave>(), 0, 0);
            MPUtils.NewProjectile(null, Projectile.Center - new Vector2(0, 700), Vector2.UnitY, ProjectileType<GarbageLightning>(), 15, 0);

        }
        if (Projectile.timeLeft <= 155)
            if (Main.rand.NextBool(5))
                Helper.DustExplosion(Projectile.Center, Vector2.One, 2, Color.Gray * 0.45f, false, false, 0.6f, 0.5f, new(Main.rand.NextFloat(-4, 4), -10));
        if (Projectile.timeLeft <= 155 && Projectile.timeLeft > 125)
        {
            Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 1, 0.1f);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return true;
    }
}
