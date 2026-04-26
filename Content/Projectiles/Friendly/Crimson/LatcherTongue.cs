using EbonianMod.Content.NPCs.Corruption;
using EbonianMod.Content.Projectiles.Cecitior;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader.IO;

namespace EbonianMod.Content.Projectiles.Friendly.Crimson;

public class LatcherTongue : ModProjectile
{
    public override string Texture => Helper.Empty;

    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
    }

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(32);
        Projectile.DamageType = DamageClass.Magic;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 200;
        Projectile.tileCollide = true;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.hide = true;
        Projectile.aiStyle = -1;
    }

    bool _isAttached;
    Vector2 _positionOffset;
    float _speed;
    int _targetIndex = -1;

    Vector2 _neckOrigin
    {
        get => new Vector2(Projectile.ai[0], Projectile.ai[1]);
    }

    public override bool? CanDamage() => !_isAttached && Projectile.ai[2] < 27;

    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.FinalDamage *= 0;
        modifiers.Knockback *= 0;
    }

    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        if(_targetIndex == -1)
        {
            _positionOffset = Projectile.Center - target.Center;
            _targetIndex = target.whoAmI;
            _isAttached = true;

            Projectile.velocity = Vector2.Zero;
            Projectile.netUpdate = true;
        }
    }

    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindProjectiles.Add(index);
    }

    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (_isAttached)
        {
            if (_targetIndex >= Main.npc.Length - 1)
            {
                Projectile.Kill();
                return;
            }

            NPC target = Main.npc[_targetIndex];
            if (!target.active || target.life <= 0 || player.DeadOrGhost)
            {
                Projectile.Kill();
                return;
            }

            Vector2 step = Projectile.Center - player.Center;
            if (Helper.Raycast(player.Center, step, step.Length()).Success || (player.whoAmI == Main.myPlayer && Main.mouseRight))
            {
                player.velocity *= 0.8f;
                Projectile.tileCollide = false;
                Projectile.timeLeft = 189;
                _isAttached = false;

                return;
            }
            Projectile.timeLeft = 190;

            if (_speed < 30)
            {
                _speed += 1.5f;
            }
            player.velocity = Helper.FromAToB(player.Center, Projectile.Center, true) * _speed;

            Projectile.Center = target.Center + _positionOffset;
            float playerSpeed = player.velocity.Length();
            if (Vector2.Distance(player.Center, Projectile.Center) < Max(playerSpeed, 16))
            {
                player.velocity *= -0.35f;
                player.immune = true;
                player.immuneTime = 20;
                target.SimpleStrikeNPC((int)(Projectile.damage * (playerSpeed / 30f) * (playerSpeed / 30f)), -(int)player.velocity.X, false, Projectile.knockBack * playerSpeed / 30f);

                SoundEngine.PlaySound(Sounds.chomp1.WithPitchOffset(Main.rand.NextFloat(-0.6f, -0.2f)), Projectile.Center);
                SoundEngine.PlaySound(Sounds.chomp2.WithPitchOffset(Main.rand.NextFloat(0.2f, 0.4f)), Projectile.Center);

                Projectile.Kill();
            }
        }
        else if (Projectile.timeLeft < 190)
        {
            Projectile.Center += Helper.FromAToB(Projectile.Center, _neckOrigin) * Projectile.ai[2];
            if (Projectile.ai[2] >= 30)
            {
                Projectile.tileCollide = false;
            }
            if (Vector2.Distance(Projectile.Center, _neckOrigin) < Projectile.ai[2])
            {
                Projectile.Kill();
            }
            Projectile.ai[2] += 3;
        }

        Projectile.netUpdate = true;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.tileCollide = false;
        Projectile.timeLeft = 189;
        Projectile.ai[2] = 27;

        Projectile.netUpdate = true;

        return false;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (_neckOrigin == Vector2.Zero) return false;

        Texture2D texture = Assets.Projectiles.Friendly.Crimson.Latcher_Chain.Value;
        Vector2 centerDifference = _neckOrigin - Projectile.Center;
        float length = centerDifference.Length();
        float angle = centerDifference.ToRotation();
        centerDifference.Normalize();

        for (float i = texture.Width / 2f; i < length; i += texture.Width)
        {
            Main.spriteBatch.Draw(texture, Projectile.Center + centerDifference * i - Main.screenPosition, null, lightColor, angle, texture.Size() / 2, 1, SpriteEffects.None, 0);
        }
       
        return false;
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Projectile.tileCollide);
        writer.Write(_isAttached);
        writer.Write(_targetIndex);
        writer.WriteVector2(_positionOffset);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Projectile.tileCollide = reader.ReadBoolean();
        _isAttached = reader.ReadBoolean();
        _targetIndex = reader.ReadInt32();
        _positionOffset = reader.ReadVector2();
    }
}
public class LatcherTongueCecitior : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.width = 12;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.tileCollide = true;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.hide = true;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
    }
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        Projectile.velocity = Vector2.Zero;
        if (Projectile.localAI[0] == 0)
        {
            Projectile.timeLeft = 200;
            Projectile.ai[1] = 1;
        }
        return false;
    }
    public override void OnHitPlayer(Player target, Player.HurtInfo info)
    {
        Projectile.velocity = Vector2.Zero;
        if (Projectile.localAI[0] == 0 && Projectile.ai[1] == 0)
        {
            Projectile.localAI[0] = target.whoAmI;
            Projectile.timeLeft = 100;
            Projectile.ai[1] = 2;
        }
    }

    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Projectile.localAI[0]);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Projectile.localAI[0] = reader.ReadSingle();
    }

    public override void AI()
    {
        if (Helper.Raycast(Projectile.Center, Vector2.UnitY, 30, true).RayLength < 20)
        {
            Projectile.velocity = Vector2.Zero;
            if (Projectile.localAI[0] == 0)
            {
                Projectile.timeLeft = 200;
                Projectile.ai[1] = 1;
            }
        }
        if (Projectile.ai[1] == 0)
            Projectile.rotation = Projectile.velocity.ToRotation();
        if (!NPC.AnyNPCs(NPCType<NPCs.Cecitior.Cecitior>()))
            Projectile.Kill();
        NPC player = Main.npc[(int)Projectile.ai[0]];
        if (player.ai[0] != 8)
            Projectile.Kill();
        if (Projectile.ai[1] == 1)
        {
            player.velocity = Helper.FromAToB(player.Center, Projectile.Center) * 25;
            if (player.Center.Distance(Projectile.Center) < 50)
            {
                Projectile.ai[2] = 1;
                MPUtils.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(0, 0), ProjectileType<FatSmash>(), 0, 0, 0, 0);

                for (int i = -6; i < 6; i++)
                {
                    if (i == 0) continue;
                    MPUtils.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, new Vector2(i * 3, Lerp(-3, -5, MathF.Abs(i) / 6)), ProjectileType<CecitiorTeeth>(), 20, 0, 0, 0);
                }
                player.velocity = Projectile.rotation.ToRotationVector2().RotatedByRandom(PiOver4) * -10f;
                Projectile.Kill();
                SoundEngine.PlaySound(Sounds.cecitiorSlam, Projectile.Center);
                Projectile.netUpdate = true;
            }
        }
        else if (Projectile.ai[1] == 2)
        {
            Player playerr = Main.player[(int)Projectile.localAI[0]];
            playerr.velocity = Helper.FromAToB(playerr.Center, player.Center, false) / 10;
            Projectile.velocity = Helper.FromAToB(Projectile.Center, player.Center) * 20;
        }
        else
        {
            if (Projectile.velocity.Length() < 24)
                Projectile.velocity *= 1.15f;
            if (Projectile.timeLeft < 100)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, 0.4f);

                if (player.Center.Distance(Projectile.Center) < 50)
                    Projectile.Kill();
                Projectile.netUpdate = true;
            }
        }
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCs.Add(index);
    }
    public override bool PreDraw(ref Color lightColor)
    {

        NPC player = Main.npc[(int)Projectile.ai[0]];
        Vector2 neckOrigin = Projectile.Center;
        Vector2 center = player.Center;
        Vector2 distToProj = neckOrigin - player.Center;
        float projRotation = distToProj.ToRotation() - 1.57f;
        float distance = distToProj.Length();
        while (distance > 20 && !float.IsNaN(distance))
        {
            distToProj.Normalize();
            distToProj *= 20;
            center += distToProj;
            distToProj = neckOrigin - center;
            distance = distToProj.Length();

            //Draw chain
            Main.spriteBatch.Draw(Assets.Projectiles.Friendly.Crimson.Latcher_Chain.Value, center - Main.screenPosition,
                null, Lighting.GetColor((int)center.X / 16, (int)center.Y / 16), projRotation,
                Assets.Projectiles.Friendly.Crimson.Latcher_Chain.Value.Size() / 2, 1f, SpriteEffects.None, 0);
        }
        return true;
    }
}
