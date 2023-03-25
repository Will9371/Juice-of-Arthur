using System.Collections;
using UnityEngine;

public class ActorEnterExit : MonoBehaviour
{
    [SerializeField] float edge = 720;
    [SerializeField] float moveTime = 1f;
    [SerializeField] bool startActive;
    
    float onStageX;
    float offStageX;
    
    bool active;
    
    //RectTransform rect;
    //void Awake() => rect = GetComponent<RectTransform>();
    void Start() 
    {
        onStageX = transform.position.x; //rect.anchoredPosition.x;
        offStageX = onStageX > 0 ? edge : -edge;
        var startX = startActive ? onStageX : offStageX;
        //rect.anchoredPosition = new Vector2(startX, rect.anchoredPosition.y);
        transform.position = new Vector3(startX, 0, 0);
        active = startActive;
    }

    public void SetActive(bool value)
    {
        if (value == active) return;
        active = value;
        
        if (value) StartCoroutine(Move(offStageX, onStageX));
        else StartCoroutine(Move(onStageX, offStageX));
    }
    
    public IEnumerator MoveIn() { yield return Move(offStageX, onStageX); }

    public IEnumerator Move(float startPosition, float endPosition)
    {
        float startTime = Time.time;
        float percent = 0f;
        float x;

        while (percent < 1f)
        {
            percent = (Time.time - startTime)/moveTime;
            x = Mathf.Lerp(startPosition, endPosition, percent);
            //rect.anchoredPosition = new Vector2(x, rect.anchoredPosition.y);
            transform.position = new Vector3(x, 0, 0);
            yield return null;
        }
    }
}
