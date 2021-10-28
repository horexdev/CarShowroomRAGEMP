using System;
using System.Collections.Generic;

#nullable disable

namespace TestServer.Database
{
    public partial class Profile
    {
        public Profile()
        {
            Showrooms = new HashSet<Showroom>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int ProfileId { get; set; }
        public string ProfileName { get; set; }
        public decimal Cash { get; set; }
        public decimal BankMoney { get; set; }

        public virtual ICollection<Showroom> Showrooms { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
