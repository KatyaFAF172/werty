using UnityEngine.Networking;
using UnityEngine;
using System.Collections;

public class Player : NetworkBehaviour
{
    #region variables

    [SyncVar]
    private bool _isDead = false;

    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    public Vector3 mapSize;

    [SyncVar]
    private int curHealth;
    //server needs to know health of each player at the specific moment of time and here's required total sync of this variable
    //with server.

    [SerializeField]
    private Behaviour[] disableOnDeath;

    #endregion

    #region Setup for player

    private bool firstSetup = true;
    public void Setup()
    {
        CmdBroadcastNewSetup();
    }

    [Command]
    private void CmdBroadcastNewSetup()
    {
        RpcSetupOnClients();
    }

    [ClientRpc]
    private void RpcSetupOnClients()
    {
        if(firstSetup)
        {
            SetDefaults();

            //firstSetup = false;
        }
    }
    #endregion

    public int GetCurHealth()
    {
        return curHealth;
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage)
    {
        if (isDead)
            return;

        curHealth -= damage;

        //Debug.Log(transform.name + " now has " + curHealth + " hp");

        if(curHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        //Disable components for dead player for evading errors
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        Collider col = GetComponent<Collider>();
        if (col)
            col.enabled = false;

        Debug.Log(transform.name + " is dead");

        //call respawn for dead player
        StartCoroutine(Respawn());

    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform spawnPoint = NetworkManager.singleton.GetStartPosition();
        //transform.position = spawnPoint.position;
        //transform.rotation = spawnPoint.rotation;

        if (isServer)
        {
            transform.position = new Vector3(mapSize.x - 15, 50, mapSize.z - 15);
        }
        else if (isClient)
        {
            transform.position = new Vector3(15, 50, 15);
        }

        yield return new WaitForSeconds(0.1f);

        Setup();

        Debug.Log(transform.name + " respawned");
    }

    private void SetDefaults()
    {
        isDead = false;

        curHealth = maxHealth;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = true;
        }

        Collider col = GetComponent<Collider>();
        if (col)
            col.enabled = true;
    }

    //void Update()
    //{
    //    if (!isLocalPlayer)
    //        return;

    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        RpcTakeDamage(999);
    //    }
    //}
}