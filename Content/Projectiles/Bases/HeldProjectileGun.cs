using System.IO;

namespace EbonianMod.Content.Projectiles.Bases
{
    public abstract class HeldProjectileGun : HeldProjectile
    {
        protected float RotationSpeed, RecoilRecoverySpeed = 0.2f, RecoilRotation;
        protected float AimingOffset;

        public override bool? CanDamage() => false;

        public override void AI()
        {
            base.AI();
            Player player = Main.player[Projectile.owner];
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - PiOver2);

            RecoilRotation = Lerp(RecoilRotation, 0, RecoilRecoverySpeed);
            Projectile.netUpdate = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Projectile.rotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Projectile.rotation = reader.ReadSingle();
        }

        protected override void LocalBehaviour(Player player)
        {
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, (Difference + new Vector2(0, AimingOffset + player.Center.Y - player.MountedCenter.Y).RotatedBy(Projectile.rotation) * Projectile.spriteDirection).ToRotation(), RotationSpeed) + RecoilRotation * RecoilMultiplier;
        }
    }

}