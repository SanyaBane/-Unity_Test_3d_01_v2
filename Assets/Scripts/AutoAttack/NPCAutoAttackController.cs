namespace Assets.Scripts.AutoAttack
{
    public class NPCAutoAttackController : AutoAttackController
    {
        protected override void Start()
        {
            base.Start();

            UpdateAutoAttackFromSO();
        }
    }
}
