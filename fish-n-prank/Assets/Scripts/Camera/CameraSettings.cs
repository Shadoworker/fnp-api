using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Configs/CameraSO")]
public class CameraSettings : ScriptableObject
{

    [FancyHeader("  Camera Settings SO  ", 1.5f, "yellow", 5.5f, order = 0)]
    [Label("")] public Empty e;

    [BoxGroup("Joystick settings")] public float m_sensivityX = 4.0f;
    [BoxGroup("Joystick settings")] public float m_sensivityY = 1.0f;
}
