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

    public void SwitchCamera()
    {
        PhotographerMode = !PhotographerMode;
        PhysicsBasedMovement.enabled = !PhotographerMode;
        //InputSystemMovementInput.enabled = !PhotographerMode;
        MainCamera.gameObject.SetActive(!PhotographerMode);
        PhotoCamera.gameObject.SetActive(PhotographerMode); 
    }
}
