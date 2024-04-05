using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeepTransformOnGrab : MonoBehaviour
{
    private Vector3 _initial_position;
    
    private void OnEnable()
    {
        var line_renderer = GetComponent<LineRenderer>();
        line_renderer.positionCount = 0;

        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable == null)
            return;

        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
        interactable.activated.AddListener(OnActivated);
        interactable.deactivated.AddListener(OnDeactivated);
    }

    private void OnDisable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable == null)
            return;

        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnActivated(ActivateEventArgs args)
    {
        Debug.Log("*** OnActivated()");

        //PublishPointing(2); // RECAPTURE_RES(2): force ROS to recapture 3D point cloud
    }

    private void OnDeactivated(DeactivateEventArgs args)
    {
        Debug.Log("*** OnDeactivated()");

        //PublishPointing(2); // RECAPTURE_RES(2): force ROS to recapture 3D point cloud
    }

    private void Update()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable == null || !interactable.isSelected)
            return;

        var line_renderer = GetComponent<LineRenderer>();
        var positions = new Vector3[] { _initial_position, transform.position };
        line_renderer.positionCount = positions.Length;
        line_renderer.widthMultiplier = 0.1f;
        //line_renderer.material = new Material(Shader.Find("Sprites/Default"));
        //line_renderer.startColor = Color.red;
        //line_renderer.endColor = Color.red;
        line_renderer.SetPositions(positions);
        
        var interactor = interactable.GetOldestInteractorSelecting();

        Debug.Log("*** Dragged");
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("*** OnSelectEntered()");

        //var attachTransform = args.interactorObject.GetAttachTransform(args.interactableObject);
        //attachTransform.position = transform.position;
        //attachTransform.rotation = transform.rotation;

        _initial_position = transform.position;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        var line_renderer = GetComponent<LineRenderer>();
        line_renderer.positionCount = 0;

        Debug.Log("*** OnSelectExited()");
    }
}