// File: System/LateBoarderCancelSystem.Types.cs
// Purpose: Small private structs used by the late-boarder cancellation and diagnostics partials.

namespace FastBoarding
{
    using Game; // GameSystemBase
    using System; // DateTime
    using Unity.Entities;
    using TransportType = Game.Prefabs.TransportType;

    public partial class LateBoarderCancelSystem : GameSystemBase
    {
        private readonly struct PassStats
        {
            public PassStats(int vehicles, int passengers, int candidates, int canceled)
            {
                Vehicles = vehicles;
                Passengers = passengers;
                Candidates = candidates;
                Canceled = canceled;
            }

            public int Vehicles { get; }

            public int Passengers { get; }

            public int Candidates { get; }

            public int Canceled { get; }
        }

        private struct UnsafeNotReadyStats
        {
            public int MissingData;

            public int NoExactVehicleInPath;

            public int Other;

            public int Total => MissingData + NoExactVehicleInPath + Other;
        }

        private readonly struct CanceledPassengerSample
        {
            public CanceledPassengerSample(TransportType transportType, Entity vehicle, Entity passenger)
            {
                TransportType = transportType;
                Vehicle = vehicle;
                Passenger = passenger;
            }

            public TransportType TransportType { get; }

            public Entity Vehicle { get; }

            public Entity Passenger { get; }
        }

        private struct FollowUpSample
        {
            public FollowUpSample(TransportType transportType, Entity vehicle, Entity passenger, uint frame, DateTime localTime)
            {
                Active = true;
                Logged = false;
                TransportType = transportType;
                Vehicle = vehicle;
                Passenger = passenger;
                Frame = frame;
                LocalTime = localTime;
            }

            public bool Active;

            public bool Logged;

            public TransportType TransportType;

            public Entity Vehicle;

            public Entity Passenger;

            public uint Frame;

            public DateTime LocalTime;
        }
    }
}
