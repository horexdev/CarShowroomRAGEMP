using System.ComponentModel.DataAnnotations.Schema;
using GTANetworkAPI;
using TestServer.Partials;

namespace TestServer.Database
{
    public partial class Vehicle : IEntity
    {
        [NotMapped]
        public GTANetworkAPI.Vehicle Entity;

        public Vehicle(){}

        public Vehicle(string modelName, int ownerId, float x, float y, float z, uint dimension)
        {
            ModelName = modelName;
            OwnerId = ownerId;
            X = x;
            Y = y;
            Z = z;
            Dimension = (int)dimension;
        }

        public void Load()
        {
            Entity = NAPI.Vehicle.CreateVehicle(NAPI.Util.VehicleNameToModel(ModelName), new Vector3(X, Y, Z), 0f,
                Color1, Color2, dimension: (uint)Dimension);
        }

        public void Save()
        {
            using var context = new EntryContext();

            context.Add(this);
            context.SaveChanges();
        }

        public void Update()
        {
            using var context = new EntryContext();

            context.Update(this);
            context.SaveChanges();
        }
    }
}