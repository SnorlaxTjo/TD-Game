using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSelection : MonoBehaviour
{
    #region Variables
    //Configurable Variables

    [Header("Setup")]
    [SerializeField] LayerMask selectableLayerBlacklist;

    [Header("Outline Configuration")]
    [SerializeField] float selectionOutlineOscillationRate = 90f;
    [Tooltip("Should not be bigger than selectionOutlineSize")]
    [SerializeField] float selectionOutlineOscillationMagnitude = 0.5f;
    [SerializeField] float selectionOutlineSize = 1f;

    //Private Variables

    RaycastHit hit;

    bool hasSelectedObject = false;

    //Cached Refrences

    GameObject hitGameobject;
    GameObject selectedGameObject;

    MeshFilter thisObjectsMeshFilter;
    MeshRenderer thisObjectsMeshRenderer;

    #endregion

    void Awake()
    {
        thisObjectsMeshFilter = gameObject.GetComponent<MeshFilter>();
        thisObjectsMeshRenderer = gameObject.GetComponent<MeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Finds the object the mouse is over
        Ray exampleRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(exampleRay, out hit/* ~selectableLayerBlacklist */))
        {
            hitGameobject = hit.collider.gameObject;
        }

        // RMB to deselect
        if (Input.GetMouseButtonDown(1))
        {
            hasSelectedObject = false;
            thisObjectsMeshRenderer.enabled = false;
            return;
        }

        //LMB to select
        if (Input.GetMouseButtonDown(0))
        {
            selectedGameObject = hitGameobject;

            transform.position = selectedGameObject.transform.position;
            hasSelectedObject = true;

            thisObjectsMeshRenderer.enabled = true;
            thisObjectsMeshFilter.mesh = selectedGameObject.GetComponent<MeshFilter>().mesh;
        }

        //Creates the "Wobble"
        if (hasSelectedObject)
        {
            Vector3 offsetVector = new Vector3(selectionOutlineSize, selectionOutlineSize, selectionOutlineSize);
            Vector3 ocillationVector = new Vector3(Mathf.Sin(selectionOutlineOscillationRate * Time.time) * selectionOutlineOscillationMagnitude, Mathf.Sin(selectionOutlineOscillationRate * Time.time) * selectionOutlineOscillationMagnitude, Mathf.Sin(selectionOutlineOscillationRate * Time.time) * selectionOutlineOscillationMagnitude);

            transform.localScale = selectedGameObject.transform.localScale + offsetVector + ocillationVector ;
        }
    }
}
