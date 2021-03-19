using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolateShaders : MonoBehaviour
{

    public ComputeShader shader;
    public RenderTexture result;

    public struct VecPairFloat
    {
        public Vector3 pos;
        public Vector3 color;
    }

    // Start is called before the first frame update
    //Note: find a better way to pass variables to other scripts.
    void Start()
    {
        RunShader();
    }

    void RunShader()
    {
        List<VectPair> data = GameObject.Find("ObjectManager").GetComponent<CreatePointArray>().vectPairList;
       // VecPairFloat[] output;

        ComputeBuffer buffer = new ComputeBuffer(data.Count, 24);
        VectPair[] dataArray = data.ToArray();
        VecPairFloat[] blittData = ConvertToBlittable(dataArray);
        buffer.SetData(blittData);

        int kernelHandle = shader.FindKernel("CSMain");

        result = new RenderTexture(256, 256, 24);
        result.enableRandomWrite = true;
        result.Create();

        shader.SetTexture(kernelHandle, "Result", result);
        shader.SetBuffer(kernelHandle, "dataBuffer", buffer);
        shader.Dispatch(kernelHandle, 256 / 8, 256 / 8, 1);
        //buffer.GetData(output);
        buffer.Dispose();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public VecPairFloat[] ConvertToBlittable(VectPair[] data)
    {
        VecPairFloat[] blittData = new VecPairFloat[data.Length];
        for (int i = 0; i < data.Length; i++)
        {
            blittData[i].pos = data[i].pos;
            blittData[i].color = data[i].color;
        }
        return blittData;
    }
}
