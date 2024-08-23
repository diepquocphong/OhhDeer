using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace Arawn.Runtime.AStarPathfinding
{
	[Title("GameObject with Offset")]
	[Category("Game Objects/GameObject with Offset")]

	[Image(typeof(IconCubeOutline), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
	[Description("Returns the position of the GameObject plus an offset in local space")]

	[Serializable]
	public class GetPositionGameObjectOffset : PropertyTypeGetPosition
	{
		[SerializeField] private PropertyGetGameObject m_GameObject = new PropertyGetGameObject();
		[SerializeField] private Vector3 m_LocalOffset = Vector3.forward;

		public override Vector3 Get(Args args)
		{
			if (this.m_GameObject == null) return default;

			Transform transform = this.m_GameObject.Get<Transform>(args);
			return transform.position + transform.TransformDirection(this.m_LocalOffset);
		}

		public static PropertyGetPosition Create => new PropertyGetPosition(
			new GetPositionGameObjectOffset()
		);

		public override string String => this.m_GameObject.ToString();
	}
}