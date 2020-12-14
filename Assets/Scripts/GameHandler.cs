using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameHandler : MonoBehaviour
{
    [Header("General")]
    public GameObject Camera;
    Camera cameraGO;
    public ViewerHander ViewerHander;//refer

    [Header("Cylinders")]
    public Panorama[] panoramas;
    public GameObject[] FINAL_CYLINDERS;





    //Coordinates
    readonly float RATIO = 1000;
    readonly float SPARE_DIST = 30;
    [HideInInspector]
    public readonly float CYLINDER_SCALE = 0.5f;


    // Readonly 
    readonly Color NORAMAL_ARROW = new Color(0.5490196f, 0.4705882f, 0.3529412f);
    readonly Color HOVER_ARROW = new Color(0.7647059f, 0.6470588f, 0.4705882f);
    readonly int MISSION_TIMER = 600;



    //Missions
    Mission[] missions;
    int currentMissionIndex; //The destination we are heading to. Represented by the index in 'currentGameDestinations' array (above)
    int firstPosition = 124;
    [HideInInspector]
    public float enterTimeToCylinder = 0;
    [HideInInspector]
    public float timeTookToAnswerOrientation = 0;

    //Cylinder
    [HideInInspector]
    public int CurrentCylinderIndex = 0;

    //Pre-made destinations
    readonly Destination BANK_DEST = new Destination(Destination.DestType.Place, new int[] { 163 }, "Bank");
    readonly Destination SUPERMARKET_DEST = new Destination(Destination.DestType.Place, new int[] { 20, 21 }, "Supermarket");
    readonly Destination MUSEUM_DEST = new Destination(Destination.DestType.Place, new int[] { 10 }, "Museum");
    readonly Destination KUZARIGARDEN_DEST = new Destination(Destination.DestType.Place, new int[] { 70, 71, 75, 80 }, "Garden");
    readonly Destination SYNAGOGUE_DEST = new Destination(Destination.DestType.Place, new int[] { 250, 251 }, "Synagogue");

    //Starting zones
    readonly int ZONE1 = 124;
    readonly int ZONE2 = 250;
    readonly int ZONE3 = 350;
    readonly int ZONE4 = 450;
    readonly int ZONE5 = 500;
    readonly int ZONE6 = 180;


    //pairs for Orientation
    readonly string[,] OrientationDestPairs = { {"Bank", "Kuzari Garden" },
                                                { "Bank", "Synagouge"},
                                                { "Supermarket", "Bank" },
                                                {"Museum", "Kuzari Garden" },
                                                {"Bank", "Museum" },
                                                {"Kuzari Garden", "Supermarket" }};

    //For Final Report
    List<string> FinalReport = new List<string>(); //Should finaly export this for final report


    Destination[] FINAL_DESTINATIONS;

    private void Awake()
    {
        //DontDestroyOnLoad(gameObject);
        LoadXMlOnStartUp();

        HideWindow(ViewerHander.GAME_WINDOW);
        HideWindow(ViewerHander.CylinderParent);
        HideWindow(ViewerHander.SETTINGS_WINDOW);
        HideWindow(ViewerHander.ORIENTATION_WINDOW);
        HideWindow(ViewerHander.TASK_WINDOW);
        HideWindow(ViewerHander.FINISHMISSION_WINDOW);
        HideWindow(ViewerHander.GAME_OVER_WINDOW);
        ShowWindow(ViewerHander.MAIN_MENU_WINDOW);


        //WILL BE DELETED ---  Need to choose which destinations according to settings
        FINAL_DESTINATIONS = new Destination[] { BANK_DEST, SUPERMARKET_DEST, MUSEUM_DEST, KUZARIGARDEN_DEST, SYNAGOGUE_DEST };


        cameraGO = Camera.GetComponent<Camera>();

        panoramas = new Panorama[FINAL_CYLINDERS.Length];
    }

    // Start is called before the first frame update
    void Start()
    {
        //BuildCylinderMap();
        InsertAlreadyBuiltMapToPanoramasArray();
        //MoveCamera(true);
    }



    RaycastHit hitInfo;
    RaycastHit tempHit = new RaycastHit();
    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying)
        {



            Vector3 CameraCenter = cameraGO.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, cameraGO.nearClipPlane));
            //   RaycastHit hit;
            //if (Physics.Raycast(CameraCenter, transform.forward, out hit, 1000)) {
            //    // Debug.Log("hit: " + hit.collider.name);
            //}


            if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out hitInfo, 100.0f))
            {


                if (hitInfo.collider.name.Contains("arrow"))
                {
                    Laser laser = hitInfo.collider.gameObject.GetComponentInChildren<Laser>();
                    laser.SetIsLaserOn(true);
                    //   hitInfo.collider.gameObject.GetComponentInChildren<MeshRenderer>().material.color = HOVER_ARROW;

                    foreach (Transform child in hitInfo.collider.gameObject.transform)
                    {
                        // do whatever you want with child transform object here
                        if (child.name.Contains("Box"))
                        {
                            child.GetComponentInChildren<MeshRenderer>().material.color = HOVER_ARROW;
                        }
                    }

                    tempHit = hitInfo;
                }

                else
                {
                    if (tempHit.collider != null)
                    {
                        if (tempHit.collider.name.Contains("arrow"))
                        {
                            //      tempHit.collider.gameObject.GetComponentInChildren<MeshRenderer>().material.color = NORAMAL_ARROW;
                            foreach (Transform child in tempHit.collider.gameObject.transform)
                            {
                                // do whatever you want with child transform object here
                                if (child.name.Contains("Box"))
                                {
                                    child.GetComponentInChildren<MeshRenderer>().material.color = NORAMAL_ARROW;
                                }
                            }
                            tempHit.collider.gameObject.GetComponentInChildren<Laser>().SetIsLaserOn(false);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Inserts all cylinders game objects to panoramas
    /// </summary>
    void InsertAlreadyBuiltMapToPanoramasArray()
    {
        for (int i = 0; i < panoramas.Length; i++)
        {
            //'fill' panoramas array with coordinate and cubemap
            panoramas[i] = new Panorama(FINAL_CYLINDERS[i]);
            panoramas[i].SetCoordinate(GetCoordinateFromString(ViewerHander.BasePanoramasCubemaps[i].ToString()));
            panoramas[i].SetRotation(GetRotationFromString(ViewerHander.BasePanoramasCubemaps[i].ToString()));
        }
    }
    /// <summary>
    ///  Check if reached destination and moves the camera to the center of the Cylinder with the number 'panoramaIndex'
    /// </summary>
    /// <param name="panoramaIndex"> index of cylinder we move to </param>
    public void MoveCamera(int cylinderIndex)
    {
        CurrentCylinderIndex = cylinderIndex;
        if (CurrentCylinderIndex != firstPosition)
        {
            HandleMission(cylinderIndex);
        }


        // Moves the camera to the center of the Cylinder with the number 'panoramaIndex'
        Quaternion rotation = Camera.transform.rotation;
        Camera.transform.position = panoramas[cylinderIndex - 1].GetCylinderObject().transform.position;

        Camera.transform.rotation = rotation;

        enterTimeToCylinder = Time.time; //Resting time for checking how long spend in cylinder
    }


    /// <summary>
    /// Called from main menu when game is start
    /// </summary>
    public void StartGame()
    {
        UseChosenDestinationsFromSettings();
        SetStartingPosition(ViewerHander.DROPDOWN_StartingZone.value + 1);
        CurrentCylinderIndex = firstPosition;

        HideWindow(ViewerHander.MAIN_MENU_WINDOW);
        HideWindow(ViewerHander.SETTINGS_WINDOW);
        ShowWindow(ViewerHander.GAME_WINDOW);
        ShowWindow(ViewerHander.CylinderParent);

        SetCylinderMeshRenderer(panoramas[firstPosition - 1].GetCylinderObject(), true); //Makes sure the first cylinder is being rendered
        IsArrowsTrigger(panoramas[firstPosition - 1].GetCylinderObject(), false); //Makes sure the arrow(s) in firs cylinder are not triggered (so we can click on them)

        MoveCamera(firstPosition);

        currentMissionIndex = -1; //Sets it to -1 because the first thing FinishMission does is currentMissionIndex++
        FinishMission();


        AddLandmarkToCylinderGameObjectName();
    }


    /// <summary>
    /// Set destinations array from chosen destinations from settings 
    /// </summary>
    void UseChosenDestinationsFromSettings()
    {
        missions = new Mission[ViewerHander.DROPDOWN_NumberOfMissions.value + 1];

        for (int i = 0; i < missions.Length; i++)
        {
            missions[i] = new Mission();
            missions[i].SetMissionTypeFromInt(ViewerHander.DROPDOWN_Missions[i].value);

            if (missions[i].GetMissionType() == Mission.MissionType.Navigation)
            {
                missions[i].destination = FINAL_DESTINATIONS[ViewerHander.DROPDOWN_Destinations[i].value]; //Sets pre-made destinations according to what dropdown has
                missions[i].destinationName = ViewerHander.DROPDOWN_Destinations[i].options[ViewerHander.DROPDOWN_Destinations[i].value].text.ToString();
            }
            else if (missions[i].GetMissionType() == Mission.MissionType.NavigationPerson)
            {
                missions[i].personName = persons[ViewerHander.DROPDOWN_Destinations[i].value].personsName;
                missions[i].destinationName = persons[ViewerHander.DROPDOWN_Destinations[i].value].location;

                for (int j = 0; j < FINAL_DESTINATIONS.Length; j++)
                {
                    if (FINAL_DESTINATIONS[j].GetDestinationName() == missions[i].destinationName)
                    {
                        missions[i].destination = FINAL_DESTINATIONS[j];
                        break;
                    }
                }
            }
            else if (missions[i].GetMissionType() == Mission.MissionType.Orientation || missions[i].GetMissionType() == Mission.MissionType.OrientationPerson)
            {
                missions[i].chosenOrientation = ViewerHander.DROPDOWN_Destinations[i].value;
            }
        }
    }


    /// <summary>
    /// Handle missions and check if finished a mission.
    /// For each missionType we should check different 'winning' conditions
    /// </summary>
    /// <param name="cylinderIndex"> The cylinder number we are currently at</param>
    void HandleMission(int cylinderIndex)
    {
        Mission.MissionType tempType = missions[currentMissionIndex].GetMissionType();
        // Handles navigation
        if (currentMissionIndex >= 0 && currentMissionIndex < missions.Length)
        {

            if (tempType == Mission.MissionType.Navigation)
            {
                missions[currentMissionIndex].destination.CheckIfVisitingThisDest(cylinderIndex);
                if (missions[currentMissionIndex].destination.DidVisitDest())
                {
                    ShowWindow(ViewerHander.FINISHMISSION_WINDOW);
                    FinalReport.Add("Reached Destination at Cylinder " + cylinderIndex);
                }
            }
            // Handles navigation person
            else if (tempType == Mission.MissionType.NavigationPerson)
            {
                missions[currentMissionIndex].destination.CheckIfVisitingThisDest(cylinderIndex);
                if (missions[currentMissionIndex].destination.DidVisitDest())
                {
                    ShowWindow(ViewerHander.FINISHMISSION_WINDOW);
                    FinalReport.Add("Reached Destination at Cylinder " + cylinderIndex);
                }
            }

            // Handles orientations missions
            else if (tempType == Mission.MissionType.Orientation || tempType == Mission.MissionType.OrientationPerson)
            {

                timeTookToAnswerOrientation = Time.time; //Resets time in order to know how long took to answer

                int rnd = Random.Range(0, 2);

                if (rnd == 0)
                {
                    ViewerHander.Button1_Text.text = OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 0];
                    ViewerHander.Button2_Text.text = OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 1];
                    FinalReport.Add("Landmark A: " + OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 0]);
                    FinalReport.Add("Landmark B: " + OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 1]);
                }
                else
                {
                    ViewerHander.Button1_Text.text = OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 1];
                    ViewerHander.Button2_Text.text = OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 0];
                    FinalReport.Add("Landmark A: " + OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 1]);
                    FinalReport.Add("Landmark B: " + OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 0]);
                }

                ShowWindow(ViewerHander.ORIENTATION_WINDOW);
            }

        }
    }


    /// <summary>
    /// Called when clicking on the ok button of the Finish Mission Window
    /// </summary>
    public void OnFinishMissionButtonClicked()
    {
        HideWindow(ViewerHander.FINISHMISSION_WINDOW);
        FinishMission();
    }

    /// <summary>
    /// Player finished a mission.
    /// if player finished all missions, game over and won
    /// else, 
    /// Starting timer
    /// </summary>
    void FinishMission()
    {
        currentMissionIndex++;

        FinalReport.Add("   ------------------   ");

        if (currentMissionIndex < missions.Length)
        {
            if (missions[currentMissionIndex].missionType == Mission.MissionType.Navigation)
            {
                FinalReport.Add("Mission " + currentMissionIndex + ": " + missions[currentMissionIndex].missionType.ToString() + " - " + missions[currentMissionIndex].GetDestinationName());
            }
            else if (missions[currentMissionIndex].missionType == Mission.MissionType.NavigationPerson)
            {
                FinalReport.Add("Mission " + currentMissionIndex + ": " + missions[currentMissionIndex].missionType.ToString() + " - "
                    + missions[currentMissionIndex].GetDestinationName() + " - " + missions[currentMissionIndex].personName);
            }
            else if (missions[currentMissionIndex].missionType == Mission.MissionType.Orientation ||
                                        missions[currentMissionIndex].missionType == Mission.MissionType.OrientationPerson)
            {
                FinalReport.Add("Mission " + currentMissionIndex + ": " + missions[currentMissionIndex].missionType.ToString());
                FinalReport.Add("Current Location: " + CurrentCylinderIndex);
            }
        }


        if (currentMissionIndex >= missions.Length)
        {
            GameOver();
        }
        else
        {
            StartCoroutine(TimeToFinishMission(currentMissionIndex));//starting timer
            ////Visuals
            //Texts
            ViewerHander.HUDMissionTitleText.text = missions[currentMissionIndex].GetMissionTitle();
            if (missions[currentMissionIndex].missionType == Mission.MissionType.Navigation)
            {
                ViewerHander.HUDDestinationNameText.text = missions[currentMissionIndex].destinationName;
                ViewerHander.MainMissionTitleText.text = missions[currentMissionIndex].GetMissionTitle();
                ViewerHander.MainDestinationNameText.text = missions[currentMissionIndex].destinationName;

            }

            else if (missions[currentMissionIndex].missionType == Mission.MissionType.NavigationPerson)
            {
                ViewerHander.HUDDestinationNameText.text = missions[currentMissionIndex].personName;
                ViewerHander.MainMissionTitleText.text = missions[currentMissionIndex].GetMissionTitle();
                ViewerHander.MainDestinationNameText.text = missions[currentMissionIndex].personName;
            }
            else if (missions[currentMissionIndex].missionType == Mission.MissionType.Orientation ||
                missions[currentMissionIndex].missionType == Mission.MissionType.OrientationPerson)
            {
                ViewerHander.HUDDestinationNameText.text = "";
                ViewerHander.MainMissionTitleText.text = "";
                ViewerHander.MainDestinationNameText.text = "";
            }
            //Mission Window
            ShowWindow(ViewerHander.TASK_WINDOW);
            HandleMission(CurrentCylinderIndex);
        }
    }


    /// <summary>
    ///    - Gets the chosen locatation, check if correct or not
    ///    - Checks if correct
    ///    - Adds to final report
    ///    - Finishes mission
    /// </summary>
    /// <param name="ButtonGameObject"></param>
    public void OnOrientationButtonClicked(Button buttonGameObject)
    {
        enterTimeToCylinder = Time.time;

        HideWindow(ViewerHander.ORIENTATION_WINDOW);


        string chosenDest = buttonGameObject.GetComponentInChildren<Text>().text;

        string realClosesDest = GetClosestDestForOrientation();

        if (chosenDest == realClosesDest)
        {
            //Mission Succeed
            FinalReport.Add("Orientation Succeed");
        }
        else
        {
            //Mission Failed
            FinalReport.Add("Orientation Failed");
        }
        float roundedFinalTime = (float)Math.Round((Time.time - timeTookToAnswerOrientation) * 100f) / 100f;
        FinalReport.Add("Time took to answer: " + roundedFinalTime + " seconds");

        FinishMission();
    }


    string GetClosestDestForOrientation()
    {
        Vector3 currentLocation = cameraGO.transform.position;

        Vector3 dest1Location = new Vector3();
        string firstLocationName = "";
        Vector3 dest2Location = new Vector3();
        string secondLocationName = "";

        for (int i = 0; i < FINAL_DESTINATIONS.Length; i++)
        {
            if (OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 0] == FINAL_DESTINATIONS[i].GetDestinationName())
            {
                dest1Location = FINAL_CYLINDERS[FINAL_DESTINATIONS[i].GetDestCylinderIndexes()[0]].transform.position;//FINAL_DESTINATIONS[i].GetDestCylinderIndexes[0]
                firstLocationName = FINAL_DESTINATIONS[i].GetDestinationName();
            }
            if (OrientationDestPairs[missions[currentMissionIndex].chosenOrientation, 1] == FINAL_DESTINATIONS[i].GetDestinationName())
            {
                dest2Location = FINAL_CYLINDERS[FINAL_DESTINATIONS[i].GetDestCylinderIndexes()[0]].transform.position;//FINAL_DESTINATIONS[i].GetDestCylinderIndexes[0]
                secondLocationName = FINAL_DESTINATIONS[i].GetDestinationName();
            }
        }

        float firstDistance = Vector3.Distance(currentLocation, dest1Location);
        float secondDistance = Vector3.Distance(currentLocation, dest2Location);

        if (firstDistance <= secondDistance)
        {
            return firstLocationName;
        }
        else
        {
            return secondLocationName;
        }
    }

    /// <summary>
    /// On game over.
    /// </summary>
    void GameOver()
    {

        string todaysDate = DateTime.Now.ToString("M-d-yy") + "_" + Regex.Replace(DateTime.Now.ToString("t"), ":", "-");

        System.IO.File.WriteAllLines(todaysDate + "-FinalReport.txt", FinalReport);
        ShowWindow(ViewerHander.GAME_OVER_WINDOW);

        Debug.Log("Game Over");
        //SceneManager.LoadScene("GameOverScene");
    }



    void SetStartingPosition(int zone)
    {
        switch (zone)
        {
            case 1:
                firstPosition = ZONE1;
                break;
            case 2:
                firstPosition = ZONE2;
                break;
            case 3:
                firstPosition = ZONE3;
                break;
            case 4:
                firstPosition = ZONE4;
                break;
            case 5:
                firstPosition = ZONE5;
                break;
            case 6:
                firstPosition = ZONE6;
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// Timer for each mission. if reaching zero, than game is over
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeToFinishMission(int missionNumber)
    {
        yield return new WaitForSeconds(MISSION_TIMER); // 10 minutes

        if (missionNumber == currentMissionIndex)
        {
            FinalReport.Add("Mission failed! Time is up!");
            FinishMission();
        }
    }


    /// <summary>
    /// Shows/Hides relevant dropdowns depending the number chosen in DROPDOWN_NumberOfMissions
    /// </summary>
    public void UpdateNumberOfMissionsInSettings()
    {
        for (int i = 0; i < ViewerHander.MISSIONS_SETTINGS.Length; i++)
        {
            if (i <= ViewerHander.DROPDOWN_NumberOfMissions.value)
            {
                ViewerHander.MISSIONS_SETTINGS[i].SetActive(true);
            }
            else
            {
                ViewerHander.MISSIONS_SETTINGS[i].SetActive(false);
            }
        }
    }
    public bool IsCurrentMissionNavigation()
    {
        if (missions[currentMissionIndex].GetMissionType() == Mission.MissionType.Orientation ||
            missions[currentMissionIndex].GetMissionType() == Mission.MissionType.OrientationPerson)
        {
            return false;
        }
        else return true;

    }

    /// <summary>
    /// Enabeling/Disabeling arrows' trigger
    /// </summary>
    /// <param name="cylinder"> The cylinder which we want to enable/disable its arrows </param>
    /// <param name="futureTrigger">
    //       If state is 'false' -> it means we can click on that arrow (the arrow is disabled)
    //       If state is 'true' -> it means we can't click on that arrow (the arrow is enabled)
    /// </param>
    public void IsArrowsTrigger(GameObject cylinder, bool futureTrigger)
    {
        foreach (Transform child in cylinder.transform)
        {
            if (child.name.Contains("arrow"))
            {
                child.gameObject.GetComponent<BoxCollider>().isTrigger = futureTrigger;
            }
        }
    }

    /// <summary>
    /// Enabeling/Disabeling a cylinder's meshrenderer (i.e: if to show or hide cylinder)
    /// </summary>
    /// <param name="cylinder"> The cylinder which we want to enable/disable its meshrenderer </param>
    /// <param name="state">
    ///         if state is 'true' -> the cylinder will be visible
    ///         if state is 'false' -> the cylinder will be invisible
    /// </param>
    public void SetCylinderMeshRenderer(GameObject cylinder, bool state)
    {
        cylinder.GetComponent<MeshRenderer>().enabled = state;
    }


    /// <summary>
    /// Gets the coordinate from the panorama's name
    /// </summary>
    /// <param name="input"> name as string of one panorama </param>
    /// <returns> coordinate </returns>
    public Coordinate GetCoordinateFromString(string input)
    {
        string[] substrings = Regex.Split(input, "LAT");
        // Gets with RegEx the latitude and longitude and returns a coordinate out of it
        return new Coordinate((Convert.ToDouble(Regex.Match(substrings[0], @"[0-9]*\.[0-9]*").Value) - SPARE_DIST) * RATIO,
            (Convert.ToDouble(Regex.Match(substrings[1], @"[0-9]*\.[0-9]*").Value) - SPARE_DIST) * RATIO);
    }

    /// <summary>
    /// Gets the rotation from the panorama's name
    /// </summary>
    /// <param name="input"> name as string of one panorama </param>
    /// <returns> rotation as int </returns>
    public int GetRotationFromString(string input)
    {
        string[] substrings = Regex.Split(input, "ROTA");
        // Gets with RegEx the rotation
        return int.Parse(Regex.Match(substrings[1], @"[0-9]+").Value);
    }


    /// <summary>
    /// Show window GameObject
    /// </summary>
    /// <param name="window"> window to show </param>
    public void ShowWindow(GameObject window)
    {
        window.SetActive(true);
    }

    /// <summary>
    /// Hide window GameObject
    /// </summary>
    /// <param name="window"> window to Hide </param>
    public void HideWindow(GameObject window)
    {
        window.SetActive(false);
    }


    /// <summary>
    /// Create a new string with cylinder number and how many seconds the player visited in that cylinder.
    /// For Example:
    ////// 3.34 seconds at cylinder 10
    ////// 0.98 seconds at cylinder 211
    ////// 15.00 seconds at cylinder 10
    ////// 9.87 seconds at cylinder 25
    /// </summary>
    public void GetDataAfterExitedCylinder(int cylinderNumber, float secondsVisited)
    {
        float rounded = (float)Math.Round(secondsVisited * 100f) / 100f; // rounds the float to be 0.00 (exmlp: 3.36, 0.59, 11.00 etc..)
        string temp = rounded + " seconds at cylinder " + cylinderNumber;
        FinalReport.Add(temp);

        // Debug.Log(temp);
    }







    /// <summary>
    /// Changes the cylinder game object's name and adds the landmark name to it.
    /// " Cylinder10 - Bank "
    /// </summary>
    void AddLandmarkToCylinderGameObjectName()
    {
        for (int i = 0; i < FINAL_DESTINATIONS.Length; i++)
        {
            if (FINAL_DESTINATIONS[i].GetDestType() == Destination.DestType.Place)
            {
                foreach (int destination in FINAL_DESTINATIONS[i].GetDestCylinderIndexes())
                {
                    string temp = " - " + FINAL_DESTINATIONS[i].GetDestinationName();
                    FINAL_CYLINDERS[destination - 1].name += temp;
                }
            }
        }

    }

    public XmlDocument doc;
    /// <summary>
    /// Called on Awake
    /// </summary>
    void LoadXMlOnStartUp()
    {
        doc = new XmlDocument();
        doc.Load("navigationperson.xml");

        GetPersonsNamesAndTheirLocationFromXML();
    }


    Person[] persons; //Persons (includeing destinations) from xml file
    /// <summary>
    /// Gets ALL persons-locations from the XML and insert them into personFromXml array
    /// </summary>
    void GetPersonsNamesAndTheirLocationFromXML()
    {
        persons = new Person[6];
        for (int i = 1; i <= persons.Length; i++)
        {
            GetsIthPersonFromDoc(i);
        }

    }

    /// <summary>
    /// Saves the i'th person from the xml into the personsFromXml array
    /// </summary>
    /// <param name="i"> The i'th person (starting from 1) from the xml file </param>
    void GetsIthPersonFromDoc(int i)
    {
        string temp = doc.InnerText;

        string[] substrings = Regex.Split(temp, ("Person" + i));

        string[] finalSubString = Regex.Split(substrings[1], ("Person" + (i + 1))); // For example: finalSubString[0] will be 'DanniBank'

        string personsName = Regex.Match(finalSubString[0], @"^[A-Z][a-z]+").Value; //Gets the name. For example: 'Danni'
        string location = Regex.Match(finalSubString[0], @"[A-Z][a-z]+$").Value;  //Gets the location. For example: 'Bank'

        persons[i - 1] = new Person(personsName, location);
    }


    /// <summary>
    /// Checks if specific mission dropdown is navigation person, and if so:
    ///     - change the destination dropdown to be the name of the i'th person
    /// In case it is not navigation person, restore options
    ///
    /// </summary>
    /// <param name="missionDropDown"></param>
    public void UpdateDestinationDropdown(GameObject missionWindow)
    {
        Dropdown missionDropdown = null;
        Dropdown destDropdown = null;

        foreach (Transform child in missionWindow.transform)
        {
            if (child.name.Contains("Dropdown_Mission")) missionDropdown = child.GetComponent<Dropdown>(); //Gets the mission drop down child
            else if (child.name.Contains("Dropdown_Dest")) destDropdown = child.GetComponent<Dropdown>(); //Gets the destination drop down child
        }

        if (missionDropdown != null && destDropdown != null)
        {
            //First, we check if it Navigation or Orientation.
            //If it is Navigation, than we call ChangeDestDropDownToLandmarks()
            //If it is Orientation, than we call ChangeDestDropDownToNumbers()
            if (missionDropdown.captionText.text.Contains("Nav"))
            {//navigation
                ChangeDestDropDownToLandmarks(destDropdown);

                if (missionDropdown.captionText.text == "Nav - Person")
                {
                    ChangeDestDropDownToNames(destDropdown);
                }
            }
            else
            { //orientation
                ChangeDestDropDownToNumbers(destDropdown);
            }
            destDropdown.captionText.text = destDropdown.options[0].text;

            //if (missionDropdown.captionText.text == "Nav - Person") {
            //    destDropdown.captionText.text = persons[int.Parse(Regex.Match(missionDropdown.name, @"[0-9]").Value) - 1].personsName;
            //    //destDropdown.transform.Find("Arrow").gameObject.SetActive(false);
            //    //destDropdown.enabled = false;

            //}
            //else {
            //    destDropdown.captionText.text = destDropdown.options[0].text;
            //    //destDropdown.enabled = true;
            //    //destDropdown.transform.Find("Arrow").gameObject.SetActive(true);
            //}
        }
    }


    /// <summary>
    /// In case of Orientation mission, we change the destination drop down' list to numbers (instead of landmarks names)
    /// </summary>
    void ChangeDestDropDownToNumbers(Dropdown dropdown)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            dropdown.options[i].text = i.ToString();
        }
    }
    /// <summary>
    /// In case of Navigation mission, we change the destination drop down' list back to landmarks names
    /// </summary>
    void ChangeDestDropDownToLandmarks(Dropdown dropdown)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            dropdown.options[i].text = FINAL_DESTINATIONS[i].GetDestinationName();
        }
    }
    /// <summary>
    /// In case of Navigation mission, we change the destination drop down' list back to landmarks names
    /// </summary>
    void ChangeDestDropDownToNames(Dropdown dropdown)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            dropdown.options[i].text = persons[i].personsName;
        }
    }




    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////////                               ////////////////////////////////////////////
    ////////////////////////////////////////////       FUNCTION ENDS HERE      ////////////////////////////////////////////
    ////////////////////////////////////////////                               ////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Panorama class - the actual cylinder
    /// </summary>
    public class Panorama
    {

        Texture cubemap;
        Material material;
        GameObject cylinder;

        Coordinate coordinate = new Coordinate();
        int rotation;


        public Panorama()
        {

            rotation = 0; // Temporary

        }

        public Panorama(GameObject _cylinder)
        {
            cylinder = _cylinder;
        }



        public void SetCubemap(Texture _cubemap)
        {
            cubemap = _cubemap;
        }

        public void SetCylinderObject(GameObject _cylinder)
        {
            cylinder = _cylinder;
        }
        public GameObject GetCylinderObject()
        {
            return cylinder;
        }

        public void SetCoordinate(Coordinate _coordinate)
        {
            coordinate = _coordinate;
        }

        public Coordinate GetCoordinate()
        {
            return coordinate;
        }

        public void SetRotation(int _rotation)
        {

            if (_rotation < 0)
            {
                rotation = 360 + _rotation;
            }
            else
            {
                rotation = _rotation;

            }
        }

        public int GetRotation()
        {
            return rotation;
        }

        public void SetMaterial(Material _baseMaterial)
        {
            // material = _baseMaterial;

            material = Material.Instantiate(_baseMaterial);
            material.name = "instantiatedMaterial";

            //material = _baseMaterial;


            material.mainTexture = cubemap;
            //material.SetTexture("_Cube", cubemap);
        }



        public void PlaceCylinderOnField(GameObject baseCylinder, GameObject cylinderParent, int index, float scale)
        {

            //make cylinder with apropriate material (out of texture) and place it in field according coordinates and ration
            cylinder = Instantiate(baseCylinder, new Vector3((float)coordinate.X, 0, (float)coordinate.Z), Quaternion.identity);

            cylinder.name = "Cylinder " + "(" + (index + 1) + ")";

            cylinder.transform.parent = cylinderParent.transform;
            cylinder.transform.localScale = new Vector3(scale, scale, scale);

            material = new Material(Shader.Find("Skybox/Cubemap")) { name = "TempMaterial" };

            material.SetTexture("_Tex", cubemap);
            material.renderQueue = 3000;

            // cylinder.GetComponent<MeshRenderer>().material = newMtl;

            cylinder.GetComponent<MeshRenderer>().material = material;

            material.SetInt("_Rotation", rotation);
        }

    }



    /// <summary>
    /// Coordinate class
    /// </summary>
    public class Coordinate
    {
        public double X;
        public double Z;


        public Coordinate()
        {
            X = 0;
            Z = 0;
        }

        public Coordinate(double _X, double _Z)
        {
            X = _X;
            Z = _Z;

        }
    }

    /// <summary>
    /// A person details for Navigation-person (we get it from XML)
    /// </summary>
    public class Person
    {
        public string personsName;
        public string location;

        public Person()
        {
            personsName = "";
            location = "";
        }

        public Person(string _personName, string _location)
        {
            personsName = _personName;
            location = _location;


        }


    }
}
