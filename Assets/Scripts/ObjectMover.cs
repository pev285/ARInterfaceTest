using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMover : MonoBehaviour {

    [SerializeField]
    private GameObject prefab;

    private Transform objTransform = null;

    private float movingSpeed = 1f;
    private float rotationSpeed = 0.005f;

    Sequence tweenSequence;

    void Start ()
    {
        CommandKeeper.UserPointedTo += MoveToNewPosition;

        tweenSequence = DOTween.Sequence();
    }


    private void MoveToNewPosition(Vector3 targetPosition)
    {
        if (objTransform == null)
        {
            Transform obj = GetOrCreateObject();
            obj.position = targetPosition;
        }
        else
        {
            float movingTime = movingSpeed * Vector3.Distance(objTransform.position, targetPosition);
            float angle = Vector3.Angle(objTransform.forward, targetPosition - objTransform.position);
            //Vector3 axis;
            //Quaternion.FromToRotation(objTransform.forward, targetPosition - objTransform.position).ToAngleAxis(out angle, out axis);
            float rotationTime = angle * rotationSpeed;

            //Sequence sequence = DOTween.Sequence();

            //tweenSequence.Kill();
            //DOTween.KillAll();

            tweenSequence.Kill();
            tweenSequence = DOTween.Sequence();
            tweenSequence.Append(objTransform.DOLookAt(targetPosition, rotationTime)).Append(objTransform.DOMove(targetPosition, movingTime));
        }

    }


    private Transform GetOrCreateObject()
    {
        if (objTransform != null)
        {
            return objTransform;
        }

        GameObject obj = Instantiate(prefab, gameObject.transform);

        objTransform = obj.transform;
        return objTransform;
    }
	
} // End Of Class //


