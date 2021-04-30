using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace FiveMPathLogger
{
    public class Class1 : BaseScript
    {
        private List<Vector3> path = new List<Vector3>();
        bool record = false;
        int recTime = 50;
        int interval = 50;
        public Class1()
        {
            Tick += onTick;
            EventHandlers["onClientResourceStart"] += new Action<string>(OnClientResourceStart);
        }

        private void OnClientResourceStart(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName) return;

            RegisterCommand("path", new Action<int, List<object>, string>((source, args, raw) =>
            {
                try
                {
                    interval = int.Parse(args[0].ToString());
                }
                catch (Exception)
                {
                    interval = 50;
                }
                
                record = !record;
            }), false);

            RegisterCommand("pathClear", new Action<int, List<object>, string>((source, args, raw) =>
            {
                record = false;
                path.Clear();

            }), false);

            RegisterCommand("pathDist", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (path.Count >= 2)
                {
                    float dist = 0;
                    for (int i = 1; i < path.Count; i++)
                    {
                        dist += CalcDist(path[i-1], path[i]);
                    }

                    TriggerEvent("chat:addMessage", new
                    {
                        color = new[] { 255, 0, 0 },
                        args = new[] {$"[PathRecorder]", $"Distance traveled: ^*{dist / 1000}km!" }
                    });
                }

            }), false);
        }

        float CalcDist(Vector3 p1, Vector3 p2)
        {
            return GetDistanceBetweenCoords(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, true);
        }
        public async Task onTick()
        {
            
            if (path.Count >= 2)
            {
                for (int i = 1; i < path.Count; i++)
                {
                    DrawALine(path[i-1], path[i]);
                }
            }

            if (record)
            {
                recTime -= 1;
                if (recTime <= 0)
                {
                    recTime = interval;
                    path.Add(Game.PlayerPed.Position);
                }
            }
        }

        void DrawALine(Vector3 p1, Vector3 p2)
        {
            DrawLine(p1.X, p1.Y, p1.Z, p2.X, p2.Y, p2.Z, 255, 255, 255, 255);
        }
    }
}
