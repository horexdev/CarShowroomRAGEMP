using System;
using System.Collections.Generic;

#nullable disable

namespace TestServer.Database
{
    public partial class Showroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public short ShowroomClass { get; set; }
        public string VehiclesList { get; set; }
        public string FirstVehicleSpawnPoint { get; set; }
        public string SecondVehicleSpawnPoint { get; set; }
        public string TestDriveVehicleSpawnPoint { get; set; }
        public string CameraPosition { get; set; }
        public int? OwnerId { get; set; }

        public virtual Profile Owner { get; set; }
    }
}
