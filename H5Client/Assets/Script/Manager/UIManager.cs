using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private static UIManager mInstance;
    public static UIManager Instance { get { if (mInstance == null) mInstance = GameObject.FindGameObjectWithTag("UI").GetComponent<UIManager>(); return mInstance; } }

    public Transform UICameraRoot;

    private Dictionary<UIWindowType, H5WindowBase> mWindowDic = new Dictionary<UIWindowType, H5WindowBase>(EnumComparer<UIWindowType>.Instance);

    IEnumerator InitUI()
    {
        foreach (UIWindowType type in System.Enum.GetValues(typeof(UIWindowType)))
        {
            if (type == UIWindowType.None || mWindowDic.ContainsKey(type))
                continue;

            var name = type.ToString();

            var load = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            while (load.isDone == false) { yield return null; }

            var uiRoot = GameObject.FindGameObjectWithTag("UICamera");
            var ui = uiRoot.GetComponentInChildren<H5WindowBase>();
            ui.TM.SetParent(UICameraRoot);
            ui.GO.SetActive(false);
            mWindowDic.Add(type, ui);

            load = SceneManager.UnloadSceneAsync(name);
            while (load.isDone == false) { yield return null; }
        }

        yield return null;
    }

    // Use this for initialization
    void Start()
    {
        StartCoroutine(InitUI());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool OpenWindow(UIWindowType type, H5WindowBase.H5WindowDataBase data = null)
    {
        if (mWindowDic.ContainsKey(type) == false || mWindowDic[type].GO.activeInHierarchy == true)
            return false;

        mWindowDic[type].GO.SetActive(true);
        if (data != null)
            mWindowDic[type].SetWindowData(data);
        mWindowDic[type].OnOpenWindow();
        return true;
    }

    public bool CloseWindow(UIWindowType type)
    {
        if (mWindowDic.ContainsKey(type) == false || mWindowDic[type].GO.activeInHierarchy == false)
            return false;

        mWindowDic[type].GO.SetActive(false);
        mWindowDic[type].OnCloseWindow();
        return true;
    }

    public bool IsWindowOpened(UIWindowType type)
    {
        if (mWindowDic.ContainsKey(type) == false)
            return false;

        return mWindowDic[type].GO.activeInHierarchy == true;
    }
}
