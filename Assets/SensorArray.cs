using System.Collections;
using static MockReadingGenerator;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

public class SensorArray : MonoBehaviour
{
    SerialPort sp;

    static int GRID_WIDTH = 4;
    static int GRID_HEIGHT = 4;
    static float GRID_X_DIST = 5.0f;
    static float GRID_Y_DIST = 5.0f;
    
    public SensorNode sensorNodePrefab;
    // public Dictionary<float[], SensorDataObj> sensorGrid;
    public Dictionary<Point, SensorNode> sensorGridGameObjs;
    SensorReadStream sensorReader;
    MockReadingGenerator mockReader;
    GameObject marker1;
    GameObject marker2;
    string arrayMockData1 = "";
    string arrayMockData2 = "";
    
    // Start is called before the first frame update
    void Start() {
        sensorGridGameObjs = new Dictionary<Point, SensorNode>();
        //sensorReader = new SensorReadStream();
        //mockReader = new MockReadingGenerator(MockReadingType.ARRAY, GRID_WIDTH, GRID_HEIGHT, sensorReader);
        marker1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        for (int i=0; i<GRID_HEIGHT; i++) {
            for (int j=0; j<GRID_WIDTH; j++) {
                //float yaw = Random.Range(0.0f,180.0f);
                float yaw = 0.0f;
                float pitch = Random.Range(-45.0f,45.0f);
                float roll = Random.Range(-30.0f,30.0f);
                arrayMockData1 += $"{{\"gridPos\":[{j},{i}],\"prox\":0,\"temp\":25.12999916,\"alt\":-48.32490158,\"hum\":19.31999969,\"orient\":[{yaw},{roll},{pitch}]}}" + ",";
            }
        }

        float[] pitchArray = new float[GRID_WIDTH];
        for (int j=0; j<GRID_WIDTH; j++) {
            pitchArray[j] = Random.Range(-180.0f,180.0f);
        }
        
        for (int i=0; i<GRID_HEIGHT; i++) {
            float roll = Random.Range(-90.0f,90.0f);
            for (int j=0; j<GRID_WIDTH; j++) {
                float pitch = pitchArray[j];
                arrayMockData2 += $"{{\"gridPos\":[{j},{i}],\"prox\":0,\"temp\":25.12999916,\"alt\":-48.32490158,\"hum\":19.31999969,\"orient\":[{0},{roll},{0}]}}" + ",";
            }
        }
    }

    // Update is called once per frame
    void Update() {
        // use mock reading (indicate parameter withSensor)
        // string reading = mockReader.nextReading();
        string reading = generateMockReading(MockReadingType.ARRAY);

        SensorArrayDataObj arrayDataObj = JsonUtility.FromJson<SensorArrayDataObj>(reading);
        // print(arrayDataObj);
        
        // Use wrapper obj around GameObject and update the GameObject via wrapper obj methods?
        updateGridGameObjs(arrayDataObj);
        //updateObjPositions();
        bfs();
    }

    // Generate mock readings in lieu of direct sensor data
    private string generateMockReading(MockReadingType mockType) {
        string output = "{\"sensorArray\":[";
        switch (mockType) {
            case MockReadingType.SENSOR:
                string reading = sensorReader.read();
                for (int i=0; i<GRID_HEIGHT; i++) {
                    for (int j=0; j<GRID_WIDTH; j++) {
                        // TODO: make this less janky, maybe add some regex instead of just hard coded substring
                        output += $"{{\"gridPos\":[{j},{i}]" + reading.Substring(16) + ",";
                    }
                }
                break;
            case MockReadingType.ARRAY:
                // output += "{"gridPos":[0,0],"prox":0,"temp":20.93999863,"alt":-70.96392059,"hum":54.25,"orient":[4.3125,36.875,-1.625]}";
                // for (int i=0; i<GRID_HEIGHT; i++) {
                //     for (int j=0; j<GRID_WIDTH; j++) {
                //         float yaw = Random.Range(0.0f,360.0f);
                //         float pitch = Random.Range(-180.0f,180.0f);
                //         float roll = Random.Range(-90.0f,90.0f);
                //         output += $"{{\"gridPos\":[{j},{i}],\"prox\":0,\"temp\":25.12999916,\"alt\":-48.32490158,\"hum\":19.31999969,\"orient\":[{yaw},{roll},{pitch}]}}" + ",";
                //     }
                // }
                output += arrayMockData1;
                break;
            default:
            case MockReadingType.SINGLE_HARDCODED:  
                for (int i=0; i<GRID_HEIGHT; i++) {
                    for (int j=0; j<GRID_WIDTH; j++) {
                        output += $"{{\"gridPos\":[{j},{i}],\"prox\":0,\"temp\":25.12999916,\"alt\":-48.32490158,\"hum\":19.31999969,\"orient\":[358.5625,-0.1875,-4.5625]}}" + ",";
                    }
                }
                break;
        }
        return output.Substring(0, output.Length - 1) + "]}";
    }

