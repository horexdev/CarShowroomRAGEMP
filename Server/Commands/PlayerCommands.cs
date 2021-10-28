using System.Linq;
using System.Timers;
using GTANetworkAPI;
using TestServer.Database;
using TestServer.ShowroomComponents;
using Vehicle = GTANetworkAPI.Vehicle;

namespace TestServer.Commands
{
    public class PlayerCommands : Script
    {
        /// <param name="player"></param>
        /// <param name="paymentType">0 - Наличными, 1 - Безналичными</param>
        [Command("buyveh")]
        public void BuyVehicle(Player player, int paymentType)
        {
            var isPlayerOnShowroomColShape = player.GetSharedData<bool>("IsOnShowroomColShape");
            var profile = player.GetData<Profile>("Profile");

            if (profile == null)
            {
                player.TriggerEvent("CloseShowroomMenu");

                return;
            }

            var vehiclePrice = player.GetData<decimal>("ShowroomVehiclePrice");
            var vehicleName = player.GetData<string>("ShowroomVehicleName");

            if (string.IsNullOrEmpty(vehicleName) || vehicleName == "" || vehiclePrice <= 0)
            {
                player.TriggerEvent("CloseShowroomMenu");

                return;
            }

            if (!isPlayerOnShowroomColShape)
            {
                player.TriggerEvent("CloseShowroomMenu");

                return;
            }

            switch (paymentType)
            {
                case 0:
                    if (profile.Cash < vehiclePrice)
                    {
                        player.TriggerEvent("CloseShowroomMenu");
                        player.SendChatMessage("У вас недостаточно денег :(");

                        return;
                    }

                    profile.Cash -= vehiclePrice;
                    break;
                case 1:
                    if (profile.BankMoney < vehiclePrice)
                    {
                        player.TriggerEvent("CloseShowroomMenu");
                        player.SendChatMessage("У вас недостаточно денег :(");

                        return;
                    }

                    profile.BankMoney -= vehiclePrice;
                    break;
                default:
                    player.TriggerEvent("CloseShowroomMenu");
                    return;
            }

            var color1 = player.GetData<int>("ShowroomVehicleColor1");
            var color2 = player.GetData<int>("ShowroomVehicleColor2");

            var vehiclePosition = player.GetData<Vector3>("VehicleAfterBuySpawnPosition");
            var vehicleEntity = NAPI.Vehicle.CreateVehicle(NAPI.Util.VehicleNameToModel(vehicleName), vehiclePosition, 0f, color1, color2, dimension: 0);

            var vehicle = new Database.Vehicle(vehicleName, profile.ProfileId, vehiclePosition.X, vehiclePosition.Y, vehiclePosition.Z, 0)
            {
                Entity = vehicleEntity,
                Color1 = color1,
                Color2 = color2
            };

            profile.Update();
            vehicle.Save();

            player.TriggerEvent("CloseShowroomMenu");
            player.SendChatMessage("Вы успешно купили машину");
        }

        [Command("color")]
        public void ChangeVehicleColor(Player player, int color1, int color2)
        {
            var isPlayerOnShowroomColShape = player.GetSharedData<bool>("IsOnShowroomColShape");

            if (!isPlayerOnShowroomColShape)
                return;

            var vehicle = player.GetData<Vehicle>("ShowroomVehicle");

            if (vehicle == null)
            {
                player.TriggerEvent("CloseShowroomMenu");
                player.SendChatMessage("Произошла ошибка при смене цвета, NullReferenceException.");

                return;
            }

            vehicle.PrimaryColor = color1;
            vehicle.SecondaryColor = color2;
        }

        [Command("testdrive")]
        public void TurnOnTestDrive(Player player)
        {
            var isPlayerOnShowroomColShape = player.GetSharedData<bool>("IsOnShowroomColShape");

            if (!isPlayerOnShowroomColShape)
                return;

            using var context = new EntryContext();

            var showroom = context.Showrooms.FirstOrDefault(s => s.Id == player.GetData<ColShape>("ColShape").GetData<int>("ShowroomId"));

            if (showroom == null)
            {
                player.TriggerEvent("CloseShowroomMenu");
                player.SendChatMessage("Произошла ошибка при попытке тест драйва, NullReferenceException.");

                return;
            }

            var showroomVehicle = player.GetData<Vehicle>("ShowroomVehicle");
            var vehicle = NAPI.Vehicle.CreateVehicle(showroomVehicle.Model,
                NAPI.Util.FromJson<Vector3>(showroom.TestDriveVehicleSpawnPoint), 90f, 5, 5, "TEST");

            if (vehicle == null)
            {
                player.TriggerEvent("CloseShowroomMenu");
                player.SendChatMessage("Произошла ошибка при попытке тест драйва, NullReferenceException.");

                return;
            }

            NAPI.Task.Run(() =>
            {
                player.SetIntoVehicle(vehicle.Handle, 0);
            }, 250);

            player.SetData("LastPosition", player.Position);

            var testDrive = new TestDrive(player, vehicle, new Timer(15000));
            showroom.TryAddTestDrive(testDrive);

            testDrive.RemoveTestDrive += id => showroom.RemoveTestDrive(id);
            testDrive.Start();

            player.SetData("TestDrive", testDrive);
        }
    }
}