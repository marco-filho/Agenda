using Agenda.Desktop.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Desktop.Data
{
    class DataService : IDataService
    {
        private readonly ApplicationDbContext context;
        private readonly AppIdentityContext identityContext;

        public DataService(ApplicationDbContext context, AppIdentityContext identityContext)
        {
            this.context = context;
            this.identityContext = identityContext;
        }

        public void StartDB()
        {
            context.Database.Migrate();
            identityContext.Database.Migrate();
        }
    }

    internal interface IDataService
    {
        public void StartDB();
    }
}