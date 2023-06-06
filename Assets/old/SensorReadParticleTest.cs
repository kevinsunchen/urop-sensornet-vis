using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class SensorReadParticleTest : MonoBehaviour
{
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
        main.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(-0.1f, -0.03f);
        main.maxParticles = 5000;

        var emission = ps.emission;
        emission.rateOverTime = 50f;
        // emission.burstCount = 1;
        // emission.SetBurst(0, new ParticleSystem.Burst(0.0f, 200, 200, -1, 0.1f));

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(PS_BOX_SCALE, PS_BOX_SCALE, PS_BOX_SCALE);
        shape.randomPositionAmount = 0.3f;
        shape.randomDirectionAmount = 0.1f;

        generatePSColliders(ps);

        var forces = ps.externalForces;
        forces.enabled = true;
    }

    void Update()
    {
        m_ForceField.shape = m_Shape;
        m_ForceField.startRange = m_StartRange;
        m_ForceField.endRange = m_EndRange;
        m_ForceField.directionX = m_Direction.x;
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

            var collider = GameObject.CreatePrimitive(PrimitiveType.Plane);
            collider.transform.parent = ps.transform;
            collider.transform.localScale = new Vector3(PS_BOX_SCALE/10.0f, PS_BOX_SCALE/10.0f, PS_BOX_SCALE/10.0f);
            collider.transform.localPosition = positions[kvp.Key];
            collider.transform.localRotation = Quaternion.Euler(kvp.Value);

            collision.AddPlane(collider.transform);
        }

    }
}
