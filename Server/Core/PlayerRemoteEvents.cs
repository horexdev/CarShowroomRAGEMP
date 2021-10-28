using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTANetworkAPI;
using TestServer.Database;
using TestServer.ShowroomComponents;
using Vehicle = GTANetworkAPI.Vehicle;

namespace TestServer.Core
{
    public class PlayerRemoteEvents : Script
    {
        [RemoteEvent("OpenShowroomMenu")]
        public void OpenShowroomMenu(Player player)
        {
            var colShape = player.GetData<ColShape>("ColShape");

            if (colShape == null)
                return;

            using var context = new EntryContext();

            var showroomId = colShape.GetData<int>("ShowroomId");
            var showroom = context.Showrooms.FirstOrDefault(s => s.Id == showroomId);

            if (showroom == null)
            {
                player.SendNotification("Не удалось открыть меню автосалона");

                return;
            }

            var position = NAPI.Util.FromJson(showroom.CameraPosition);

            player.TriggerEvent("SetCamera", position[0], position[1], position[2], position[3], position[4], position[5]);

            var vehicleListWithPrices = showroom.GetVehicles();
            var vehiclesList = vehicleListWithPrices.Keys.ToList();

            var vehiclesNames = string.Join(", ", vehiclesList);

            player.SendNotification($"Вы зашли в автосалон \"{showroom.Name}\"");

            if (vehiclesList.Count == 0)
            {
                player.SendNotification("В автосалоне нет машин");

                player.TriggerEvent("CloseShowroomMenu");
            }
            else
            {
                var vehicleName = vehiclesList.First();
                var vehiclePosition = NAPI.Util.FromJson<Vector3>(showroom.FirstVehicleSpawnPoint);
                var dimension = (uint)(player.Id + showroomId);

                player.SendNotification($"Список доступных авто: {vehiclesNames}");
                player.TriggerEvent("MoveCamera", vehiclePosition.X, vehiclePosition.Y, vehiclePosition.Z);
                player.Dimension = dimension;

                var vehicle = NAPI.Vehicle.CreateVehicle(NAPI.Util.VehicleNameToModel(vehicleName), vehiclePosition, -145, 3, 3,
                    "SHOWROOM", dimension: dimension);
                player.SendChatMessage($"Автомобиль: {vehicleName} | Цена: {vehicleListWithPrices[vehicleName]}");

                player.SetData("ShowroomVehicle", vehicle);
                player.SetData("ShowroomVehicleName", vehicleName);
                player.SetData("ShowroomVehiclePrice", vehicleListWithPrices[vehicleName]);
                player.SetData("VehicleAfterBuySpawnPosition", NAPI.Util.FromJson<Vector3>(showroom.SecondVehicleSpawnPoint));
                player.SetData("ShowroomVehicleColor1", vehicle.PrimaryColor);
                player.SetData("ShowroomVehicleColor2", vehicle.SecondaryColor);
            }
        }

        [RemoteEvent("CloseShowroomMenu")]
        public void CloseShowroomMenu(Player player)
        {
            var vehicle = player.GetData<Vehicle>("ShowroomVehicle");

            vehicle?.Delete();

            player.Dimension = 0;
            player.ResetData("ShowroomVehicle");
            player.ResetData("ShowroomVehicleName");
            player.ResetData("ShowroomVehiclePrice");
            player.ResetData("ShowroomId");
            player.ResetData("VehicleAfterBuySpawnPosition");
            player.ResetData("ShowroomVehicleColor1");
            player.ResetData("ShowroomVehicleColor2");
        }

        /// <param name="player"></param>
        /// <param name="way">0 - Лево, 1 - Право</param>
        [RemoteEvent("ChangeShowroomVehicle")]
        public void ChangeShowroomVehicle(Player player, int way)
        {
            var colShape = player.GetData<ColShape>("ColShape");

            if (colShape == null)
            {
                return;
            }

            using var context = new EntryContext();

            var showroomId = colShape.GetData<int>("ShowroomId");
            var showroom = context.Showrooms.FirstOrDefault(s => s.Id == showroomId);

            if (showroom == null)
            {
                return;
            }

            var vehicleListWithPrices = showroom.GetVehicles();
            var vehiclesNames = vehicleListWithPrices.Keys.ToList();
            var vehicle = player.GetData<Vehicle>("ShowroomVehicle");
            var vehicleName = player.GetData<string>("ShowroomVehicleName");
            var vehiclePosition = NAPI.Util.FromJson<Vector3>(showroom.FirstVehicleSpawnPoint);
            var dimension = (uint)(player.Id + showroomId);

            if (vehicle == null)
            {
                player.TriggerEvent("CloseShowroomMenu");

                return;
            }

            if (string.IsNullOrEmpty(vehicleName) || vehicleName == "")
            {
                player.TriggerEvent("CloseShowroomMenu");

                return;
            }

            vehicle.Delete();

            var index = vehiclesNames.IndexOf(vehicleName);

            index = way switch
            {
                0 => index == 0 ? vehiclesNames.Count - 1 : index - 1,
                1 => index == vehiclesNames.Count - 1 ? 0 : index + 1,
                _ => index
            };

            vehicleName = vehiclesNames.ElementAt(index);
            var vehiclePrice = vehicleListWithPrices[vehicleName];

            vehicle = NAPI.Vehicle.CreateVehicle(NAPI.Util.VehicleNameToModel(vehicleName), vehiclePosition, -145, 3, 3, "SHOWROOM", dimension: dimension);

            player.SetData("VehicleAfterBuySpawnPosition", NAPI.Util.FromJson<Vector3>(showroom.SecondVehicleSpawnPoint));
            player.SendChatMessage($"Автомобиль: {vehicleName} | Цена: {vehiclePrice}");
            player.SetData("ShowroomVehicle", vehicle);
            player.SetData("ShowroomVehiclePrice", vehiclePrice);
            player.SetData("ShowroomVehicleName", vehiclesNames[index]);
            player.SetData("ShowroomVehicleColor1", vehicle.PrimaryColor);
            player.SetData("ShowroomVehicleColor2", vehicle.SecondaryColor);
        }

        [RemoteEvent("EmergencyTestDriveStop")]
        public void EmergencyTestDriveStop(Player player)
        {
            var testDrive = player.GetData<TestDrive>("TestDrive");

            if (testDrive == null)
                return;

            player.SendChatMessage("Вы разбили машину 0_0");
            testDrive.Stop();

            player.ResetData("TestDrive");
        }
    }
}