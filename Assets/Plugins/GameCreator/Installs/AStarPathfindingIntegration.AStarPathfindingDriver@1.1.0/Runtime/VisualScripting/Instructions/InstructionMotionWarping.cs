using System;
using System.Threading.Tasks;
using Arawn.Runtime.AStarPathfinding;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.VisualScripting;
using UnityEngine;

namespace GameCreator.Runtime.Melee
{
	[Version(1, 0, 0)]

	[Title("Motion Warping")]
	[Description("Applies motion warping to a character based on curves over the duration of an animation clip.")]

	[Category("Characters/Navigation/Motion Warping")]

	[Parameter("Transform", "The Transform of the game object to apply motion warping.")]
	[Parameter("Animation Clip", "The animation clip you want to warp.")]
	[Parameter("X Curve", "Curve to adjust the X position over the animation duration.")]
	[Parameter("Y Curve", "Curve to adjust the Y position over the animation duration.")]
	[Parameter("Z Curve", "Curve to adjust the Z position over the animation duration.")]
	[Parameter("Space Option", "Apply the motion warping in World or Self space.")]
	[Parameter("Effect Only AStar Pathfinding Driver", "Apply this only for characters with AStar Pathfinding Driver")]

	[Keywords("Motion", "Warping", "Animation", "Curve", "Transform")]
	[Image(typeof(IconWheel), ColorTheme.Type.Green)]  // You might want to change this to a more appropriate icon if available.

	[Serializable]
	public class InstructionMotionWarping : TInstructionTransform
	{
		[Tooltip("The animation clip you want to warp.")]
		[SerializeField] private AnimationClip targetAnimationClip;

		[SerializeField] private PropertyGetInteger m_Power = new PropertyGetInteger(1);

		[Tooltip("Curve to adjust the X position over the animation duration.")]
		[SerializeField] private AnimationCurve xCurve = AnimationCurve.Linear(0, 0, 1, 0);

		[Tooltip("Curve to adjust the Y position over the animation duration.")]
		[SerializeField] private AnimationCurve yCurve = AnimationCurve.Linear(0, 0, 1, 0);

		[Tooltip("Curve to adjust the Z position over the animation duration.")]
		[SerializeField] private AnimationCurve zCurve = AnimationCurve.Linear(0, 0, 1, 0);

		[Tooltip("Apply the motion warping in World or Self space.")]
		[SerializeField] private Space spaceOption = Space.Self;

		[Tooltip("Layers that represent objects for collision.")]
		[SerializeField] private LayerMask collisionLayers = 1 << 0;

		[Tooltip("Effect only for characters with AStar Pathfinding Driver.")]
		[SerializeField] private PropertyGetBool m_AffectsOnlyAStarPathfindingDriver = new PropertyGetBool();

		private UnitDriver m_Driver = new UnitDriver(new UnitDriverAStarPath());

		protected override async Task Run(Args args)
		{
			int power = (int) this.m_Power.Get(args);
			bool effectOnlyAStarPathfindingDriver = m_AffectsOnlyAStarPathfindingDriver.Get(args);
			GameObject gameObject = this.m_Transform.Get(args);
			if (gameObject == null) return;

			Character character = gameObject.GetComponent<Character>();
			if (character == null) return;

			// Check if the driver type matches
			if (effectOnlyAStarPathfindingDriver)
			{
				if (m_Driver.Wrapper.GetType() != character.Driver.GetType()) return;
			}

			CollisionHandler collisionHandler = gameObject.GetComponent<CollisionHandler>();
			if (collisionHandler == null)
			{
				collisionHandler = gameObject.AddComponent<CollisionHandler>();
				collisionHandler.Setup(collisionLayers);
			}

			float clipDuration = targetAnimationClip.length;
			float startTime = UnityEngine.Time.time;

			while (UnityEngine.Time.time - startTime < clipDuration)
			{
				float normalizedTime = (UnityEngine.Time.time - startTime) / clipDuration;

				Vector3 warpingOffset = new Vector3(
					xCurve.Evaluate(normalizedTime),
					yCurve.Evaluate(normalizedTime),
					zCurve.Evaluate(normalizedTime)
				);

				if (collisionHandler.HasCollided)
				{
					// Handle collision behavior here if needed
					collisionHandler.ResetCollision();
				}
				else
				{
					if (spaceOption == Space.World)
					{
						gameObject.transform.position += warpingOffset * (UnityEngine.Time.deltaTime * power);
					}
					else
					{
						gameObject.transform.localPosition += gameObject.transform.rotation * warpingOffset * (UnityEngine.Time.deltaTime * power); 
					}
				}

				await NextFrame();
			}
		}

		private class CollisionHandler : MonoBehaviour
		{
			private LayerMask collisionLayers;
			public bool HasCollided { get; private set; }
			public Vector3 CollisionPoint { get; private set; }

			public void Setup(LayerMask collisionLayers)
			{
				this.collisionLayers = collisionLayers;
			}

			void OnCollisionEnter(Collision collision)
			{
				if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
				{
					HasCollided = true;
					CollisionPoint = collision.contacts[0].point;
				}
			}

			public void ResetCollision()
			{
				HasCollided = false;
				CollisionPoint = Vector3.zero;
			}
		}
	}
}