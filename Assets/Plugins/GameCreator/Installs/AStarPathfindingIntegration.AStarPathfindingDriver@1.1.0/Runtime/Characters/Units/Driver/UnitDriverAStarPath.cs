using System;
using UnityEngine;
using Pathfinding;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Characters;
using Transform = UnityEngine.Transform;

namespace Arawn.Runtime.AStarPathfinding
{
	[Title("A* AI Path Agent")]
	[Image(typeof(IconCharacterWalk), ColorTheme.Type.White, typeof(OverlayArrowRight))]
	[Category("A* AI Path Agent")]
	[Description(
		"Moves the Character using A* Pathfinding's AI Path Agent. " +
		"Requires a scene with an A* Pathfinder and Grid Graph"
	)]

	[Serializable]
	public class UnitDriverAStarPath : TUnitDriver
	{
		// EXPOSED MEMBERS: -----------------------------------------------------------------------
		[SerializeField] private float m_AStarHeight = 2f;
		[SerializeField] private float m_GroundcheckRaycastLength = 3.5f;
		[SerializeField] private bool m_CanMove = true;
		[SerializeField] private bool m_AlwaysDrawGizmos = true;
		[SerializeField] private float m_PickNextWaypointDist = 2f;
		[SerializeField] private float m_AutoRepathMaximumPeriod = 2f;
		[Range(1, 20)]
		[SerializeField] private float m_AutoRepathSensitivity = 10f;
		[SerializeField] private OrientationMode m_OrientationMode = OrientationMode.ZAxisForward;
		[SerializeField] private bool m_EnableRotation = true;
		[SerializeField] private float m_SlowdownDistance = 0.6f;
		[SerializeField] private float m_EndReachedDistance = 0.2f;
		[SerializeField] private CloseToDestinationMode m_WhenCloseToDestination = CloseToDestinationMode.Stop;
		[SerializeField] private bool m_ConstrainInsideGraph = false;
		[SerializeField] private LayerMask m_GroundLayerMask = ~0; // Default to all layers

		// MEMBERS: -------------------------------------------------------------------------------
		[NonSerialized] protected AIPath Agent;
		[NonSerialized] protected Seeker Seeker;
		[NonSerialized] protected Vector3 MoveDirection;
		[NonSerialized] private Vector3 m_Velocity = Vector3.zero;
		[NonSerialized] private Vector3 m_PreviousPosition = Vector3.zero;
		[NonSerialized] private Transform m_FollowTarget;
		[NonSerialized] private float m_FollowMinDistance;
		[NonSerialized] private float m_FollowMaxDistance;

		// INTERFACE PROPERTIES: ------------------------------------------------------------------
		public override Vector3 WorldMoveDirection => this.m_Velocity;
		public override Vector3 LocalMoveDirection => this.Transform.InverseTransformDirection(this.WorldMoveDirection);

		public override float SkinWidth => 0f;
		public override bool IsGrounded
		{
			get
			{
				if (this.Agent == null)
				{
					return false;
				}

				float raycastDistance = m_GroundcheckRaycastLength;
				Vector3 raycastOrigin = this.Character.transform.position - Vector3.up * (this.Agent.height / 2 - 0.1f);

				bool isGrounded = Physics.Raycast(raycastOrigin, Vector3.down, raycastDistance, m_GroundLayerMask, QueryTriggerInteraction.Ignore);

				return isGrounded;
			}
		}

		public override Vector3 FloorNormal => Vector3.up;

		public override bool Collision
		{
			get => true;
			set => _ = value;
		}

		public override Axonometry Axonometry
		{
			get => null;
			set => _ = value;
		}

		// INITIALIZERS: --------------------------------------------------------------------------
		public UnitDriverAStarPath()
		{
			this.MoveDirection = Vector3.zero;
		}

		public override void OnDispose(Character character)
		{
			base.OnDispose(character);
			if (this.Agent != null)
			{
				UnityEngine.Object.Destroy(this.Agent);
			}
		}

		public override void OnStartup(Character character)
		{
			base.OnStartup(character);

			this.Seeker = this.Character.GetComponent<Seeker>();
			if (this.Seeker == null)
			{
				this.Seeker = this.Character.gameObject.AddComponent<Seeker>();
			}

			this.Agent = this.Character.GetComponent<AIPath>();
			if (this.Agent == null)
			{
				this.Agent = this.Character.gameObject.AddComponent<AIPath>();
			}

			this.Agent.height = m_AStarHeight;
			this.Agent.canMove = m_CanMove;
			this.Agent.alwaysDrawGizmos = m_AlwaysDrawGizmos;
			this.Agent.pickNextWaypointDist = m_PickNextWaypointDist;
			this.Agent.orientation = this.m_OrientationMode;
			this.Agent.enableRotation = m_EnableRotation;
			this.Agent.maxAcceleration = 9999f;
			this.Agent.slowdownDistance = this.m_SlowdownDistance;
			this.Agent.endReachedDistance = this.m_EndReachedDistance;
			this.Agent.autoRepath.maximumPeriod = m_AutoRepathMaximumPeriod;
			this.Agent.autoRepath.sensitivity = m_AutoRepathSensitivity;
			this.Agent.whenCloseToDestination = m_WhenCloseToDestination;
			this.Agent.constrainInsideGraph = m_ConstrainInsideGraph;
		}

		// UPDATE METHODS: ------------------------------------------------------------------------
		public override void OnUpdate()
		{
			if (this.Character.IsDead) return;

			this.UpdateProperties(this.Character.Motion);
			this.UpdateTranslation(this.Character.Motion);
		}

