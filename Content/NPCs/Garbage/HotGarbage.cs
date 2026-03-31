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
        NextAttack = State.SummonDrones;
        
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
            
            case State.SummonDrones:
                DoSummonDrones();
                break;
            
            case State.ReticleMissiles:
                DoReticleMissiles();
                break;
        }
    }
}