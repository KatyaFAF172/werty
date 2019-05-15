using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    #region variables
    
    [SerializeField] RectTransform thrusterFuelFill;

    [SerializeField] RectTransform curHealthFill;

    private PlayerController controller;
    private Player player;
    private float valueHealth;
    
    #endregion

    public void SetController(PlayerController controller)
    {
        this.controller = controller;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    private void SetFuelAmount (float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }

    private void SetHealthAmount(int amount)
    {
        valueHealth = (float) amount / 100;
        curHealthFill.localScale = new Vector3(valueHealth, 1f, 1f);
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetCurHealth());
    }
}