		protected virtual void UpdateFollowTarget()
		{
			if (m_FollowTarget == null) return;

			float distanceToTarget = Vector3.Distance(this.Transform.position, m_FollowTarget.position);

			if (distanceToTarget > m_FollowMaxDistance || distanceToTarget < m_FollowMinDistance)
			{
				Vector3 direction = (m_FollowTarget.position - this.Transform.position).normalized;
				Vector3 targetPosition = m_FollowTarget.position - direction * Mathf.Clamp(distanceToTarget, m_FollowMinDistance, m_FollowMaxDistance);

				Location targetLocation = new Location(targetPosition);
				this.Character.Motion.MoveToLocation(targetLocation, this.Character.Motion.StopThreshold, null, 1);
			}
			else
			{
				Vector3 horizontalCurrentPosition = new Vector3(this.Transform.position.x, 0, this.Transform.position.z);
				Vector3 horizontalTargetPosition = new Vector3(m_FollowTarget.position.x, 0, m_FollowTarget.position.z);
				float horizontalDistanceToTarget = Vector3.Distance(horizontalCurrentPosition, horizontalTargetPosition);

				if (horizontalDistanceToTarget <= m_EndReachedDistance)
				{
					this.Character.Motion.StopToDirection(1);
				}
			}
		}

		protected virtual void UpdateProperties(IUnitMotion motion)
		{
			this.MoveDirection = Vector3.zero;

			this.Agent.radius = motion.Radius;
			this.Agent.height = m_AStarHeight;

			this.Agent.maxSpeed = motion.LinearSpeed;
			this.Agent.rotationSpeed = motion.AngularSpeed;

			this.Agent.maxAcceleration = motion.UseAcceleration ? (motion.Acceleration + motion.Deceleration) / 2f : 9999f;

			this.Agent.canMove = m_CanMove;
			this.Agent.alwaysDrawGizmos = m_AlwaysDrawGizmos;
			this.Agent.pickNextWaypointDist = m_PickNextWaypointDist;
			this.Agent.orientation = this.m_OrientationMode;
			this.Agent.enableRotation = m_EnableRotation;
			this.Agent.slowdownDistance = m_SlowdownDistance;
			this.Agent.endReachedDistance = m_EndReachedDistance;
			this.Agent.autoRepath.maximumPeriod = m_AutoRepathMaximumPeriod;
			this.Agent.autoRepath.sensitivity = m_AutoRepathSensitivity;
			this.Agent.whenCloseToDestination = m_WhenCloseToDestination;
			this.Agent.constrainInsideGraph = m_ConstrainInsideGraph;
		}

		protected virtual void UpdateTranslation(IUnitMotion motion)
		{
			if (!this.Agent.enabled)
			{
				Debug.LogWarning("AIPath component is not enabled", this.Character.gameObject);
				return;
			}

			switch (motion.MovementType)
			{
				case Character.MovementType.MoveToDirection:
					this.UpdateMoveToDirection(motion);
					break;

				case Character.MovementType.MoveToPosition:
					this.UpdateMoveToPosition(motion);
					this.UpdateMoveToDirection(motion);
					break;

				case Character.MovementType.None:
					this.Agent.isStopped = true;
					if (!this.Agent.canMove)
					{
						this.MoveDirection = Vector3.zero;
						this.Agent.Move(this.MoveDirection * this.Character.Time.DeltaTime);
					}
					break;

				default: throw new ArgumentOutOfRangeException();
			}

			Vector3 currentPosition = this.Transform.position;

			this.m_Velocity = Vector3.Normalize(currentPosition - this.m_PreviousPosition) * this.MoveDirection.magnitude;

			this.m_PreviousPosition = currentPosition;

			UpdateFollowTarget();
		}

		// POSITION METHODS: ----------------------------------------------------------------------
		protected virtual void UpdateMoveToDirection(IUnitMotion motion)
		{
			this.Agent.isStopped = false;

			if (this.Agent.canMove == false)
			{
				this.MoveDirection = Vector3.zero;
				this.Agent.Move(this.MoveDirection * this.Character.Time.DeltaTime);
			}
			else
			{
				this.MoveDirection = motion.MoveDirection;
				this.Agent.Move(this.MoveDirection * this.Character.Time.DeltaTime);
			}
		}

		protected virtual void UpdateMoveToPosition(IUnitMotion motion)
		{
			this.Agent.isStopped = false;

			// Using Seeker
			/*
			if (this.Seeker != null)
			{
				this.Seeker.StartPath(this.Transform.position, motion.MovePosition, OnPathComplete);
			}
			*/
			// Using Construct
			Path p = ABPath.Construct(m_PreviousPosition, motion.MovePosition);
			this.Agent.SetPath(p);
		}

		private void OnPathComplete(Path p)
		{
			if (p.error)
			{
				Debug.LogWarning("Path calculation failed: " + p.errorLog);
				return;
			}

			this.Agent.SetPath(p);
		}

		// INTERFACE METHODS: ---------------------------------------------------------------------
		public override void SetPosition(Vector3 position)
		{
			this.Transform.position = position;
		}

		public override void SetRotation(Quaternion rotation)
		{
			this.Transform.rotation = rotation;
		}

		public override void SetScale(Vector3 scale)
		{
			this.Transform.localScale = scale;
		}

		public override void AddPosition(Vector3 amount)
		{
			this.SetPosition(this.Transform.position + amount);
		}

		public override void AddRotation(Quaternion amount)
		{
			this.Transform.rotation *= amount;
		}

		public override void AddScale(Vector3 scale)
		{
			this.Transform.localScale += scale;
		}

		public override void ResetVerticalVelocity() { }

		// STRING: --------------------------------------------------------------------------------
		public override string ToString() => "A* Pathfinding";
	}
}
