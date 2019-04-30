using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField]    //make private variables to be shown in the inspector
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;
    [SerializeField]
    private GameObject[] prefabs;

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;
    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;
    private float thrusterFuelAmount = 1f;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    private PlayerMotor motor;
    
    // Start is called before the first frame update
    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    // Update is called once per frame
    void Update()
    {
        #region movement
        //calculate movement velocity as 3d vector
        float xMov = Input.GetAxisRaw("Horizontal");    //allows to set programmers function of working with movement to make smooth movement in network
        float zMov = Input.GetAxisRaw("Vertical");

        Vector3 movHorizontal = transform.right * xMov; //transform.right shows direction of movement on xAxis (1, 0, 0).
        Vector3 movVertical = transform.forward * zMov;
        
        //calculation of movement
        Vector3 velocity = (movHorizontal + movVertical).normalized * speed;
        //use calculated movement
        motor.Move(velocity);
        #endregion

        #region CameraTurning
        //---------------------------------------------------------------------------------------
        //Calculate rotation as 3d vector (turn around) for X
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0f, yRot, 0f) * lookSensitivity;

        //apply rotation
        motor.Rotate(rotation);

        //---------------------------------------------------------------------------------------
        //Calculate rotation as 3d vector (turn around) for Y
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * lookSensitivity;

        //apply rotation
        motor.RotateCamera(cameraRotationX);
        //----------------------------------------------------------------------------------------
        #endregion

        #region JetPack jump
        //calculate thruster force based on the user input
        Vector3 thrusterForceLoc = Vector3.zero;
        if(Input.GetButton("Jump") && thrusterFuelAmount > 0f)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if(thrusterFuelAmount >= 0.01f) //fix of infinite jump up by pressing space
            {
                thrusterForceLoc = Vector3.up * thrusterForce;
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0f, 1f);    //needs to keep amount of fuel in interval from 0 to 1

        //apply thruster force
        motor.ApplyThrusterF(thrusterForceLoc);
        #endregion

        if (Input.GetKeyDown(KeyCode.K))
        {
            PlayerRayCast();
        }
    }

    #region createObject

    [System.Obsolete]
    public void PlayerRayCast()
    {

        RaycastHit hit;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out hit, 3.0f))
        {
            Transform objectHit = hit.transform;
            GameObject created = CreateObject(prefabs[0]);

            Vector3 hitPosition = hit.point;
            Vector3 objPosition = hit.transform.position;



            created.transform.position = detectSpawnPosition(hitPosition, objPosition, hit.transform.localScale, created.transform.localScale);


            // Do something with the object that was hit by the raycast.
        }

    }

    Vector3 detectSpawnPosition(Vector3 hitPosition, Vector3 objPosition, Vector3 hittedObjScale, Vector3 creatingObjScale)
    {

        double[] diferrences = new double[3];

        diferrences[0] = Mathf.Abs(hitPosition.x - objPosition.x);
        diferrences[1] = Mathf.Abs(hitPosition.y - objPosition.y);
        diferrences[2] = Mathf.Abs(hitPosition.z - objPosition.z);

        int i = diferrences.ToList().IndexOf(diferrences.Max());

        switch (i)
        {
            case 0:
                return new Vector3(
                    objPosition.x + (hittedObjScale.x * 0.5f * (hitPosition.x - objPosition.x) > 0 ? 1 : -1),
                    objPosition.y,
                    objPosition.z);
            case 1:
                return new Vector3(
                    objPosition.x,
                    objPosition.y + (hittedObjScale.y * 0.5f * (hitPosition.y - objPosition.y) > 0 ? 1 : -1),
                    objPosition.z);
            case 2:
                return new Vector3(
                    objPosition.x,
                    objPosition.y,
                    objPosition.z + (hittedObjScale.z * 0.5f * (hitPosition.z - objPosition.z) > 0 ? 1 : -1));

        }
        return Vector3.zero;
    }

    [System.Obsolete]
    GameObject CreateObject(GameObject prefab)
    {
        GameObject obj = (GameObject)Instantiate(prefab);

        //Spawn the bullet on the clients
        NetworkServer.Spawn(obj);

        return obj;
    }

    #endregion

}
