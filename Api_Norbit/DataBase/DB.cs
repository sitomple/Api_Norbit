using Api_Norbit.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Norbit.DataBase
{
    /// <summary>
    /// Хранит в себе модели БД и подключение к ней.
    /// </summary>
    public class DB : DbContext
    {
        public DbSet<UserModel> User { get; set; }
        public DbSet<User_markModel> User_mark { get; set; }
        public DbSet<SubjectModel> Subject { get; set; }
        public DbSet<RoleModel> Role { get; set; }
        public DbSet<ClassModel> Grade { get; set; } 
        public DB()
        {
            Database.EnsureCreated();
        }
        /// <summary>
        /// Осуществляет подключение к бд
        /// </summary>
        /// <param name="optionsBuilder">Передаваемый сборщик настроек для подключения</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=School Diary;Username=postgres;Password=Qwerty1qaz");
        }
    }
}
