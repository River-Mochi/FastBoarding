// File: System/LateBoarderCancelSystem.Helpers.cs
// Purpose: Helper methods for passenger checks, cancellation edits, and transport-type detection.

namespace FastBoarding
{
    using Game; // GameSystemBase
    using Game.Creatures; // Human, CurrentVehicle, Passenger flags
    using Game.Pathfind; // PathOwner, PathElement
    using Game.Vehicles; // Passenger
    using System; // Math
    using System.Collections.Generic; // HashSet
    using Unity.Entities; // Entity, EntityCommandBuffer
    using PrefabRef = Game.Prefabs.PrefabRef; // prefab lookup
    using PublicTransportVehicleData = Game.Prefabs.PublicTransportVehicleData; // transport type source
    using TransportType = Game.Prefabs.TransportType; // bus/train/etc.

    public partial class LateBoarderCancelSystem : GameSystemBase
    {
        private bool CanSafelyCancelPassenger(Entity vehicleEntity, Entity passenger)
        {
            UnsafeNotReadyStats ignoredStats = default;
            return CanSafelyCancelPassenger(vehicleEntity, passenger, ref ignoredStats);
        }

        private bool CanSafelyCancelPassenger(Entity vehicleEntity, Entity passenger, ref UnsafeNotReadyStats unsafeStats)
        {
            if (IsGroupPassenger(passenger))
            {
                // Group boarding has extra leader/member rules. Keep the first beta pass solo-only.
                unsafeStats.Other++;
                return false;
            }

            if (!EntityManager.HasComponent<Game.Creatures.Resident>(passenger) ||
                !EntityManager.HasComponent<Human>(passenger) ||
                !EntityManager.HasComponent<PathOwner>(passenger) ||
                !EntityManager.HasBuffer<PathElement>(passenger))
            {
                unsafeStats.MissingData++;
                return false;
            }

            var pathOwner = EntityManager.GetComponentData<PathOwner>(passenger);
            var pathElements = EntityManager.GetBuffer<PathElement>(passenger);

            // Only inspect the remaining path from the current cursor forward.
            // Older elements before m_ElementIndex are already behind the cim.
            int startIndex = Math.Max(0, pathOwner.m_ElementIndex);

            // Only cancel when the cim still has this exact vehicle in the remaining path.
            for (var i = startIndex; i < pathElements.Length; i++)
            {
                if (pathElements[i].m_Target == vehicleEntity)
                {
                    return true;
                }
            }

            unsafeStats.NoExactVehicleInPath++;
            return false;
        }

        private static void AccumulateCanceledCount        private static void AccumulateCanceledCount(
            TransportType transportType,
            int count,
            ref int busCanceled,
            ref int trainCanceled,
            ref int tramCanceled,
            ref int subwayCanceled,
            ref int shipCanceled,
            ref int ferryCanceled,
            ref int airCanceled)
        {
            switch (transportType)
            {
                case TransportType.Bus:
                    busCanceled += count;
                    break;
                case TransportType.Train:
                    trainCanceled += count;
                    break;
                case TransportType.Tram:
                    tramCanceled += count;
                    break;
                case TransportType.Subway:
                    subwayCanceled += count;
                    break;
                case TransportType.Ship:
                    shipCanceled += count;
                    break;
                case TransportType.Ferry:
                    ferryCanceled += count;
                    break;
                case TransportType.Airplane:
                    airCanceled += count;
                    break;
            }
        }

        private static bool TryCountCanceledSample(
            TransportType transportType,
            ref int busSampleCount,
            ref int trainSampleCount,
            ref int tramSampleCount,
            ref int subwaySampleCount,
            ref int shipSampleCount,
            ref int ferrySampleCount,
            ref int airSampleCount)
        {
            switch (transportType)
            {
                case TransportType.Bus:
                    return TryIncrementSampleCount(ref busSampleCount);
                case TransportType.Train:
                    return TryIncrementSampleCount(ref trainSampleCount);
                case TransportType.Tram:
                    return TryIncrementSampleCount(ref tramSampleCount);
                case TransportType.Subway:
                    return TryIncrementSampleCount(ref subwaySampleCount);
                case TransportType.Ship:
                    return TryIncrementSampleCount(ref shipSampleCount);
                case TransportType.Ferry:
                    return TryIncrementSampleCount(ref ferrySampleCount);
                case TransportType.Airplane:
                    return TryIncrementSampleCount(ref airSampleCount);
                default:
                    return false;
            }
        }

