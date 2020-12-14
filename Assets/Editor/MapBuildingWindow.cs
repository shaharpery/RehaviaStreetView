using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class MapBuildingWindow : EditorWindow
{
    Texture2D sectionTexture;
    Rect section;

    public ViewerHander viewerHander;
    private MapBuilder mapBuilder;

    public GameObject CylinderParent;
    public GameObject BaseCylinder;
    public Texture[] BasePanoramasCubemaps;

    public Material BaseMaterial;


    public GameHandler gameHandler;



    [MenuItem("Window/Build Cylinder Map")]
    static void OpenWindow()
    {

        MapBuildingWindow window = (MapBuildingWindow)GetWindow(typeof(MapBuildingWindow));
        window.minSize = new Vector2(300, 300);
        window.maxSize = new Vector2(300, 300);
        window.Show();
    }

    /// <summary>
    /// Similar to Start/Awake
    /// </summary>
    private void OnEnable()
    {
        gameHandler = FindObjectOfType<GameHandler>();
        mapBuilder = FindObjectOfType<MapBuilder>();

        InitTextures();
    }


    void InitTextures()
    {
        sectionTexture = new Texture2D(1, 1);
        sectionTexture.SetPixel(0, 0, new Color(192, 192, 192));
        sectionTexture.Apply();

        //GUILayout.BeginArea(section);


        //GUILayout.EndArea();
    }


    /// <summary>
    /// Similar to Update
    /// </summary>
    private void OnGUI()
    {
        DrawLayouts();
    }

    void DrawLayouts()
    {
        section.x = 0;
        section.y = 0;
        section.width = Screen.width;
        section.height = Screen.height;

        GUI.DrawTexture(section, sectionTexture);


        if (GUILayout.Button("Build Cylinders", GUILayout.Height(40)))
        {
            Debug.Log("Clicked!!");
            Build();
        }

        if (GUILayout.Button("Place Arrows", GUILayout.Height(40)))
        {
            Debug.Log("Clicked!!");
            InsertArrows();
        }

        if (GUILayout.Button(" Map Transforamtion", GUILayout.Height(40)))
        {
            Debug.Log("Clicked!!");
            MapTransforamtion();
        }
    }



    void Build()
    {
        mapBuilder.BuildCylinderMap();
    }

    void InsertArrows()
    {
        mapBuilder.PlaceArrowsInAllFinalCylinders();
    }

    void MapTransforamtion()
    {
        mapBuilder.CylinderMapTransforamtion();
    }


}
