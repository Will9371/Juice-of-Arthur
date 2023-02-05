using UnityEngine;
using UnityEngine.Events;

public class ClickEvent : MonoBehaviour
{
    [SerializeField] UnityEvent onMouseDown;
    void OnMouseDown() => onMouseDown.Invoke();
}
