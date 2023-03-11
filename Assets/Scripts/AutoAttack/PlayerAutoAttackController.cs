using Assets.Scripts.Managers;

namespace Assets.Scripts.AutoAttack
{
    public class PlayerAutoAttackController : AutoAttackController
    {
        protected override void Update()
        {
            base.Update();

            if (_health.IsAlive && GameManager.Instance.InputController_WoW.IsWeaponSheathPressed)
                WeaponSheathed = !WeaponSheathed;
        }
    }
}
