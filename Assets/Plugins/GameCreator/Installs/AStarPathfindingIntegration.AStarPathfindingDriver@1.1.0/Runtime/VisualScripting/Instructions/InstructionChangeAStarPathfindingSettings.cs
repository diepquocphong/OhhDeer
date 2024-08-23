using System;
using System.Threading.Tasks;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Characters;
using Pathfinding;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting.Arawn
{
    [Version(1, 0, 1)]
    [Title("Change AIPath Settings")]
    [Description("Changes the settings of the AIPath component for a character.")]
    [Image(typeof(IconCharacterWalk), ColorTheme.Type.White)]
    [Category("A* Pathfinding/AI Path Settings")]

    [Parameter("Can Move", "Enable or disable character movement.")]
    [Parameter("Always Draw Gizmos", "Enable or disable drawing gizmos for the AIPath component in the editor.")]
    [Parameter("Pick Next Waypoint Dist", "Set the distance to the next waypoint before the character starts moving toward it.")]
    [Parameter("Auto Repath Maximum Period", "Set the maximum period for automatic repathing.")]
    [Parameter("Auto Repath Sensitivity", "Set the sensitivity for automatic repathing.")]
    [Parameter("Orientation Mode", "Set the orientation mode for the character. 2D vs 3D.")]
    [Parameter("Enable Rotation", "Enable or disable rotation for the character.")]
    [Parameter("Slowdown Distance", "Set the distance from the target at which the character starts to slow down.")]
    [Parameter("End Reached Distance", "Set the distance from the target considered as \"reached\".")]
    [Parameter("When Close To Destination", "Determine what the character does when close to the destination (e.g., stop or continue to the next waypoint).")]
    [Parameter("Constrain Inside Graph", "Enable or disable constraining the character inside the navigation graph.")]
    [Parameter("Start End Modifier", "Modify the start and end points of the generated path.")]

    [Parameter("Character", "The game object with a GC2 Character component")]

    [Keywords("A* Pathfinding", "AI", "Settings", "Seeker", "AIPath")]

    [Serializable]
    public class InstructionChangeAStarPathfindingSettings : Instruction
    {
        [SerializeField] private PropertyGetGameObject m_Character = GetGameObjectPlayer.Create();

        [SerializeField] private bool ChangeCanMove = false;
        [SerializeField] private bool m_CanMove;

        [SerializeField] private bool ChangeAlwaysDrawGizmos = false;
        [SerializeField] private bool m_AlwaysDrawGizmos;

        [SerializeField] private bool ChangePickNextWaypointDist = false;
        [SerializeField] private float m_PickNextWaypointDist;

        [SerializeField] private bool ChangeMaximumPeriod = false;
        [SerializeField] private float m_AutoRepathMaximumPeriod;

        [SerializeField] private bool ChangeSensitivity = false;
        [Range(1, 20)]
        [SerializeField] private float m_AutoRepathSensitivity;

        [SerializeField] private bool ChangeOrientationMode = false;
        [SerializeField] private OrientationMode m_OrientationMode;

        [SerializeField] private bool ChangeEnableRotation = false;
        [SerializeField] private bool m_EnableRotation;

        [SerializeField] private bool ChangeSlowDownDistance = false;
        [SerializeField] private float m_SlowdownDistance;

        [SerializeField] private bool ChangeEndReachedDistance = false;
        [SerializeField] private float m_EndReachedDistance;

        [SerializeField] private bool ChangeWhenCloseToDestination = false;
        [SerializeField] private CloseToDestinationMode m_WhenCloseToDestination;

        [SerializeField] private bool ChangeConstrainInsideGraph = false;
        [SerializeField] private bool m_ConstrainInsideGraph;

        [SerializeField] private bool ChangeStartEndModifier = false;
        [SerializeField] private StartEndModifier m_StartEndModifier;

        protected override Task Run(Args args)
        {

            GameObject characterGameObject = this.m_Character.Get(args);
            AIPath aiPath = characterGameObject.GetComponent<AIPath>();
            Seeker seeker = characterGameObject.GetComponent<Seeker>();


            if (ChangeCanMove)
            {
                aiPath.canMove = this.m_CanMove;
            }

            if (ChangeAlwaysDrawGizmos)
            {
                aiPath.alwaysDrawGizmos = this.m_AlwaysDrawGizmos;
            }

            if (ChangePickNextWaypointDist)
            {
                aiPath.pickNextWaypointDist = this.m_PickNextWaypointDist;
            }

            if (ChangeMaximumPeriod)
            {
                aiPath.autoRepath.maximumPeriod = this.m_AutoRepathMaximumPeriod;
            }

            if (ChangeSensitivity)
            {
                aiPath.autoRepath.sensitivity = this.m_AutoRepathSensitivity;
            }

            if (ChangeOrientationMode)
            {
                aiPath.orientation = this.m_OrientationMode;
            }

            if (ChangeEnableRotation)
            {
                aiPath.enableRotation = this.m_EnableRotation;
            }

            if (ChangeSlowDownDistance)
            {
                aiPath.slowdownDistance = this.m_SlowdownDistance;
            }

            if (ChangeEndReachedDistance)
            {
                aiPath.endReachedDistance = this.m_EndReachedDistance;
            }

            if (ChangeWhenCloseToDestination)
            {
                aiPath.whenCloseToDestination = this.m_WhenCloseToDestination;
            }

            if (ChangeConstrainInsideGraph)
            {
                aiPath.constrainInsideGraph = this.m_ConstrainInsideGraph;
            }

            if (ChangeStartEndModifier)
            {
                seeker.startEndModifier = this.m_StartEndModifier;
            }

            return DefaultResult;
        }
    }
}