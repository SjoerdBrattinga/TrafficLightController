using System.Timers;
using ControllerSDM.Enums;

namespace ControllerSDM.TrafficLights
{
    public class BusLight : TrafficLight
    {
        public BusLight(int id) : base(id)
        {
            Id = id;
            Status = (int) LightStatus.Red;
            Time = -1;
            OrangeTime = 2;
            ClearanceTime = 10;
        }

        public override void SetGreen()
        {
            IsChangingColor = true;
            RedTime = 0;
            GreenStatusRequested = false;

            var timer = new Timer(ClearanceTime * 1000);
            timer.Elapsed += (s, e) =>
            {
                //Status = (int) light.DirectionRequests[0]
                Status = (int)DirectionRequests[0];
                IsChangingColor = false;
            };
            timer.AutoReset = false;
            timer.Start();
        }
    }
}
