using System.Timers;
using ControllerSDM.Enums;
using Newtonsoft.Json;

namespace ControllerSDM.TrafficLights
{
    public class TrafficLight
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public int Time { get; set; }

        [JsonIgnore]
        public int Count { get; set; }

        [JsonIgnore]
        public int?[] DirectionRequests { get; set; }

        [JsonIgnore]
        public int ClearanceTime { get; set; }

        [JsonIgnore]
        public int RedTime { get; set; }

        [JsonIgnore]
        public int GreenTime { get; set; }

        [JsonIgnore]
        public int OrangeTime { get; set; }

        [JsonIgnore]
        public bool GreenStatusRequested { get; set; }

        [JsonIgnore]
        public bool IsChangingColor { get; set; }

        [JsonIgnore]
        public int Priority { get; set; }

        [JsonIgnore]
        public int MinGreenTime { get; set; }

        [JsonIgnore]
        public int MaxGreenTime { get; set; }


        public TrafficLight(int id)
        {
            Id = id;
            Status = (int)LightStatus.Red;
            Time = -1;
        }

        public virtual void SetGreen()
        {
            IsChangingColor = true;
            RedTime = 0;
            GreenStatusRequested = false;

            var timer = new Timer(ClearanceTime * 1000);
            timer.Elapsed += (s, e) =>
            {
                Status = (int)LightStatus.Green;
                IsChangingColor = false;
            };
            timer.AutoReset = false;
            timer.Start();
        }

        public virtual void SetRed()
        {
            IsChangingColor = true;
            GreenTime = 0;
            
            Status = (int)LightStatus.Orange;

            var timer = new Timer(OrangeTime * 1000);
            timer.Elapsed += (s, e) =>
            {
                Status = (int)LightStatus.Red;
                IsChangingColor = false;
            };
            timer.AutoReset = false;
            timer.Start();
        }

        public void HandleUpdate(TrafficUpdate trafficUpdate)
        {
            var previousCount = Count;
            Count = trafficUpdate.Count;
            if (trafficUpdate.DirectionRequests != null) DirectionRequests = trafficUpdate.DirectionRequests;

            if (previousCount == 0 && Count > 0 && Status == (int)LightStatus.Red) 
            {
                GreenStatusRequested = true;
            }
            if (Count > previousCount && Status == (int)LightStatus.Red) 
            {
                GreenStatusRequested = true;
            }
            if (Count == 0)
            {
                GreenStatusRequested = false;
            }
        }

        public void TimerTick()
        {
            if (Status == (int)LightStatus.Red && !IsChangingColor)
            {
                RedTime++;
            }
            else if (Status == (int)LightStatus.Green && !IsChangingColor)
            {
                GreenTime++;
            }

            SetPriority();
        }

        private void SetPriority()
        {
            if (GreenStatusRequested)
            {
                Priority = Count + RedTime;
            }
            else
            {
                Priority = 0;
            }
        }
    }
}
