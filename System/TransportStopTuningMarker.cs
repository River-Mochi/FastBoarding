// File: System/TransportStopTuningMarker.cs
// Purpose: Marker component recording the current tuned stop values.

namespace BoardingTime
{
    using Unity.Entities;

    public struct TransportStopTuningMarker : IComponentData
    {
        public float m_LoadingFactor;

        public float m_BoardingTime;
    }
}
