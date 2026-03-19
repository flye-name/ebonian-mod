using EbonianMod.Content.Dusts;
using EbonianMod.Content.NPCs.Corruption;
using EbonianMod.Content.NPCs.Garbage.Projectiles;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Core.Systems.Cinematic;
using System;


namespace EbonianMod.Content.NPCs.Garbage;

[AutoloadBossHead]
public partial class HotGarbage : ModNPC
{
    public Player player => Main.player[NPC.target];
    public State NextAttack = State.OpenLid;
    public State NextAttack2 = State.TrashBags;
    public override void PostAI() => NPC.spriteDirection = NPC.direction;

    public override void AI()
    {
        AmbientFX();

        TargetingLogic();
        
        HandleLidAI();

        switch (AIState)
        {
            case State.Death:
                DoDeath(); break;
            
            case State.Intro: 
                DoIntro(); break;
            
            case State.Idle:
                DoIdle(); break;
            
            case State.WarningForDash:
            case State.Dash:
                DoDash();
                break;
            
            case State.SlamPreperation:
            case State.SlamSlamSlam:
                DoSlam();
                break;
            
            case State.WarningForBigDash:
            case State.BigDash:
                DoBigDash();
                break;
            
            case State.SpewFire:
            case State.SpewFire2:    
            case State.GiantFireball:    
                DoFireSpewAttacks();
                break;
            
            case State.TrashBags:
                DoTrashBags();
                break;
            
            case State.SodaMissiles:
                DoSodaMissiles();
                break;
            
            case State.MailBoxes:
                DoMailBoxes();
                break;
            
            case State.SateliteLightning:
                DoSatelites();
                break;
            
            case State.PipeBombAirstrike:
                DoPipebombAirstrike();
                break;
            
            case State.MassiveLaser:
                DoMassiveLaser();
                break;
        }
        
        if (AIState == State.SummonDrones)
        {
            AITimer++;
            if (AITimer == 10)
                SoundEngine.PlaySound(SoundID.Zombie67.WithPitchOffset(-1f), NPC.Center);
            
            if (AITimer > 10 && AITimer < 30&& AITimer % 2 == 0)
                MPUtils.NewProjectile(null, NPC.Center + Main.rand.NextVector2Circular(AITimer -10f, AITimer- 10f), Vector2.Zero, ProjectileType<GreenShockwave>(), 0, 0); 

            if (AITimer > 45 && AITimer < 180 && AITimer % 7 == 0)
            {
                MPUtils.NewProjectile(null, NPC.Center - new Vector2(850, 600), new Vector2(7, 10) * Main.rand.NextFloat(0.9f, 1.1f), ProjectileType<LaserDrone>(), 10, 0);
            }
        }
    }
}