using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class KeepTransformOnGrab : MonoBehaviour
{
    private void OnEnable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable == null)
            return;

        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    // コンポーネントが無効化された時に実行
    private void OnDisable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        if (interactable == null)
            return;

        interactable.selectEntered.RemoveListener(OnSelectEntered);
        interactable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("*** OnSelectEntered()");

        var attachTransform = args.interactorObject.GetAttachTransform(args.interactableObject);
        attachTransform.position = transform.position;
        attachTransform.rotation = transform.rotation;
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        Debug.Log("*** OnSelectExited()");

    }
}