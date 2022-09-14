using Fusion;
using Fusion.Sockets;
using Photon.Voice.Fusion;
using Photon.Voice.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    // fusion's network runner
    [SerializeField] private NetworkRunner thisRunner;
    
    // our player prefab does not have a body yet
    [SerializeField] private NetworkPrefabRef playerPrefab;

    // keeps track of networked players
    private Dictionary<PlayerRef, NetworkObject> spawnedPlayers = new Dictionary<PlayerRef, NetworkObject>(); 

    // manages the part of user interface which is networked
    [SerializeField] private GameObject simpleLobbyInterface, hostInterface, clientInterface, hostOfflineConfirmationUI, clientOfflineConfirmationUI;
    [SerializeField] private TMPro.TMP_InputField sessionNameInputField;
    [SerializeField] private TMPro.TMP_Text lobbyWarningText, hostStatusText, clientStatusText, networkedShapeManagerStatus;
    [SerializeField] private SwitchButton nexusButton;
    [SerializeField] private ConfirmationButton offlineButton;
    private string hostStatusTemplate = "hosting {0}  to:  {1} user{2}";
    private string positiveLobby = "All good! We're online!";
    private string negativeLobby = "Failed to connect to the network!\n Reason: {0}";
    private readonly string waitLobby = "Connecting...";
   
    // activates our input provider on connection
    [SerializeField] private InputProvider networkInputProvider;

    // upon player instantiation, we pass in the category manager to the networked shape manager
    [SerializeField] private CatManager catManager;
    [SerializeField] private CatButton catButton;

    /// voice stuff

    // Photon Voice Connection
    [SerializeField] private VoiceConnection voiceConnection;
    // Recorder
    [SerializeField] private Recorder primaryRecorder;
    // Mic Button
    [SerializeField] private SwitchButton micButton;


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
        Time.timeScale = 1.0f;
        thisRunner.Shutdown();
        SceneManager.LoadScene(0);
    }
    #endregion

    // mic button
    public void OnClickMicButton()
    {
        primaryRecorder.TransmitEnabled = !primaryRecorder.TransmitEnabled;
        micButton.Switch(primaryRecorder.TransmitEnabled);
        
    }

    private async void startGame(GameMode mode, string roomName)
    {
        thisRunner.ProvideInput = true;
        lobbyWarningText.text = waitLobby;
        var result = await thisRunner.StartGame(new StartGameArgs() 
        {
            GameMode  = mode,
            SessionName = string.IsNullOrEmpty(roomName) ? "Common Room" : roomName,
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
        
        if(result.Ok)
        {
            // all good
            lobbyWarningText.text = positiveLobby;
            // load proper user interface for the scene 
            StartCoroutine(waitToLoadHostOrClientInterface());
            // activates our input provider
            networkInputProvider.enabled = true;
            
        }
        else
        {
            lobbyWarningText.text = string.Format(negativeLobby, result.ShutdownReason);
        }
    }
    private IEnumerator waitToLoadHostOrClientInterface()
    {
        yield return new WaitForSeconds(0.8f);

        simpleLobbyInterface.SetActive(false);
        offlineButton.gameObject.SetActive(true);

        if(thisRunner.IsServer)
        {
            hostInterface.SetActive(true);
            offlineButton.SetConfirmationUI(hostOfflineConfirmationUI);
            yield return null;
        }

        if(thisRunner.IsClient)
        {
            clientInterface.SetActive(true);
            offlineButton.SetConfirmationUI(clientOfflineConfirmationUI);

            // temporary
            Destroy(ClickAndDrag.Instance.gameObject);
            yield return null;
        }
        
    }

    private void Awake()
    {
        primaryRecorder = voiceConnection.PrimaryRecorder;
    }
    private void Update()
    {
        // update interface
        if(thisRunner != null)
        {
            if(thisRunner.IsServer)
            {
                var count = spawnedPlayers.Count;
                hostStatusText.text = string.Format(
                    hostStatusTemplate,
                    thisRunner.SessionInfo.Name,
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
                player,
                (runner, o) => {
                    // initialize data and connects some of the networked UI
                    var shapeManager = o.GetComponent<NetworkedShapeManager>();
                    shapeManager.OnInit(
                        networkedShapeManagerStatus,
                        nexusButton,
                        catManager
                    );
                    catButton.SetNetworkedShapeManager(shapeManager);
                    
                }
            );
     
            // Debug.Log("Spawning Server");
            spawnedPlayers.Add(player, networkPlayerObject);
            // var count = spawnedPlayers.Count;
            // if(count > 1)
            // {
            //     Debug.LogFormat("A Player joined our game! Now we're {0} players!", count);
            // }
        }

        if(runner.IsClient)
        {
            Debug.Log("Spawning Server");
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
        if(shutdownReason == ShutdownReason.Ok)
        {
            if(thisRunner)
            {
                Destroy(gameObject);
            }
            SceneManager.LoadScene(0);
        }
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) 
    {
        Debug.Log("Network Manager: OnDisconnectedFromServer");

    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) 
    { 
        Debug.Log("Network Manager: OnConnectFailed");
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }


    #endregion
}
