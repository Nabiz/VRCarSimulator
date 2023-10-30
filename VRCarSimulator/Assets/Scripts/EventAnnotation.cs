using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PupilLabs;

public class EventAnnotation : MonoBehaviour
{
    [SerializeField] AnnotationPublisher annotationPublisher;
    [SerializeField] RecordingController recordingController;

    [SerializeField] string infoText;
    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "PlayerCar")
        {
            if (recordingController.IsRecording)
            {
                annotationPublisher.SendAnnotation("event", 0.0f, new Dictionary<string, object> { {"info", infoText} });
            }
        }

    }
}
