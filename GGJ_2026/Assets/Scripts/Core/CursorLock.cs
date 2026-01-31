using UnityEngine;

/// <summary>
/// Единственная ответственность: скрывать курсор и блокировать его в центре окна,
/// когда игра в фокусе (активное окно).
/// </summary>
public class CursorLock : MonoBehaviour
{
    [SerializeField] private bool lockOnEnable = true;
    [SerializeField] private bool unlockOnDisable = true;
    [SerializeField] private bool reLockOnApplicationFocus = true;

    private void OnEnable()
    {
        if (lockOnEnable)
            Lock();
    }

    private void OnDisable()
    {
        if (unlockOnDisable)
            Unlock();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (reLockOnApplicationFocus && hasFocus)
            Lock();
    }

    private void Lock()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Unlock()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