        private static bool TryIncrementSampleCount(ref int sampleCount)
        {
            if (sampleCount >= MaxSampledCimsPerModePerUpdate)
            {
                return false;
            }

            sampleCount++;
            return true;
        }

        private void RecordCanceledCounts(
            int busCanceled,
            int trainCanceled,
            int tramCanceled,
            int subwayCanceled,
            int shipCanceled,
            int ferryCanceled,
            int airCanceled)
        {
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Bus, busCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Train, trainCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Tram, tramCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Subway, subwayCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Ship, shipCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Ferry, ferryCanceled);
            TransitWaitStatus.RecordLateBoardersCanceled(World, TransportType.Airplane, airCanceled);
        }

        private bool QueuePassengerCancellation(ref EntityCommandBuffer ecb, Entity vehicleEntity, Entity passenger)
        {
            if (!EntityManager.HasComponent<CurrentVehicle>(passenger) ||
                !EntityManager.HasComponent<Game.Creatures.Resident>(passenger) ||
                !EntityManager.HasComponent<Human>(passenger) ||
                !EntityManager.HasComponent<PathOwner>(passenger) ||
                !EntityManager.HasBuffer<PathElement>(passenger))
            {
                return false;
            }

            var pathOwner = EntityManager.GetComponentData<PathOwner>(passenger);
            var pathElements = EntityManager.GetBuffer<PathElement>(passenger);

            // Match vanilla's "search from the current path cursor onward" behavior.
            int startIndex = Math.Max(0, pathOwner.m_ElementIndex);

            // -1 is a sentinel meaning "vehicle not found yet".
            // It is never used as an array index; the < 0 check below aborts first.
            int vehiclePathIndex = -1;
            for (var i = startIndex; i < pathElements.Length; i++)
            {
                if (pathElements[i].m_Target == vehicleEntity)
                {
                    vehiclePathIndex = i;
                    break;
                }
            }

            if (vehiclePathIndex < 0)
            {
                // Safety stop: if the vehicle disappeared from the remaining path, do nothing.
                return false;
            }

            ecb.RemoveComponent<CurrentVehicle>(passenger);

            // Clear "in vehicle" state so the cim is treated as no longer attached to this vehicle.
            var resident = EntityManager.GetComponentData<Game.Creatures.Resident>(passenger);
            resident.m_Flags &= ~ResidentFlags.InVehicle;
            resident.m_Timer = 0;
            ecb.SetComponent(passenger, resident);

            var human = EntityManager.GetComponentData<Human>(passenger);
            // Clear urgency from the missed boarding attempt before vanilla resumes control.
            human.m_Flags &= ~(HumanFlags.Run | HumanFlags.Emergency);
            ecb.SetComponent(passenger, human);

            // Replace the path buffer during ECB playback so vanilla can continue from the next leg.
            // Copy starts after the missed vehicle, so the missed leg is removed.
            DynamicBuffer<PathElement> newPath = ecb.SetBuffer<PathElement>(passenger);
            for (var i = vehiclePathIndex + 1; i < pathElements.Length; i++)
            {
                newPath.Add(pathElements[i]);
            }

            // Restart from the beginning of the trimmed buffer.
            pathOwner.m_ElementIndex = 0;
            ecb.SetComponent(passenger, pathOwner);
            return true;
        }

        private static void QueueVehiclePassengerBuffer(
            ref EntityCommandBuffer ecb,
            Entity vehicleEntity,
            DynamicBuffer<Passenger> passengers,
            HashSet<Entity> canceledPassengers)
        {
            // Rebuild the passenger buffer once so entries are not removed while iterating it.
            DynamicBuffer<Passenger> newPassengers = ecb.SetBuffer<Passenger>(vehicleEntity);
            for (var i = 0; i < passengers.Length; i++)
            {
                if (!canceledPassengers.Contains(passengers[i].m_Passenger))
                {
                    newPassengers.Add(passengers[i]);
                }
            }
        }

        private TransportType GetTransportType(Entity vehicleEntity)
        {
            if (!EntityManager.HasComponent<PrefabRef>(vehicleEntity))
            {
                return TransportType.None;
            }

            Entity prefab = EntityManager.GetComponentData<PrefabRef>(vehicleEntity).m_Prefab;
            if (!EntityManager.HasComponent<PublicTransportVehicleData>(prefab))
            {
                return TransportType.None;
            }

            return EntityManager.GetComponentData<PublicTransportVehicleData>(prefab).m_TransportType;
        }
    }
}
