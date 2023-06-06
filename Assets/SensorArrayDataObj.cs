using UnityEngine;

[System.Serializable]
public class SensorArrayDataObj
{
    public SensorDataObj[] sensorArray;

    public override string ToString() {
        var output = "[";
        foreach (SensorDataObj sensorDataObj in sensorArray) {
            output += sensorDataObj.ToString() + ",";
        }
        return output.Substring(0, output.Length - 1) + "]";
    }
}
