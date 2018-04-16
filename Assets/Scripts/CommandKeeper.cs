using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommandKeeper {

    public static Action<string> WriteLineDebug = (a) => { };

    public static Action<Vector3> UserPointedTo = (a) => { };


    public static Action<bool> SetPlanesOn = (a) => { };
    public static Action<bool> SetPointCloudOn = (a) => { };

    public static Func<bool> GetPlanesOn = () => { return false; };
    public static Func<bool> GetPointCloudOn = () => { return false; };

    public static Func<List<Vector3>> GetPointCloudCoordsList = () => { return null; };

}
