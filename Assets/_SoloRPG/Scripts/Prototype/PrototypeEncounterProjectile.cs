using UnityEngine;

namespace SoloRPG.Prototype
{
    public sealed class PrototypeEncounterProjectile : MonoBehaviour
    {
        private const float Speed = 8f;
        private const float Radius = 0.13f;
        private const float Lifetime = 5f;
        private Vector3 direction;
        private float remaining = Lifetime;

        public static PrototypeEncounterProjectile Launch(Vector3 origin, Vector3 target)
        {
            GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.name = "Enemy Projectile";
            ball.GetComponent<Collider>().enabled = false;
            Destroy(ball.GetComponent<Collider>());
            ball.transform.position = origin;
            ball.transform.localScale = Vector3.one * (Radius * 2f);
            var color = new MaterialPropertyBlock();
            color.SetColor("_BaseColor", new Color(1f, 0.62f, 0.04f));
            ball.GetComponent<Renderer>().SetPropertyBlock(color);
            var projectile = ball.AddComponent<PrototypeEncounterProjectile>();
            projectile.direction = (target - origin).normalized;
            return projectile;
        }

        public bool Advance(float elapsed, Collider player, Collider floor, out bool hitPlayer)
        {
            Vector3 origin = transform.position;
            float distance = Speed * Mathf.Min(remaining, elapsed);
            hitPlayer = false;
            foreach (Collider overlap in Physics.OverlapSphere(origin, Radius, ~0, QueryTriggerInteraction.Ignore))
            {
                if (overlap == player || overlap == floor)
                {
                    hitPlayer = overlap == player;
                    return false;
                }
            }

            Collider first = null;
            float nearest = float.PositiveInfinity;
            foreach (RaycastHit hit in Physics.SphereCastAll(origin, Radius, direction, distance, ~0,
                QueryTriggerInteraction.Ignore))
            {
                if ((hit.collider == player || hit.collider == floor) && hit.distance < nearest)
                {
                    nearest = hit.distance;
                    first = hit.collider;
                }
            }

            if (first != null)
            {
                hitPlayer = first == player;
                return false;
            }

            transform.position = origin + direction * distance;
            remaining -= elapsed;
            return remaining > 0f;
        }
    }
}
