using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast;
using System.Threading.Tasks;

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

    private ModelDataList DataList;

    public List<GameObject> LoadedModels = new List<GameObject>();

    private int CurrentModelIndex = 0;
    private bool firstTimeLoad;
    private bool isAllDataLoaded;


    public async void OnLoadbtnClicked()
    {
        await FetchData();        
    }

    async Task FetchData()
    {
        UIManager.instance.LoadingDataUI();
        UnityWebRequest request = UnityWebRequest.Get(Config_url);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch model data: " + request.error);
            UIManager.instance.TryAgainUI("Failed to Fetch data. Try again");
            return;
        }

        string jsonText = request.downloadHandler.text;
        DataList = JsonUtility.FromJson<ModelDataList>(jsonText);

        foreach (var modelData in DataList.models)
        {
            await DownloadAndSpawnModel(modelData);
            UIManager.instance.UpdateLoadingText(modelData.name + "model is being Cooked" );
        }

        UIManager.instance.LoadingDone();
    }

    async Task DownloadAndSpawnModel(ModelData data)
    {
        GltfImport gltf = new GltfImport();

        bool success = await gltf.Load(new System.Uri(data.url));
        if (!success)
        {
            Debug.LogError($"Failed to load model: {data.name}");
            UIManager.instance.TryAgainUI("Failed to load models. Try again");
            return;
        }

        GameObject model = new GameObject(data.name);
        await gltf.InstantiateMainSceneAsync(model.transform);

        model.transform.position = ToVector3(data.position);
        model.transform.localScale = ToVector3(data.scale);
        model.transform.SetParent(ContenParent.transform, false);
        model.SetActive(false);

        LoadedModels.Add(model);

        
        if (LoadedModels.Count == DataList.models.Count)
        {
            isAllDataLoaded = true;
        }
    }

    private Vector3 ToVector3(float[] values)
    {
        return new Vector3(values[0], values[1], values[2]);
    }

    public void OnSeen()
    {
        ContenParent.gameObject.SetActive(true);
        
        if (isAllDataLoaded)
        {
            if (!firstTimeLoad)
            {
                LoadedModels[0].gameObject.SetActive(true);
                UpdateModelData();
                firstTimeLoad = true;
            }
        }
        else
        {
            Debug.Log("Models Not Loaded yet");
        }
    }
    public void OnUnseen()
    {
        ContenParent.gameObject.SetActive(false);
    }

    public void NextModel()
    {
        CurrentModelIndex++;
        if (CurrentModelIndex >= LoadedModels.Count)
        {
            CurrentModelIndex = 0;
        }

        for (int i = 0; i < LoadedModels.Count; i++)
        {
            if (i == CurrentModelIndex)
            {
                LoadedModels[i].gameObject.SetActive(true);
            }
            else
            {
                LoadedModels[i].gameObject.SetActive(false);
            }
        }
        UpdateModelData();
    }

    public void PreviousModel()
    {
        CurrentModelIndex--;
        if (CurrentModelIndex < 0)
        {
            CurrentModelIndex = LoadedModels.Count - 1;
        }

        for (int i = 0; i < LoadedModels.Count; i++)
        {
            if (i == CurrentModelIndex)
            {
                LoadedModels[i].gameObject.SetActive(true);
            }
            else
            {
                LoadedModels[i].gameObject.SetActive(false);
            }
        }
        UpdateModelData();
    }

    private void UpdateModelData()
    {
        ModelData data = DataList.models[CurrentModelIndex];
        UIManager.instance.HudUIUpdate(data.name);
    }
}
