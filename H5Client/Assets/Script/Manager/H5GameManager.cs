using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class H5GameManager : MonoBehaviour
{
    private static H5GameManager mInstance;
    public static H5GameManager Instance { get { if (mInstance == null) mInstance = GameObject.FindGameObjectWithTag("Game").GetComponent<H5GameManager>(); return mInstance; } }

    Coroutine Co_InitGame;
    
    void Start()
    {
        Co_InitGame = StartCoroutine(InitGame());
    }

    IEnumerator InitGame()
    {
        yield return StartCoroutine(UIManager.Instance.InitLoadingScene());

        UIManager.Instance.OpenWindow(UIWindowType.Loading);

        IEnumerator<float> uiInit = UIManager.Instance.InitUI();
        while (uiInit.MoveNext())
        {
            float percent = uiInit.Current;
            UIManager.Instance.SetLoadingPercent(percent);
            yield return null;
        }

        while(Input.GetMouseButtonDown(0) == false)
            yield return null;

        UIManager.Instance.CloseWindow(UIWindowType.Loading);
    }
    
    void Update()
    {

    }
}
