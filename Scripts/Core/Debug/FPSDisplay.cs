using System.Collections;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private float count;

    private IEnumerator Start()
    {
        GUI.depth = 2;
        while (true)
        {
            count = 1f / Time.unscaledDeltaTime;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = h * 2 / 50,
            normal = { textColor = Color.white }
        };

        Rect rect = new Rect(50, 50, w, h * 2 / 100);
        GUI.Label(rect, string.Format("{0:0.} FPS", Mathf.Round(count)), style);
    }
}
