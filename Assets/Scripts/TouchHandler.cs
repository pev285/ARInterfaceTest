using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchHandler : MonoBehaviour {

    //[SerializeField]
    //private GameObject cursor;

    [SerializeField]
    private LayerMask raycastLayerMask;

    [SerializeField]
    private float maxDistance = 30.0f;


    private GraphicRaycaster[] graphicRaycasters;


    private Camera mainCamera = null;

    void Start()
    {
        graphicRaycasters = FindObjectsOfType<GraphicRaycaster>();

        mainCamera = Camera.main;
    }


    void Update () {


        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began /*|| touch.phase == TouchPhase.Moved*/)
            {

                Vector2 touchPosition = touch.position;

                if (UITouched(touchPosition))
                {
                    //CommandKeeper.WriteLineDebug("UI Touched");
                }
                else 
                {
                    // if touched not UI //

                    if (CommandKeeper.GetPointCloudOn())
                    {
                        // Searching nearest pointcloud point and set position near it //
                        Vector3 worldTouchPosition = mainCamera.ScreenToWorldPoint(touchPosition);



                        List<Vector3> points = CommandKeeper.GetPointCloudCoordsList();

                        if (points != null)
                        {

                            Vector3 screenTouchPosition = new Vector3(touchPosition.x, touchPosition.y, 0);//mainCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

                            Vector3 nearestPoint = points[0];
                            float minimalScreenSqrDistance = (screenTouchPosition - mainCamera.WorldToScreenPoint(nearestPoint)).sqrMagnitude;

                            for (int i = 1; i < points.Count; i++)
                            {
                                float sqrDistance = (screenTouchPosition - mainCamera.WorldToScreenPoint(points[i])).sqrMagnitude;

                                if (sqrDistance < minimalScreenSqrDistance)
                                {
                                    minimalScreenSqrDistance = sqrDistance;
                                    nearestPoint = points[i];
                                }
                            }

                            // Now we know nearest point in projection on screen //

                            Vector3 nearestPointVector = nearestPoint - worldTouchPosition;
                            //float nearestPointWorldDistance = Vector3.Distance(worldTouchPosition, nearestPoint);

                            Vector3 forward = mainCamera.transform.forward.normalized;

                            Vector3 targetPosition = mainCamera.ScreenPointToRay(screenTouchPosition).GetPoint(Vector3.Dot(forward, nearestPointVector));
                            //Vector3 targetPosition = worldTouchPosition + forward * (Vector3.Dot(forward, nearestPointVector));

                            CommandKeeper.UserPointedTo(targetPosition);

                        } // if points != null //

                    }
                    else if (CommandKeeper.GetPlanesOn())
                    {
                        // Raycasting for planes //

                        Vector3 viewPortPoint = mainCamera.ScreenToViewportPoint(touchPosition);

                        //CommandKeeper.WriteLineDebug("touchpos=" + position + "viewPPoint=" + viewPortPoint);

                        Ray ray = mainCamera.ViewportPointToRay(viewPortPoint);

                        RaycastHit hit;


                        //Transform cameraTransform = Camera.main.transform;
                        //Vector3 pos = cameraTransform.position;
                        //Vector3 fwd = cameraTransform.forward;
                        //if (Physics.Raycast(pos, fwd, out hit))
                        //{
                        //    CommandKeeper.WriteLineDebug("centerhit=" + hit.collider.name + " at " + hit.point);
                        //    cursor.transform.position = hit.point;
                        //}


                        if (Physics.Raycast(ray, out hit, maxDistance, raycastLayerMask))
                        {
                            Vector3 worldPosition = hit.point;
                            //CommandKeeper.WriteLineDebug("HitCollider=" + hit.collider.name);
                            //CommandKeeper.WriteLineDebug("worldPos=" + worldPosition);

                            CommandKeeper.UserPointedTo(worldPosition);

                            //cursor.transform.position = worldPosition;
                        }

                    } // if raycasting planes //

                } // if touched not UI //


            } // if touch.phase //
        } // if touchCount //
		
	} // Update() //


    private bool UITouched(Vector2 touchpoint)
    {
        PointerEventData data = new PointerEventData(null);
        data.position = touchpoint;

        bool uiHit = false;

        foreach (var raycaster in graphicRaycasters)
        {
            List<RaycastResult> results = new List<RaycastResult>();

            raycaster.Raycast(data, results);
            if (results.Count != 0)
            {
                uiHit = true;
            }
        }

        return uiHit;
    } // UITouched() //


} // end of class //
