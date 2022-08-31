using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// add buttons should start the enum
// add buttons should follow shapetypes order
enum NetworkedBoardButtons
{
    AddSphere = 0,
    AddPyramid = 1,
    AddCube = 2,
    RemoveLast = 3,
    RemoveAll = 4,
    Split = 5,
    Nexus = 6

}
public struct InputData : INetworkInput
{
    public NetworkButtons Buttons;
    public NetworkBehaviourId SelectedShapeId;
    public Color SelectedShapeColor;

    // network string is limited to 512 chars. 
    // This is enforced via TMPro's in editor option [limited to 500 chars]
    public NetworkString<_512> InputText;
}
