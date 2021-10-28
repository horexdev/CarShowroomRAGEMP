using System;
using System.Collections.Generic;

#nullable disable

namespace TestServer.Database
{
    public partial class Vehicle
    {
        public int VehicleId { get; set; }
        public int? OwnerId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public int Dimension { get; set; }
        public string ModelName { get; set; }
        public int Color1 { get; set; }
        public int Color2 { get; set; }

        public virtual Profile Owner { get; set; }
    }
}
