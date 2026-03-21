using EbonianMod.Common.Players;
using EbonianMod.Core.Systems.Verlets;
using System;
using System.IO;
namespace EbonianMod.Content.Items.Accessories;

public class TinyBrain : ModNPC //the class name is a reference to my brain.
{
    public override string Texture => Helper.AssetPath + "Items/Accessories/TinyBrain";
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 4;
        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }
    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 40;
        NPC.damage = 0;
        NPC.defense = 5;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0;
        NPC.aiStyle = 0;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.lifeMax = 400;
        NPC.friendly = true;
        NPC.npcSlots = 0;
        NPC.dontTakeDamage = false;
    }
    public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hitinfo, int damage)
    {
        if (projectile.timeLeft > 2 && projectile.penetrate > 1 && projectile.velocity != Vector2.Zero && projectile.ModProjectile.ShouldUpdatePosition())
            projectile.Kill();
    }

    public override bool CheckDead()
    {
        if (Main.dedServ)
            return true;
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/TinyBrainGore").Type, NPC.scale);
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/TinyBrainGore2").Type, NPC.scale);
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/TinyBrainGore3").Type, NPC.scale);
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/TinyBrainGore4").Type, NPC.scale);
        return true;
    }
    public override void AI()
    {
        Player player = Main.player[(int)NPC.ai[1]];
        NPC.timeLeft = 10;
        NPC.ai[0] += 4;
        NPC.ai[3] = MathHelper.Lerp(NPC.ai[3], 1, 0.1f);
        NPC.Center = player.Center + new Vector2(90 + MathF.Sin(NPC.ai[0] * 0.01f) * 20, 0).RotatedBy(Helper.CircleDividedEqually(NPC.ai[2], 8) + MathHelper.ToRadians(NPC.ai[0] + MathF.Sin(NPC.ai[0])) * 0.7f) * NPC.ai[3];
        AccessoryPlayer modPlayer = player.GetModPlayer<AccessoryPlayer>();
        if (!modPlayer.brainAcc)
        {
            NPC.active = false;
        }
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter % 5 == 0)
        {
            NPC.frame.Y += frameHeight;
            if (NPC.frame.Y > 3 * frameHeight)
                NPC.frame.Y = 0;
        }
    }
    Verlet verlet;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 pos, Color drawColor)
    {
        if (verlet is null)
            verlet = new Verlet(NPC.Center, 8, 14, stiffness: 70);
        else
        {
            verlet.Update(Main.player[(int)NPC.ai[1]].Center, NPC.Center);
            verlet.Draw(spriteBatch, new VerletDrawData(new VerletTextureData(Helper.AssetPath+"Items/Accessories/BrainAcc_Chain")));
        }
        return true;
    }
}