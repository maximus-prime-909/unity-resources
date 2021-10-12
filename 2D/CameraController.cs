using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Variables
    public Transform Target;
    public Vector3 offset;

    void Start()
    {
        offset = transform.position - Target.position;
    }

    // Update is called once per frame
    private void LateUpdate() 
    {
        Vector3 desiredPos = new Vector3(Target.position.x + offset.x, Target.position.y + offset.y, Target.position.z + offset.z);
        transform.position = desiredPos;
        
    }
}