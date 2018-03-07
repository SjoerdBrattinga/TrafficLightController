namespace ControllerSDM
{
    public class TrafficUpdate
    {
        public int LightId { get; set; }
        public int Count { get; set; }
        public int?[] DirectionRequests { get; set; } 
    }

    public class TrafficUpdateWrapper
    {
        public TrafficUpdate TrafficUpdate { get; set; }
    }
}
