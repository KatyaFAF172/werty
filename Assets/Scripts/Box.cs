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
