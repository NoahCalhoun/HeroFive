﻿using System.Collections;
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
            if (type == UIWindowType.None)
                continue;

            var name = type.ToString();

            var load = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
            while (load.isDone == false) { yield return null; }

            var uiRoot = GameObject.FindGameObjectWithTag("UICamera");
            var ui = uiRoot.GetComponentInChildren<H5WindowBase>();
            ui.TM.SetParent(UICameraRoot);
            //ui.GO.SetActive(false);
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
}