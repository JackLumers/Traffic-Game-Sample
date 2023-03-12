using UnityEngine;

namespace TrafficGame.Scripts.EventTriggerVolumes
{
    public class CarFinishVolume : BaseEventTriggerVolume<CarFinishVolume>
    {
        [SerializeField] private bool _isScoring;

        public bool IsScoring => _isScoring;
    }
}