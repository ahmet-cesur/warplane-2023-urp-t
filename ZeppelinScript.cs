using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeppelinScript : MonoBehaviour
{
    BonusEnemyScript bes;
    private float h;
    Transform tr;
    public Vector3 fallVector;
    public Vector3 rotateVector;

    private void Awake()
    {
        tr= transform;
    }
    void Start()
    {
        bes = GetComponent<BonusEnemyScript>();
        h = bes.health;
        fallVector=Vector3.zero;
        rotateVector=Vector3.zero;
    }

    
    void Update()
    {
        if (h != bes.health)
        {           
            h=bes.health;
            bes.canvasScript.GiveExplosion(gameObject.transform.position);
        }
        if(bes.health<=0)
        {
            fallVector.y += Time.deltaTime;
            fallVector.z = fallVector.y*0.2f;
            rotateVector.x = fallVector.z;
            tr.Rotate(rotateVector * Time.deltaTime);
            tr.position += -1f*fallVector*Time.deltaTime;
        }
        
    }
}
