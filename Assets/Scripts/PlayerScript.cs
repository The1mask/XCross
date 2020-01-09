using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public  int Player;
    public Material material;
    public int Shining = 0;
    // Start is called before the first frame update
    void Start()
    {
        material = this.GetComponent<Renderer>().material;
        if (Player == 0)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Shining == 1)
        {
            float shininess = Mathf.PingPong(Time.time, 1.0f);
            material.SetFloat("_OperationBlend_Fade_1", shininess);
        }
    }
    public void SetSecColor(Vector4 color)
    {
        material = this.GetComponent<Renderer>().material;
        material.SetVector("_TintRGBA_Color_2", color);
    }
    public void SetMainColor(Vector4 color)
    {
        material = this.GetComponent<Renderer>().material;
        material.SetVector("_TintRGBA_Color_1", color);
    }

}
