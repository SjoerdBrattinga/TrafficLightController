using System;
using System.Collections.Generic;
using System.Linq;
using ControllerSDM.Enums;
using ControllerSDM.TrafficLights;

namespace ControllerSDM
{
    public class Intersection
    {
        public List<TrafficLight> Lights;
        public List<TrafficLight> LightsByPriority;
        public List<CarLight> CarLights;
        public BusLight BusLight;
        public List<BicycleLight> BicycleLights;
        public List<PedestrianLight> PedestrianLights;
        public List<TrainLight> TrainLights;
        public CrossingBarrier CrossingBarrier;
        
        public readonly Dictionary<int, List<int>> Checks = new Dictionary<int, List<int>>
        {
            {101, new List<int> {105, 107, 201, 301, 302, 303, 305, 401, 402, 407, 412}},
            {102, new List<int> {105, 107, 108, 109, 110, 201, 302, 303, 402, 407}}, // 501, 502
            {103, new List<int> {105, 106, 107, 109, 110, 201, 302, 303, 304, 402, 405, 407, 410}},
            {104, new List<int> {106, 110, 302, 303, 304, 403, 404, 408, 409, 201}},
            {105, new List<int> {101, 102, 103, 106, 107, 110, 201, 301, 304, 305, 401, 404, 409, 412}},
            {106, new List<int> {103, 104, 105, 109, 110, 302, 303, 403, 408, 201}}, //, 501, 502
            {107, new List<int> {101, 102, 103, 105, 109, 110, 201, 301, 305, 401, 412}}, //, 501, 502
            {108, new List<int> {102, 301, 305, 406, 411}}, //, 501, 502
            {109, new List<int> {102, 103, 106, 107, 301, 304, 305, 405, 406, 410, 411}},
            {110, new List<int> {102, 103, 104, 105, 106, 107, 201, 301, 302, 303, 305, 403, 408, 412}}, 
            {201, new List<int> {101, 102, 103, 104, 105, 106, 107, 110, 301, 302, 303, 304, 305, 401, 403, 404, 408, 409, 412}},
            {301, new List<int> {101, 105, 107, 108, 109, 110, 201}},
            {302, new List<int> {101, 102, 103, 104, 106, 110, 201}},
            {303, new List<int> {101, 102, 103, 104, 106, 110, 201}},
            {304, new List<int> {104, 105, 103, 109, 201}},
            {305, new List<int> {101, 105, 107, 108, 109, 110, 201}},
            {401, new List<int> {101, 105, 107, 201}},
            {402, new List<int> {101, 102, 103}},
            {403, new List<int> {104, 106, 110, 201}},
            {404, new List<int> {104, 105, 201}},
            {405, new List<int> {103, 109}},
            {406, new List<int> {108, 109, 110}},
            {407, new List<int> {101, 102, 103}},
            {408, new List<int> {104, 106, 110, 201}},
            {409, new List<int> {104, 105, 201}},
            {410, new List<int> {103, 109}},
            {411, new List<int> {108, 109, 110}},
            {412, new List<int> {101, 105, 107, 201}},
            {501, new List<int> {102, 106, 107, 108}},
            {502, new List<int> {102, 106, 107, 108}},
            {601, new List<int> {102, 106, 107, 108}}
            
        };

        private readonly int[] _trafficLightIds =
        {
            101,102,103,104,105,106,107,108,109,110,    // CarLights
            201,                                        // BusLight
            301,302,303,304,305,                        // BicycleLights
            401,402,403,404,405,406,                    // PedestrianLights
            407,408,409,410,411,412,
            501,502,                                    // TrainLights
            601                                         // CrossingBarrier
        };

        private int _lastClear;

        public Intersection()
        {
            Lights = new List<TrafficLight>();

            LightsByPriority = new List<TrafficLight>();

            CarLights = new List<CarLight>();
            BicycleLights = new List<BicycleLight>();
            PedestrianLights = new List<PedestrianLight>();
            TrainLights = new List<TrainLight>();

            CreateLights();
        }

        private void CreateLights()
        {
            foreach (var id in _trafficLightIds)
            {
                if (id > 100 && id <= 110)
                {
                    var carLight = new CarLight(id);
                    CarLights.Add(carLight);
                }
                else if (id == 201)
                {
                    BusLight = new BusLight(id);
                }
                else if (id > 300 && id <= 305)
                {
                    var bicycleLight = new BicycleLight(id);
                    BicycleLights.Add(bicycleLight);
                }
                else if (id > 400 && id <= 412)
                {
                    var pedestrianLight = new PedestrianLight(id);
                    PedestrianLights.Add(pedestrianLight);
                }
                else if (id > 500 && id <= 502)
                {
                    var trainLight = new TrainLight(id);
                    TrainLights.Add(trainLight);
                }
                else if (id == 601)
                {
                    CrossingBarrier = new CrossingBarrier(id);
                }
            }

            Lights.AddRange(CarLights);
            Lights.Add(BusLight);
            Lights.AddRange(BicycleLights);
            Lights.AddRange(PedestrianLights);
            Lights.AddRange(TrainLights);
            Lights.Add(CrossingBarrier);
        }

        public void HandleUpdate(TrafficUpdate trafficUpdate)
        {
            var light = GetLightById(trafficUpdate.LightId);
            light.HandleUpdate(trafficUpdate);
        }

        public void BusClear(int lightId)
        {
            if (_lastClear <= 10) return;

            Console.WriteLine("ForceClear for: " + lightId);
            var checkList = Checks[lightId];
           
            foreach (var id in checkList)
            {
                var light = GetLightById(id);

                if (light.Status != (int) LightStatus.Green || light.IsChangingColor) continue;

                light.SetRed();
                Console.WriteLine("Set to red: " + light.Id);
            }
            if (!CheckIfLightCanBeGreen(lightId)) return;

            Console.WriteLine("Light can be green");
            GetLightById(lightId).SetGreen();
            Console.WriteLine("Set to green: " + lightId);
            _lastClear = 0;
        }
        
        public void ForceClear(int lightId)
        {
            if (_lastClear <= 10) return;

            Console.WriteLine("ForceClear for: " + lightId);
            var checkList = Checks[lightId];

            foreach (var id in checkList)
            {
                var light = GetLightById(id);

                if (light.Status != (int) LightStatus.Green || light.IsChangingColor) continue;

                light.SetRed();
                Console.WriteLine("Set to red: " + light.Id);
            }

            if (!CheckIfLightCanBeGreen(lightId)) return;

            Console.WriteLine("Light can be green");
            GetLightById(lightId).SetGreen();
            Console.WriteLine("Set to green: " + lightId);
            Console.WriteLine("ForceClear done");
            _lastClear = 0;
        }

        public void OrderByPriority()
        {
            LightsByPriority = Lights.OrderByDescending(x => x.Priority).ToList();
        }

        public bool CheckIfLightCanBeGreen(int lightId)
        {
            var checkList = Checks[lightId];

            foreach (var id in checkList)
            {
                var light = GetLightById(id);
                switch (light.Status)
                {
                    case (int) LightStatus.Green:
                        return false;
                    case (int) LightStatus.Red when light.IsChangingColor:
                        return false;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
            }
            return true; 
        }

        public TrafficLight GetLightById(int lightId)
        {
            var light = Lights.Find(x => x.Id == lightId);
            return light;
        }

        public void TimerTick()
        {
            _lastClear++;
        }
    }
}
