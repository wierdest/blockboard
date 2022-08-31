using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputProvider : SimulationBehaviour, INetworkRunnerCallbacks
{
    
    private InputData inputData = new InputData();
    private NetworkRunner runner;

    // reference the color picker so we can collect the color
    [SerializeField] private ColorPicker colorPicker;
    // store last color to save redundancy calls that would change the value to the default black.
    private Color lastColor;

    // reference the inputField so we can collect text
    [SerializeField] private TMPro.TMP_InputField inputField;
    
    // store last string to save redundancy calls
    private NetworkString<_512> lastInputText;
    private NetworkBehaviourId lastShapeId;

    public void OnEnable()
    {
        // add this to the callbacks
        runner = FindObjectOfType<NetworkRunner>();
        if(runner != null)
        {
            Debug.Log("Input Provider: Adding callbacks!");
            runner.AddCallbacks(this);
        }

        // set colorPicker callback
        lastColor = Color.clear;
        colorPicker.SetOnValueChangeCallback(colorPickerCallback);
        lastInputText = "";
    }

    public void OnDisable()
    {
        if(runner != null)
            runner.RemoveCallbacks(this);
    }

    #region BUTTONS
	public void OnClickAddSphere()
	{
        Debug.Log("Clicked Add Sphere!!");
        inputData.Buttons.Set(NetworkedBoardButtons.AddSphere, true);
	}

    public void OnClickAddCube()
	{
        inputData.Buttons.Set(NetworkedBoardButtons.AddCube, true);

	}
	public void OnClickAddPyramid()
	{
        inputData.Buttons.Set(NetworkedBoardButtons.AddPyramid, true);
	}

    public void OnClickUndoButton()
    {
        // removes last
        inputData.Buttons.Set(NetworkedBoardButtons.RemoveLast, true);
    }

    public void OnClickRemoveButton()
    {
        inputData.Buttons.Set(NetworkedBoardButtons.RemoveAll, true);
    }

	public void OnClickSplitButton()
	{
        inputData.Buttons.Set(NetworkedBoardButtons.Split, true);
		
	}

    public void OnClickNexusButton()
    {
        inputData.Buttons.Set(NetworkedBoardButtons.Nexus, true);
    }

    #endregion

    #region SELECTION

    public void SelectNetworkedShape(NetworkBehaviourId shapeId)
    {
        lastShapeId = shapeId;
    }

    #endregion

   
    #region COLOR

    private void colorPickerCallback(Color color)
    {
        lastColor = color;
    }

    #endregion

    
    #region TEXT

    public void OnInputValueChanged()
    {
        var value = inputField.text;
        if(string.IsNullOrEmpty(value))
        {
            return;
        }
        lastInputText = value;
    }

    #endregion

   
    #region NETWORK RUNNER CALLBACKS

	public void OnInput(NetworkRunner runner, NetworkInput input) 
	{
        // Debug.Log("Input Provider, Get Input!");
        // only updates the color if it's a new one
        if(!lastColor.Equals(inputData.SelectedShapeColor))
        {
            inputData.SelectedShapeColor = lastColor;
        }

        if(!lastShapeId.Equals(inputData.SelectedShapeId))
        {
            inputData.SelectedShapeId = lastShapeId;
        }

        if(!lastInputText.Equals(inputData.InputText))
        {
            inputData.InputText = lastInputText;
        }
        
        // set input data on tick 
		input.Set(inputData);
		// reset the input struct to start with a clean slate when polling for next tick
		inputData = default;
	}
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }


    #endregion

   
}
