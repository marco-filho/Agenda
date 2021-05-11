using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Agenda.Desktop.Data;
using Agenda.Desktop.Models;
using Microsoft.AspNetCore.Identity;

namespace Agenda.Desktop.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the AppIdentityUser class
    public class AppIdentityUser : IdentityUser
    {
        public override string Id { get; set; }
        public string Nome { get; set; }
    }
}
