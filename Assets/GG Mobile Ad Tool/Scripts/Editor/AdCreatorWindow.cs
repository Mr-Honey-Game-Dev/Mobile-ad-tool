using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;


public enum AdWindowMode
{
    Create,Load
}
public class AdCreatorWindow : EditorWindow
{
    // Common variables
    #region Variables
    AdWindowMode mode =AdWindowMode.Load;
    Color originalGUIColor;
    int tab = 0;
    string logoImgPath;
    string prefabPath;
    string dataSavePath;
    #endregion


    // Variables for edit tab
    #region Edit tab variables
    public AdData adData;
    string fileName = "Ad name";
    public TextAsset adTextFile;
    public Texture2D adImageTex;
    public string headLine;
    public int rating = -1;
    public string desc;
    public int price = -1;
    public Color themeColor;
    bool create = false;
    bool loaded = false;
    #endregion


    // Variables for Instantiate tab
    #region Instantiate Tab variables
    public TextAsset adTextFileInstantiate;
    public AdData adDataInstantiate;
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 scale=Vector3.one;
    AdDisplay adDisplayPrefab;
    #endregion


    // Initialization for window and getting paths according to the script location
    #region WindowInit
    [MenuItem("Window/Mobile Ad Editor")]  
    public static void ShowWindow()
    {
        GetWindow<AdCreatorWindow>("Mobile ad utility");// (typeof(AdCreatorWindow));
    }

