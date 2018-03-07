using ControllerSDM.Enums;

namespace ControllerSDM.TrafficLights
{
    public class CrossingBarrier : TrafficLight
    {
        public CrossingBarrier(int id) : base(id)
        {
            Id = id;
            Status = (int) LightStatus.Green;
            Time = -1;
        }

        public override void SetRed()
        {

        }

        public override void SetGreen()
        {

        }
    }
}
