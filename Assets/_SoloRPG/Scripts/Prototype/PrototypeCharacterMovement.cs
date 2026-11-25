using UnityEngine;
using UnityEngine.InputSystem;

namespace SoloRPG.Prototype
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypeCharacterMovement : MonoBehaviour
    {
        private const float MoveSpeed = 4f;

        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private BoxCollider floor;

        private InputAction moveAction;

        private void Awake()
        {
            moveAction = inputActions.FindAction("Player/Move", throwIfNotFound: true).Clone();
        }

        private void OnEnable() => moveAction.Enable();

        private void OnDisable() => moveAction.Disable();

        private void OnDestroy() => moveAction.Dispose();

        private void Update()
        {
            Vector2 input = Vector2.ClampMagnitude(moveAction.ReadValue<Vector2>(), 1f);
            if (input == Vector2.zero)
                return;

            Vector3 position = transform.position;
            Vector3 requested = position + new Vector3(input.x, 0f, input.y) * (MoveSpeed * Time.deltaTime);
            Bounds floorBounds = floor.bounds;
            Bounds characterBounds = characterController.bounds;

            requested.x = Mathf.Clamp(
                requested.x,
                position.x + floorBounds.min.x - characterBounds.min.x,
                position.x + floorBounds.max.x - characterBounds.max.x);
            requested.z = Mathf.Clamp(
                requested.z,
                position.z + floorBounds.min.z - characterBounds.min.z,
                position.z + floorBounds.max.z - characterBounds.max.z);

            characterController.Move(requested - position);
        }
    }
}
