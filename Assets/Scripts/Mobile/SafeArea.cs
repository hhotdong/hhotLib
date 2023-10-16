using UnityEngine;

namespace hhotLib.Common
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform panel;
        private Rect          lastSafeArea = new Rect(0, 0, 0, 0);

        public void Refresh()
        {
            Rect safeArea = Screen.safeArea;
            if (safeArea != lastSafeArea)
            {
                lastSafeArea = safeArea;
                Apply(safeArea);
            }
        }

        private void Apply(Rect r)
        {
            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            panel.anchorMin = anchorMin;
            panel.anchorMax = anchorMax;
            Debug.Log($"New safe area applied to {name}: x={r.x}, y={r.y}, w={r.width}, h={r.height} " +
                $"on full extents w={Screen.width}, h={Screen.height}");
        }

        private void Awake()
        {
            panel = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Refresh();
        }
    }
}