using ControllerSDM.Enums;

namespace ControllerSDM.TrafficLights
{
    public class CarLight : TrafficLight
    {
        public CarLight(int id) : base(id)
        {
            Id = id;
            Status = (int) LightStatus.Red;
            Time = -1;
            ClearanceTime = 6;
            OrangeTime = 3;
            MinGreenTime = 2;
            MaxGreenTime = 5;
        }
    }
}
