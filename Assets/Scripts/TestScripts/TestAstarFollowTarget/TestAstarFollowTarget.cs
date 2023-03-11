using Pathfinding;
using UnityEngine;

public class TestAstarFollowTarget : MonoBehaviour
{
    public Transform Ally;
    public Transform Enemy;
    public Transform Player;
    
    public void Attack()
    {
        Debug.Log("Attack()");
        Ally.GetComponent<AIPath>().destination = Enemy.position;
        // Ally.GetComponent<AIPath>().canMove = false;
    }
    
    public void GoToPlayer()
    {
        Debug.Log("GoToPlayer()");
        Ally.GetComponent<AIPath>().destination = Player.position;
        // Ally.GetComponent<AIPath>().canMove = true;
    }
}
