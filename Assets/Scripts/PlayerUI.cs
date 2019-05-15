using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    #region variables
    
    [SerializeField]
    RectTransform thrusterFuelFill;

    private PlayerController controller;
    
    #endregion

    public void SetController(PlayerController controller)
    {
        this.controller = controller;
    }

    private void SetFuelAmount (float amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, amount, 1f);
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
    }
}
