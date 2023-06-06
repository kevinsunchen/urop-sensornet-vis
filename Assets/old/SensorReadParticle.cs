using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SensorReadParticle : MonoBehaviour
{
    SerialPort sp;

    static int NUM_SENSORS = 4;

    public GameObject sensorObjPrefab;
    public static GameObject[] sensorObjList;

    public ParticleSystemForceFieldShape m_Shape = ParticleSystemForceFieldShape.Box;
    public const float PS_BOX_SCALE = 4.0f; 
    public float m_StartRange = 0.0f;
    public float m_EndRange = 3.0f;
    public Vector3 m_Direction = Vector3.zero;
    public float m_Gravity = 0.0f;
    public float m_GravityFocus = 0.0f;
    public float m_RotationSpeed = 0.0f;
    public float m_RotationAttraction = 0.0f;
    public Vector2 m_RotationRandomness = Vector2.zero;
    public float m_Drag = 0.0f;
    public bool m_MultiplyDragByParticleSize = false;
    public bool m_MultiplyDragByParticleVelocity = false;

    private ParticleSystemForceField m_ForceField;

    void Start()
    {
        // Create a Force Field
        var go = new GameObject("ForceField", typeof(ParticleSystemForceField));
        // go.transform.position = new Vector3(0, 2, 0);
        // go.transform.rotation = Quaternion.Euler(new Vector3(90.0f, 0.0f, 0.0f));

        m_ForceField = go.GetComponent<ParticleSystemForceField>();

        // Configure Particle System
        // transform.position = new Vector3(0, -4, 0);
        // transform.rotation = Quaternion.identity;
        var ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.duration = 10.0f;
        main.startLifetime = float.PositiveInfinity;
        main.startSize = new ParticleSystem.MinMaxCurve(0.1f, 0.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(-0.1f, -0.03f);
        main.maxParticles = 1000;

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = 0.5f;
        noise.quality = ParticleSystemNoiseQuality.High;

        var emission = ps.emission;
        emission.enabled = false;
        // emission.rateOverTime = 0.0f;
        // emission.burstCount = 1;
        // emission.SetBurst(0, new ParticleSystem.Burst(0.0f, 200, 200, -1, 0.1f));

        var emitParams = new ParticleSystem.EmitParams();
        ps.Emit(emitParams, 500);

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(PS_BOX_SCALE, PS_BOX_SCALE, PS_BOX_SCALE);
        shape.randomPositionAmount = 0.3f;
        shape.randomDirectionAmount = 0.1f;

        generatePSColliders(ps);

        var forces = ps.externalForces;
        forces.enabled = true;

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
            sp.ReadTimeout = 5000;
            sp.DtrEnable = true;
            sp.Handshake = Handshake.None;
            if (sp.IsOpen) { print("Open"); }
        }

        sensorObjList = new GameObject[NUM_SENSORS];
    }

    void Update()
    {
        m_ForceField.shape = m_Shape;
        m_ForceField.startRange = m_StartRange;
        m_ForceField.endRange = m_EndRange;
        // m_ForceField.directionX = m_Direction.x;
        // m_ForceField.directionY = m_Direction.y;
        // m_ForceField.directionZ = m_Direction.z;
        m_ForceField.gravity = m_Gravity;
        // m_ForceField.gravityFocus = m_GravityFocus;
        // m_ForceField.rotationSpeed = m_RotationSpeed;
        // m_ForceField.rotationAttraction = m_RotationAttraction;
        // m_ForceField.rotationRandomness = m_RotationRandomness;
        // m_ForceField.drag = m_Drag;
        // m_ForceField.multiplyDragByParticleSize = m_MultiplyDragByParticleSize;
        // m_ForceField.multiplyDragByParticleVelocity = m_MultiplyDragByParticleVelocity;
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

                // if (sensorObjList[0] == null) {
                //     // TODO
                //     GameObject obj = Instantiate(sensorObjPrefab);
                //     sensorObjList[0] = obj;
                // }

                // GameObject sensorObj = sensorObjList[0];
                // tempEffects(sensorObj, dataObj.temp);
                // proxEffects(sensorObj, dataObj.prox);
                // orientEffects(sensorObj, dataObj.orient);

                m_ForceField.directionY = -(dataObj.prox*3)/255.0f;

            } 
            catch (System.TimeoutException) { 
                print("timeout ------------------------------------------------------");
            } 
            catch (System.FormatException) {
                print("Input was not formatted correctly: " + reading);
            }
        }
    }

    void generatePSColliders(ParticleSystem ps) {
        var rotations = new Dictionary<string, Vector3>{
            {"TOP", new Vector3(180.0f, 0.0f, 0.0f)},
            {"BOTTOM", new Vector3(0.0f, 0.0f, 0.0f)},
            {"FRONT", new Vector3(90.0f, 0.0f, 0.0f)},
            {"BACK", new Vector3(-90.0f, 0.0f, 0.0f)},
            {"LEFT", new Vector3(-90.0f, -90.0f, 0.0f)},
            {"RIGHT", new Vector3(-90.0f, 90.0f, 0.0f)}
        };
        var positions = new Dictionary<string, Vector3>{
            {"TOP", new Vector3(0.0f, PS_BOX_SCALE/2.0f, 0.0f)},
            {"BOTTOM", new Vector3(0.0f, -PS_BOX_SCALE/2.0f, 0.0f)},
            {"FRONT", new Vector3(0.0f, 0.0f, -PS_BOX_SCALE/2.0f)},
            {"BACK", new Vector3(0.0f, 0.0f, PS_BOX_SCALE/2.0f)},
            {"LEFT", new Vector3(-PS_BOX_SCALE/2.0f, 0.0f, 0.0f)},
            {"RIGHT", new Vector3(PS_BOX_SCALE/2.0f, 0.0f, 0.0f)}
        };

        foreach (KeyValuePair<string, Vector3> kvp in rotations) {
            var collision = ps.collision;
            collision.enabled = true;
            collision.type = ParticleSystemCollisionType.Planes;
            collision.mode = ParticleSystemCollisionMode.Collision3D;
            collision.bounceMultiplier = 0.5f;
            collision.radiusScale = 5;

            var collider = GameObject.CreatePrimitive(PrimitiveType.Plane);
            collider.transform.parent = ps.transform;
            collider.transform.localScale = new Vector3(PS_BOX_SCALE/10.0f, PS_BOX_SCALE/10.0f, PS_BOX_SCALE/10.0f);
            collider.transform.localPosition = positions[kvp.Key];
            collider.transform.localRotation = Quaternion.Euler(kvp.Value);

            collision.AddPlane(collider.transform);
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
