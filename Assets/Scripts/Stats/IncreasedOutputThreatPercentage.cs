using System.Linq;
using Assets.Scripts.Buffs.Behaviours;
using Assets.Scripts.Interfaces;
using Assets.Scripts.Stats.Wrapped;

namespace Assets.Scripts.Stats
{
    public class IncreasedOutputThreatPercentage : BaseCachedStat<WrappedFloat>
    {
        private IBaseCreature _iBaseCreature { get; }
        
        public IncreasedOutputThreatPercentage(IBaseCreature iIBaseCreature)
        {
            _iBaseCreature = iIBaseCreature;
        }

        protected override void Calculate()
        {
            // Calculate
            var allBuffs = _iBaseCreature.BuffsController.GetAllBuffs();
            var tmp1 = allBuffs.SelectMany(x => x.BaseBuffSO.BuffBehavioursSO);
            var buffBehavioursIncreaseOutputThreat = tmp1.Where(x => x is IBuffBehaviourIncreaseOutputThreat).Cast<IBuffBehaviourIncreaseOutputThreat>();

            float increaseOutputThreatPercentageSum = 0;
            foreach (var element in buffBehavioursIncreaseOutputThreat)
            {
                increaseOutputThreatPercentageSum += element.GetIncreaseOutputThreatPercentage();
            }

            _wrappedValue.Value = increaseOutputThreatPercentageSum;
        }
    }
}