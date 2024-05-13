using UnityEngine;

public class SelfDestructScript : MonoBehaviour
{
    public float life;   
    void Start()
    {
        Destroy(gameObject,life);
    }


}
