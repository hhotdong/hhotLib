using UnityEngine;

namespace hhotLib
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform panel;
        private Rect lastSafeArea = new Rect(0, 0, 0, 0);

        private void Awake()
        {
            panel = GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            Rect safeArea = UnityEngine.Screen.safeArea;
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

            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}