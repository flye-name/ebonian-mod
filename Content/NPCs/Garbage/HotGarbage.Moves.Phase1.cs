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
            if (AITimer < 50)
            {
	            NPC.velocity = NPC.velocity.RotatedBy(ToRadians(-NPC.direction * 2));
            }

            if (AITimer < 200)
                NPC.noTileCollide = true;
            if (AITimer >= 50 && AITimer < 181)
            {
                if (AITimer < 176)
                    DisposablePosition = player.Center - new Vector2(-player.velocity.X * 20, 500);
                NPC.direction = NPC.spriteDirection = 1;
                NPC.rotation = Utils.AngleLerp(NPC.rotation, ToRadians(90), 0.05f);
                if (AITimer % 8 == 0)
                    NPC.velocity = Helper.FromAToB(NPC.Center, DisposablePosition, false) * MathHelper.Lerp(0.025f, 0.056f, Helper.Saturate((AITimer - 50f) / 50f));
            }
            if (AITimer == 2)
                SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
            if (AITimer == 181)
            {
                NPC.velocity = Vector2.Zero;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);
            }
            if (AITimer == 200)
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
}