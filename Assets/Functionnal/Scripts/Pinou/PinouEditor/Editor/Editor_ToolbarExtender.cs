#pragma warning disable 0649, 0414
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityToolbarExtender;
using Pinou.Networking;
using Mirror.FizzySteam;
using Mirror;

namespace Pinou.Editor
{
	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
		}

		private static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			/*if (_mode == null)
			{
				_mode = PinouUtils.GameObject.FindObjectOfType<PinouNetworkManager>().Transport is TelepathyTransport;
			}

			if ((bool)_mode)
			{
				if (GUILayout.Button(new GUIContent("Telepathy", "Switch to Steam Network Mode")))
				{
					PinouUtils.GameObject.FindObjectOfType<PinouNetworkManager>().SetTransport(PinouUtils.GameObject.FindObjectOfType<TelepathyTransport>());
					_mode = false;
				}
			}
			else
			{
				if (GUILayout.Button(new GUIContent("FizzySteam", "Switch to Telepathy Network Mode")))
				{
					PinouUtils.GameObject.FindObjectOfType<PinouNetworkManager>().SetTransport(PinouUtils.GameObject.FindObjectOfType<FizzySteamworks>());
					_mode = true;
				}
			}*/
		}
	}
}