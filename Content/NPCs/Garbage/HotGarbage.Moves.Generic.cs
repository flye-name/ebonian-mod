using System;
using EbonianMod.Content.NPCs.Garbage.Projectiles;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Core.Systems.Cinematic;

namespace EbonianMod.Content.NPCs.Garbage;

public partial class HotGarbage : ModNPC
{
	void HandleLidAI() {
		if (AIState == State.CloseLid)
		{
            AnimationStyle = AnimationStyles.Close;
			AITimer++;
			if (AITimer > 20)
			{
				AITimer = 0;
				AIState = State.Idle;
				NPC.netUpdate = true;
			}
		}
		if (AIState == State.OpenLid)
		{
            AnimationStyle = AnimationStyles.Open;
			AITimer++;
			if (NextAttack2 == State.FallOver)
				NPC.rotation -= ToRadians(-0.9f * 5 * NPC.direction);
			if (AITimer == 1)
			{
				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, NPC.Center);
				MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<GreenShockwave>(), 0, 0);
			}
			if (AITimer >= 20)
			{
				AITimer = 0;
				AIState = NextAttack2;

				NPC.netUpdate = true;
			}
		}
	}
	
    void DoIdle()
    {
        AnimationStyle = AnimationStyles.Idle;
        AITimer++;
        
        NPC.noTileCollide = false;
        NPC.dontTakeDamage = false;
        NPC.damage = 0;
        NPC.rotation = Lerp(NPC.rotation, 0, 0.35f);
        NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
        
        Phase();
        
        float velocityX = Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.033f;
        velocityX = MathHelper.Clamp(velocityX, -15f, 15f);
        NPC.velocity.X = Lerp(NPC.velocity.X, velocityX, 0.12f);
        if (player.Distance(NPC.Center) < 70)
            AITimer++;
        if (player.Distance(NPC.Center) < 40)
            AITimer++;
        if (MathF.Abs(player.Center.X - NPC.Center.X) < 50 && player.Center.Y < NPC.Center.Y - 100)
            AITimer += 2;

        if (AITimer > 150)
            NPC.velocity.X *= 0.98f;
        
        if (AITimer >= 200)
        {
            NPC.velocity.X = 0;
            AITimer = 0;
            if (PerformedFullMoveset)
            {
                NextAttack = Main.rand.Next(AttackPool);
                if (NextAttack == State.OpenLid)
                    NextAttack2 = Main.rand.Next(OpenAttackPool);
            }
            AIState = NextAttack;
            if (NextAttack == State.OpenLid)
                NPC.frame.Y = 0;
            
            NPC.netUpdate = true;
        }
    }
    
	void DoDeath() {
            NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            if (NPC.Grounded())
            {
                AITimer++;
                if (AITimer == -74)
                {
                    NPC.netUpdate = true;
                    DisposablePosition = NPC.Center;
                    if (!Main.dedServ)
                        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/GarbageSiren");
                    CameraSystem.ChangeCameraPos(NPC.Center - new Vector2(0, 50), 130, null, 1.4f, InOutQuart);
                }
                if (AITimer == -30)
                {
                    CameraSystem.ChangeZoom(80, new ZoomInfo(2.5f, 1f, InOutElastic, InOutCirc));
                    MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ChargeUp>(), 0, 0);
                }
                if (AITimer > -30)
                    AITimer2++;
            }
            else
            {
                if (AITimer < 20)
                    NPC.velocity.Y++;
            }
            if (AITimer > 20)
                Phase();
            if (AITimer == 0)
            {
                CameraSystem.ScreenShakeAmount = 20;

            }

            if (AITimer is >= 0 and <= 20)
                AnimationStyle = AnimationStyles.Open;
            if (AITimer is > 20 and <= 40)
                AnimationStyle = AnimationStyles.Close;
            if (AITimer > 40)
            {
                if (NPC.velocity.Length() > 4f)
                    AnimationStyle = AnimationStyles.Boost;
                else
                    AnimationStyle = AnimationStyles.BoostWarning;
            }
            if (AITimer >= 40 && AITimer <= 20)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y == 76)
                        SoundEngine.PlaySound(SoundID.Item37, NPC.Center);
                    if (NPC.frame.Y > 0)
                    {
                        NPC.frame.Y -= 76;
                    }
                }
            }
            if (AITimer2 >= 22 && AITimer2 < 40 && AITimer2 % 2 == 0)
            {
                for (int i = -1; i < 1; i++)
                {
                    Projectile a = MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(2, 4) * i, NPC.height / 2 - 8), new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageFlame>(), 15, 0);
                    if (a is not null)
                    {
                        a.timeLeft = 170;
                        a.SyncProjectile();
                    }
                }
            }
            bool nukeExist = false;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type == ProjectileType<HotGarbageNuke>()) nukeExist = true;
            }
            if (AITimer == 20 && !nukeExist)
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDustPerfect(NPC.Center - new Vector2(Main.rand.NextFloat(-30, 30), 20), DustID.Smoke, -Vector2.UnitY.RotatedByRandom(PiOver4) * Main.rand.NextFloat(5, 20));
                }
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.hostile && proj.type != ProjectileType<HotGarbageNuke>()) proj.Kill();
                }
                SoundEngine.PlaySound(Sounds.firework.WithVolumeScale(2), NPC.Center);
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center - new Vector2(0, 50), Vector2.Zero, ProjectileType<HotGarbageNuke>(), 0, 0);
            }
            if (!Main.dedServ)
            {
                Main.musicNoCrossFade[MusicLoader.GetMusicSlot(Mod, "Assets/Music/GarbageSiren")] = true;
                Main.musicNoCrossFade[0] = true;
                if (AITimer >= 634)
                {
                    Main.musicFade[MusicLoader.GetMusicSlot(Mod, "Assets/Music/GarbageSiren")] = 1;
                    Music = 0;
                }
            }
            if (AITimer >= 665)
            {
                NPC.immortal = false;
                NPC.dontTakeDamage = false;
                NPC.StrikeInstantKill();
            }
            if (AITimer2 == 7)
                SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
            if (AITimer2 < 22 && AITimer2 >= 0)
            {
                NPC.velocity.X = Lerp(NPC.velocity.X, 14f * NPC.direction, 0.15f);
            }
            if (AITimer2 >= 22)
            {
                NPC.velocity *= 0.96f;
            }
            if (AITimer2 == 40 || AITimer2 < 0)
            {
                if (player.Center.Distance(DisposablePosition) < 650)
                {
                    NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                    NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                }
                else
                {
                    NPC.spriteDirection = DisposablePosition.X > NPC.Center.X ? 1 : -1;
                    NPC.direction = DisposablePosition.X > NPC.Center.X ? 1 : -1;
                }
            }
            if (AITimer2 >= 65)
            {
                NPC.netUpdate = true;
                AITimer2 = -50;
            }
            if (AITimer % 20 == 0 && AITimer > 30 && AITimer < 630)
            {
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), DisposablePosition - Vector2.UnitY * 1000, new Vector2(Main.rand.NextFloat(-30, 30) * Main.rand.NextFloat(1f, 2f), Main.rand.NextFloat(-5, 1)), ProjectileType<GarbageGiantFlame>(), 15, 0, ai0: 1);
            }
    }

    void DoIntro()
    {
        AnimationStyle = AnimationStyles.Still;
        if (AITimer > 30)
            AnimationStyle = AnimationStyles.Intro;
        
        if (!NPC.collideY && AITimer2 < 150)
        {
            if (Helper.Raycast(NPC.Center, Vector2.UnitY, 80).RayLength > 50)
                NPC.position.Y += NPC.velocity.Y * 0.5f;
        }
        else 
            AITimer++;
        NPC.dontTakeDamage = true;
        AITimer2++;
        if (AITimer == 1)
        {
            SoundEngine.PlaySound(SoundID.Item70, NPC.Center);
            NPC.netUpdate = true;
            NPC.position.Y -= NPC.velocity.Y;
            foreach (Player p in Main.ActivePlayers)
                if (!p.dead && p.Distance(NPC.Center) < 3000)
                {
                    player.JumpMovement();
                    player.velocity.Y = -10;
                }
            MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + NPC.height * 0.5f * Vector2.UnitY, new Vector2(0, 0), ProjectileType<GarbageImpact>(), 0, 0, 0, 0);
            CameraSystem.ChangeCameraPos(NPC.Center - new Vector2(0, 50), 150, null, 1.5f, InOutQuart);
        }
        if (AITimer == 50)
        {
            SoundEngine.PlaySound(Sounds.garbageAwaken);
        }
        if (AITimer == 75)
        {
            CameraSystem.ChangeZoom(75, new ZoomInfo(2, 1f, InOutBounce, InOutCirc));
            for (int i = 0; i < 3; i++)
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<BloodShockwave2>(), 0, 0);
        }
        if (AITimer > 130)
        {
            NPC.Center += new Vector2(2 * NPC.direction, 0);
            NPC.frame.X = 80;
            NPC.frame.Y = 0;
            AIState = State.Idle;
            AITimer = 0;
            AITimer2 = 0;
            NextAttack = State.WarningForDash;

            NPC.netUpdate = true;
        }
    }
}