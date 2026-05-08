using System.Collections;
using UnityEngine;

public class DestroyMyself : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DestroyMe());
    }

    private IEnumerator DestroyMe()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }
}
