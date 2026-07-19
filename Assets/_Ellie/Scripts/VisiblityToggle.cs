using CarGame;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class VisiblityToggle : MonoBehaviour
{
    bool visible;
    CanvasGroup layoutGroup;

    private void Start()
    {
        layoutGroup = GetComponent<CanvasGroup>();
        layoutGroup.alpha = 0f;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.P)) 
        {
            if (visible)
            {
                layoutGroup.alpha = 0;
                visible = false;
            }
            else 
            {
                layoutGroup.alpha = 1;
                visible = true;
            }
        }*/
    }

    [SerializeField] private TextMeshProUGUI performanceLabel;
    public void OnPerformanceToggle(bool enabled)
    {
        performanceLabel.text = "TEST " + (enabled ? "ON" : "OFF");

        ChunkCullingManager.Instance.SetEnabled(enabled);
    }
}
