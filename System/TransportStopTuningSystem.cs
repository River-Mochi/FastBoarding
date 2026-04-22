// File: System/TransportStopTuningSystem.cs
// Purpose: Applies boarding-speed slider values to public transport stop prefabs.

namespace BoardingTime
{
    using Game;
    using Game.Common;
    using Game.Prefabs;
    using Game.Simulation;
    using Game.Tools;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;

    public partial class TransportStopTuningSystem : GameSystemBase
    {
        private const float FloatEpsilon = 0.0001f;

        private EntityQuery m_StopPrefabQuery;
        private PrefabSystem? m_PrefabSystem;
        private int m_AppliedRevision = -1;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_PrefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            m_StopPrefabQuery = SystemAPI.QueryBuilder()
                .WithAll<PrefabData, TransportStopData>()
                .WithNone<Deleted, Destroyed, Temp, Overridden>()
                .Build();

            // This system runs as a one-shot pass when settings change.
            RequireForUpdate(m_StopPrefabQuery);
        }

        protected override void OnUpdate()
        {
            if (m_AppliedRevision == BoardingRuntimeSettings.StopTuningRevision)
            {
                Enabled = false;
                return;
            }

            // Query first, then apply changes by entity so prefab tuning does not mutate during query enumeration.
            m_StopPrefabQuery.CompleteDependency();
            var entities = m_StopPrefabQuery.ToEntityArray(Allocator.Temp);
            var updatedPrefabs = 0;

            foreach (var prefabEntity in entities)
            {
                if (m_PrefabSystem == null ||
                    !m_PrefabSystem.TryGetPrefab<PrefabBase>(prefabEntity, out var prefab) ||
                    !prefab.TryGet<TransportStop>(out var authoringStop))
                {
                    continue;
                }

                if (!authoringStop.m_PassengerTransport)
                {
                    continue;
                }

                int speedFactor = GetSpeedFactor(authoringStop.m_TransportType);
                float speedMultiplier = speedFactor;
                var baseEffectiveLoading = math.max(0f, 1f + authoringStop.m_LoadingFactor);
                var tunedStop = EntityManager.GetComponentData<TransportStopData>(prefabEntity);

                // Always recompute from prefab authoring values so repeated slider changes never double-scale.
                // Higher loading factor helps passengers board faster; lower boarding time shortens dwell.
                tunedStop.m_LoadingFactor = baseEffectiveLoading * speedMultiplier - 1f;
                tunedStop.m_BoardingTime = authoringStop.m_BoardingTime / speedMultiplier;
                EntityManager.SetComponentData(prefabEntity, tunedStop);
                updatedPrefabs++;

                if (speedFactor == Setting.DefaultSpeedFactor)
                {
                    // Vanilla slider value removes our marker so saved data stays clean.
                    if (EntityManager.HasComponent<TransportStopTuningMarker>(prefabEntity))
                    {
                        EntityManager.RemoveComponent<TransportStopTuningMarker>(prefabEntity);
                    }
                }
                else
                {
                    var marker = new TransportStopTuningMarker
                    {
                        m_LoadingFactor = tunedStop.m_LoadingFactor,
                        m_BoardingTime = tunedStop.m_BoardingTime
                    };

                    if (EntityManager.HasComponent<TransportStopTuningMarker>(prefabEntity))
                    {
                        var currentMarker = EntityManager.GetComponentData<TransportStopTuningMarker>(prefabEntity);
                        if (math.abs(currentMarker.m_LoadingFactor - marker.m_LoadingFactor) > FloatEpsilon ||
                            math.abs(currentMarker.m_BoardingTime - marker.m_BoardingTime) > FloatEpsilon)
                        {
                            EntityManager.SetComponentData(prefabEntity, marker);
                        }
                    }
                    else
                    {
                        EntityManager.AddComponentData(prefabEntity, marker);
                    }
                }
            }

            entities.Dispose();
            m_AppliedRevision = BoardingRuntimeSettings.StopTuningRevision;
            Enabled = false;
            LogUtils.Info(
                Mod.s_Log,
                () => $"Applied stop tuning to {updatedPrefabs} transport stop prefabs. {BoardingRuntimeSettings.DescribeForLog()}");
        }

        private static int GetSpeedFactor(TransportType transportType)
        {
            switch (transportType)
            {
                case TransportType.Bus:
                    return BoardingRuntimeSettings.BusBoardingSpeedFactor;
                case TransportType.Train:
                case TransportType.Tram:
                case TransportType.Subway:
                    return BoardingRuntimeSettings.RailBoardingSpeedFactor;
                case TransportType.Ship:
                case TransportType.Ferry:
                    return BoardingRuntimeSettings.WaterBoardingSpeedFactor;
                case TransportType.Airplane:
                    return BoardingRuntimeSettings.AirBoardingSpeedFactor;
                default:
                    return Setting.DefaultSpeedFactor;
            }
        }
    }
}
