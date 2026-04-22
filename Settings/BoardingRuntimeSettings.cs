// File: Settings/BoardingRuntimeSettings.cs
// Purpose: Runtime settings snapshot shared by Options UI setters and ECS systems.

namespace BoardingTime
{
    using System;

    [Flags]
    public enum BoardingRuntimeChangeFlags
    {
        None = 0,
        StopTuning = 1 << 0,
        LateBoarders = 1 << 1
    }

    /// <summary>
    /// Runtime snapshot of the current mod settings for ECS systems.
    /// This mirrors the applied options into simple static values
    /// and exposes separate revision counters so each system
    /// only wakes when its own inputs changed.
    /// </summary>
    public static class BoardingRuntimeSettings
    {
        public static int StopTuningRevision { get; private set; }

        public static int LateBoarderRevision { get; private set; }

        public static int BusBoardingSpeedFactor { get; private set; } = Setting.DefaultSpeedFactor;

        public static int RailBoardingSpeedFactor { get; private set; } = Setting.DefaultSpeedFactor;

        public static int WaterBoardingSpeedFactor { get; private set; } = Setting.DefaultSpeedFactor;

        public static int AirBoardingSpeedFactor { get; private set; } = Setting.DefaultSpeedFactor;

        public static bool CancelLateBoarders { get; private set; } = false;

        public static BoardingRuntimeChangeFlags Apply(Setting settings)
        {
            BoardingRuntimeChangeFlags changes = BoardingRuntimeChangeFlags.None;

            // Clamp loaded .coc values before systems see them.
            int bus = ClampSpeedFactor(settings.BusBoardingSpeedFactor);
            int rail = ClampSpeedFactor(settings.RailBoardingSpeedFactor);
            int water = ClampSpeedFactor(settings.WaterBoardingSpeedFactor);
            int air = ClampSpeedFactor(settings.AirBoardingSpeedFactor);

            bool stopChanged = false;

            if (BusBoardingSpeedFactor != bus)
            {
                BusBoardingSpeedFactor = bus;
                stopChanged = true;
            }

            if (RailBoardingSpeedFactor != rail)
            {
                RailBoardingSpeedFactor = rail;
                stopChanged = true;
            }

            if (WaterBoardingSpeedFactor != water)
            {
                WaterBoardingSpeedFactor = water;
                stopChanged = true;
            }

            if (AirBoardingSpeedFactor != air)
            {
                AirBoardingSpeedFactor = air;
                stopChanged = true;
            }

            if (stopChanged)
            {
                // One shared revision is enough because all stop-prefab tuning is recalculated together.
                StopTuningRevision++;
                changes |= BoardingRuntimeChangeFlags.StopTuning;
            }

            if (CancelLateBoarders != settings.CancelLateBoarders)
            {
                CancelLateBoarders = settings.CancelLateBoarders;
                // The live boarding system only needs to wake when this behavior toggle changes.
                LateBoarderRevision++;
                changes |= BoardingRuntimeChangeFlags.LateBoarders;
            }

            return changes;
        }

        public static bool SetBusBoardingSpeedFactor(int value)
        {
            value = ClampSpeedFactor(value);
            if (BusBoardingSpeedFactor == value)
            {
                return false;
            }

            BusBoardingSpeedFactor = value;
            StopTuningRevision++;
            return true;
        }

        public static bool SetRailBoardingSpeedFactor(int value)
        {
            value = ClampSpeedFactor(value);
            if (RailBoardingSpeedFactor == value)
            {
                return false;
            }

            RailBoardingSpeedFactor = value;
            StopTuningRevision++;
            return true;
        }

        public static bool SetWaterBoardingSpeedFactor(int value)
        {
            value = ClampSpeedFactor(value);
            if (WaterBoardingSpeedFactor == value)
            {
                return false;
            }

            WaterBoardingSpeedFactor = value;
            StopTuningRevision++;
            return true;
        }

        public static bool SetAirBoardingSpeedFactor(int value)
        {
            value = ClampSpeedFactor(value);
            if (AirBoardingSpeedFactor == value)
            {
                return false;
            }

            AirBoardingSpeedFactor = value;
            StopTuningRevision++;
            return true;
        }

        public static bool SetCancelLateBoarders(bool value)
        {
            if (CancelLateBoarders == value)
            {
                return false;
            }

            CancelLateBoarders = value;
            LateBoarderRevision++;
            return true;
        }

        public static string DescribeForLog()
        {
            return
                $"bus={BusBoardingSpeedFactor}x, rail={RailBoardingSpeedFactor}x, " +
                $"ship+ferry={WaterBoardingSpeedFactor}x, air={AirBoardingSpeedFactor}x, " +
                $"cancelLate={CancelLateBoarders}";
        }

        private static int ClampSpeedFactor(int value)
        {
            if (value < Setting.MinSpeedFactor)
            {
                return Setting.MinSpeedFactor;
            }

            if (value > Setting.MaxSpeedFactor)
            {
                return Setting.MaxSpeedFactor;
            }

            return value;
        }
    }
}
