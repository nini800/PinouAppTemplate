using System.Collections.Generic;
using UnityEngine;

namespace Pinou
{
	public class PinouParticleSystem : PinouBehaviour
	{
        [SerializeField] private ParticleSystem _ps;

        protected override void OnAwake()
        {
            _emissionModule = _ps.emission;
        }
        private ParticleSystem.EmissionModule _emissionModule;

        public void SetEmission(float _emission)
        {
            _emissionModule.rateOverTime = _emission;
        }

        public void PlayFromStart()
		{
            _ps.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            _ps.Play();
		}

#if UNITY_EDITOR
        protected override void E_OnValidate()
        {
            AutoFindReference(ref _ps);
        }
#endif
    }
}