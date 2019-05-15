using UnityEngine;
using UnityEngine.Networking;

public class Box : NetworkBehaviour
{
    [SerializeField] private int maxBoxHealth = 50;
    private int _curBoxHealth;

    [Command]
    public void CmdTakeBoxDamage(int damage)
    {
        _curBoxHealth -= damage;
        
        //Debug.Log(transform.name + " now has " + curBoxHealth.ToString() + " hp");

        if (_curBoxHealth <= 0)
        {
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }

    void Start()
    {
        _curBoxHealth = maxBoxHealth;
    }
}
