using UnityEngine;
using UnityEngine.InputSystem;

namespace SoloRPG.Prototype
{
    [DefaultExecutionOrder(-100)]
    public sealed class PrototypeFollowCamera : MonoBehaviour
    {
        private const float PivotHeight = 0.65f;
        private const float ShoulderOffset = 0.55f;
        private const float MouseDegreesPerPixel = 0.12f;
        private const float StickDegreesPerSecond = 120f;

        [SerializeField] private Transform target;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField, Min(0.01f)] private float distance = 3f;

        private InputAction lookAction;
        private float yaw;
        private float pitch = 12f;
        private bool ownsCursorLock;

        public Quaternion HorizontalRotation => Quaternion.Euler(0f, yaw, 0f);

        private void Awake()
        {
            lookAction = inputActions.FindAction("Player/Look", throwIfNotFound: true).Clone();
        }

        private void OnEnable() => lookAction.Enable();

        private void OnDisable()
        {
            lookAction.Disable();
            ReleaseCursor();
        }

        private void OnDestroy() => lookAction.Dispose();

        private void Start()
        {
            yaw = target.eulerAngles.y;
            LateUpdate();
        }

        private void Update()
        {
            bool hasGameFocus = HasGameFocus();
            if (!hasGameFocus || Keyboard.current?.escapeKey.wasPressedThisFrame == true)
                ReleaseCursor();
            else if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                Cursor.lockState = CursorLockMode.Locked;
                ownsCursorLock = true;
            }

            Vector2 look = lookAction.ReadValue<Vector2>();
            switch (lookAction.activeControl?.device)
            {
                case Gamepad:
                    look *= StickDegreesPerSecond * Time.deltaTime;
                    break;
                case Mouse when hasGameFocus && Cursor.lockState == CursorLockMode.Locked:
                    look *= MouseDegreesPerPixel;
                    break;
                default:
                    return;
            }

            yaw = Mathf.Repeat(yaw + look.x, 360f);
            pitch = Mathf.Clamp(pitch - look.y, -10f, 25f);
        }

        private static bool HasGameFocus()
        {
#if UNITY_EDITOR
            return Application.isFocused &&
                UnityEditor.EditorWindow.focusedWindow?.GetType().FullName == "UnityEditor.GameView";
#else
            return Application.isFocused;
#endif
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
                ReleaseCursor();
        }

        private void ReleaseCursor()
        {
            if (!ownsCursorLock)
                return;

            Cursor.lockState = CursorLockMode.None;
            ownsCursorLock = false;
        }

        private void LateUpdate()
        {
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 pivot = target.position + Vector3.up * PivotHeight;
            Vector3 offset = rotation * new Vector3(ShoulderOffset, 0f, -distance);
            transform.SetPositionAndRotation(pivot + offset, rotation);
        }
    }
}
