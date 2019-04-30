using UnityEngine;

[System.Serializable]   //opens this class to the inspector in order to change properties of each gun
public class PlayerWeapon
{
    public string name = "MP-5";

    public int damage = 10;  //setting damage for gun
    public float range = 100f;  //setting effective distance for gun

    public float fireRate = 0f;

    public GameObject model;
}