    // Update GameObjs stored in dictionary (or create them if they don't exist)
    private void updateGridGameObjs(SensorArrayDataObj arrayDataObj) {
        foreach (SensorDataObj dataObj in arrayDataObj.sensorArray) {
            Point gridPos = new Point(dataObj.gridPos);
            //print((int)dataObj.orient[0] + ", " + (int)dataObj.orient[1] + ", " + (int)dataObj.orient[2]);

            // TODO: if sensorGridGameObjs dictionary does not contain a GameObj for this gridPos, create a new GameObj with the data from dataObj
            if (!sensorGridGameObjs.ContainsKey(gridPos)) {
                var sensorNodePosition = new Vector3(gridPos.x*GRID_X_DIST, 0, gridPos.y*GRID_Y_DIST);
                SensorNode sensorNode = Instantiate(sensorNodePrefab, sensorNodePosition, this.transform.rotation, this.transform);
                sensorNode.setData(dataObj);
                sensorGridGameObjs[gridPos] = sensorNode;
            }
            // TODO: otherwise, update the existing GameObj with the data from dataObj
            else {
                sensorGridGameObjs[gridPos].setData(dataObj);
                // TODO:
            }
        }
    }

    private void bfs() {
        List<List<Point>> level = new List<List<Point>>{new List<Point>{new Point(0,0)}};
        HashSet<Point> seen = new HashSet<Point>{new Point(0,0)};
        while (level[^1].Count > 0) {
            level.Add(new List<Point>());
            foreach (Point s in level[^2]) {
                print("s: " + s);
                Point right = new Point(s.x + 1, s.y);
                Point up = new Point(s.x, s.y + 1);
                
                if (!seen.Contains(right) && sensorGridGameObjs.ContainsKey(right)) {
                    level[^1].Add(right);
                    seen.Add(right);
                    print("right: " + right);
                    
                    Vector3 offset_s_right = offsetFromAngle(sensorGridGameObjs[s].transform.rotation.eulerAngles, "x");
                    Vector3 offset_right_s = offsetFromAngle(sensorGridGameObjs[right].transform.rotation.eulerAngles, "x");
                    sensorGridGameObjs[right].transform.position = sensorGridGameObjs[s].transform.position + offset_s_right + offset_right_s;
                }
                if (!seen.Contains(up) && sensorGridGameObjs.ContainsKey(up)) {
                    level[^1].Add(up);
                    seen.Add(up);
                    print("up: " + up);

                    Vector3 offset_s_up = offsetFromAngle(sensorGridGameObjs[s].transform.rotation.eulerAngles, "z");
                    Vector3 offset_up_s = offsetFromAngle(sensorGridGameObjs[up].transform.rotation.eulerAngles, "z");
                    sensorGridGameObjs[up].transform.position = sensorGridGameObjs[s].transform.position + offset_s_up + offset_up_s;
                }

            }
        }
    }

    private void updateObjPositions() {
        Vector3 rot_0_0 = sensorGridGameObjs[new Point(0, 0)].transform.rotation.eulerAngles;
        Vector3 offset_0_0_x = offsetFromAngle(rot_0_0, "x");
        marker1.transform.position = sensorGridGameObjs[new Point(0, 0)].transform.position + offset_0_0_x;

        Vector3 rot_1_0 = sensorGridGameObjs[new Point(1, 0)].transform.rotation.eulerAngles;
        Vector3 offset_1_0_x = offsetFromAngle(rot_1_0, "x");
        sensorGridGameObjs[new Point(1, 0)].transform.position = sensorGridGameObjs[new Point(0, 0)].transform.position + offset_0_0_x + offset_1_0_x;

        Vector3 offset_0_0_z = offsetFromAngle(rot_0_0, "z");
        marker2.transform.position = sensorGridGameObjs[new Point(0, 0)].transform.position + offset_0_0_z;

        Vector3 rot_0_1 = sensorGridGameObjs[new Point(0, 1)].transform.rotation.eulerAngles;
        Vector3 offset_0_1_z = offsetFromAngle(rot_0_1, "z");
        sensorGridGameObjs[new Point(0, 1)].transform.position = sensorGridGameObjs[new Point(0, 0)].transform.position + offset_0_0_z + offset_0_1_z;
    }

    /**
     * angle in degrees
     */
    private Vector3 offsetFromAngle(Vector3 rot, string axis) {
        float sideLen;
        float angle;
        if (axis.Equals("x")) {
            sideLen = GRID_X_DIST;
            angle = (rot[2]*Mathf.PI)/180.0f;
        } else {
            sideLen = GRID_Y_DIST;
            angle = (rot[0]*Mathf.PI)/180.0f;
        }
        float x_offset = Mathf.Cos(angle)*sideLen;
        float y_offset = Mathf.Sin(angle)*sideLen;
        if (axis.Equals("x")) {
            return new Vector3(x_offset, y_offset, 0);
        } else {
            return new Vector3(0, -y_offset, x_offset);
        }
    }




    // // TODO refine to return RGB tuple
    // private float scaleRawTemp(double rawTemp) {
    //     return (float) ((rawTemp - 24) / 5.0);
    // }

    // private void tempEffects(GameObject sensorObj, float temp) {
    //     var sensorRenderer = sensorObj.GetComponent<Renderer>();
    //     Color scaledTempColor = new Color(scaleRawTemp(temp),0,0,1);
    //     print(scaledTempColor);
    //     sensorRenderer.material.SetColor("_Color", scaledTempColor);
    // }

    // private void proxEffects(GameObject sensorObj, int prox) {
    //     float scaleChange1D = (float) (1.25 - (prox / 255.0));
    //     print(scaleChange1D);
    //     sensorObj.transform.localScale = new Vector3(scaleChange1D, scaleChange1D, scaleChange1D);
    // }

    // private void orientEffects(GameObject sensorObj, float[] orient) {
    //     Vector3 currPos = sensorObj.transform.position;
    //     Quaternion newRotation = Quaternion.Euler(orient[2], orient[0], orient[1]);
    //     sensorObj.transform.SetPositionAndRotation(currPos, newRotation);
    // }
}
