using System;
using System.Text;
using System.Timers;
using ControllerSDM.Enums;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace ControllerSDM
{
    public class Controller : MessagingService
    {
        private readonly Intersection _intersection;

        private Timer _timer;

        public Controller()
        {
            _intersection = new Intersection();
        }

        public void Start()
        {
            Console.WriteLine("Controller started");

            SetupMessaging();
            Console.WriteLine("Connection made");
            ReceiveTrafficUpdates();
            Console.WriteLine("Receiving updates");
            SetTimer();
            UpdateLightStatus();
            //Parallel.Invoke(UpdateLightStatus, _timer.Start);
        }

        public void UpdateLightStatus()
        {
            while (true)
            {
                _intersection.OrderByPriority();
                
                foreach (var light in _intersection.Lights)
                {
                    if (light.GetType().Name == "BusLight" && light.Count == 0 &&
                        light.Status > (int) LightStatus.Orange && !light.IsChangingColor)
                    {
                        light.SetRed();
                    }
                        
                    else 
                    {
                        if (light.Count == 0 && light.GreenTime > light.MinGreenTime && !light.IsChangingColor)
                        {
                            light.SetRed();
                            Console.WriteLine("Set to red: " + light.Id + " count:" + light.Count);
                        }
                        if (light.Count > 0 && light.GreenTime > light.MaxGreenTime && !light.IsChangingColor)
                        {
                            light.SetRed();
                            Console.WriteLine("Set to red: " + light.Id + " count:" + light.Count);
                        }
                    }
                }

                foreach (var light in _intersection.LightsByPriority)
                {
                    if (light.GetType().Name == "BusLight")
                    {
                        if (light.Count > 0 && light.RedTime > 10)
                        {   
                            if(_intersection.CheckIfLightCanBeGreen(light.Id))
                                light.SetGreen();
                            else
                                _intersection.BusClear(light.Id);
                            
                        }
                            
                    }

                    if (light.GetType().Name == "CarLight")
                    {
                        if (light.Priority <= 0) continue;
                        if (light.RedTime > 60)
                        {
                            _intersection.ForceClear(light.Id);
                            continue;
                        }
                        if (!_intersection.CheckIfLightCanBeGreen(light.Id)) continue;
                        if (light.Status != (int)LightStatus.Red || light.Count <= 0) continue;
                        if (light.RedTime > 3)
                        {
                            light.SetGreen();

                            Console.WriteLine("Set to green: " + light.Id + " count:" + light.Count);
                            Console.WriteLine(light.Priority);
                        }
                         
                    }
                    else if (light.GetType().Name == "BicycleLight")
                    {
                        if (light.Priority <= 0) continue;
                        if (light.RedTime > 60)
                        {
                            _intersection.ForceClear(light.Id);
                            continue;
                        }
                        if (!_intersection.CheckIfLightCanBeGreen(light.Id)) continue;
                        if (light.Status != (int)LightStatus.Red || light.Count <= 0) continue;
                        if (light.RedTime > 20)
                        {
                            light.SetGreen();

                            Console.WriteLine("Set to green: " + light.Id + " count:" + light.Count);
                            Console.WriteLine(light.Priority);
                        }
                         
                    }
                    else if (light.GetType().Name == "PedestrianLight")
                    {
                        if (light.Priority <= 0) continue;
                        if (light.RedTime > 60)
                        {
                            _intersection.ForceClear(light.Id);
                        }
                        if (!_intersection.CheckIfLightCanBeGreen(light.Id)) continue;
                        if (light.Status != (int)LightStatus.Red || light.Count <= 0) continue;
                        if (light.RedTime > 20)
                        {
                            light.SetGreen();

                            Console.WriteLine("Set to green: " + light.Id + " count:" + light.Count);
                            Console.WriteLine(light.Priority);
                        }

                    }
                }
            }
        }

        private void PrintDebugMessage()
        {
            Console.WriteLine("Green Lights");
            foreach (var light in _intersection.CarLights)
            {
                if (light.Status == (int)LightStatus.Green)
                    Console.WriteLine(light.Id);
            }
        }

        public override void ConsumerOnReceived(object sender, BasicDeliverEventArgs ea)
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body);

            var trafficUpdateWrapper = JsonConvert.DeserializeObject<TrafficUpdateWrapper>(message);
            var trafficUpdate = trafficUpdateWrapper.TrafficUpdate;

            _intersection.HandleUpdate(trafficUpdate);
        }

        public void SetTimer()
        {
            _timer = new Timer();
            _timer.Elapsed += OnTimedEvent;
            _timer.Interval = 1000;
            _timer.Start();
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            foreach (var light in _intersection.Lights)
            {
                light.TimerTick();
            }
            _intersection.TimerTick();
            Reply();
            //_time++;

        }

        public void Reply()
        {
            var message = CreateMessage();
            //Console.WriteLine(message);
            SendMessage(message);
        }

        private void PrintLights()
        {
            var message = CreateMessage();
            Console.WriteLine(message);
        }

        public string CreateMessage()
        {
            dynamic lightsWrapper = new
            {
                _intersection.Lights
            };

            var message = JsonConvert.SerializeObject(lightsWrapper);

            return message;
        }
    }
}
