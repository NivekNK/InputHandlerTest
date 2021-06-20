using NK.FSM;
using UnityEngine.InputSystem;
using UnityEngine;

namespace NK.Player
{
    // Enum used for a easy to know input state
    public enum Inputs
    {
        Tap,
        Press,
        Hold
    }

    // Simple MonoBehaviour Class
    // Inherit from a abstract Class for other uses
    public class InputHandler : InputManager
    {
#if UNITY_EDITOR
        [SerializeField] private bool debug;
#endif

        // Inputs
        public Vector2 Movement { get; private set; }

        // A array bool is used to save the different states that a input can have
        public bool[] XInput { get; } = new bool[3];
        public bool[] AInput { get; } = new bool[3];

        // Initialization of the controls, Class created by the Engine
        private PlayerControls playerControls;

        private void Awake()
        {
            playerControls = new PlayerControls();
            Init();
        }

        // Initialization of the Input Actions, subscribing the methods
        // that define the inputs variables
        private void Init()
        {
            playerControls.Gameplay.Movement.performed += SetMovement;

            playerControls.Gameplay.XInputHold.performed += ctx => HoldInput(ctx, XInput);
            playerControls.Gameplay.XInput.performed += ctx => TapInput(ctx, XInput);
            playerControls.Gameplay.XInput.canceled += ctx => PressInput(ctx, XInput);

            playerControls.Gameplay.AInputHold.performed += ctx => HoldInput(ctx, AInput);
            playerControls.Gameplay.AInput.performed += ctx => TapInput(ctx, AInput);
            playerControls.Gameplay.AInput.canceled += ctx => PressInput(ctx, AInput);
        }

        // Used to reset the inputs that are not in holding state
        private void LateUpdate()
        {
            ResetInputs();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            playerControls.Enable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            playerControls.Disable();
        }

        // Simple usage of Input System, using the default Input Action Interactions
        private void SetMovement(InputAction.CallbackContext ctx)
        {
            Movement = ctx.ReadValue<Vector2>();
            Movement.Normalize();
#if UNITY_EDITOR
            if (debug) GlobalDebug.Log(Movement);
#endif
        }

        #region Input Control

        // Use the Tap Interaction
        private void TapInput(InputAction.CallbackContext ctx, bool[] inputs)
        {
            if (ctx.performed)
            {
                inputs[(int)Inputs.Press] = false;
                inputs[(int)Inputs.Tap] = true;
#if UNITY_EDITOR
                if (debug) GlobalDebug.Log("Tap");
#endif
            }
        }

        // Use the Tap Interaction
        private void PressInput(InputAction.CallbackContext ctx, bool[] inputs)
        {
            if (ctx.canceled)
            {
                inputs[(int)Inputs.Tap] = false;
                inputs[(int)Inputs.Press] = true;
                inputs[(int)Inputs.Hold] = true;
#if UNITY_EDITOR
                if (debug) GlobalDebug.Log("Pressed");
#endif
            }
        }

        // Use the Press Interaction in ReleaseOnly
        private void HoldInput(InputAction.CallbackContext ctx, bool[] inputs)
        {
            // Reset the hold input, previously pressed in the PressInput Method
            if (ctx.performed)
            {
                inputs[(int)Inputs.Hold] = false;
#if UNITY_EDITOR
                if (debug) GlobalDebug.Log("Released");
#endif
            }
        }

        // Reset the inputs that are not in holding state
        private void ResetInputs()
        {
            XInput[(int)Inputs.Press] = false;
            XInput[(int)Inputs.Tap] = false;

            AInput[(int)Inputs.Press] = false;
            AInput[(int)Inputs.Tap] = false;
        }

        #endregion
    }
}