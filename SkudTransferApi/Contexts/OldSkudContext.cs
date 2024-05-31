using Microsoft.EntityFrameworkCore;
using ASPWeb.Models;
using ControllerDomain.Entities;

namespace SkudTransferApi.Contexts
{
    public class OldSkudContext : DbContext
    {
        public DbSet<Users> Users { get; set; } = null!;
        public DbSet<Cards> Cards { get; set; } = null!;
        public DbSet<ASPWeb.Models.Controllers> Controllers { get; set; } = null!;
        public DbSet<Events> Events { get; set; } = null!;
        public DbSet<EventCodes> EventCodes { get; set; } = null!;
        public DbSet<Workers> Workers { get; set; } = null!;
        public DbSet<Groups> Groups { get; set; } = null!;
        public DbSet<AccessGroups> AccessGroups { get; set; } = null!;
        public DbSet<MessagesDB> Messages { get; set; } = null!;
        public DbSet<RelationsControllersAccessGroups> RelationsControllersAccessGroups { get; set; } = null!;
        public DbSet<RelationsControllersWorkers> RelationsControllersWorkers { get; set; } = null!;
        public OldSkudContext(DbContextOptions<OldSkudContext> options) : base(options)
        {
        }
    }
}
