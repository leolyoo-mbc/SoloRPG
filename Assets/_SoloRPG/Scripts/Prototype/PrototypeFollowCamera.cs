using UnityEngine;

namespace SoloRPG.Prototype
{
    public sealed class PrototypeFollowCamera : MonoBehaviour
    {
        private static readonly Vector3 Offset = new Vector3(0f, 5f, -7f);

        [SerializeField] private Transform target;

        private void Start()
        {
            transform.SetPositionAndRotation(
                target.position + Offset,
                Quaternion.LookRotation(-Offset, Vector3.up));
        }

        private void LateUpdate()
        {
            transform.position = target.position + Offset;
        }
    }
}
