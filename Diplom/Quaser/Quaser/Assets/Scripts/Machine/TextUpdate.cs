using Assets.Scripts.Machine;
using UnityEngine;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour
{
    private static Text[] texts;

    private void Start()
    {
        texts = gameObject.GetComponentsInChildren<Text>();
    }

    public static void Change(Tool tool)
    {
        texts[2].text = $"�����: {tool.Length}";
        texts[1].text = $"������: {tool.Radius}";
        texts[0].text = $"���: {tool.Name}";
    }
}
