using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider provider;

    private InputAction activateTeleport;
    private InputAction cancelTeleport;
    
    private bool teleportIsActive;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor.enabled = false;

        activateTeleport = actionAsset.FindActionMap("XRI Lefthand Locomotion").FindAction("Teleport Mode Activate");
        activateTeleport.Enable();
        activateTeleport.performed += OnTeleportActivate;
        
        cancelTeleport = actionAsset.FindActionMap("XRI Lefthand Locomotion").FindAction("Teleport Mode Cancel");
        cancelTeleport.Enable();
        cancelTeleport.performed += OnTeleportCancel;
    }

    public void OnDestroy()
    {
        activateTeleport.performed -= OnTeleportActivate;
        cancelTeleport.performed -= OnTeleportCancel;
    }

    // Update is called once per frame
    void Update()
    {
        if (teleportIsActive)
        {
            return;
        }

        if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            rayInteractor.enabled = false;
            teleportIsActive = false;
            return;
        }

        TeleportRequest request = new TeleportRequest()
        {
            destinationPosition = hit.point,
        };

        provider.QueueTeleportRequest(request);

        rayInteractor.enabled = false;
        teleportIsActive = false;
    }

    private void OnTeleportActivate(InputAction.CallbackContext context)
    {
        if (!teleportIsActive)
        {
            rayInteractor.enabled = true;
            teleportIsActive = true;
        }
    }

    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        if (teleportIsActive && rayInteractor.enabled == true)
        {
            rayInteractor.enabled = false;
            teleportIsActive = false;
        }
    }
}
