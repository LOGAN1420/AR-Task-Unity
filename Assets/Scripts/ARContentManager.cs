using Siccity.GLTFUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class ARContentManager : MonoBehaviour
{
    [Serializable]
    public class ModelData
    {
        public int id;
        public string name;
        public string url;
        public float[] position;
        public float[] scale;
    }

    [Serializable]
    public class ModelDataList
    {
        public List<ModelData> models;
    }

    public string Config_url = "https://logan1420.github.io/AR-Content/config.json";
    
    public GameObject ContenParent;

    public GameObject Loadingscreen;

    private List<GameObject> LoadedModels = new List<GameObject>();


    public void OnLoadbtnClicked()
    {
        StartCoroutine(FetchData());
        UIManager.instance.LoadingDataUI();
    }

    IEnumerator FetchData()
    {
        UnityWebRequest request = UnityWebRequest.Get(Config_url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch model data: " + request.error);
            UIManager.instance.TryAgainUI("Failed to Fetch data. Try again");
            yield break;
        }

        string jsonText = request.downloadHandler.text;
        ModelDataList dataList = JsonUtility.FromJson<ModelDataList>(jsonText);

        foreach (var modelData in dataList.models)
        {
            StartCoroutine(DownloadAndSpawnModel(modelData));
            UIManager.instance.UpdateLoadingText(modelData.name + "model is being Cooked" );
        }


    }

    IEnumerator DownloadAndSpawnModel(ModelData data)
    {
        UnityWebRequest request = UnityWebRequest.Get(data.url);
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to download model: " + request.error);
            UIManager.instance.TryAgainUI("Failed to download model");
            yield break;
        }

        byte[] glbBytes = request.downloadHandler.data;
        ImportSettings settings = new ImportSettings();
        GameObject model = Importer.LoadFromBytes(glbBytes, settings, out AnimationClip[] clips);

        model.transform.position = ToVector3(data.position);
        model.transform.localScale = ToVector3(data.scale);
        model.transform.SetParent(ContenParent.transform, false);
        model.SetActive(false);

        LoadedModels.Add(model);

        /*
        //play animation
        if (clips.Length > 0)
        {
            clips[0].legacy = true;

            Animation anim = model.AddComponent<Animation>();
            anim.AddClip(clips[0], clips[0].name);
            anim.Play(clips[0].name);
        }*/
    }

    private Vector3 ToVector3(float[] values)
    {
        return new Vector3(values[0], values[1], values[2]);
    }

    public void OnSeen()
    {
        ContenParent.gameObject.SetActive(true);
        LoadedModels[0].gameObject.SetActive(true);
    }
    public void OnUnseen()
    {
        ContenParent.gameObject.SetActive(false);
    }
}
