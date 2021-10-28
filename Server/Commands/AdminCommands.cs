using System.Collections.Generic;
using GTANetworkAPI;
using TestServer.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace TestServer.Commands
{
    public class AdminCommands : Script
    {
        [Command("setupshowroom")]
        private void SetUpShowroom(Player player, string showroomName, short showroomClass)
        {
            var showroom = new Showroom(showroomName, showroomClass, player.Position.X, player.Position.Y, player.Position.Z);

            showroom.Save();
            showroom.Load();
        }

        [Command("tpshowroom")]
        private void TpToShowroom(Player player, int showroomId)
        {
            using var context = new EntryContext();

            var showroom = context.Showrooms.AsNoTracking().FirstOrDefault(s => s.Id == showroomId);

            if (showroom == null)
            {
                player.SendNotification("Автосалона под таким Id не существует");
            }
            else
            {
                player.Position = new Vector3(showroom.X, showroom.Y, showroom.Z);

                player.SendNotification($"Вы телепортировались к автосалону {showroom.Name}");
            }
        }

        [Command("setvehs", GreedyArg = true)]
        public void SetVehiclesToShowroom(Player player, int showroomId, string names)
        {
            using var context = new EntryContext();

            var showroom = context.Showrooms.FirstOrDefault(s => s.Id == showroomId);

            if (showroom == null)
            {
                player.SendNotification("Автосалона под таким Id не существует");
            }
            else
            {
                var vehiclesNames = names.Split(' ').ToList();
                var vehicles = vehiclesNames.ToDictionary(name => name, name => 0m);

                showroom.VehiclesList = NAPI.Util.ToJson(vehicles);
                showroom.Update();
            }
        }

        [Command("setspawnpoint")]
        public void SetFirstVehicleSpawnPoint(Player player, int showroomId, int spawnPointNumber)
        {
            using var context = new EntryContext();

            var showroom = context.Showrooms.FirstOrDefault(s => s.Id == showroomId);

            if (showroom == null)
            {
                player.SendNotification("Автосалона под таким Id не существует");
            }
            else
            {
                var position = new Vector3(player.Position.X, player.Position.Y, player.Position.Z);

                switch (spawnPointNumber)
                {
                    case 1:
                        showroom.FirstVehicleSpawnPoint = NAPI.Util.ToJson(position);
                        break;
                    case 2:
                        showroom.SecondVehicleSpawnPoint = NAPI.Util.ToJson(position);
                        break;
                }

                showroom.Update();
            }
        }

        [Command("setcameraposition")]
        public void SetCameraPosition(Player player, int showroomId)
        {
            using var context = new EntryContext();

            var showroom = context.Showrooms.FirstOrDefault(s => s.Id == showroomId);

            if (showroom == null)
            {
                player.SendNotification("Автосалона под таким Id не существует");
            }
            else
            {
                var position = new List<float> { player.Position.X, player.Position.Y, player.Position.Z, player.Rotation.X, player.Rotation.Y, player.Rotation.Z };

                showroom.CameraPosition = NAPI.Util.ToJson(position);

                showroom.Update();
            }
        }

        [Command("testdrivespawn")]
        public void SetTestDriveVehicleSpawnPoint(Player player, int showroomId)
        {
            using var context = new EntryContext();

            var showroom = context.Showrooms.FirstOrDefault(s => s.Id == showroomId);

            if (showroom == null)
            {
                player.SendNotification("Автосалона под таким Id не существует");
            }
            else
            {
                var position = new Vector3(player.Position.X, player.Position.Y, player.Position.Z);

                showroom.TestDriveVehicleSpawnPoint = NAPI.Util.ToJson(position);

                showroom.Update();
            }
        }

        [Command("freezelocal")]
        public void FreezeLocalPlayer(Player player, bool freeze)
        {
            player.TriggerEvent("FreezeLocalPlayer", freeze);
        }

        [Command("dimension")]
        public void SetDimension(Player player, int dimension)
        {
            if (dimension < 0)
            {
                player.SendChatMessage($"Вы сейчас в {player.Dimension} измерении");

                return;
            }

            player.Dimension = (uint) dimension;
        }

        [Command("tpcoord")]
        public void TpToCoord(Player player, float x, float y, float z)
        {
            player.Position = new Vector3(x, y, z);
        }

        [Command("getintoveh")]
        public void GetIntoVehicle(Player player, int vehId)
        {
            NAPI.Player.SetPlayerIntoVehicle(player, NAPI.Pools.GetAllVehicles().ElementAt(vehId).Handle, 0);
        }
    }
}