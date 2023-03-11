using Assets.Scripts.Health;
using Assets.Scripts.Interfaces;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UI.Frames
{
    public class CombatTargetSingle : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI CreatureNameTextMeshProUGUI;
        [SerializeField] private HealthBarGUI HealthBarGUI;
        [SerializeField] private TargetCastBar TargetCastBar;

        public IBaseCreature OwnerCreature { get; private set; }

        public void SetupCreature(IBaseCreature baseCreature)
        {
            OwnerCreature = baseCreature;

            CreatureNameTextMeshProUGUI.text = OwnerCreature.ITargetable.NameWhenTargeted;

            OwnerCreature.Health.CurrentHPChanged += Health_CurrentHPChanged;
            Health_CurrentHPChanged(OwnerCreature.Health);
            
            TargetCastBar.Setup();
            TargetCastBar.UpdateOwnerInfo(OwnerCreature);
        }

        public void OnDestroy()
        {
            TargetCastBar.UpdateOwnerInfo(null);
            OwnerCreature.Health.CurrentHPChanged -= Health_CurrentHPChanged;
        }

        private void Health_CurrentHPChanged(BaseHealth baseHealth)
        {
            HealthBarGUI.SetHitPoints(baseHealth.CurrentHP, baseHealth.MaxHP);
        }
    }
}
