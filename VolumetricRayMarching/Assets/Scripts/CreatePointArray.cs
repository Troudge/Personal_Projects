using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePointArray : MonoBehaviour
{
    //public variables
    public int numPoints = 10;
    public GameObject prefab;
    public Vector3 startPos = new Vector3(0, 0, 0);
    public List<VectPair> vectPairList = new List<VectPair>();

    // Start is called before the first frame update
    //Note: find a better way to pass variables to other scripts.*****
    void Start()
    {
        for (int i = 0; i < numPoints; i++)
        {
            for (int j = 0; j < numPoints; j++)
            {
                GameObject point = Instantiate(prefab, new Vector3(startPos.x + (5 * j), startPos.y + (5 * i), startPos.z), Quaternion.identity);
                VectPair temp = new VectPair(new Vector3(point.transform.position.x, point.transform.position.y, point.transform.position.z), new Vector3(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255)));
                vectPairList.Add(temp);
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
