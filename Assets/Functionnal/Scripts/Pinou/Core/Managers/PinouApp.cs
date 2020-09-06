#pragma warning disable 0649, 0414
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
	public enum AppMode
	{
		Build,
		Development
	}

	public partial class PinouApp : PinouSingleton<PinouApp>
	{
		[System.Serializable]
		public class ManagerParameters
		{
			[SerializeField] private PinouManagerData _managerData;
			[SerializeField] private string _managerVariableName;

			public PinouManagerData ManagerData => _managerData;
			public string ManagerVariableName => _managerVariableName;
		}

		#region Attributes, Accessors & Mutators
		[Header("Parameters")]
		[Space]
		[SerializeField, ReadOnly] private AppMode _appMode = AppMode.Build;
		[SerializeField] private bool _networkMode = false;
		[SerializeField] private List<ManagerParameters> _managers;

		[Header("References")]
		[Space]
		[SerializeField, ReadOnly] private PinouFakePlayersCreator _fakePlayersCreator;

		[Header("Misc.")]
		[Space]
		[SerializeField] private string _autoscriptPath;

		private List<PinouManagerData.PinouManager> _managersInstances = new List<PinouManagerData.PinouManager>();
		public static bool SafeStarted { get; private set; } = false;
		public static bool NetworkMode => Instance._networkMode;

		public AppMode AppMode => _appMode;
		#endregion
		#region Behaviour
		protected override void OnAwake()
		{
			base.OnAwake();
			InstiantiateManagers();

			for (int i = 0; i < _managersInstances.Count; i++)
			{
				_managersInstances[i].SlaveAwake();
			}
		}

		protected override void OnStart()
		{
			for (int i = 0; i < _managersInstances.Count; i++)
			{
				_managersInstances[i].SlaveStart();
			}

			OnSafeStart.Invoke();
			SafeStarted = true;
		}

		private void FixedUpdate()
		{
			for (int i = 0; i < _managersInstances.Count; i++)
			{
				_managersInstances[i].SlaveFixedUpdate();
			}
		}

		private void Update()
		{
			for (int i = 0; i < _managersInstances.Count; i++)
			{
				_managersInstances[i].SlaveUpdate();
			}

#if UNITY_EDITOR
			E_HandleMaximizeWindowInPlayMode();
#endif
		}

		#endregion

		#region Utilities
		private void InstiantiateManagers()
		{
			for (int i = 0; i < _managers.Count; i++)
			{
				PinouManagerData.PinouManager manager = _managers[i].ManagerData.BuildManagerInstance();
				_managersInstances.Add(manager);

				TryFillReference(_managers[i], manager);
			}
		}
		private void TryFillReference(ManagerParameters managerParams, PinouManagerData.PinouManager manager)
		{
			if (managerParams.ManagerData.GetType() == manager.Data.GetType())
			{
				FieldInfo managerField = GetType().GetField("_" + managerParams.ManagerVariableName, BindingFlags.NonPublic | BindingFlags.Static);
				if (managerField != null)
				{
					managerField.SetValue(this, manager);
				}
				else
				{
					Debug.LogError("No autoscript field found for " + managerParams.ManagerVariableName + ".");
				}
			}
		}
		#endregion

		#region Events
		/// <summary>
		/// Behaves like Start unity's function but triggers after PinouApp Scene is done loading.
		/// </summary>
		public static new PinouUtils.Delegate.Action OnSafeStart { get; private set; } = new PinouUtils.Delegate.Action();
		#endregion

		#region Editor
#if UNITY_EDITOR
		protected override void E_OnValidate()
		{
			AutoFindReference(ref _fakePlayersCreator);

			PinouAutoscript.UpdatePinouAppAutoScript(_autoscriptPath, _managers.ToArray());
		}

		[Button("Switch to Dev Mode"), ShowIf("_appMode", AppMode.Build)]
		private void E_SetToDevMode()
		{
			_appMode = AppMode.Development;
			_fakePlayersCreator.enabled = true;
		}
		[Button("Switch to Build Mode"), ShowIf("_appMode", AppMode.Development)]
		private void E_SetToBuildMode()
		{
			_appMode = AppMode.Build;
			_fakePlayersCreator.enabled = false;
		}

		protected override void E_OnDrawGizmos()
		{
			for (int i = 0; i < _managers.Count; i++)
			{
				if (_managers[i].ManagerData == null)
				{
					continue;
				}
				_managers[i].ManagerData.OnDrawGizmos();
			}
		}

		private void E_HandleMaximizeWindowInPlayMode()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Space) && UnityEngine.Input.GetKey(KeyCode.LeftShift))
			{
				EditorWindow.focusedWindow.maximized = !EditorWindow.focusedWindow.maximized;
			}
		}
#endif
		#endregion
	}
}