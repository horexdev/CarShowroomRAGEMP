using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GTANetworkAPI;
using TestServer.Partials;
using TestServer.ShowroomComponents;

namespace TestServer.Database
{
    public partial class Showroom : IEntity
    {
        private const int BlipId = 672;

        private const int MarkerId = 36;

        private ColShape _colShape;

        private readonly List<TestDrive> _testDrives = new List<TestDrive>();

        public Showroom(string name, short showroomClass, float x, float y, float z)
        {
            Name = name;
            ShowroomClass = showroomClass;
            X = x;
            Y = y;
            Z = z;
        }

        public void TryAddTestDrive(TestDrive testDrive)
        {
            if (_testDrives.Contains(testDrive))
                throw new ArgumentException();

            _testDrives.Add(testDrive);
        }

        public void RemoveTestDrive(int playerId)
        {
            var testDrive = _testDrives.FirstOrDefault(t => t.Player.Id == playerId);

            if (testDrive == null)
                throw new WarningException($"Не удалось найти TestDrive с PlayerId равному {playerId}");

            testDrive.RemoveTestDrive -= RemoveTestDrive;

            _testDrives.Remove(testDrive);
        }

        public TestDrive FindTestDrive(int playerId)
        {
            var testDrive = _testDrives.FirstOrDefault(t => t.Player.Id == playerId);

            if (testDrive == null)
                throw new WarningException($"Не удалось найти TestDrive с PlayerId равному {playerId}");

            return testDrive;
        }

        public Dictionary<string, decimal> GetVehicles()
        {
            return NAPI.Util.FromJson<Dictionary<string, decimal>>(VehiclesList);
        }

        public void Load()
        {
            NAPI.Marker.CreateMarker(MarkerId, new Vector3(X, Y, Z), new Vector3(0, 0, 0),
                new Vector3(0, 0, 0), 0.8f, new Color(0, 0, 255));
            NAPI.Blip.CreateBlip(BlipId, new Vector3(X, Y, Z), 1.2f, 0);
            _colShape = NAPI.ColShape.CreateCylinderColShape(new Vector3(X, Y, Z), 1f, 1f);
            _colShape.SetData("Name", "ShowroomColShape");
            _colShape.SetData("ShowroomId", Id);
        }

        public void Save()
        {
            throw new System.NotImplementedException();
        }

        public void Update()
        {
            using var context = new EntryContext();

            context.Update(this);
            context.SaveChanges();
        }
    }
}