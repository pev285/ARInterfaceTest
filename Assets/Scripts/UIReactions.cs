using System.Collections;
using System.Collections.Generic;
using UnityARInterface;
using UnityEngine;

public class UIReactions : MonoBehaviour {

    public void OnPlanesVisibilityChanged(bool on)
    {
        CommandKeeper.WriteLineDebug("Planes on = " + on);

        CommandKeeper.SetPlanesOn(on);
    }

    public void OnPointsVisibilityChanged(bool on)
    {
        CommandKeeper.WriteLineDebug("Points on = " + on);

        CommandKeeper.SetPointCloudOn(on);
    }

}
