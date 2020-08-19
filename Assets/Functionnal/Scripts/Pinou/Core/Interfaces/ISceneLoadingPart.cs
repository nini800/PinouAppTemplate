using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pinou.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
	public interface ISceneLoadingPart
	{
		void PerformLoadPart();
	}
}