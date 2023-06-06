using UnityEngine;

[System.Serializable]
public class SensorDataObj
{
    public int[] gridPos;
    public int prox;
    public float temp;
    public float alt;
    public float hum;
    public float[] orient;

    public override string ToString() {
        return "{gridPosition: (" + gridPos[0] + "," + gridPos[1] + ")" +
                "; prox: " + prox + 
                "; temp: " + temp +
                "; alt: " + alt +
                "; hum: " + hum +
                "; orient: " + orient[0] + "," + orient[1] + "," + orient[2] + "}";
    }
}
