using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    public GameHandler gameHandler;
    public ViewerHander viewerHander;

    //  public Texture[] BasePanoramasCubemaps;

    public Material BaseMaterial;
    public GameObject BaseCylinder;
    public GameObject CylinderParent;
    public GameObject ColliderForCloseCylinders;

    GameHandler.Panorama[] panoramas;


    // Read Only
    readonly float RADIUS_FOR_GETTING_CLOSE_CYLINDERS = 1.7f;



    public void BuildCylinderMap()
    {
        gameHandler = FindObjectOfType<GameHandler>();
        CylinderParent = GameObject.Find("Cylinders");

        panoramas = new GameHandler.Panorama[viewerHander.BasePanoramasCubemaps.Length];

        for (int i = 0; i < panoramas.Length; i++)
        {

            //'fill' panoramas array with coordinate and cubemap
            panoramas[i] = new GameHandler.Panorama();

            panoramas[i].SetCoordinate(gameHandler.GetCoordinateFromString(viewerHander.BasePanoramasCubemaps[i].ToString()));
            panoramas[i].SetRotation(gameHandler.GetRotationFromString(viewerHander.BasePanoramasCubemaps[i].ToString()) - 180);
            panoramas[i].SetCubemap(viewerHander.BasePanoramasCubemaps[i]);
            panoramas[i].SetMaterial(BaseMaterial);

            //  CheckMinMax(panoramas[i].GetCoordinate());
        }

        for (int i = 0; i < panoramas.Length; i++)
        {
            panoramas[i].PlaceCylinderOnField(BaseCylinder, CylinderParent, i, gameHandler.CYLINDER_SCALE);
            panoramas[i].GetCylinderObject().GetComponent<MeshRenderer>().enabled = false;

        }
    }


    public void PlaceArrowsInAllFinalCylinders()
    {
        for (int i = 0; i < gameHandler.FINAL_CYLINDERS.Length; i++)
        {
            Collider[] nearestCylinders = GetNearestCylindersToGivenCylinder(gameHandler.FINAL_CYLINDERS[i]);
            for (int j = 0; j < nearestCylinders.Length; j++)
            {
                InsertAnArrowToCylinder(gameHandler.FINAL_CYLINDERS[i], nearestCylinders[j].transform.gameObject, j);
            }
        }
    }


    public void CylinderMapTransforamtion()
    {
        for (int i = 0; i < gameHandler.FINAL_CYLINDERS.Length; i++)
        {
            GameHandler.Coordinate tempCoord = new GameHandler.Coordinate(gameHandler.FINAL_CYLINDERS[i].transform.position.x, gameHandler.FINAL_CYLINDERS[i].transform.position.z);
            CheckMinMax(tempCoord);
        }
        double deltaX = (((maxX - minX) / 2) + minX);
        double deltaZ = (((maxZ - minZ) / 2) + minZ);


        foreach (var cylinder in gameHandler.FINAL_CYLINDERS)
        {
         //   cylinder.transform.position -= new Vector3((float)deltaX, 0f, (float)deltaZ);

            cylinder.transform.position *= 5;
        }
    }


    private double minX = Double.MaxValue;//sets to infinty
    private double maxX = Double.MinValue;
    private double minZ = Double.MaxValue;
    private double maxZ = Double.MinValue;
    ///    PROBABLY WE ARE NOT GOING TO USE IT
    /// Check for max and min coordinates
    /// <param name = "_coordinate" ></ param >
    void CheckMinMax(GameHandler.Coordinate _coordinate)
    {
        //Check X
        if (_coordinate.X < minX)
        {
            minX = _coordinate.X;

        }

        else if (_coordinate.X > maxX)
        {
            maxX = _coordinate.X;

        }

        //Check Y
        if (_coordinate.Z < minZ)
        {
            minZ = _coordinate.Z;

        }

        else if (_coordinate.Z > maxZ)
        {
            maxZ = _coordinate.Z;
        }
    }

    /// <summary>
    /// Placing arrows in a given cylinder
    /// </summary>
    /// <param name="currentCylinder"> the cylinder we place the arrows in </param>
    /// <param name="targetCylinder"> the cylinder the arrow should look at </param>
    /// <param name="arrowNum"> The arrow's number in same cylinder. used to change the arrows' names </param>
    private void InsertAnArrowToCylinder(GameObject currentCylinder, GameObject targetCylinder, int arrowNum)
    {
        GameObject tempArrow = Instantiate(viewerHander.ArrowPrefab);
        tempArrow.name = "arrow" + arrowNum;
        tempArrow.transform.parent = currentCylinder.transform;
        tempArrow.transform.localPosition = new Vector3(0, -0.2f, 0);
        tempArrow.transform.localScale = new Vector3(1, 1, 1);


        tempArrow.transform.LookAt(targetCylinder.transform); // Makes the arrow look at the targer cylinder
        tempArrow.transform.eulerAngles = new Vector3(0, tempArrow.transform.eulerAngles.y, 0); // makes the arrow be parallel to the ground

        tempArrow.transform.localPosition = tempArrow.transform.parent.InverseTransformDirection(tempArrow.transform.forward) * 0.4f; // Places the arrow in the edge of the cylinder
        tempArrow.transform.localPosition -= new Vector3(0, 0.2f, 0);
    }

    /// <summary>
    /// Gets all cylinder in the radius of RADIUS_FOR_GETTING_CLOSE_CYLINDERS
    /// </summary>
    /// <returns> returns an array of nearest cylinder </returns>
    private Collider[] GetNearestCylindersToGivenCylinder(GameObject givenCylinder)
    {
        GameObject tempColliderGameObject = Instantiate(ColliderForCloseCylinders);
        tempColliderGameObject.transform.parent = givenCylinder.transform;
        tempColliderGameObject.transform.localPosition = new Vector3(0, 0, 0);
        tempColliderGameObject.transform.localScale = new Vector3(1, 1, 1);

        Collider[] tempCollider = Physics.OverlapSphere(tempColliderGameObject.transform.position, RADIUS_FOR_GETTING_CLOSE_CYLINDERS);


        List<Collider> tempList = tempCollider.ToList<Collider>();
        for (int i = 0; i < tempList.Count; i++)
        {


            if (tempList[i].name == givenCylinder.GetComponent<Collider>().name)
            {
                tempList.RemoveAt(i);

                DestroyImmediate(tempColliderGameObject);
                return tempList.ToArray();
            }
        }


        if (tempColliderGameObject.GetType() != typeof(Camera))
        {
            DestroyImmediate(tempColliderGameObject);
        }
        return null;
    }
}
