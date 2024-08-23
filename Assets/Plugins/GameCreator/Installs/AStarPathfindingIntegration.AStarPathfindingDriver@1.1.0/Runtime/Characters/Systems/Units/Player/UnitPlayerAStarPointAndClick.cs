using System;
using System.Collections.Generic;
using GameCreator.Runtime.Characters;
using UnityEngine;
using UnityEngine.InputSystem;
using GameCreator.Runtime.Common;

namespace Arawn.Runtime.AStarPathfinding
{
	[Title("A* LocalSpace Point & Click")]
	[Image(typeof(IconLocationDrop), ColorTheme.Type.Purple)]

	[Category("A* LocalSpace Point & Click")]
	[Description(
		"Moves the Player while Platforming where the pointer's position clicks from the Main Camera's perspective"
	)]

	[Serializable]
	public class UnitPlayerAStartPointAndClick : TUnitPlayer
	{
		private const int BUFFER_SIZE = 32;

		// RAYCAST COMPARER: ----------------------------------------------------------------------

		private static readonly RaycastComparer RAYCAST_COMPARER = new RaycastComparer();

		private class RaycastComparer : IComparer<RaycastHit>
		{
			public int Compare(RaycastHit a, RaycastHit b)
			{
				return a.distance.CompareTo(b.distance);
			}
		}

		// EXPOSED MEMBERS: -----------------------------------------------------------------------

		[SerializeField]
		private InputPropertyButton m_InputMove;

		[SerializeField]
		private LayerMask m_LayerMask;

		[SerializeField]
		private PropertyGetInstantiate m_Indicator;

		// MEMBERS: -------------------------------------------------------------------------------

		[NonSerialized] private RaycastHit[] m_HitBuffer;

		[NonSerialized] private bool m_Press;
		[NonSerialized] private Location m_Location;

		[NonSerialized] private Vector3 m_RelativePositionOnPlatform;
		[NonSerialized] private Transform m_ClickedPlatform;

		[NonSerialized] private string m_PlatformTag = "Platform";

		// INITIALIZERS: --------------------------------------------------------------------------

		public UnitPlayerAStartPointAndClick()
		{
			this.m_LayerMask = -1;
			this.m_Indicator = new PropertyGetInstantiate
			{
				usePooling = true,
				size = 5,
				hasDuration = true,
				duration = 1f
			};

			this.m_InputMove = InputButtonMousePress.Create();
		}

		public override void OnStartup(Character character)
		{
			base.OnStartup(character);
			this.m_InputMove.OnStartup();
			IUnitDriver driver = character.Driver;

			if (driver is UnitDriverLocalSpaceRichAI platformingDriver)
			{
				m_PlatformTag = platformingDriver.m_PlatformTag;
			}
			else
			{
				//Debug.LogError("Driver is not set to Platforming Driver");
				this.m_PlatformTag = "Platform";
			}
		}

		public override void OnDispose(Character character)
		{
			base.OnDispose(character);
			this.m_InputMove.OnDispose();
		}

		public override void OnEnable()
		{
			base.OnEnable();

			this.m_HitBuffer = new RaycastHit[BUFFER_SIZE];

			this.m_InputMove.RegisterStart(this.OnStartPointClick);
			this.m_InputMove.RegisterPerform(this.OnPerformPointClick);
		}

		public override void OnDisable()
		{
			base.OnDisable();
			this.m_HitBuffer = Array.Empty<RaycastHit>();

			this.m_InputMove.ForgetStart(this.OnStartPointClick);
			this.m_InputMove.ForgetPerform(this.OnPerformPointClick);

			this.Character.Motion?.MoveToDirection(Vector3.zero, Space.World, 0);
		}

		// UPDATE METHODS: ------------------------------------------------------------------------

		public override void OnUpdate()
		{
			base.OnUpdate();
			this.m_InputMove.OnUpdate();

			if (!this.m_Location.HasPosition(this.Character.gameObject)) return;

			GameObject user = this.Character.gameObject;
			Vector3 position;

			// If the click was on a moving platform, adjust the target position
			if (m_ClickedPlatform != null)
			{
				position = m_ClickedPlatform.TransformPoint(m_RelativePositionOnPlatform);
				this.m_Location = new Location(position);  // Update the target location.

				// Calculate the actual target position based on the moving platform
				Vector3 targetPosition = m_ClickedPlatform.TransformPoint(m_RelativePositionOnPlatform);

				// Check the distance to the target position
				float distanceToTarget = Vector3.Distance(this.Character.transform.position, targetPosition);

				if (distanceToTarget < 0.05f)  // Adjust the threshold as needed
				{
					return;  // Stop the rest of the update to prevent further movements
				}
			}
			else
			{
				position = this.m_Location.GetPosition(user);
			}

#if UNITY_EDITOR
			Debug.DrawLine(position, position + Vector3.up * 5f);
#endif

			Vector3 directionToTarget = (position - this.Character.transform.position).normalized;
			float speed = this.Character.Motion?.LinearSpeed ?? 0f;

			this.Character.Motion?.MoveToLocation(this.m_Location, 0.1f, null, 0);

			if (this.m_Press && this.m_Location.HasPosition(this.Character.gameObject))
			{
				this.m_Indicator.Get(
					this.Character.gameObject,
					position,
					Quaternion.identity
				);
			}

			this.m_Press = false;
		}


		// PRIVATE METHODS: -----------------------------------------------------------------------

		private void OnStartPointClick()
		{
			if (!this.Character.IsPlayer) return;
			if (!this.Character.Player.IsControllable) return;

			this.m_Press = true;
		}

		private void OnPerformPointClick()
		{
			if (!this.Character.IsPlayer) return;
			if (!this.m_IsControllable) return;

			Camera camera = ShortcutMainCamera.Get<Camera>();

			Ray ray = camera.ScreenPointToRay(Application.isMobilePlatform
				? Touchscreen.current.primaryTouch.position.ReadValue()
				: Mouse.current.position.ReadValue()
			);

			int hitCount = Physics.RaycastNonAlloc(
				ray, this.m_HitBuffer,
				Mathf.Infinity, this.m_LayerMask,
				QueryTriggerInteraction.Ignore
			);

			Array.Sort(this.m_HitBuffer, 0, hitCount, RAYCAST_COMPARER);

			m_ClickedPlatform = null;  // Reset by default

			for (int i = 0; i < hitCount; ++i)
			{
				int colliderLayer = this.m_HitBuffer[i].transform.gameObject.layer;
				if ((colliderLayer & LAYER_UI) > 0) return;

				if (this.m_HitBuffer[i].transform.IsChildOf(this.Transform)) continue;

				Vector3 point = this.m_HitBuffer[i].point;

				// Check if the clicked object is a moving platform.
				if (this.m_HitBuffer[i].collider.CompareTag(m_PlatformTag))
				{
					m_ClickedPlatform = this.m_HitBuffer[i].transform;
					m_RelativePositionOnPlatform = m_ClickedPlatform.InverseTransformPoint(point);
					//Debug.Log("Clicked on a moving platform at relative position: " + m_RelativePositionOnPlatform);
				}
				else
				{
					//Debug.Log("Clicked on a non-platform object.");
				}

				this.m_Location = new Location(point);
				this.InputDirection = Vector3.Scale(point - this.Character.transform.position, Vector3Plane.NormalUp);

				return;
			}
		}


		// STRING: --------------------------------------------------------------------------------

		public override string ToString() => "A* LocalSpace Point & Click";
	}
}