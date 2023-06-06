using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class MockReadingGenerator
{
    public enum MockReadingType {
        SINGLE_HARDCODED,
        SENSOR,
        ARRAY
    }
    private MockReadingType mockType;
    private int gridWidth;
    private int gridHeight;
    private SensorReadStream sensorReader;
    private string initReading;

    public MockReadingGenerator(MockReadingType mockType, int gridWidth, int gridHeight, SensorReadStream sensorReader) {
        this.mockType = mockType;
        this.gridWidth = gridWidth;
        this.gridHeight = gridHeight;
        this.sensorReader = sensorReader;
        this.initReading = generateReading();
    }

    private string generateReading() {
        Debug.Log(this.mockType);
        string output = "{\"sensorArray\":[";
        switch (mockType) {
            case MockReadingType.SENSOR:
                string reading = sensorReader.read();
                for (int i=0; i<gridHeight; i++) {
                    for (int j=0; j<gridWidth; j++) {
                        // TODO: make this less janky, maybe add some regex instead of just hard coded substring
                        output += $"{{\"gridPos\":[{j},{i}]" + reading.Substring(16) + ",";
                    }
                }
                break;
            case MockReadingType.ARRAY:
                // output += "{"gridPos":[0,0],"prox":0,"temp":20.93999863,"alt":-70.96392059,"hum":54.25,"orient":[4.3125,36.875,-1.625]}";
                // float yaw = Random.Range(0.0f,360.0f);

                float[] pitchArray = new float[gridWidth];
                for (int j=0; j<gridWidth; j++) {
                    pitchArray[j] = Random.Range(-180.0f,180.0f);
                }
                
                for (int i=0; i<gridHeight; i++) {
                    float roll = Random.Range(-90.0f,90.0f);
                    for (int j=0; j<gridWidth; j++) {
                        float pitch = pitchArray[j];
                        output += $"{{\"gridPos\":[{j},{i}],\"prox\":0,\"temp\":25.12999916,\"alt\":-48.32490158,\"hum\":19.31999969,\"orient\":[{0},{roll},{0}]}}" + ",";
                    }
                }
                break;
            default:
            case MockReadingType.SINGLE_HARDCODED:  
                for (int i=0; i<gridHeight; i++) {
                    for (int j=0; j<gridWidth; j++) {
                        output += $"{{\"gridPos\":[{j},{i}],\"prox\":0,\"temp\":25.12999916,\"alt\":-48.32490158,\"hum\":19.31999969,\"orient\":[358.5625,-0.1875,-4.5625]}}" + ",";
                    }
                }
                break;
        }
        return output.Substring(0, output.Length - 1) + "]}";
    }

    public string nextReading() {
        if (mockType == MockReadingType.ARRAY) {
            return initReading;
        }
        return generateReading();
    }

}