    private void OnEnable()
    {
        logoImgPath= Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(RootPath)))+"/Images/"+"SdkLogo.png";
        prefabPath= Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(RootPath)))+"/Prefab/"+"AdPrefab.prefab";
        dataSavePath = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(RootPath))) + "/Generated Ad Data/";
        adDisplayPrefab = (AdDisplay)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(AdDisplay));
    }

    public static string RootPath
    {
        get
        {
            var g = AssetDatabase.FindAssets($"t:Script {nameof(AdCreatorWindow)}");
            return AssetDatabase.GUIDToAssetPath(g[0]);
        }
    }

    #endregion


    void SdkLogo()
    {
        Texture banner = (Texture)AssetDatabase.LoadAssetAtPath(logoImgPath, typeof(Texture));
        GUIStyle gUIStyle = new GUIStyle();
        gUIStyle.fixedHeight = 120;
        gUIStyle.alignment = TextAnchor.UpperCenter;
        GUILayout.Box(banner, gUIStyle);
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    private void OnGUI()
    {
        originalGUIColor = GUI.color;
        SdkLogo();
        tab = GUILayout.Toolbar(tab, new string[] { "Edit Ad Data", "Instantiate Ad" });
        switch (tab) 
        {
            case 0: //ceeate Ad
                ShowEditAdPanel();
                break;
            case 1:
                ShowInstantiateOptions();
                break;       
        }
  
    }


    /// <summary>
    /// Functions to show and handle Instantiate tab
    /// </summary>
    #region Instantiate tab
    void ShowInstantiateOptions() 
    {
        EditorGUILayout.Space();
        FileLoadOptionsInstantiate();
        EditorGUILayout.Space();

        pos = EditorGUILayout.Vector3Field("Ad Position (Local to parent)", pos);
        rot= EditorGUILayout.Vector3Field("Ad Rotation (Local Eulers to parent)", rot);
        scale = EditorGUILayout.Vector3Field("Ad scale (Local to parent)", scale);
        EditorGUILayout.Space();

        if (adTextFileInstantiate!=null && GUILayout.Button("Instantiate"))
        {
           InstantiatePrefab();
        }

        EditorGUILayout.LabelField("Tip: Selected gameobject in the scene will be the parent the instantiated ad");
    }

    void FileLoadOptionsInstantiate()
    {
        EditorGUILayout.LabelField("Select a Json file with Ad Data");
        adTextFileInstantiate = EditorGUILayout.ObjectField("Ad Data file", adTextFileInstantiate, typeof(TextAsset), false) as TextAsset;        
    }

    void InstantiatePrefab() 
    {
        try
        {
            adDataInstantiate = JsonUtility.FromJson<AdData>(adTextFileInstantiate.text);
        }
        catch 
        {
            Debug.LogError("GG MOBILE SDK ERROR : Cannot Convert Json to Object. Invalid Json");
            return;
        }
        Transform parent = Selection.activeTransform;
        AdDisplay adDisplay = Instantiate(adDisplayPrefab);
        adDisplay.transform.parent=parent;
        adDisplay.transform.localPosition = pos;
        adDisplay.transform.localRotation = Quaternion.Euler(rot);
        adDisplay.transform.localScale = scale;
        adDisplay.Bind(adDataInstantiate);
        Debug.Log("Ad Instantiated");
    }

    #endregion



    /// <summary>
    /// Functions to show and handle edit data tab
    /// </summary>
    #region Edit data Tab 

    void ShowEditAdPanel()
    {
        create = mode == AdWindowMode.Create;
        EditorGUILayout.Space();

        CreateOrLoadToggle();

        if (!create && !loaded) FileLoadOptions();

        EditorGUILayout.Space();

        if (create || loaded) AdEditOptions();

    }
    void AdEditOptions()
    {
        if(!loaded)
        fileName = EditorGUILayout.TextField("Ad Name", fileName);

        adImageTex = EditorGUILayout.ObjectField("Ad Image", adImageTex, typeof(Texture2D), false) as Texture2D;
        if (adImageTex != null && loaded)
        {
            try
            {
                Texture2D previewTex = new Texture2D(24, 24);
                byte[] buffer = adImageTex.EncodeToPNG();
                previewTex.LoadRawTextureData(buffer);
                GUIStyle gUIStyle = new GUIStyle();
                gUIStyle.fixedWidth = 50;
                gUIStyle.fixedHeight = 50;
                GUILayout.Label(adImageTex, gUIStyle);
            }
            catch
            {
                Debug.LogWarning("Error loading texture");
            }
        }
        GUI.color = Color.red+Color.yellow;
        EditorGUILayout.LabelField("Texture should be marked as readable and texture type should be Sprite & 2D");
        GUI.color = originalGUIColor;

        if (loaded)
        {
            GUI.color = Color.yellow + Color.red;
            EditorGUILayout.LabelField("Preview");
            GUI.color = originalGUIColor;
        }

        headLine = EditorGUILayout.TextField("Headline", headLine);
        
        rating = EditorGUILayout.IntField("Rating", rating);

        GUI.color = Color.red;
        if (rating<0 || rating>5)
            EditorGUILayout.LabelField("Rating Should be between 0 and 5",EditorStyles.miniLabel);
        GUI.color = originalGUIColor;

        desc = EditorGUILayout.TextField("Description", desc);
        price = EditorGUILayout.IntField("Price", price);
        
        GUI.color = Color.red;
        if (price < 0 )
            EditorGUILayout.LabelField("Price Should greater or equal to zero", EditorStyles.miniLabel);
        GUI.color = originalGUIColor;

        themeColor = EditorGUILayout.ColorField("Theme Color", themeColor);
        themeColor.a = 1;

        if (GUILayout.Button("Save Ad file"))
        {
            SaveFile();
        }
        if (File.Exists("Assets/Mobile Ads/Ad Data/" + fileName + ".json"))
        {
            GUI.color = Color.red;
            EditorGUILayout.LabelField("WARNING : ! Saving data will over write " + fileName + " Data !", EditorStyles.boldLabel);
        }

    }
    void CreateOrLoadToggle()
    {
        EditorGUILayout.Separator();
        if (GUILayout.Button("Create"))
        {
            mode = AdWindowMode.Create;
            loaded = false;
            ResetProperties();
        }
        if (GUILayout.Button("Load"))
        {
            mode = AdWindowMode.Load;
            loaded = false;
            ResetProperties();
        }
        EditorGUILayout.LabelField((mode==AdWindowMode.Create?"Create mode:":"Load and Edit mode:"), EditorStyles.boldLabel);
        EditorGUILayout.Space();
    }

    void FileLoadOptions()
    {
        EditorGUILayout.LabelField("Load data from a file");
        adTextFile = EditorGUILayout.ObjectField("Ad Data file", adTextFile, typeof(TextAsset), false) as TextAsset;

        if (adTextFile != null && GUILayout.Button("Load Ad File"))
        {
            try
            {
                LoadSavedFile();
            }
            catch
            {
                Debug.LogError("GG MOBILE SDK ERROR : Please select a valid Ad Json");
                return;
            }            
        }
    }

    void SaveFile() 
    {

        if (headLine == "" || rating <= -1 || price <= -1 || desc == "" || adImageTex == null)
        {
            Debug.LogError("GG MOBILE SDK ERROR : Ad Creation Failed : One or more properties are empty or invalid");
            return;
        }
       adData =new AdData(headLine, rating, desc, price, themeColor,adImageTex);

        try
        { 
            string json = JsonUtility.ToJson(adData);

            Directory.CreateDirectory(dataSavePath);
            try
            {
                if (loaded && adTextFile != null)
                    File.WriteAllText(AssetDatabase.GetAssetPath(adTextFile), json);
                else
                {
                    File.WriteAllText(dataSavePath +"Ad-" +fileName + ".json", json);
                }
            }
            catch 
            {
                Debug.LogError("File Creation Failed");
                return;
            }
        }
        catch
        {
            Debug.LogError("Json Creation Failed");
            return;
        }
        
        loaded = false;
        ResetProperties();
        Debug.Log("GG MOBILE AD SDK: File saved");
        tab = 1;
        AssetDatabase.Refresh();
    }
    void ResetProperties() 
    {
        fileName = string.Empty;
        adTextFile = null;
        adImageTex = null;
        headLine = string.Empty;
        rating = -1;
        desc = string.Empty;
        price = -1;
    }

    void LoadSavedFile()  
    {
        try
        {
            adData = JsonUtility.FromJson<AdData>(adTextFile.text);
        }
        catch(Exception e)
        {
            Debug.LogError("GG MOBILE SDK ERROR : Cannot Convert Json to Object. Invalid Json");
            throw e;
        }
        try 
        {
            Texture2D texture = new Texture2D(1,1, TextureFormat.ARGB32, false);
            texture.LoadImage(adData.adImage);
            adImageTex=texture;
        }
        catch(Exception e) 
        {
            Debug.LogError("GG MOBILE SDK ERROR : Cannot Load Image");
            throw e;
        }
        loaded = true;
        fileName = adTextFile.name;
        headLine = adData.headLine;
        rating = adData.rating;
        desc = adData.desc;
        price =adData.price;


    }
    #endregion



  

   
}
