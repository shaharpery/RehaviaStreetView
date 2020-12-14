using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ViewerHander : MonoBehaviour
{
  
    public GameObject CylinderParent;
   // public GameObject BaseCylinder;
    public Texture[] BasePanoramasCubemaps;
    public GameObject ArrowPrefab;
  //  public Material BaseMaterial;

    [Header("Windows")]
    public GameObject MAIN_MENU_WINDOW;
    public GameObject SETTINGS_WINDOW;
    public GameObject GAME_WINDOW;
    public GameObject TASK_WINDOW;
    public GameObject ORIENTATION_WINDOW;
    public GameObject FINISHMISSION_WINDOW;
    public GameObject GAME_OVER_WINDOW;

    [Header("Setting")]
    public GameObject[] MISSIONS_SETTINGS;
    public Dropdown DROPDOWN_NumberOfMissions;
    public Dropdown DROPDOWN_StartingZone;
    public Dropdown[] DROPDOWN_Missions;
    public Dropdown[] DROPDOWN_Destinations;

    [Header("Missions")]
    //Task Window
    public Text MainMissionTitleText;
    public Text MainDestinationNameText;

    // Orientation Window
    public Text Button1_Text;
    public Text Button2_Text;


    [Header("HUD")]
    public Text HUDMissionTitleText;
    public Text HUDDestinationNameText;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
