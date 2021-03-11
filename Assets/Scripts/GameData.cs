using System.Collections.Generic;
using System;
using UnityEngine;

public static class GameData
{
    public static List<Rigidbody> ballsList = new List<Rigidbody>();
    public static List<Rigidbody> pooledBalls = new List<Rigidbody>();
    public static Queue<Rigidbody> mergeBallsQueue = new Queue<Rigidbody>();
    public static bool repelBalls = false;
}