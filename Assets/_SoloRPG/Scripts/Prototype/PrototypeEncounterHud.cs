using UnityEngine;
using UnityEngine.UI;

namespace SoloRPG.Prototype
{
    [RequireComponent(typeof(Canvas))]
    public sealed class PrototypeEncounterHud : MonoBehaviour
    {
        private Font font;
        private Text health;
        private Text prompt;
        private RectTransform crosshair;
        private RectTransform shotRing;

        private void Awake()
        {
            font = Font.CreateDynamicFontFromOSFont(new[] { "Malgun Gothic", "Arial" }, 24);
            health = CreateText("Health", new Vector2(0.5f, 1), new Vector2(0, -30), 22);
            prompt = CreateText("Prompt", new Vector2(0.5f, 0), new Vector2(0, 54), 26);
            crosshair = CreateMark("Crosshair", transform, Vector2.zero, new Vector2(4, 4)).rectTransform;
            shotRing = new GameObject("Shot Indicator", typeof(RectTransform)).GetComponent<RectTransform>();
            shotRing.SetParent(crosshair, false);
            CreateMark("Top", shotRing, new Vector2(0, 11), new Vector2(2, 6));
            CreateMark("Bottom", shotRing, new Vector2(0, -11), new Vector2(2, 6));
            CreateMark("Left", shotRing, new Vector2(-11, 0), new Vector2(6, 2));
            CreateMark("Right", shotRing, new Vector2(11, 0), new Vector2(6, 2));
            shotRing.gameObject.SetActive(false);
        }

        public void Show(int playerHealth, int opponentHealth, string message, bool aiming, bool firing)
        {
            health.text = $"플레이어  {playerHealth} / 3       적  {opponentHealth} / 3";
            prompt.text = message;
            crosshair.gameObject.SetActive(aiming);
            shotRing.gameObject.SetActive(firing);
        }

        private Text CreateText(string label, Vector2 anchor, Vector2 position, int size)
        {
            var text = new GameObject(label, typeof(RectTransform), typeof(Text), typeof(Outline))
                .GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.alignment = TextAnchor.MiddleCenter;
            text.raycastTarget = false;
            RectTransform rect = text.rectTransform;
            rect.SetParent(transform, false);
            rect.anchorMin = rect.anchorMax = anchor;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(480, 44);
            return text;
        }

        private static Image CreateMark(string label, Transform parent, Vector2 position, Vector2 size)
        {
            var mark = new GameObject(label, typeof(RectTransform), typeof(Image), typeof(Outline))
                .GetComponent<Image>();
            mark.raycastTarget = false;
            mark.rectTransform.SetParent(parent, false);
            mark.rectTransform.anchoredPosition = position;
            mark.rectTransform.sizeDelta = size;
            return mark;
        }

        private void OnDestroy() => Destroy(font);
    }
}
