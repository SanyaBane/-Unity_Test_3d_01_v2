using System.Linq;
using Assets.Scripts.Buffs.Behaviours;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Stats.Wrapped;

namespace Assets.Scripts.Stats
{
    public class IncreasedDamagePercentage : BaseCachedStat<WrappedFloat>
    {
        private IBaseCreature _iBaseCreature { get; }
        
        public IncreasedDamagePercentage(IBaseCreature iIBaseCreature)
        {
            _iBaseCreature = iIBaseCreature;
        }

        protected override void Calculate()
        {
            var allBuffs = _iBaseCreature.BuffsController.GetAllBuffs();
            var tmp1 = allBuffs.SelectMany(x => x.BaseBuffSO.BuffBehavioursSO);
            var buffBehavioursIncreaseDamage = tmp1.Where(x => x is IBuffBehaviourIncreaseDamage).Cast<IBuffBehaviourIncreaseDamage>();

            float increaseDamagePercentageSum = 0;
            foreach (var element in buffBehavioursIncreaseDamage)
            {
                increaseDamagePercentageSum += element.GetIncreaseDamagePercentage();
            }

            _wrappedValue.Value = increaseDamagePercentageSum;
        }
    }
}