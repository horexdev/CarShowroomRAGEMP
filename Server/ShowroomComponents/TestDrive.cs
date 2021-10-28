using System.Timers;
using GTANetworkAPI;
using Vehicle = GTANetworkAPI.Vehicle;

namespace TestServer.ShowroomComponents
{
    public class TestDrive
    {
        public Player Player { get; }

        public Vehicle Vehicle { get; }

        public Timer Timer { get; }

        public delegate void TestDriveHandler(int playerId);
        public event TestDriveHandler RemoveTestDrive;

        public TestDrive(Player player, Vehicle vehicle, Timer timer)
        {
            Player = player; 
            Vehicle = vehicle;
            Timer = timer;
        }

        public void Start()
        {
            Player.TriggerEvent("CloseShowroomMenu");
            Player.TriggerEvent("TestDriveStatus", true);
            Player.SendChatMessage("[Тестовое время] Для тест драйва у вас есть 15 секунд");

            Timer.Elapsed += (sender, args) =>
            {
                Stop();
            };

            Timer.Start();
        }

        public void Stop()
        {
            NAPI.Task.Run(() =>
            {
                Vehicle?.Delete();

                Player.SendChatMessage("Тест драйв окончен");
                Player.Position = Player.GetData<Vector3>("LastPosition");
                Player.Dimension = 0;
                Player.TriggerEvent("TestDriveStatus", false);

                RemoveTestDrive?.Invoke(Player.Id);
            });

            Timer.Dispose();
        }
    }
}