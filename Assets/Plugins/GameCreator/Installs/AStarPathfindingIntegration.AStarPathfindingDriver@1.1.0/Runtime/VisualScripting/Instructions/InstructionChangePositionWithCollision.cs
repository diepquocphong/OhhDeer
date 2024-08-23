using System;
using System.Threading.Tasks;
using Arawn.Runtime.AStarPathfinding;
using GameCreator.Runtime.Characters;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Change Position with Collision")]
    [Description("Changes the position with Collision of a game object over time")]

    [Image(typeof(IconVector3), ColorTheme.Type.Yellow)]

    [Category("Transforms/Change Position with Collision")]

    [Parameter("Position", "The desired position of the game object")]
    [Parameter("Space", "If the transformation occurs in local or world space")]
    [Parameter("Duration", "How long it takes to perform the transition")]
    [Parameter("Easing", "The change rate of the translation over time")]
    [Parameter("Wait to Complete", "Whether to wait until the translation is finished or not")]

    [Keywords("Location", "Translate", "Move", "Displace")]
    [Serializable]
    public class InstructionChangePositionWithCollision : TInstructionTransform
    {
        [SerializeField] private string wallTag = "Wall";
        [SerializeField] private string groundTag = "Ground";
        [SerializeField] private ChangePosition m_Position = new ChangePosition(Vector3.up);

        [Space] [SerializeField] private Space m_Space = Space.Self;
        [SerializeField] private Transition m_Transition = new Transition();
        [SerializeField] private PropertyGetDecimal m_ColliderRadius = new PropertyGetDecimal();
        [SerializeField] private PropertyGetBool m_AffectsOnlyAStarPathfindingDriver = new PropertyGetBool();

		private UnitDriver m_Driver = new UnitDriver(new UnitDriverAStarPath());


		// PROPERTIES: ----------------------------------------------------------------------------

		public override string Title => $"Move {this.m_Transform} {this.m_Position}";

        // RUN METHOD: ----------------------------------------------------------------------------

        protected override async Task Run(Args args)
        {
	        bool effectOnlyAStarPathfindingDriver = this.m_AffectsOnlyAStarPathfindingDriver.Get(args);

	        GameObject gameObject = this.m_Transform.Get(args);
	        float radius = (float)this.m_ColliderRadius.Get(args);
	        if (gameObject == null) return;

	        Character character = gameObject.GetComponent<Character>();
	        if (character == null) return;

	        // Check if the driver type matches
	        if (effectOnlyAStarPathfindingDriver == true)
	        {
				if (this.m_Driver.Wrapper.GetType() != character.Driver.GetType()) return;
			}

	        CollisionHandler collisionHandler = gameObject.GetComponent<CollisionHandler>();
	        if (collisionHandler == null)
	        {
		        collisionHandler = gameObject.AddComponent<CollisionHandler>();
		        collisionHandler.Setup(wallTag, groundTag);
	        }

	        CharacterController characterController = gameObject.GetComponent<CharacterController>();
	        if (characterController == null)
	        {
		        characterController = gameObject.AddComponent<CharacterController>();
		        characterController.radius = radius;
	        }

			Vector3 valueSource = gameObject.transform.position;
			/*
            Vector3 valueTarget = this.m_Position.Get(
                valueSource,
                args,
                this.m_Space,
                gameObject.transform.parent
            );
            */

			Vector3 positionChange = this.m_Position.Get(valueSource, args);

			// If the space is local, transform the position change to local space relative to the parent
			if (this.m_Space == Space.Self && gameObject.transform.parent != null)
			{
				positionChange = gameObject.transform.parent.InverseTransformDirection(positionChange);
			}

			// Calculate the valueTarget based on the space setting
			Vector3 valueTarget;
			if (this.m_Space == Space.World)
			{
				valueTarget = valueSource + positionChange;
			}
			else
			{
				// If the space is local, transform the position change back to world space before applying
				valueTarget = gameObject.transform.TransformPoint(positionChange);
			}

			Vector3 direction = (valueTarget - valueSource).normalized;
            float distance = Vector3.Distance(valueSource, valueTarget);

            ITweenInput tween = new TweenInput<float>(
                0,
                distance,
                this.m_Transition.Duration,
                (a, b, t) =>
                {
                    if (collisionHandler.HasCollided)
                    {
                        gameObject.transform.position = collisionHandler.CollisionPoint;
                        collisionHandler.ResetCollision();
                    }
                    else
                    {
                        float deltaDistance = Mathf.LerpUnclamped(a, b, t);
                        characterController.Move(direction * deltaDistance);
                    }
                },
                Tween.GetHash(typeof(Transform), "position"),
                this.m_Transition.EasingType,
                this.m_Transition.Time
            );

            Tween.To(gameObject, tween);
            if (this.m_Transition.WaitToComplete) await this.Until(() => tween.IsFinished);
        }
    }

    public class CollisionHandler : MonoBehaviour
    {
        private string wallTag;
        private string groundTag;
        public bool HasCollided { get; private set; }
        public Vector3 CollisionPoint { get; private set; }

        public void Setup(string wallTag, string groundTag)
        {
            this.wallTag = wallTag;
            this.groundTag = groundTag;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == wallTag || collision.gameObject.tag == groundTag)
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
