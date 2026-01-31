using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public InputSystemMovementInput InputSystemMovementInput;
    public PhysicsBasedMovement PhysicsBasedMovement;
    public MouseLook MouseLook;

    public Camera MainCamera;
    public Camera PhotoCamera;
    [FormerlySerializedAs("PixelVisibilityChecker")]
    public PhotoMaker photoMaker;

    public bool PhotographerMode = false;

    private void Start()
    {
        InputSystemMovementInput.OnPhotoMaked += MakePhoto;
        InputSystemMovementInput.OnCameraSwitched += SwitchCamera;
    }

    public void MakePhoto()
    {
        if (PhotographerMode)
        {
            photoMaker.MakePhoto();
        }
    }

    public void SetGameplayInputEnabled(bool enabled)
    {
        if (InputSystemMovementInput != null) InputSystemMovementInput.enabled = enabled;
        if (MouseLook != null) MouseLook.enabled = enabled;
        if (PhysicsBasedMovement != null) PhysicsBasedMovement.enabled = enabled && !PhotographerMode;
    }

    public void SetPhotographerMode(bool on)
    {
        if (PhotographerMode == on) return;
        PhotographerMode = on;
        if (PhysicsBasedMovement != null) PhysicsBasedMovement.enabled = !PhotographerMode;
        if (MainCamera != null) MainCamera.gameObject.SetActive(!PhotographerMode);
        if (PhotoCamera != null) PhotoCamera.gameObject.SetActive(PhotographerMode);
    }

    public void SwitchCamera()
    {
        if (PhotographerMode)
            Game.Instance.ScreenController.PopScreen();
        else
            Game.Instance.ScreenController.PushScreen<CameraScreen>();
    }


}
