using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class Laser : MonoBehaviour {


  //  int layerMask = 1 << 9;
    private LineRenderer lr;
    private GameHandler gameHandler;

    private bool isLaserOn = false; // if we look at the arrow of this laser

    void Start() {
        lr = GetComponent<LineRenderer>();

        gameHandler = GameObject.Find("GameHandler").GetComponent<GameHandler>();
    }

    // Update is called once per frame
    void Update() {

        if (Application.isPlaying) {
            if (isLaserOn) {
                if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.W)) {
                    if (gameHandler.IsCurrentMissionNavigation()) {
                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, transform.forward, out hit)) {
                            if (hit.collider) {
                                lr.SetPosition(1, hit.point);

                                Debug.Log("hit name: " + hit.collider.gameObject.name); // hit.collider.gameObject.name -> gives us the cylinder gameobject name, for example: Cylinder (1)

                                if (hit.collider.gameObject.name.Contains("Cylinder")) {
                                    int panoramaIndex = int.Parse(Regex.Match(hit.collider.gameObject.name, @"[0-9]+").Value);


                                    gameHandler.IsArrowsTrigger(transform.parent.parent.parent.parent.gameObject, true); //Current cylinder we are moving from
                                    gameHandler.SetCylinderMeshRenderer(transform.parent.parent.parent.parent.gameObject, false); //Make current cylinder invisible

                                    int prevCylinderIndex = int.Parse(Regex.Match(transform.parent.parent.parent.parent.gameObject.name, @"[0-9]+").Value);
                                    gameHandler.GetDataAfterExitedCylinder(prevCylinderIndex, Time.time - gameHandler.enterTimeToCylinder);
                                    gameHandler.MoveCamera(panoramaIndex);

                                    gameHandler.IsArrowsTrigger(hit.collider.gameObject, false); // new cylinder we are moving to
                                    gameHandler.SetCylinderMeshRenderer(hit.collider.gameObject, true); //Make next cylinder visible


                                }

                            }
                        }
                        else lr.SetPosition(1, new Vector3(0, 0, -100f));
                        isLaserOn = false;
                    }
                }

            }
        }
    }

    /// <summary>
    /// For enabeling/disabeling arrows' trigger. Later on the arrows' triggers will be change according to isLaserOn
    /// </summary>
    /// <param name="state">
    /// If state is 'false' -> it means we can click on that arrow (the arrow is disabled)
    /// If state is 'true' -> it means we can't click on that arrow (the arrow is enabled)
    /// </param>
    public void SetIsLaserOn(bool state) {
        isLaserOn = state;
    }


}