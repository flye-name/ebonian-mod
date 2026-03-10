using System;
using System.IO;

namespace EbonianMod.Content.Items.Weapons.Magic;
public class ThawGauntlet : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/ThawGauntlet";
    public override void SetDefaults()
    {
        Item.damage = 20;
        Item.DamageType = DamageClass.Magic;
        Item.mana = 10;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.crit = -4;
        Item.knockBack = 4f;
        Item.width = 30;
        Item.height = 30;
        Item.useTime = 15;
        Item.useAnimation = 15;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.autoReuse = false;
        Item.channel = true;
        Item.value = Item.buyPrice(0, 0, 0, 1);
        Item.rare = ItemRarityID.Green;
        Item.shoot = ProjectileType<ThawGauntletP>();
    }
    public override bool CanShoot(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<ThawGauntletP>()] < 1 && player.statMana > 5;
    }
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Silk, 5)
            .AddIngredient(ItemID.IceBlock, 50)
            .AddRecipeGroup(RecipeGroup.RegisterGroup("AnyGoldBars", new RecipeGroup(() => $"{Lang.GetItemNameValue(ItemID.PlatinumBar)} / {Lang.GetItemNameValue(ItemID.GoldBar)}", ItemID.PlatinumBar, ItemID.GoldBar)), 5)
            .AddTile(TileID.Anvils)
            .Register();
    }
}

public class ThawGauntletP : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/ThawGauntletP";
    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(16);
        Projectile.friendly = true;
        Projectile.penetrate = 1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 30;
        Projectile.netUpdate = true;
        Projectile.netUpdate2 = true;
        Projectile.netImportant = true;
        Projectile.ownerHitCheck = true;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, Main.player[Projectile.owner].direction == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None, 0);
        return false;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, new Vector3(0, 169, 255) / 255 * 0.5f);
        Player player = Main.player[Projectile.owner];
        if (Projectile.ai[1] < 1)
        {
            if (player.whoAmI == Main.myPlayer)
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Helper.FromAToB(player.Center, Main.MouseWorld) * 5, ModContent.ProjectileType<ThawGauntletP2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            Projectile.ai[1] = 1;
        }
        if (Projectile.timeLeft < 28)
            Projectile.timeLeft++;
        
        if (player.whoAmI == Main.myPlayer)
            Projectile.velocity = Main.MouseWorld - player.Center;
        if (Projectile.velocity != Projectile.oldVelocity)
            Projectile.netUpdate = true;
        Projectile.rotation = Projectile.velocity.ToRotation();
        
        if (player.whoAmI == Projectile.owner && player.whoAmI == Main.myPlayer)
            player.direction = Main.MouseWorld.X >= player.Center.X ? 1 : -1;
        player.itemTime = 2;
        player.itemAnimation = 2;
        Projectile.ModProjectile.DrawHeldProjInFrontOfHeldItemAndArms = true;
        player.heldProj = Projectile.whoAmI;
        Projectile.Center = player.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        if (!player.channel || player.statMana <= 0) Projectile.Kill();
    }
}

