using UnityEngine;
using UnityEngine.UI;

public class ProSwitchStateDrawer : MonoBehaviour
{
    OutSwitch Switch;
    public Text Text;
    public int ScoreColliders()
    {
        if (Switch.IsPro)
        {
            return Switch.Pro.GetComponents<Collider2D>().Length;
        }
        else
        {
            return Switch.NoPro.GetComponents<Collider2D>().Length;
        }
    }
    private void Awake()
    {
        Switch = FindObjectOfType<OutSwitch>();
    }
    private void Update()
    {
        if (Switch.IsPro)
        {
            Text.text = $"Pro({ScoreColliders()} colliders)";
        }
        else
        {
            Text.text = $"Default({ScoreColliders()} colliders)";
        }
    }
}