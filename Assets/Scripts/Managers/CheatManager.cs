using Assets.Scripts;
using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField] Vector3 _teleportPosition = Vector3.zero;
    [SerializeField] Vector3 _teleportRotation = Vector3.zero;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            TestTeleport(GameManager.Instance.PlayerCreature, _teleportPosition, Quaternion.Euler(_teleportRotation));
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            TestTeleport(GameManager.Instance.PlayerCreature, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

    private void TestTeleport(IBaseCreature baseCreature, Vector3 pos, Quaternion rotation)
    {
        baseCreature.GetRootObjectTransform().position = pos;
        baseCreature.GetRootObjectTransform().rotation = rotation;
    }
}
