using System.Collections;
using UnityEngine;

public class CurtainCall : MonoBehaviour
{
    [SerializeField] ActorEnterExit[] actors;
    [SerializeField] float bowDuration = 1f;
    [SerializeField] GameObject quitButton;
    
    void Start() => StartCoroutine(Process());

    IEnumerator Process()
    {
        yield return null;
        var bowDelay = new WaitForSeconds(bowDuration);
        
        foreach (var actor in actors)
        {
            yield return actor.MoveIn();
            yield return bowDelay;
        }
        
        quitButton.SetActive(true);
    }
}
