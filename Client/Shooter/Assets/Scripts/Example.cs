using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Example : MonoBehaviour
{
    [SerializeField] private string URL;
    //private void Awake() => Debug.Log("Awake");

    //Action action; //—оздание безым€нного метода


    private void Start()
    {
        //StartRun(URL);
        //Debug.Log("Start");

        //_action = StartRun;

        //_action();

        //_action2 = Test;
        //_action();
        //StartRun(URL, Success, Error);
        StartRun(URL, Success, (s) =>
        {
            Debug.Log("");
            StartRun("", null, null);
            Debug.LogError("„то-то пошло не так" + s); 
        }); //можно вместо s написать string s, но мы внизу уже указывали что метод принимает string, поэтому нет смысла указывать дважды

        //Ѕезым€нный метод:
        //action = () =>
        //{
        //    Debug.LogError("„то-то пошло не так");
        //};
    }

    //private Action _action;
    //private Action<int, int, string, float, Test> _action2;

    //private void Test(int i, int j, string s, float f, Test t) { }
    public void StartRun(string url, Action<string> callback, Action<string> error = null) => StartCoroutine(Run(url, callback, error)); //можно убрать error из корутины и ошибки не будет
    //{
    //    StartCoroutine(Run());
    //}

    private IEnumerator Run(string url, Action<string> callback, Action<string> error = null)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if(www.result != UnityWebRequest.Result.Success)
        {
            //Debug.Log(www.error);
            error?.Invoke(www.error); //error? то же самое что и if (error != null)            
            //yield break;
        }
        else
        {
            callback?.Invoke(www.downloadHandler.text);
        }

        //Debug.Log(www.downloadHandler.text);
        www.Dispose(); //уничтожить www после отработки корутины, чтобы не переполн€лась оперативна€ пам€ть

        ////чтобы не уничтожать www, можно сделать по-другому:
        //using (UnityWebRequest www = UnityWebRequest.Get(url))
        //{
        //    yield return www.SendWebRequest();

        //    if (www.result != UnityWebRequest.Result.Success) error?.Invoke(www.error);
        //    else callback?.Invoke(www.downloadHandler.text);
        //}
    }

    private void Error(string e) => Debug.LogError(e);
    private void Success(string result) => Debug.Log(result);
    private void Test(string s)
    {
        Debug.LogError("„то-то пошло не так" + s);
    }
    
}
