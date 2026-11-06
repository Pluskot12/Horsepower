using UnityEngine;

namespace CarGame
{
    public class RepairTool : MonoBehaviour
    {
        public void OnUse(Tool tool)
        {
            Debug.Log("repair");
        }
    }
}
