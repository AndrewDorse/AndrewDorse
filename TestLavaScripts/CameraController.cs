using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{

   
    float distance = 12f;
    float height = 10f;
    

    public Transform camTarget;

  

    private Vector3 offset;
    

   


    void Start()
    {
        transform.rotation = Quaternion.Euler(36f, 0.0f, 0.0f);
    }
    

    void LateUpdate()
    {
        //Check if the camera has a target to follow
        if (!camTarget)
            return;

        Vector3 pos = Vector3.zero;
        pos.x = camTarget.position.x;
        pos.y = camTarget.position.y + height;
        pos.z = camTarget.position.z - distance;

        
        transform.position = pos + offset; 
        
    }


   

   


}
