using ControllerSDM.Enums;

namespace ControllerSDM.TrafficLights
{
    public class TrainLight : TrafficLight
    {
        public TrainLight(int id) : base(id)
        {
            Id = id;
            Status = (int) LightStatus.Red;
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
