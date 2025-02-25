using UnityEngine;

public class VisionDetection : MonoBehaviour
{
    private EnemyStateMachine enemyStateMachine;

    void Start()
    {
        enemyStateMachine = GetComponentInParent<EnemyStateMachine>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyStateMachine.playerDetect = true;
            Debug.Log("Player detected");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemyStateMachine.playerDetect = false;
            Debug.Log("Player lost");
        }
    }
}