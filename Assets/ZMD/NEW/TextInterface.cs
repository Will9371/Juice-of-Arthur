using UnityEngine;
using UnityEngine.UI;

public class TextInterface : MonoBehaviour
{
    [SerializeField] Text text;
    public void Input(float value) { text.text = value.ToString("F2"); }
}
