﻿using System;
using System.Collections.Generic;

namespace EbonianMod.Content.Projectiles.Friendly.Corruption;

public class CorruptionHitscan : ModProjectile
{
    public override string Texture => Helper.Empty;

    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(5, 5);
        Projectile.penetrate = 1;
        Projectile.extraUpdates = 12;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 6000;
        Projectile.friendly = true;
    }

    public override void AI()
    {
        if (Projectile.timeLeft == 6000) return;

        for(float i = 0; i < 1; i += 0.25f)
        {
            Dust.NewDustPerfect(Projectile.Center - Projectile.velocity * i, DustID.Demonite, Main.rand.NextVector2Circular(0.2f, 0.2f), Scale: 0.9f);
        }
    }

    public override void OnKill(int timeLeft)
    {
        float angle = Projectile.velocity.ToRotation();
        for (int i = 0; i < 15; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.Demonite, (angle + Main.rand.NextFloat(-Pi / 6, Pi / 6)).ToRotationVector2() * Main.rand.NextFloat(2, 4), Scale: 1f);
        }
    }
}