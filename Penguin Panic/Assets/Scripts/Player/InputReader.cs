using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

[CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event UnityAction<Vector2> Move = delegate { };
    public event UnityAction Pause = delegate { };

    private InputSystem_Actions input;

    //Connects to the input system by setting callback to go to this script
    private void OnEnable()
    {
        if (input == null)
        {
            input = new InputSystem_Actions();
            input.Player.SetCallbacks(this);
        }

        input.Player.Enable();
    }

    //Enable and disable inputs
    public void EnablePlayerActions()
    {
        input.Player.Enable();
        input.UI.Enable();
    }
    public void DisablePlayerActions()
    {
        input.Player.Disable();
        input.UI.Disable();
    }

    private void OnDisable()
    {
        if (input != null)
        {
            input.Player.Disable();
            input.UI.Disable();
        }
    }
    private void OnDestroy()
    {
        if (input != null)
        {
            input.Player.Disable();
            input.UI.Disable();
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        Move?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        Pause?.Invoke();
    }
}