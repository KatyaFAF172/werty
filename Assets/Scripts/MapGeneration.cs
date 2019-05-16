using UnityEngine;
using UnityEngine.Networking;
public class MapGeneration : NetworkBehaviour
{
    public GameObject[] prefab;
    public Vector3 mapSize;

    void Start()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int z = 0; z < mapSize.z; z++)
            {
                float noice = Mathf.PerlinNoise(x / mapSize.x, z / mapSize.z);
                int y = Mathf.RoundToInt(noice * mapSize.y);

                int posY = y - 20;

                GameObject obj = Instantiate(y < 5 ? prefab[0] :
                                             y < 8 ? prefab[1] :
                                             y < 11 ? prefab[2] :
                                                      prefab[3]);
                                            
                obj.transform.position = new Vector3(x, posY, z);
                
                NetworkServer.Spawn(obj);
            }
        }
    }
}
