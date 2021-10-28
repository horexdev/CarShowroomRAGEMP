using System.Linq;
using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using TestServer.Database;

namespace TestServer.Core
{
    public class Resources : Script
    {
        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            using var context = new EntryContext();

            var showrooms = context.Showrooms.AsNoTracking().ToList();

            foreach (var showroom in showrooms)
            {
                showroom.Load();
            }
        }
    }
}