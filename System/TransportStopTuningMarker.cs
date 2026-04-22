// File: System/TransportStopTuningMarker.cs
// Purpose: Marker component recording the current tuned stop values.

namespace FastBoarding
{
    using Unity.Entities;

    /// <summary>
    /// Runtime marker proving a prefab has Fast Boarding values applied.
    /// Returning sliders to 1x removes it.
    /// </summary>
    public struct TransportStopTuningMarker : IComponentData
    {
        public float m_LoadingFactor;

        public float m_BoardingTime;
    }
}
