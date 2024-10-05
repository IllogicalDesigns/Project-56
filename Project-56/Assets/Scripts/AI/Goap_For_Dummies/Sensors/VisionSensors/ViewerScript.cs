using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewerScript : MonoBehaviour
{
    IVisionSensor visionSensor;
    [SerializeField] GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        visionSensor = GetComponent<IVisionSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        visionSensor.CanWeSeeTarget(target);
    }
}
