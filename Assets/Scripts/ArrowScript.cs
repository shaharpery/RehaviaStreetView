using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    RaycastHit HitInfo;

    //public Material BaseArrowMaterial;

    //public Material BaseRedMaterial;

    private MeshRenderer meshRenderer;


    private Material myMaterial;

    private GameObject child1;
  
    //private Material myMaterial;



    // Start is called before the first frame update
    private void Awake() {
        child1 = gameObject.transform.GetChild(0).gameObject;

        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    private void Start() {


        child1.GetComponent<MeshRenderer>().material = Instantiate<Material>(child1.GetComponent<MeshRenderer>().material);


    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out HitInfo, 100.0f)) {
            Debug.Log("Hit info: " + HitInfo.collider.name);

            //meshRenderer.material.SetColor("_Color", Color.green);


            child1.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.cyan);
        }
        else {
            meshRenderer.material.SetColor("_Color", Color.blue);
            //rend.material = BaseArrowMaterial;

        }
    }



}
