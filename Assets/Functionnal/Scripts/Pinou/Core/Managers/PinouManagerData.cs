using UnityEngine;
using Sirenix.OdinInspector;

namespace Pinou
{
	public class PinouManagerData : SerializedScriptableObject
	{
		public virtual PinouManager BuildManagerInstance()
		{
			return new PinouManager(this);
		}

		public virtual void OnDrawGizmos()
		{

		}

		public class PinouManager : ISlave
		{
			public PinouManager(PinouManagerData dataRef)
			{
				Data = dataRef;
			}

			public PinouManagerData Data { get; private set; }

			/// <summary>
			/// Do not need base
			/// </summary>
			public virtual void SlaveAwake()
			{
			}
			/// <summary>
			/// Do not need base
			/// </summary>
			public virtual void SlaveFixedUpdate()
			{
			}
			/// <summary>
			/// Do not need base
			/// </summary>
			public virtual void SlaveStart()
			{
			}
			/// <summary>
			/// Do not need base
			/// </summary>
			public virtual void SlaveUpdate()
			{
			}
			/// <summary>
			/// Do not need base
			/// </summary>
			public virtual void SlaveDrawGizmos()
			{
			}
		}
	}
	
}