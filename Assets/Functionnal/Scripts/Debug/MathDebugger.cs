#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pinou.UI;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinou
{
	public class MathDebugger : PinouBehaviour
	{
		[SerializeField] private Vector3 _bossPos;
		[SerializeField] private Vector3 _npcPos;
		[SerializeField] private Vector3 _playerPos;
		[SerializeField] private float _npcOrientation;
		[SerializeField] private float _rayWidth;

		private void OnDrawGizmos()
		{
			Gizmos.color = new Color(0.3f,0f,1f);
			Gizmos.DrawSphere(_bossPos, 0.15f);
			Gizmos.color = Color.yellow;
			Gizmos.DrawSphere(_npcPos, 0.1f);
			Gizmos.DrawSphere(_npcPos + new Vector3(Mathf.Cos(_npcOrientation), Mathf.Sin(_npcOrientation)) * 0.2f, 0.03f);

			if (PlayerBetweenBossAndNpc(_bossPos.x, _bossPos.y, _npcPos.x, _npcPos.y, _playerPos.x, _playerPos.y, _rayWidth * 0.5f))
			{
				Gizmos.color = Color.green;
			}
			else
			{
				Gizmos.color = Color.red;
			}
			Gizmos.DrawSphere(_playerPos, 0.1f);

			Gizmos.color = Color.yellow;
			float halfPi = Mathf.PI * 0.5f;
			float bDiffX = _bossPos.x - _npcPos.x;
			float bDiffY = _bossPos.y - _npcPos.y;
			float bDiffLength = Mathf.Sqrt(bDiffX * bDiffX + bDiffY * bDiffY);
			Vector3 forDir = new Vector3(Mathf.Cos(_npcOrientation), Mathf.Sin(_npcOrientation));
			Vector3 rigDir = new Vector3(Mathf.Cos(_npcOrientation - halfPi), Mathf.Sin(_npcOrientation - halfPi));
			Gizmos.DrawLine(_npcPos, _npcPos + rigDir * _rayWidth * 0.5f);
			Gizmos.DrawLine(_npcPos, _npcPos - rigDir * _rayWidth * 0.5f);
			Gizmos.DrawLine(_npcPos + rigDir * _rayWidth * 0.5f, _npcPos + rigDir * _rayWidth * 0.5f + forDir * bDiffLength);
			Gizmos.DrawLine(_npcPos - rigDir * _rayWidth * 0.5f, _npcPos - rigDir * _rayWidth * 0.5f + forDir * bDiffLength);

		}
		private bool PlayerBetweenBossAndNpc(float bossX, float bossY, float npcX, float npcY, float pX, float pY, float rayHalfWidth)
		{
			float bDiffX = bossX - npcX;
			float bDiffY = bossY - npcY;
			float bDiffLength = Mathf.Sqrt(bDiffX * bDiffX + bDiffY * bDiffY);
			bDiffX /= bDiffLength;
			bDiffY /= bDiffLength;

			float npcO = bDiffY < 0 ? -Mathf.Acos(bDiffX) : Mathf.Acos(bDiffX);
			float npcRightO = npcO - Mathf.PI * 0.5f;

			_npcOrientation = npcO;

			float pDiffX = (pX - npcX);
			float pDiffY = (pY - npcY);

			float forwardDistance = pDiffX * Mathf.Cos(npcO) + pDiffY * Mathf.Sin(npcO);
			float rightDistance = pDiffX * Mathf.Cos(npcRightO) + pDiffY * Mathf.Sin(npcRightO);

			return 
				forwardDistance >= 0f && forwardDistance <= bDiffLength && 
				rightDistance >= -rayHalfWidth && rightDistance <= rayHalfWidth;
		}
	}
}