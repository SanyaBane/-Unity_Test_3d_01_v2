using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Abilities.General.ScriptableObjects;
using Assets.Scripts.Enums;
using UnityEngine;

namespace Assets.Scripts.AutoAttack
{
    public class PlayerAutoAttackAssigner : MonoBehaviour
    {
        public List<AbilityAutoAttackSO> ListAbilityAutoAttackSO;

        public void SetJob(EJob job)
        {
            var autoAttackForJob = ListAbilityAutoAttackSO
                .First(x => x.Job == job);

            var currentAutoAttackComponent = this.GetComponent<AutoAttackController>();
            currentAutoAttackComponent.ChangeAutoAttackSO(autoAttackForJob);

            //if (currentAutoAttackComponent != null)
            //    Destroy(currentAutoAttackComponent);

            //var newAutoAttackComponent = this.gameObject.AddComponent<AutoAttack>();
            //newAutoAttackComponent.Setup(autoAttackForJob);
        }
    }
}
