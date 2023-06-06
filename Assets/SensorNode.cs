using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorNode : MonoBehaviour
{
    public Point gridPos;
    // public int prox;
    // public float temp;
    // public float alt;
    // public float hum;
    // public float[] orient;

    // public void Init() {
        
    // }

    public void setData(SensorDataObj dataObj) {
        this.gridPos = new Point(dataObj.gridPos);
        renderTempEffects(dataObj.temp);
        renderProxEffects(dataObj.prox);
        renderOrientEffects(dataObj.orient);
        // print("setting data " + dataObj);
    }

    private float scaleRawTemp(double rawTemp) {
        return (float) ((rawTemp - 24) / 5.0);
    }

    private void renderTempEffects(float temp) {
        var sensorRenderer = this.GetComponent<Renderer>();
        Color scaledTempColor = new Color(scaleRawTemp(temp),0,0,1);
        // print(scaledTempColor);
        sensorRenderer.material.SetColor("_Color", scaledTempColor);
    }

    private void renderProxEffects(int prox) {
        float scaleChange1D = (float) (1.25 - (prox / 255.0));
        // print(prox + " " + scaleChange1D);
        this.transform.localScale = new Vector3(1, System.Math.Min(1, scaleChange1D), 1);
    }

    private void renderOrientEffects(float[] orient) {
        Vector3 currPos = this.transform.position;
        // Quaternion newRotation = Quaternion.Euler(orient[1], orient[0], orient[2]);
        // this.transform.SetPositionAndRotation(currPos, this.transform.parent.rotation * newRotation);
        this.transform.eulerAngles = new Vector3(orient[1], orient[0], -orient[2]);
        // this.transform.parent.rotation = newRotation;
    }

    // public override string ToString() {
    //     return "{gridPosition: " + gridPos +
    //             "; prox: " + prox + 
    //             "; temp: " + temp +
    //             "; alt: " + alt +
    //             "; hum: " + hum +
    //             "; orient: " + orient[0] + "," + orient[1] + "," + orient[2] + "}";
    // }
}
