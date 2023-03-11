using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class SceneManager : MonoBehaviour
    {
        private GameObject PlayerObject;

        private CharacterController playerCharController;

        private void Start()
        {
            PlayerObject = GameManager.Instance.PlayerGameObject;
            //playerCharController = PlayerObject.GetComponent<CharacterController>();
        }

        private void Update()
        {
            //Vector3 startPosition = PlayerObject.transform.position + new Vector3(0, playerCharController.height, 0);
            //Vector3 endPosition = startPosition + Camera.main.transform.forward;

            //Debug.DrawLine(startPosition, endPosition, Color.green);

            //startPosition = Vector3.zero + new Vector3(0, playerCharController.height, 0);
            //endPosition = startPosition + Camera.main.transform.forward;

            //Debug.DrawLine(startPosition, endPosition, Color.green);
        }
    }
}
