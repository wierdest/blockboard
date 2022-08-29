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
    public void OnEnable()
    {
        // add this to the callbacks
        runner = FindObjectOfType<NetworkRunner>();
        if(runner != null)
        {
            Debug.Log("Adding callbacks!");
            runner.AddCallbacks(this);
        }
    }

    public void OnDisable()
    {
        if(runner != null)
            runner.RemoveCallbacks(this);
    }

    #region ONCLICK

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

	public void OnClickSplit()
	{
		
	}


    #endregion

   

    #region NETWORK RUNNER CALLBACKS

	public void OnInput(NetworkRunner runner, NetworkInput input) 
	{
        // Debug.Log("Input Provider, Get Input!");
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
