using EF7ColumnJSON.Entities;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp1.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Halfling> Halflings => Set<Halfling>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //User
            modelBuilder.Entity<User>().HasKey(t => t.Id);

            modelBuilder.Entity<User>().Property(t => t.Username)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<User>()
                .OwnsOne(b => b.Address, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });

            modelBuilder.Entity<User>()
                .OwnsMany(b => b.Posts, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(post => post.Comments);
                });

            ////Post
            //modelBuilder.Entity<Post>().HasKey(t => t.Id);

            //modelBuilder.Entity<Post>().Property(t => t.Title)
            //    .HasMaxLength(200)
            //    .IsRequired();

            //modelBuilder.Entity<Post>().Property(t => t.Body)
            //    .HasMaxLength(4000)
            //    .IsRequired();

            ////Comment
            //modelBuilder.Entity<Comment>().HasKey(t => t.Id);

            //modelBuilder.Entity<Comment>().Property(t => t.Name)
            //    .HasMaxLength(100)
            //    .IsRequired();

            //modelBuilder.Entity<Comment>().Property(t => t.Body)
            //    .HasMaxLength(1000)
            //    .IsRequired();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }


        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.MigrateAsync();

            // seed data
            var jsonPath = @"data.json";
            var json = File.ReadAllText(jsonPath);
            var posts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Post>>(json);

            posts[0].User.Address = new Address("Street 1", "Sevilla", "Sevilla", "Spain", "111");
            posts[1].User.Address = new Address("Street 2", "Madrid", "Madrid", "Spain", "222");
            posts[2].User.Address = new Address("Street 3", "Barcelona", "Barcelona", "Spain", "333");

            await context.AddRangeAsync(posts);
            await context.SaveChangesAsync();

            //hierarchies
            await context.AddRangeAsync(
                new Halfling(HierarchyId.Parse("/"), "Balbo", 1167),
                new Halfling(HierarchyId.Parse("/1/"), "Mungo", 1207),
                new Halfling(HierarchyId.Parse("/2/"), "Pansy", 1212),
                new Halfling(HierarchyId.Parse("/3/"), "Ponto", 1216),
                new Halfling(HierarchyId.Parse("/4/"), "Largo", 1220),
                new Halfling(HierarchyId.Parse("/5/"), "Lily", 1222),
                new Halfling(HierarchyId.Parse("/1/1/"), "Bungo", 1246),
                new Halfling(HierarchyId.Parse("/1/2/"), "Belba", 1256),
                new Halfling(HierarchyId.Parse("/1/3/"), "Longo", 1260),
                new Halfling(HierarchyId.Parse("/1/4/"), "Linda", 1262),
                new Halfling(HierarchyId.Parse("/1/5/"), "Bingo", 1264),
                new Halfling(HierarchyId.Parse("/3/1/"), "Rosa", 1256),
                new Halfling(HierarchyId.Parse("/3/2/"), "Polo"),
                new Halfling(HierarchyId.Parse("/4/1/"), "Fosco", 1264),
                new Halfling(HierarchyId.Parse("/1/1/1/"), "Bilbo", 1290),
                new Halfling(HierarchyId.Parse("/1/3/1/"), "Otho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/"), "Falco", 1303),
                new Halfling(HierarchyId.Parse("/3/2/1/"), "Posco", 1302),
                new Halfling(HierarchyId.Parse("/3/2/2/"), "Prisca", 1306),
                new Halfling(HierarchyId.Parse("/4/1/1/"), "Dora", 1302),
                new Halfling(HierarchyId.Parse("/4/1/2/"), "Drogo", 1308),
                new Halfling(HierarchyId.Parse("/4/1/3/"), "Dudo", 1311),
                new Halfling(HierarchyId.Parse("/1/3/1/1/"), "Lotho", 1310),
                new Halfling(HierarchyId.Parse("/1/5/1/1/"), "Poppy", 1344),
                new Halfling(HierarchyId.Parse("/3/2/1/1/"), "Ponto", 1346),
                new Halfling(HierarchyId.Parse("/3/2/1/2/"), "Porto", 1348),
                new Halfling(HierarchyId.Parse("/3/2/1/3/"), "Peony", 1350),
                new Halfling(HierarchyId.Parse("/4/1/2/1/"), "Frodo", 1368),
                new Halfling(HierarchyId.Parse("/4/1/2.5/1/"), "Cuong", 1994),
                new Halfling(HierarchyId.Parse("/4/1/3/1/"), "Daisy", 1350),
                new Halfling(HierarchyId.Parse("/3/2/1/1/1/"), "Angelica", 1381));

            await context.SaveChangesAsync();

        }
    }
}
