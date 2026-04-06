using System;
using EbonianMod.Content.Dusts;
using EbonianMod.Content.NPCs.Garbage.Projectiles;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Core.Systems.Cinematic;

namespace EbonianMod.Content.NPCs.Garbage;

public partial class HotGarbage : ModNPC
{
	void DoDash()
	{
		AITimer++;
		if (AIState == State.WarningForDash)
		{
			AnimationStyle = AnimationStyles.BoostWarning;
			FacePlayer();
            
			NPC.velocity.X *= 0.99f;
			if (AITimer == 20)
			{
				SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
				MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
			}
			if (AITimer >= 55)
			{
				NPC.velocity.X = 0;
				AITimer = 0;
				AITimer2 = 0;
				AITimer3 = 0;
				AIState = State.Dash;
			}
		}
		else
		{
			bool phased = Phase();

			const int DashInterval = 65;
			NPC.damage = 60;
			
			if (NPC.velocity.Length() > 2f && AITimer3 < DashInterval-10)
				AnimationStyle = AnimationStyles.Boost;
			else
				AnimationStyle = AnimationStyles.BoostWarning;
            
			if ((int)AITimer3 == 7)
				SoundEngine.PlaySound(Sounds.exolDash with { Volume = 0.2f, Pitch = 0.5f }, NPC.Center);
			
			if (AITimer3 < 22)
			{
				NPC.velocity.X = Lerp(NPC.velocity.X, 20f * NPC.direction, 0.15f);
			}
			else
			{
				NPC.velocity *= 0.96f;
				if (NPC.velocity.Length() < 4f)
					FacePlayer();
                
				if (AITimer3 < 40 && NPC.Grounded(1f) && !phased)
				{
					MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-4, 4), NPC.height / 2f + 6), new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageDashFlames>(), 15, 0, ai2: (1f - AITimer2 / 40f) * Main.rand.NextFloat(0.2f, 0.4f));
				}
			}
			
			if (++AITimer3 >= DashInterval)
			{
				AITimer3 = 0;
				NPC.netUpdate = true;
			}
			
			if (AITimer >= DashInterval * 3)
			{
				NPC.velocity = Vector2.Zero;
				ResetTo(State.OpenLid, State.SpewFire);
			}
		}
	}

	void DoSlam()
	{
		AITimer++;
		if (AIState == State.SlamPreperation)
        {
	        AnimationStyle = AnimationStyles.BoostWarning;
            if (AITimer > 15)
            {
	            AnimationStyle = AnimationStyles.Boost;
	            NPC.rotation += ToRadians(MathHelper.Lerp(-.9f, -1.7f, AITimer / 75f) * NPC.direction);
	            NPC.noGravity = true;
	            NPC.noTileCollide = true;
            }
	
            Vector2 velocity = new Vector2(NPC.direction * MathHelper.Lerp(0.5f, 1, AITimer / 75f), 0).RotatedBy(NPC.rotation);
			NPC.velocity = velocity * (AITimer / 75f) * 25;
            if (AITimer >= 75)
            {
	            NPC.noTileCollide = false;
	            NPC.noGravity = false;
                NPC.netUpdate = true;
                AITimer = 0;
                AITimer2 = 0;
                NPC.velocity *= 0.5f;
                AIState = State.SlamSlamSlam;
            }
        }
        else if (AIState == State.SlamSlamSlam)
        {
            AnimationStyle = AnimationStyles.BoostWarning;
            if (MathF.Abs(NPC.velocity.Y) > 1f && (AITimer > 200 || AITimer < 50))
                AnimationStyle = AnimationStyles.Boost;
            
            NPC.noGravity = true;
            NPC.damage = 60;
            if (AITimer == 2)
	            SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
            
            if (AITimer < 50)
	            NPC.velocity = NPC.velocity.RotatedBy(ToRadians(-NPC.direction * 2));

            if (AITimer < 200)
                NPC.noTileCollide = true;
            if (AITimer >= 50 && AITimer < 171)
            {
                if (AITimer < 176)
                    DisposablePosition = player.Center - new Vector2(-player.velocity.X * 20, 500);
                NPC.direction = NPC.spriteDirection = 1;
                NPC.rotation = Utils.AngleLerp(NPC.rotation, ToRadians(90), 0.05f);
                if (AITimer % 8 == 0)
                    NPC.velocity = Helper.FromAToB(NPC.Center, DisposablePosition, false) * MathHelper.Lerp(0.025f, 0.056f, Helper.Saturate((AITimer - 50f) / 50f));
            }
            if (AITimer is > 175 and < 186 && AITimer % 2 == 0)
            {
                NPC.velocity = Vector2.Zero;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center + new Vector2(0, -100 + (AITimer - 175) * 35), Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0, ai0: 1);
            }
            if (AITimer == 202)
            {
                SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
                for (int i = -4; i < 4; i++)
                {
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(6 * i * Main.rand.NextFloat(0.7f, 1.2f), 3), ProjectileType<GarbageGiantFlame>(), 15, 0, ai0: 1);
                }
                NPC.velocity = new Vector2(0, 35);
            }
            if (AITimer > 200 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
            {
                NPC.noTileCollide = false;
            }
            if (AITimer > 200 && !NPC.collideY && NPC.noTileCollide)
            {
                NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-1, 1);
                NPC.velocity.Y += 0.015f;
            }
            if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 1f)) && AITimer2 == 0 && AITimer >= 200)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                NPC.velocity = -Vector2.UnitY * 3;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FlameExplosionWSpriteHostile>(), 0, 0);
                AITimer2 = 1;
            }
            if (AITimer2 >= 1)
            {
                NPC.velocity.Y += 0.1f;
                AITimer2++;
            }
            if (AITimer2 >= 50)
            {
                NPC.noGravity = false;
                NPC.velocity = Vector2.Zero;
                ResetTo(State.WarningForBigDash);
            }
        }
	}
	
	void DoBigDash() {
		AITimer++;
		if (AIState == State.WarningForBigDash)
		{
			AnimationStyle = AnimationStyles.BoostWarning;
			NPC.velocity.X = Helper.FromAToB(NPC.Center, player.Center).X * -1;
			if (AITimer == 10)
			{
				SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
				MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0, ai1: 0.5f);
			}
			NPC.velocity -= new Vector2(NPC.direction * 0.005f * AITimer, 0);
			FacePlayer();
			if (AITimer >= 50)
			{
				NPC.netUpdate = true;
				NPC.velocity.X = 0;
				AITimer = 0;
				AITimer2 = 0;
				AIState = State.BigDash;
				NPC.velocity = Vector2.Zero;
			}
		}
		else if (AIState == State.BigDash)
		{
			AnimationStyle = AnimationStyles.Boost;
			Phase();
			NPC.damage = 90;
			NPC.rotation = Lerp(NPC.rotation, 0, 0.35f);
			if (AITimer == 2)
				SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
			if (AITimer < 12)
				NPC.velocity += new Vector2(NPC.direction * MathHelper.Lerp(1, 4, AITimer / 110f), 0);

			if (AITimer > 90)
			{
				NPC.velocity.X *= 0.93f;
				AnimationStyle = AnimationStyles.BoostWarning;
			}
			else
				MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<GarbageDashFlames>(), 15, 0, ai2: 2);
            
			if (AITimer % 12 == 0)
				for (int i = 0; i < 2; i++)
					MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageFlame>(), 15, 0);
			
            
			if (AITimer >= 110)
			{
				ResetTo(State.OpenLid, State.TrashBags);
				AITimer = -50;
			}
		}
	}
	
	void DoFireSpewAttacks() {
		AnimationStyle = AnimationStyles.Open;
		AITimer++;
		Phase();

		void SpitOutDust()
		{
			Vector2 velocity = new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)).RotatedByRandom(1);
			Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), DustType<GarbageFlameDust>(), velocity * Main.rand.NextFloat(0.5f, 1), newColor: Color.OrangeRed, Scale: 0.15f);
		}
		
        if (AIState == State.SpewFire)
        {
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.5f, 0.15f);
            if (AITimer % 6 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                for (int i = -2; i < 2; i++)
                {
	                SpitOutDust();
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                }
            }
            if (AITimer >= 100)
	            ResetTo(State.SlamPreperation, null, true);
        }
        else if (AIState == State.SpewFire2)
        {
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.5f, 0.15f);
            if (AITimer % 6 == 0 && AITimer > 30)
            {
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                for (int i = 0; i < 4; i++)
                {
	                Vector2 velocity = new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)).RotatedByRandom(1);
	                Dust.NewDustPerfect(NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), DustType<GarbageFlameDust>(), velocity * Main.rand.NextFloat(0.5f, 1), newColor: Color.OrangeRed, Scale: 0.15f);
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                }
            }
            if (AITimer >= 70)
	            ResetTo(State.OpenLid, State.SateliteLightning, true);
        }
        else if (AIState == State.GiantFireball)
        {
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.01f, 0.15f);
            FacePlayer();
            if (AITimer == 10)
            {
                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                
                for (int i = 0; i < 10; i++)
	                SpitOutDust();
                
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
            }
            if (AITimer == 20)
            {
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                
                for (int i = 0; i < 15; i++)
	                SpitOutDust();
                
                for (int i = 0; i < 3; i++)
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
            }
            if (AITimer == 80)
            {
                CameraSystem.ScreenShakeAmount = 12;
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-0.4f).WithVolumeScale(1.1f), NPC.Center);
                
                for (int i = 0; i < 30; i++)
	                SpitOutDust();
                
                for (int i = 0; i < 5; i++)
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -7 - Main.rand.NextFloat(4, 7)), ProjectileType<GarbageGiantFlame>(), 15, 0, ai2: 1);
            }
            if (AITimer >= 80)
	            ResetTo(State.MassiveLaser, null, true);
        }
	}

	void DoTrashBags()
	{
		AITimer++;
		AITimer3++;
		
		Phase();
		FacePlayer();
		
		if (AITimer > 125)
			NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.043f, 0.12f);
		else
			NPC.velocity.X *= 0.99f;
		
		if (AITimer < 100)
			AnimationStyle = AnimationStyles.Open;
		else if (AITimer < 125)
			AnimationStyle = AnimationStyles.Close;
		else
			AnimationStyle = AnimationStyles.Idle;
            
		if (AITimer == 1 && MPUtils.NotMPClient)
		{
			AITimer3 = Main.rand.Next(10000000);
			NPC.netUpdate = true;
		}
		UnifiedRandom rand = new((int)AITimer3);
		if (AITimer <= 60 && AITimer % 5 == 0)
		{
			SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
			MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-15, -7)) * 0.5f, ProjectileType<GarbageBag>(), 15, 0, player.whoAmI);
		}

		if (AITimer % 3 == 0 && AITimer > 100) 
			MPUtils.NewProjectile(NPC.GetSource_FromThis(), new Vector2(player.Center.X + 600 * rand.NextFloat(-1, 1), player.Center.Y - Main.rand.NextFloat(700, 1000)), new Vector2(rand.NextFloat(-1, 1) * 5f, 1) , ProjectileType<GarbageBag>(), 15, 0, player.whoAmI);
		
		if (AITimer >= 170)
			ResetTo(State.OpenLid, State.SodaMissiles);
	}

	void DoSodaMissiles()
	{
		AnimationStyle = AnimationStyles.Open;
		
		if (AITimer == 1 && !MPUtils.NotMPClient)
		{
			AITimer3 = Main.rand.Next(10000000);
			NPC.netUpdate = true;
		}
		AITimer3++;
		AITimer++;
		Phase();
		FacePlayer();
		NPC.velocity.X = Lerp(NPC.velocity.X, 0, 0.05f);
                
		UnifiedRandom rand = new((int)AITimer3);
		if (AITimer % 3 == 0 && AITimer < 60 && AITimer > 20)
		{
			SoundEngine.PlaySound(SoundID.Item156, NPC.Center);
			float angle = rand.NextFloat(-3, 3);
			if (angle is < 0 and > -1)
				angle = -1;
			if (angle is >= 0 and < 1)
				angle = 1;
			Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(rand.NextFloat(-4, 4), -7), ProjectileType<GarbageMissile>(), 15, 0, player.whoAmI, ToRadians(angle));
		}
            
		if (AITimer >= 60)
			ResetTo(State.MailBoxes, null, true);
	}

	void DoMailBoxes()
	{
		AITimer++;
		FacePlayer();
		AnimationStyle = AnimationStyles.Idle;
		
		if (AITimer == 20)
		{
			SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
		}

		
		if (AITimer is >= 20 and <= 50)
		{
			AnimationStyle = AnimationStyles.Constipated;
			if (AITimer % 10 == 0)
				MPUtils.NewProjectile(null, NPC.Center - new Vector2(0, 20), -Vector2.UnitY, ProjectileType<GarbageTechTelegraph>(), 0, 0, ai1: AITimer / 30f);
		}

		if (AITimer > 60 && AITimer < 82)
		{
			MPUtils.NewProjectile(NPC.GetSource_FromThis(), Helper.Raycast(NPC.Center - new Vector2(Main.rand.NextFloat(-2000, 2000), 200), Vector2.UnitY, 600, true).Point, Vector2.Zero, ProjectileType<Mailbox>(), 15, 0);
		}
		if (AITimer >= 120)
		{
			NPC.velocity = Vector2.Zero;
			ResetTo(State.OpenLid, State.SpewFire2);
			AITimer = -80;
		}
	}

	void DoSatelites()
	{
		AITimer++;
		
		AnimationStyle = AnimationStyles.Open;
            
		if (AITimer == 1 && MPUtils.NotMPClient)
		{
			SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
			AITimer3 = Main.rand.Next(10000000);
                
			NPC.netUpdate = true;
		}
		
		AITimer3++;
		UnifiedRandom rand = new((int)AITimer3);
		if (AITimer >= 20)
		{
			if (AITimer % 20 == 0)
			{
				MPUtils.NewProjectile(null, NPC.Center, new Vector2(Main.rand.NextFloat(1, 5) * NPC.direction, Main.rand.NextFloat(-5, -3)), ProjectileType<GarbageDrone>(), 20, 0, ai1: Helper.FromAToB(NPC.Center, player.Center + player.velocity * 2, false).X, ai2: rand.NextFloat(0.02f, 0.035f));
			}

			if (AITimer % 5 == 0)
				MPUtils.NewProjectile(null, NPC.Center, new Vector2(Main.rand.NextFloat(1, 5) * NPC.direction, Main.rand.NextFloat(-5, -3)), ProjectileType<GarbageDrone>(), 20, 0, ai1: rand.NextFloat(-1500, 1500), ai2: rand.NextFloat(0.02f, 0.035f));
		}

		if (AITimer >= 100)
		{
			ResetTo(State.PipeBombAirstrike, null, true);
			AITimer = -230;
		}
	}
	
	void DoPipebombAirstrike() 
	{
		if (AITimer > 25)
		{
			AnimationStyle = AnimationStyles.BoostWarning;
			if (!NPC.Grounded() && NPC.velocity.Length() > 4f)
				AnimationStyle = AnimationStyles.Boost;
		}

		if (AITimer > 200 && NPC.Grounded())
			AnimationStyle = AnimationStyles.Idle;
		
		AITimer++;
		if (AITimer == 2)
			SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
		if (AITimer <= 75)
		{
			if (AITimer > 25)
			{
				NPC.rotation += ToRadians(-0.9f * 2.5f * NPC.direction);
				float progress = (AITimer - 25f) / 50f;
				Vector2 velocity = new Vector2(NPC.direction * MathHelper.Lerp(0.5f, 1, progress), 0).RotatedBy(NPC.rotation);
				Vector2 newVelocity = velocity * progress * 50;
				NPC.velocity = new Vector2(MathHelper.Lerp(NPC.velocity.X, newVelocity.X, progress), newVelocity.Y);

				NPC.noTileCollide = true;
			}
			else
			{
				NPC.direction = player.Center.X < NPC.Center.X ? 1 : -1; // Mirrored, don't replace with FacePlayer.
				NPC.velocity.X = NPC.direction * AITimer * 0.3f;
				Phase();
			}
		}
		
		if (AITimer >= 75 && AITimer < 150)
		{
			NPC.damage = 60;
			if (AITimer < 150)
				DisposablePosition = player.Center;
			NPC.direction = NPC.spriteDirection = 1;
			NPC.rotation = Lerp(NPC.rotation, ToRadians(90), 0.15f);
			NPC.velocity = Helper.FromAToB(NPC.Center, DisposablePosition - new Vector2(-player.velocity.X * 10, 700), false) * 0.05f;
		}
		
		if (AITimer is > 150 and < 165 && AITimer % 2 == 0)
		{
			NPC.velocity = Vector2.Zero;
			MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center + new Vector2(0, -100 + (AITimer - 150) * 35), Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0, ai0: 1);
		}
		if (AITimer > 170 && AITimer <= 200 && AITimer % 3 == 0)
			MPUtils.NewProjectile(null, Main.rand.NextVector2FromRectangle(NPC.getRect()), Vector2.UnitY.RotatedByRandom(PiOver2) * Main.rand.NextFloat(5, 10), ProjectileType<Pipebomb>(), 15, 0);
		
		if (AITimer == 200)
		{
			SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
			NPC.velocity = new Vector2(0, 30);
		}

		bool canHitTiles = NPC.Center.Y > player.Center.Y - NPC.width * 0.4f;
		
		if (AITimer > 200 && canHitTiles)
			NPC.noTileCollide = false;
		
		if (AITimer > 200 && !canHitTiles)
		{
			NPC.velocity.Y += 0.1f;
			NPC.position.Y += NPC.velocity.Y;
		}
		if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer >= 200)
		{
			SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
			NPC.velocity = new Vector2(-NPC.direction * 4, -5);
			MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FlameExplosionWSpriteHostile>(), 16, 0);
			AITimer2 = 1;
		}
		if (AITimer2 >= 1)
		{
			NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
			NPC.velocity.Y += 0.1f;
			NPC.velocity.X *= 0.97f;
			AITimer2++;
		}
		if (AITimer2 >= 50)
		{
			NPC.velocity = Vector2.Zero;
			ResetTo(State.OpenLid, State.GiantFireball);
		}
	}

    void DoMassiveLaser()
    {
        AnimationStyle = AnimationStyles.BoostWarning;
        if (AITimer > 25 && NPC.velocity.Length() > 4f)
            AnimationStyle = AnimationStyles.Boost;
        
        AITimer++;

        Vector2 thrusterPosition = NPC.Center + new Vector2(NPC.width * -NPC.direction * 0.52f, 4).RotatedBy(NPC.rotation);
        if (AITimer < 100)
        {
	        if (AITimer > 45)
	        {
		        thrusterFlareAlpha = MathHelper.Lerp(thrusterFlareAlpha, 1, 0.1f);
		        
		        NPC.rotation += ToRadians(-0.9f * 5.6f * NPC.direction);
		        float progress = (AITimer - 45f) / 55f;
		        Vector2 velocity = new Vector2(NPC.direction * MathHelper.Lerp(0.5f, 1f, progress), 0).RotatedBy(NPC.rotation);
		        Vector2 newVelocity = velocity * progress * 120;
		        NPC.velocity = new Vector2(MathHelper.Lerp(NPC.velocity.X, newVelocity.X * 0.3f, progress), newVelocity.Y * 1.4f);

		        NPC.noTileCollide = true;
	        }
	        else
	        {
		        if (AITimer % 15 == 0)
			        MPUtils.NewProjectile(NPC.InheritSource(NPC), thrusterPosition, -NPC.rotation.ToRotationVector2() * new Vector2(NPC.direction, 0), ProjectileType<HeatBlastVFX>(), 0, 0);

		        if (AITimer < 25)
					thrusterFlareAlpha = MathHelper.Lerp(thrusterFlareAlpha, 0.5f, 0.1f);
		        else 
			        thrusterFlareAlpha = MathHelper.Lerp(thrusterFlareAlpha, 0.75f, 0.05f);
		        
		        if (AITimer < 2)
					FacePlayer();
		        NPC.velocity.X = NPC.direction * AITimer * 0.1f;
		        Phase();
	        }
        }
        else
        {
	        NPC.noTileCollide = false;
	        if (AITimer < 200)
				NPC.rotation = Utils.AngleLerp(NPC.rotation, PiOver2 * NPC.direction, 0.2f);
        }

        Phase();

        if (AITimer > 90 && (int)AITimer3 != 3)
        {
            NPC.damage = 60;
            DisposablePosition = NPC.Center;
            NPC.velocity.X *= 0.9f;
	        NPC.velocity.Y += 1;
	        NPC.Center += Vector2.UnitY * MathHelper.Clamp(NPC.velocity.Y, 0, 20);
        }
        
        bool colliding = NPC.Grounded(0.95f);
        if (colliding && AITimer > 100 && AITimer < 360)
        {
            if ((int)AITimer3 != 3)
            {
                DisposablePosition = NPC.Center + new Vector2(0, NPC.height * 0.5f);
                AITimer3 = 3;
                for (int i = 0; i < 4; i++)
                    MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center + Main.rand.NextVector2Circular(15, 15), Vector2.Zero, ProjectileType<FlameExplosionWSpriteHostile>(), 17, 0);
	            MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<GarbageImpact>(), 0, 0);
            }
            else
            {
	            if (DisposablePosition.Distance(NPC.Center) < 1000f)
					NPC.Center = Vector2.Lerp(NPC.Center, DisposablePosition + Main.rand.NextVector2Circular(AITimer2 * 10f, AITimer2), 0.2f);
                NPC.velocity = Vector2.Zero;
            }
            if (AITimer % 3 - (int)AITimer2 == 0)
            {
                Vector2 pos = NPC.Center - new Vector2(Main.rand.NextFloat(-NPC.width, NPC.width) * 0.7f, NPC.height * 0.3f);
                Dust.NewDustPerfect(pos, DustType<GarbageFlameDust>(), -Vector2.UnitY.RotatedByRandom(0.6f) * Main.rand.NextFloat(10, 15), newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.105f, 0.25f));
            }
        }

        if (AITimer is > 100 and < 340 && AITimer % 25 == 0)
        {
	        MPUtils.NewProjectile(null, thrusterPosition, -Vector2.UnitY.RotatedByRandom(0.85f) * Main.rand.NextFloat(10, 20), ProjectileType<GarbageGiantFlame>(), 15, 0, ai2: 1);
        }
        
        if (AITimer == 80)
        {
            SoundEngine.PlaySound(Sounds.eruption.WithVolumeScale(0.8f), NPC.Center);
            if (!Main.dedServ)
                LaserSoundSlot = SoundEngine.PlaySound(Sounds.garbageLaser.WithVolumeScale(1.35f), NPC.Center);
            CameraSystem.ScreenShakeAmount = 5;
            AITimer2 = 1;
            MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
            MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageLaserSmall1>(), 100, 0, ai0: NPC.whoAmI);
        }
        if (AITimer == 180)
        {
            if (!Main.dedServ)
                if (SoundEngine.TryGetActiveSound(LaserSoundSlot, out var sound))
                {
                    sound.Pitch += 0.3f;
                    sound.Volume += 0.3f;
                }
            CameraSystem.ScreenShakeAmount = 10;
            AITimer2 = 1.5f;
            MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
        }
        if (AITimer == 240)
        {
            if (!Main.dedServ)
                if (SoundEngine.TryGetActiveSound(LaserSoundSlot, out var sound))
                {
                    sound.Pitch += 0.4f;
                    sound.Volume += 0.4f;
                }
            CameraSystem.ScreenShakeAmount = 15;
            AITimer2 = 2.25f;
            MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
        }
        
        if (AITimer == 340)
            MPUtils.NewProjectile(null, NPC.Center - new Vector2(6 * NPC.direction, 40), -Vector2.UnitY * 10, ProjectileType<GarbageGiantFlame>(), 20, 0, ai2: 1);
        
        if (AITimer >= 400)
        {
	        NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
	        thrusterFlareAlpha = MathHelper.Lerp(thrusterFlareAlpha, 0, 0.1f);
        }

        if (AITimer >= 440)
        {
            if (!Main.dedServ)
                if (SoundEngine.TryGetActiveSound(LaserSoundSlot, out var sound))
                {
                    sound.Stop();
                }
            NPC.velocity = Vector2.Zero;
            ResetTo(State.SummonDrones);
        }
    }
    
    void DoSummonDrones()
    {
	    FacePlayer();
	    AnimationStyle = AnimationStyles.Idle;

	    if (AITimer < 45)
		    AnimationStyle = AnimationStyles.Close;
	    
	    AITimer++;
	    if (AITimer == 10)
	    {
		    SoundEngine.PlaySound(SoundID.Zombie67.WithPitchOffset(-1f), NPC.Center);
	    }

	    if (AITimer > 10 && AITimer < 30 && AITimer % 2 == 0) 
	    {
		    MPUtils.NewProjectile(null, NPC.Center + new Vector2(NPC.direction * (AITimer - 10) * -60 + NPC.direction * 200, -500), Vector2.UnitY.RotatedBy(-NPC.direction), ProjectileType<GarbageTechTelegraph>(), 0, 0, ai1: 2);

		    float rotation = Main.rand.NextFloat(-0.5f, 0.5f);
		    MPUtils.NewProjectile(null, NPC.Center - new Vector2(0, 40).RotatedBy(rotation), -Vector2.UnitY.RotatedBy(rotation), ProjectileType<GarbageTechTelegraph>(), 0, 0, ai1: .5f);    
	    }

	    if (AITimer > 45 && AITimer < 180 && AITimer % 7 == 0)
	    {
		    MPUtils.NewProjectile(null, NPC.Center - new Vector2(850 * NPC.direction, 600), new Vector2(7 * NPC.direction, 10) * Main.rand.NextFloat(0.9f, 1.1f), ProjectileType<LaserDrone>(), 10, 0);
	    }
	    
	    if (AITimer > 230) 
		    ResetTo(State.OpenLid, State.ReticleMissiles);
    }

    void DoReticleMissiles()
    {
	    if (AITimer < 60)
			AnimationStyle = AnimationStyles.Open;
	    else if (AITimer < 100)
		    AnimationStyle = AnimationStyles.Close;
	    else if (NPC.velocity.Length() < 1f && AITimer < 210)
		    AnimationStyle = AnimationStyles.BoostWarning;
	    else if (AITimer < 210)
		    AnimationStyle = AnimationStyles.Boost;
	    else 
		    AnimationStyle = AnimationStyles.Idle;
	    
	    AITimer++; 
	    
	    if (AITimer <= 60 && AITimer % 5 == 0)
	    {
		    Vector2 pos = player.Center + Main.rand.NextVector2Circular(200, 200);
		    
		    SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
		    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Top, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-15, -7)) * 0.5f, ProjectileType<GarbageReticleMissile>(), 15, 0, ai0: pos.X, ai1: pos.Y, ai2: (AITimer == 55 ? -2 : 1));
	    }

	    if (AITimer > 100)
	    {
		    Phase();

		    if (AITimer < 130)
			    NPC.velocity.X -= NPC.direction * 0.1f;

		    if (AITimer == 152)
		    {
			    SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
			    NPC.velocity.X = 0;
		    }

		    if (AITimer is > 152 and < 162)
			    NPC.velocity += new Vector2(NPC.direction * MathHelper.Lerp(0.5f, 2, (AITimer - 152) / 10f), 0);
		    if (AITimer > 210)
			    NPC.velocity.X *= 0.9f;
		    
		    if (NPC.velocity.Length() > 4f)
				MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ProjectileType<GarbageDashFlames>(), 15, 0, ai2: 2);
	    }
	    
	    if (AITimer is < 100 or > 240)
		    FacePlayer();

	    if (AITimer > 300)
		    ResetTo(State.WarningForDash, null, true);
    }
}