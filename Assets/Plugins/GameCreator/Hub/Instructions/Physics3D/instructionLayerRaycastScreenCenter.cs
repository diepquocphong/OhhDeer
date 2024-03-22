using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;


[Version(1, 0, 0)]

[Title("Layered Raycast to Screen Center")]
[Description("Gets the worldspace position of the cursor as a Vector3 with a layermask.")]

[Category("Physics 3D/Layered Raycast Screen Center")]

[Parameter("LayerMask", "The LayerMask included in the raycast")]
[Parameter("Position", "The variable to store the Vector3 in")]

[Keywords("Ray", "Raycast", "Store", "Position")]
[Image(typeof(IconVector3), ColorTheme.Type.Green)]

[Serializable]
public class instructionLayerRaycastScreenCenter : Instruction
{

    [SerializeField] public LayerMask m_layerMask = -1;
    [SerializeField] public PropertySetVector3 m_Position = new PropertySetVector3();

    Camera m_ray;
    RaycastHit m_hitData;
    Vector3 m_worldPosition;
    protected override Task Run(Args args)
    {
        {
            m_ray = Camera.main;
            if (Physics.Raycast(m_ray.transform.position, m_ray.transform.forward, out m_hitData, 1000, m_layerMask))
            {

                m_worldPosition = m_hitData.point;
                m_Position.Set(m_worldPosition, args);

            }
        }
        return DefaultResult;
    }
}
