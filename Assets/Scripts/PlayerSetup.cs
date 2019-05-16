﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    #region variables
    
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] string remoteLayerName = "RemotePlayer";
    [SerializeField] GameObject playerUIPrefab;
    private GameObject playerUIInstance;

    Camera sceneCamera;
    
    #endregion
    
    // Start is called before the first frame update
    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        } else
        {
            sceneCamera = Camera.main;
            if (sceneCamera)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            
            //create playerUI
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //configure player UI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
            if (!ui)
                Debug.LogError("No playerUI component on PlayerUI prefab");
            ui.SetController(GetComponent<PlayerController>());
            ui.SetPlayer(GetComponent<Player>());

            GetComponent<Player>().Setup();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();

        GameManager.RegisterPlayer(netID, player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if (sceneCamera)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
}