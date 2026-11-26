using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SoloRPG.Prototype
{
    [DefaultExecutionOrder(-50)]
    public sealed class PrototypeRangedEncounter : MonoBehaviour
    {
        private enum RoundState { Waiting, Playing, Won, Lost }

        private const int StartingHealth = 3;
        private const float FireInterval = 2f;
        private const float WarningDuration = 0.5f;
        private const float FlashDuration = 0.15f;
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
        private static readonly Color HitColor = new Color(1f, 0.22f, 0.13f);

        [SerializeField] private PrototypeFollowCamera followCamera;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private PrototypeCharacterMovement movement;
        [SerializeField] private CharacterController player;
        [SerializeField] private Renderer playerBody;
        [SerializeField] private Collider floor;
        [SerializeField] private Collider opponent;
        [SerializeField] private Renderer opponentBody;
        [SerializeField] private Transform muzzle;
        [SerializeField] private Renderer warning;
        [SerializeField] private PrototypeEncounterHud hud;

        private readonly List<PrototypeEncounterProjectile> projectiles = new();
        private MaterialPropertyBlock feedback;
        private Vector3 startPosition;
        private Quaternion startRotation;
        private RoundState state;
        private int playerHealth;
        private int opponentHealth;
        private float clock;
        private float nextShot;
        private float playerFlashUntil;
        private float opponentFlashUntil;
        private float shotFeedbackUntil;
        private float resultFlashClock;

        private bool IsFinished => state == RoundState.Won || state == RoundState.Lost;

        private void Awake() => feedback = new MaterialPropertyBlock();

        private void Start()
        {
            startPosition = player.transform.position;
            startRotation = player.transform.rotation;
            feedback.SetColor(BaseColor, new Color(1f, 0.75f, 0.05f));
            warning.SetPropertyBlock(feedback);
            ResetRound();
        }

        private void Update()
        {
            if (IsFinished && Keyboard.current?.rKey.wasPressedThisFrame == true)
                ResetRound();

            if (state == RoundState.Waiting && followCamera.CapturedMouseThisFrame)
                state = RoundState.Playing;

            movement.enabled = state == RoundState.Playing && followCamera.CanUseMouse;
        }

        private void LateUpdate()
        {
            if (state == RoundState.Playing && followCamera.CanUseMouse && !followCamera.CapturedMouseThisFrame)
                AdvanceRound(Time.deltaTime);
            else if (IsFinished)
                resultFlashClock += Time.deltaTime;

            RefreshDisplay();
        }

        private void AdvanceRound(float elapsed)
        {
            clock += elapsed;
            Physics.SyncTransforms();
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                FirePlayerShot();
                if (IsFinished)
                    return;
            }

            for (int i = 0; i < projectiles.Count;)
            {
                if (projectiles[i].Advance(elapsed, player, floor, out bool hitPlayer))
                {
                    i++;
                    continue;
                }

                RetireProjectile(i);
                if (!hitPlayer)
                    continue;

                playerFlashUntil = clock + FlashDuration;
                if (--playerHealth == 0)
                {
                    Finish(RoundState.Lost);
                    return;
                }
            }

            while (clock >= nextShot)
            {
                var projectile = PrototypeEncounterProjectile.Launch(muzzle.position, player.bounds.center);
                projectiles.Add(projectile);
                nextShot += FireInterval;
            }
        }

        private void FirePlayerShot()
        {
            shotFeedbackUntil = clock + 0.1f;
            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Collider first = null;
            float nearest = float.PositiveInfinity;
            foreach (RaycastHit hit in Physics.RaycastAll(ray, Mathf.Infinity, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider == player || hit.collider.transform.IsChildOf(player.transform) ||
                    hit.distance >= nearest)
                    continue;

                first = hit.collider;
                nearest = hit.distance;
            }

            if (first != opponent)
                return;

            opponentFlashUntil = clock + FlashDuration;
            if (--opponentHealth == 0)
                Finish(RoundState.Won);
        }

        private void Finish(RoundState result)
        {
            state = result;
            resultFlashClock = clock;
            movement.enabled = false;
            followCamera.enabled = false;
            ClearProjectiles();
        }

        private void ResetRound()
        {
            ClearProjectiles();
            state = RoundState.Waiting;
            playerHealth = opponentHealth = StartingHealth;
            clock = playerFlashUntil = opponentFlashUntil = shotFeedbackUntil = 0f;
            nextShot = FireInterval;
            player.enabled = false;
            player.transform.SetPositionAndRotation(startPosition, startRotation);
            player.enabled = true;
            followCamera.enabled = true;
            followCamera.ResetView();
            movement.enabled = false;
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            warning.enabled = state == RoundState.Playing && clock >= nextShot - WarningDuration;
            float flashClock = IsFinished ? resultFlashClock : clock;
            SetHitFlash(playerBody, flashClock < playerFlashUntil);
            SetHitFlash(opponentBody, flashClock < opponentFlashUntil);

            string message = state switch
            {
                RoundState.Waiting => "화면을 클릭해 시작",
                RoundState.Won => "승리   ·   R 키로 다시 시작",
                RoundState.Lost => "패배   ·   R 키로 다시 시작",
                _ => followCamera.CanUseMouse ? "" : "일시정지   ·   화면을 클릭해 계속"
            };
            bool aiming = movement.enabled && state == RoundState.Playing;
            hud.Show(playerHealth, opponentHealth, message, aiming, aiming && clock < shotFeedbackUntil);
        }

        private void SetHitFlash(Renderer body, bool flashing)
        {
            feedback.Clear();
            if (flashing)
                feedback.SetColor(BaseColor, HitColor);
            body.SetPropertyBlock(feedback);
        }

        private void ClearProjectiles()
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
                RetireProjectile(i);
        }

        private void RetireProjectile(int index)
        {
            GameObject ball = projectiles[index].gameObject;
            ball.SetActive(false);
            Destroy(ball);
            projectiles.RemoveAt(index);
        }
    }
}
