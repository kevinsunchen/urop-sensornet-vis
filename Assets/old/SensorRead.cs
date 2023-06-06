using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SensorRead : MonoBehaviour
{
    SerialPort sp;

    static int NUM_SENSORS = 4;

    public GameObject sensorObjPrefab;
    public static GameObject[] sensorObjList;
    
    // Start is called before the first frame update
    void Start()
    {
        string the_com="";
        print("Hello");
        foreach (string mysps in SerialPort.GetPortNames()) {
            print(mysps); 
            //if (mysps != "COM1") { the_com = mysps; break; }
        }
        print(the_com);
        the_com = "/dev/tty.usbmodem11301";
        sp = new SerialPort("/dev/tty.usbmodem11301", 115200);
        if (!sp.IsOpen) {
            print("Opening " + the_com + ", baud 115200");
            sp.Open();
            sp.ReadTimeout = 1000;
            sp.DtrEnable = true;
            sp.Handshake = Handshake.None;
            if (sp.IsOpen) { print("Open"); }
        }

        sensorObjList = new GameObject[NUM_SENSORS];
    }

    // Update is called once per frame
    void Update() {
        if (!sp.IsOpen) {
            sp.Open();
            print("opened sp");
        }
        if (sp.IsOpen) {
            string reading = "";
            try {
                reading = sp.ReadLine();
                SensorDataObj dataObj = JsonUtility.FromJson<SensorDataObj>(reading);
                print(dataObj);

                if (sensorObjList[0] == null) {
                    // TODO
                    GameObject obj = Instantiate(sensorObjPrefab);
                    sensorObjList[0] = obj;
                }

                GameObject sensorObj = sensorObjList[0];
                tempEffects(sensorObj, dataObj.temp);
                proxEffects(sensorObj, dataObj.prox);
                orientEffects(sensorObj, dataObj.orient);

            } 
            catch (System.TimeoutException) { 
                print("timeout ------------------------------------------------------");
            } 
            catch (System.FormatException) {
                print("Input was not formatted correctly: " + reading);
            }
        }
    }

    // TODO refine to return RGB tuple
    private float scaleRawTemp(double rawTemp) {
        return (float) ((rawTemp - 24) / 5.0);
    }

    private void tempEffects(GameObject sensorObj, float temp) {
        var sensorRenderer = sensorObj.GetComponent<Renderer>();
        Color scaledTempColor = new Color(scaleRawTemp(temp),0,0,1);
        print(scaledTempColor);
        sensorRenderer.material.SetColor("_Color", scaledTempColor);
    }

    private void proxEffects(GameObject sensorObj, int prox) {
        float scaleChange1D = (float) (1.25 - (prox / 255.0));
        print(scaleChange1D);
        sensorObj.transform.localScale = new Vector3(scaleChange1D, scaleChange1D, scaleChange1D);
    }

    private void orientEffects(GameObject sensorObj, float[] orient) {
        Vector3 currPos = sensorObj.transform.position;
        Quaternion newRotation = Quaternion.Euler(orient[2], orient[0], orient[1]);
        sensorObj.transform.SetPositionAndRotation(currPos, newRotation);
    }
}
