using UnityEngine;
public class FootStepsBehaviour : MonoBehaviour
{
    [SerializeField]
    public static int footstepamount = 0;

    public static int totalSpawned = 0;
    public void Clean()
    {
        footstepamount++;
        Debug.Log("CLEAN");
        Destroy(gameObject);
        
    }
}
