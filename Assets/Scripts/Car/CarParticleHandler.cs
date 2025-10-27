using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarParticleHandler : MonoBehaviour
{
    [Header("Systems")]
    public CarAssembly carAssembly;
    public CarInteractables interactables;
    public CarController controller;

    [Header("Particles")]
    [Space(20)]

    [Header("InstallationParticles")]
    public ParticleSystem batteryInstallationParticle;
    public ParticleSystem steeringWheelInstallationParticle;

}
