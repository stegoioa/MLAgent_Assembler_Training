using UnityEngine;

public class Agent_Collider : MonoBehaviour

    
{
    public bool Collision = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Limit")
        {
            Collision = true;
            Debug.Log("ColliderScript Detected Collision");
            
        }
    }
}
