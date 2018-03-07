using ControllerSDM.Enums;

namespace ControllerSDM.TrafficLights
{
    public class BicycleLight : TrafficLight
    {
        public BicycleLight(int id) : base(id)
        {
            Id = id;
            Status = (int) LightStatus.Red;
            Time = -1;
            ClearanceTime = 5;
            OrangeTime = 4;
            MinGreenTime = 6;
            MaxGreenTime = 10;
        }
    }
}
