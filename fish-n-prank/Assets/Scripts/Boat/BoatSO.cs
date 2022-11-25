using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Configs/BoatSO")]
public class BoatSO : ScriptableObject
{
    [BoxGroup("Buoyancy")] public float m_floatingPower;
    [BoxGroup("Buoyancy")] public float m_waterHeight;


    [BoxGroup("Physics")] public float m_mass;
    [BoxGroup("Physics")] public float m_underWaterDragForce;
    [BoxGroup("Physics")] public float m_underWaterAngularDragForce;
    [BoxGroup("Physics")] public float m_airDragForce;
    [BoxGroup("Physics")] public float m_airAngularDrag;

    [BoxGroup("Boat Infos")] public float m_steermPower;
    [BoxGroup("Boat Infos")] public float m_power;
    [BoxGroup("Boat Infos")] public float m_maxSpeed;
    [BoxGroup("Boat Infos")] public float m_reverseSpeed;
    [BoxGroup("Boat Infos")] public float m_drag;
    [BoxGroup("Boat Infos")] public float m_steerPower;
}
