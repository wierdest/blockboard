using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum NetworkedBoardButtons
{
    AddSphere = 0,
    AddPyramid = 1,
    AddCube = 2,
    Split = 3
}
public struct InputData : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector3 CurrentShapeDragPosition;
}
