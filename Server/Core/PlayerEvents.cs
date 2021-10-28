using System;
using System.Linq;
using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using TestServer.Database;

namespace TestServer.Core
{
    public class PlayerEvents : Script
    {
        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Player player)
        {
            using var context = new EntryContext();

            var profile = context.Profiles.AsNoTracking().FirstOrDefault(p => p.ProfileName == player.Name);

            if (profile == null)
            {
                var newProfile = new Profile
                {
                    ProfileName = player.Name,
                    Cash = 500000,
                    BankMoney = 1000000
                };

                context.Add(newProfile);
                context.SaveChanges();

                player.SetData("Profile", newProfile);
            }
            else
            {
                player.SetData("Profile", profile);
            }

            player.Dimension = 0;
        }

        [ServerEvent(Event.PlayerEnterColshape)]
        public void OnPlayerEnterColShape(ColShape colShape, Player player)
        {
            if (colShape.GetData<string>("Name") == "ShowroomColShape")
            {
                player.SetSharedData("IsOnShowroomColShape", true);
                player.SetData("ColShape", colShape);
            }
        }

        [ServerEvent(Event.PlayerExitColshape)]
        public void OnPlayerExitColShape(ColShape colShape, Player player)
        {
            if (colShape.GetData<string>("Name") == "ShowroomColShape")
            {
                player.ResetSharedData("IsOnShowroomColShape");
                player.ResetData("ColShape");
            }
        }
    }
}