public class ThawGauntletP2 : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/ThawGauntletP2";
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item27.WithVolumeScale(0.7f), Projectile.Center);
        Helper.SpawnDust(Projectile.Center, Projectile.Size, DustID.Ice, Vector2.Zero, 25, new Action<Dust>((target) => { target.noGravity = true; target.scale = Main.rand.NextFloat(0.6f, 0.9f); }
        ));
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 5;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(26, 20);
        Projectile.friendly = true;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 300;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;

    }
    public override bool ShouldUpdatePosition()
    {
        return Projectile.ai[0] != 0;
    }
    public override bool? CanDamage()
    {
        return Projectile.ai[0] != 0 || (Projectile.oldPos[3] != Vector2.Zero && Projectile.Center.Distance(Projectile.oldPos[3] + Projectile.Size / 2) > 60);
    }
    bool didAlpha;
    float alpha, vel = 5;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(alpha);
        writer.Write(vel);
        writer.Write(didAlpha);
        writer.Write(Projectile.localAI[0]);
        writer.Write(Projectile.localAI[1]);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        alpha = reader.ReadSingle();
        vel = reader.ReadSingle();
        didAlpha = reader.ReadBoolean();
        Projectile.localAI[0] = reader.ReadSingle();
        Projectile.localAI[1] = reader.ReadSingle();
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (alpha > 0)
            alpha -= 0.1f;
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Texture2D glow = Assets.Items.Weapons.Magic.ThawGauntletP2_Glow.Value;
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.localAI[0], Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        Main.spriteBatch.Reload(BlendState.Additive);
        Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex.Size() / 2, Projectile.localAI[0], Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        if (Projectile.damage > 1 && Projectile.ai[0] != 0)
            Projectile.damage /= 2;
    }
    public override void AI()
    {
        Projectile.ai[1]++;
        Lighting.AddLight(Projectile.Center, new Vector3(0, 169, 255) / 255 * 0.5f);
        Player player = Main.player[Projectile.owner];
        if (Projectile.ai[0] == 0)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Projectile.Center = player.Center + Helper.FromAToB(player.Center, Main.MouseWorld) * 40;
                Projectile.velocity = Helper.FromAToB(player.Center, Main.MouseWorld) * 5;
            }

            if (Projectile.localAI[0] < 1)
                Projectile.localAI[0] += 0.05f;
            else
            {
                if (!didAlpha)
                {
                    didAlpha = true;
                    alpha = 1f;
                }
            }
            if (player.ZoneSnow)
                Projectile.timeLeft = 600;
            else
                Projectile.timeLeft = 300;
        }
        if (Projectile.ai[0] == 0 && (!player.channel || player.statMana <= 0))
        {
            if (Projectile.localAI[0] < 0.95f)
                Projectile.Kill();
            else
                Projectile.ai[0] = 1;
        }
        if (Projectile.ai[0] != 0)
        {
            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0 ? 1 : -1;

            if (Helper.Raycast(Projectile.Center, Vector2.UnitY, 12).Success)
            {
                Collision.StepUp(ref Projectile.position, ref Projectile.velocity, Projectile.width, Projectile.height, ref Projectile.stepSpeed, ref Projectile.gfxOffY);
                Projectile.velocity.Y = 0;
                if (Helper.Raycast(Projectile.Center, Vector2.UnitY, 12).RayLength > 2 && Helper.Raycast(Projectile.Center, -Vector2.UnitY, 12).RayLength > 2 && Projectile.ai[2] < 3)
                {
                    Projectile.Center = Helper.Raycast(Projectile.Center - Vector2.UnitY * 10, Vector2.UnitY, 100).Point - new Vector2(0, 10);
                    Tile tile = Framing.GetTileSafely(Projectile.Center.ToTileCoordinates16().ToPoint());
                    if (tile.HasTile && !tile.IsActuated && WorldGen.SolidTile(tile))
                        Projectile.ai[2]++;

                    Projectile.netUpdate = true;
                }
                else
                {
                    Projectile.timeLeft -= 300;
                }
                Helper.SpawnDust(Projectile.Bottom + new Vector2(5 * -Projectile.direction, -4), Vector2.One, DustID.Frost, new Vector2(-Projectile.velocity.X, -2), 2, new Action<Dust>((target) => { target.noGravity = true; target.scale = Main.rand.NextFloat(0.6f, 0.9f); }
                ));
                if (Projectile.localAI[1] < 1)
                    Projectile.velocity.X = vel * Projectile.direction;
                Projectile.localAI[1]++;
            }
            else
            {
                if (Projectile.ai[2] > 0)
                {
                    Projectile.ai[2] = 0;
                    Projectile.netUpdate = true;
                }
                if (Projectile.localAI[1] != 0)
                    Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 10, 0.25f);
                else
                    Projectile.velocity.Y += 0.15f;
            }
            // Projectile.velocity.X = 10 * Projectile.direction;
            vel *= 0.995f;
            Projectile.ai[1]--;
            int i2 = (int)((Projectile.position.X - 8f) / 16f);
            int num219 = (int)(Projectile.position.Y / 16f);
            bool bounce = false;
            bool bounce2 = false;
            bool stepUp = false;
            if (WorldGen.SolidTile(i2, num219))
            {
                if (WorldGen.SolidTile(i2, num219 + 1))
                {
                    if (Projectile.ai[1] < 0 && !WorldGen.SolidTile(i2, num219 + 2))
                        Projectile.Center -= Vector2.UnitY * 16;
                    else
                        bounce = true;
                    Projectile.ai[1] = 30;
                    Projectile.netUpdate = true;
                }
                else
                    bounce = true;
            }
            i2 = (int)((Projectile.position.X + (float)Projectile.width + 8f) / 16f);
            if (WorldGen.SolidTile(i2, num219))
            {
                if (WorldGen.SolidTile(i2, num219 + 1))
                {
                    if (Projectile.ai[1] < 0 && !WorldGen.SolidTile(i2, num219 + 2))
                        Projectile.Center -= Vector2.UnitY * 16;
                    else
                        bounce2 = true;
                    Projectile.ai[1] = 30;
                    Projectile.netUpdate = true;
                }
                else
                    bounce2 = true;
            }
            if (bounce)
            {
                Projectile.velocity.X = vel;
            }
            else if (bounce2)
            {
                Projectile.velocity.X = -vel;
            }
        }
        else
            Projectile.netUpdate = true;
    }
}