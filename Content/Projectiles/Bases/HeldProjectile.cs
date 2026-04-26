using System.IO;

namespace EbonianMod.Content.Projectiles.Bases
{
    public abstract class HeldProjectile : ModProjectile
    {
        protected int ItemType;
        protected float AttackDelayMultiplier, RecoilMultiplier, AttackSpeedMultiplier;
        protected Vector2 Difference, HoldOffset;
        protected bool PlayerFacesCursor = true;

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        protected void CalculateAttackSpeedParameters(float baseValue)
        {
            float itemTime = Main.player[Projectile.owner].itemTime;
            AttackDelayMultiplier = itemTime / baseValue;
            AttackSpeedMultiplier = baseValue / itemTime;
            RecoilMultiplier = Min(AttackDelayMultiplier, 1);
            Projectile.netUpdate = true;
        }

        public override bool ShouldUpdatePosition() => false;

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            player.itemTime = 2;
            player.itemAnimation = 2;

            if (Projectile.timeLeft < 1)
            {
                Projectile.timeLeft = 1000;
            }

            if (!player.active || player.DeadOrGhost || player.CCed || player.HeldItem.type != ItemType)
            {
                Projectile.Kill();
            }

            if (player.whoAmI == Main.myPlayer)
            {
                Difference = Main.MouseWorld - player.Center;
                if (PlayerFacesCursor)
                {
                    Projectile.spriteDirection = Difference.X > 0 ? 1 : -1;
                    player.ChangeDir(Projectile.spriteDirection);
                }
                LocalBehaviour(player);
            }

            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + new Vector2(HoldOffset.X, HoldOffset.Y * Projectile.spriteDirection).RotatedBy(Projectile.rotation);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(AttackDelayMultiplier);
            writer.Write(AttackSpeedMultiplier);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            AttackDelayMultiplier = reader.ReadSingle();
            AttackSpeedMultiplier = reader.ReadSingle();
        }

        protected virtual void LocalBehaviour(Player player) { }
    }

}