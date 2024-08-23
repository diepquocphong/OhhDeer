using System;
using GameCreator.Runtime.Common;
using UnityEngine;
using GameCreator.Runtime.Characters;

namespace Arawn.Runtime.AStarPathfinding
{
	[Title("Character with Offset")]
	[Category("Characters/Character with Offset")]

	[Image(typeof(IconCharacter), ColorTheme.Type.Yellow, typeof(OverlayArrowRight))]
	[Description("Returns the position of the Character plus an offset in local space")]

	[Serializable]
	public class GetPositionCharacterOffset : PropertyTypeGetPosition
	{
		[SerializeField] private PropertyGetGameObject m_GameObject = GetGameObjectPlayer.Create();
		[SerializeField] private Vector3 m_LocalOffset = Vector3.forward;

		public override Vector3 Get(Args args)
		{
			if (this.m_GameObject == null) return default;

			Transform transform = this.m_GameObject.Get<Transform>(args);
			return transform.position + transform.TransformDirection(this.m_LocalOffset);
		}

		public static PropertyGetPosition Create => new PropertyGetPosition(
			new GetPositionCharacterOffset()
		);

		public override string String => this.m_GameObject.ToString();
	}
}