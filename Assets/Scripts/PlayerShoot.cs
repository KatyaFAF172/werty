using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask mask;

    private WeaponManager weaponManager;
    private PlayerWeapon curWeapon;

    private void Start()
    {
        if (!cam)
        {
            Debug.LogError("PlayerShoot: No camera referenced");
            this.enabled = false;
        }

        weaponManager = GetComponent<WeaponManager>();

        //-----------------------------------------------------------
        //fixing graphics if there is an object with clildren inside
        //weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);
        //Transform parent = weaponGFX.transform;
        //foreach (Transform child in parent)
        //    { child.gameObject.layer = LayerMask.NameToLayer(weaponLayerName); }
        //-------------------------------------------------------------



        //weaponGFX.layer = LayerMask.NameToLayer(weaponLayerName);
    }

    private void Update()
    {
        curWeapon = weaponManager.GetCurrentWeapon();

        if(curWeapon.fireRate <= 0f)
        {
            if (Input.GetButtonDown("Fire1"))    //make shoot via left click of the mouse
            {
                Shoot();
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f/curWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    //Is called on the server when a player shoots
    [Command]
    void CmdOnShoot()
    {
        RpcDoShootEffect();
    }

    //is called on all clients when we need to do a shoot effect
    [ClientRpc]
    void RpcDoShootEffect()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    [Client]
    void Shoot()
    {
        if (!isLocalPlayer)
            return;

        Debug.Log("Shoot");

        //We are shooting, call the OnShoot method on server
        CmdOnShoot();

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, curWeapon.range, mask))
        {
            //our bullet hits object in range of weapon if it has correct property
            if(hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(hit.collider.name, curWeapon.damage);
            }
            //Debug.Log("We hit" + hit.collider.name);
        }
    }

    [Command]
    void CmdPlayerShot(string playerID, int damage)
    {
        Debug.Log(playerID + " has been shot");
        //Destroy(GameObject.Find(ID));
        Player player = GameManager.GetPlayer(playerID);    //find player that gets damage
        player.RpcTakeDamage(damage);    //taking damage from shooting
    }
}
