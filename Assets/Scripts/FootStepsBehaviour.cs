using UnityEngine;
public class FootStepsBehaviour : MonoBehaviour
{
    [SerializeField]
    public int footstepamount = 0;

    public int totalSpawned = 0;
    public void Clean()
    {
        footstepamount++;
        Debug.Log("CLEAN");
        Destroy(gameObject);
    }
}
