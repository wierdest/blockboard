using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // fusion's network runner
    [SerializeField] private NetworkRunner runner;

    // our player prefab does not have a body yet
    [SerializeField] private NetworkPrefabRef playerPrefab;

    // keeps track of networked players
    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>(); 

    // manages user interface
    [SerializeField] private GameObject simpleLobbyInterface, hostInterface, clientInterface;
    [SerializeField] private TMPro.TMP_InputField sessionNameInputField;
    [SerializeField] private TMPro.TMP_Text lobbyWarningText, hostStatusText, clientStatusText;
    private readonly string hostStatusTemplate = "hosting {0}  to:  {1} user{2}";

    // activates our input provider on connection
    [SerializeField] private InputProvider networkInputProvider;


    #region LOBBY INTERFACE
    public void OnClickHostButton()
    {
        startGame(GameMode.Host, sessionNameInputField.text);
    }

    public void OnClickJoinButton()
    {
        startGame(GameMode.Client, sessionNameInputField.text);
    }

    public void OnClickBackButton()
    {
        if(runner)
        {
            runner.Shutdown();
        }
        SceneManager.LoadScene(0);
    }
    #endregion

    private async void startGame(GameMode mode, string roomName)
    {
		if(runner == null)
		{
			runner = gameObject.AddComponent<NetworkRunner>();
			runner.ProvideInput = true;

			var result = await runner.StartGame(new StartGameArgs() 
			{
				GameMode  = mode,
				SessionName = string.IsNullOrEmpty(roomName) ? "Common Room" : roomName,
				Scene = SceneManager.GetActiveScene().buildIndex,
				SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
			});

			if(result.Ok)
			{
				// all good
				lobbyWarningText.text = "All good! We're online!";
				// load proper user interface for the scene 
				StartCoroutine(waitToLoadHostOrClientInterface());

                // activates our input provider
                networkInputProvider.enabled = true; 

			}
			else
			{
				lobbyWarningText.text = "Couldn't start Network Session! Going back to Offline Start Board!";
				SceneManager.LoadScene(0);
			}

		}

    }
    private IEnumerator waitToLoadHostOrClientInterface()
    {
        yield return new WaitForSeconds(1.5f);

        simpleLobbyInterface.SetActive(false);

        if(runner.IsServer)
        {
            hostInterface.SetActive(true);
            yield return null;
        }

        if(runner.IsClient)
        {
            clientInterface.SetActive(true);
            yield return null;
        }
        
    }

    private void Update()
    {
        // update interface
        if(runner != null)
        {
            if(runner.IsServer)
            {
                var count = spawnedPlayers.Count;
                hostStatusText.text = string.Format(
                    hostStatusTemplate,
                    runner.SessionInfo.Name,
                    count - 1,
                    count - 1 > 1 ? "s" : ""
                );

            }
        }
    }

    #region RUNNER CALLBACKS

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) 
    { 
        if(runner.IsServer)
        {   
            NetworkObject networkPlayerObject = runner.Spawn(
				playerPrefab, Vector3.zero, 
				Quaternion.identity, 
				player
			);
            spawnedPlayers.Add(player, networkPlayerObject);
            var count = spawnedPlayers.Count;
            if(count > 1)
            {
                Debug.LogFormat("A Player joined our game! Now we're {0} players!", count);
            }
        }

        if(runner.IsClient)
        {
            // if we're client set up our session info
            clientStatusText.text = string.Format(
                clientStatusText.text,
                runner.SessionInfo.Name
            );
        }
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) 
    { 
        if(spawnedPlayers.TryGetValue(player, out NetworkObject networkPlayerObject))
        {
            runner.Despawn(networkPlayerObject);
            spawnedPlayers.Remove(player);
        }

        Debug.LogFormat("A Player left our game! Now we're {0}", spawnedPlayers.Count);
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) 
    {
        Debug.LogFormat("NetworkManager OnShutdown with reason {0}!", shutdownReason);
        Destroy(runner);
    }
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
