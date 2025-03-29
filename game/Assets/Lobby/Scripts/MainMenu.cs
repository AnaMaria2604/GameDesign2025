using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Permissions;
using UnityEngine;
using Mirror;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public static int GetAvailablePort()
    {
        TcpListener listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public void HostLobby()
    {
    int port = GetAvailablePort();
    networkManager.GetComponent<TelepathyTransport>().port = (ushort)port;

        UnityEngine.Debug.Log("Hosting on port: " + port);

    networkManager.StartHost();
    landingPagePanel.SetActive(false);
    }

}