using TestServer.Partials;

namespace TestServer.Database
{
    public partial class Profile : IEntity
    {
        public void Load()
        {
            throw new System.NotImplementedException();
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