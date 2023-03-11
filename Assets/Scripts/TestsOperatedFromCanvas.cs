using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public class TestsOperatedFromCanvas : MonoBehaviour
    {
        public void TestResetActionBarsData()
        {
            GameManager.Instance.GUIManager.TestResetActionBarsData();
        }

        public void TestPlayerConsecutiveAbilities()
        {
            StartCoroutine(TestPlayerConsecutiveAbilitiesCoroutine());
        }

        private IEnumerator TestPlayerConsecutiveAbilitiesCoroutine()
        {
            // Debug.Log($"Start {nameof(TestPlayerConsecutiveAbilitiesCoroutine)}");

            var playerCreature = GameManager.Instance.PlayerCreature;
            var playerAbilitiesController = playerCreature.AbilitiesController;

            // playerCreature.AutoAttackController.TryAutoAttackSelectedTarget();
            // yield return new WaitForSeconds(0.3f);

            // var abilityFirst = playerAbilitiesController.Abilities.FirstOrDefault(x => x.AbilitySO.Id == "WAR_StormsEye");
            // var abilitySecond = playerAbilitiesController.Abilities.FirstOrDefault(x => x.AbilitySO.Id == "WAR_Maim");
            var abilityFirst = playerAbilitiesController.GetAbilityById("BLM_Fire3");
            var abilitySecond = playerAbilitiesController.GetAbilityById("BLM_Fire1");

            // Debug.Log($"Try start {abilityStormsEye.AbilitySO.Name}");
            playerAbilitiesController.TryStartCast(abilityFirst);

            float cooldown = Mathf.Max(abilityFirst.AbilityCooldown.GetAbilityCooldown, playerAbilitiesController.TimeUntilGlobalCooldownFinish);
            yield return new WaitForSeconds(cooldown + 0.05f);

            // Debug.Log($"Try start {abilityMaim.AbilitySO.Name}");
            playerAbilitiesController.TryStartCast(abilitySecond);

            // Debug.Log($"Finish {nameof(TestPlayerConsecutiveAbilitiesCoroutine)}");
        }

        public void TestPlayerCastTwoAbilitiesAtTheSameTime()
        {
            // Debug.Log($"Start {nameof(TestPlayerCastTwoAbilitiesAtTheSameTime)}");

            var playerCreature = GameManager.Instance.PlayerCreature;
            var playerAbilitiesController = playerCreature.AbilitiesController;

            var abilityStormsEye = playerAbilitiesController.GetAbilityById("WAR_StormsEye");
            var abilityMaim = playerAbilitiesController.GetAbilityById("WAR_Maim");

            playerAbilitiesController.TryStartCast(abilityStormsEye);
            playerAbilitiesController.TryStartCast(abilityMaim);
        }

        public void TestPlayerConsecutiveSameAbilities()
        {
            StartCoroutine(TestPlayerConsecutiveSameAbilitiesCoroutine());
        }

        private IEnumerator TestPlayerConsecutiveSameAbilitiesCoroutine()
        {
            var playerCreature = GameManager.Instance.PlayerCreature;
            var playerAbilitiesController = playerCreature.AbilitiesController;

            var abilityFire1 = playerAbilitiesController.GetAbilityById("BLM_Fire1");
            if (abilityFire1 == null)
            {
                Debug.LogError($"{nameof(abilityFire1)} == null");
                yield break;
            }

            playerAbilitiesController.TryStartCast(abilityFire1);
            yield return new WaitForSeconds(Mathf.Max(abilityFire1.AbilityCooldown.GetAbilityCooldown, playerAbilitiesController.TimeUntilGlobalCooldownFinish) + 0.02f);

            playerAbilitiesController.TryStartCast(abilityFire1);
            yield return new WaitForSeconds(Mathf.Max(abilityFire1.AbilityCooldown.GetAbilityCooldown, playerAbilitiesController.TimeUntilGlobalCooldownFinish) + 0.02f);

            playerAbilitiesController.TryStartCast(abilityFire1);
            yield return new WaitForSeconds(Mathf.Max(abilityFire1.AbilityCooldown.GetAbilityCooldown, playerAbilitiesController.TimeUntilGlobalCooldownFinish) + 0.02f);

            playerAbilitiesController.TryStartCast(abilityFire1);
            yield return new WaitForSeconds(Mathf.Max(abilityFire1.AbilityCooldown.GetAbilityCooldown, playerAbilitiesController.TimeUntilGlobalCooldownFinish) + 0.02f);

            playerAbilitiesController.TryStartCast(abilityFire1);
            yield return new WaitForSeconds(Mathf.Max(abilityFire1.AbilityCooldown.GetAbilityCooldown, playerAbilitiesController.TimeUntilGlobalCooldownFinish) + 0.02f);
        }

        public void TestPlayerDisengageEveryone()
        {
            Debug.Log("TestPlayerDisengageEveryone()");
            var playerCreature = GameManager.Instance.PlayerCreature;
            playerCreature.CombatInfoHandler.DisengageCombatWithEveryone();
        }

        public void PartyAttackTarget()
        {
            Debug.Log("PartyAttackTarget()");
            var playerCreature = GameManager.Instance.PlayerCreature;
            GameManager.Instance.PartyManager.AttackTarget(playerCreature.PlayerTargetHandler.SelectedTarget);
        }
        
        public void PartyFollowLeader()
        {
            Debug.Log("PartyFollowLeader()");
            GameManager.Instance.PartyManager.FollowLeader();
        }
        
        public void PartyWithdrawToLeader()
        {
            Debug.Log("PartyWithdrawToLeader()");
            GameManager.Instance.PartyManager.WithdrawToLeader();
        }
    }
}