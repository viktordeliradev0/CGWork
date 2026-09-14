using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CGWork.Models;

namespace CGWork.Data
{
    // Наследяваме IdentityDbContext<ApplicationUser>, за да имаме поддръжка за потребители, роли и Identity таблиците
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Таблици в базата данни
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<LessonSession> LessonSessions { get; set; }
        public DbSet<Semester> Semesters { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Задължително извикване на базовия метод за правилната конфигурация на Identity таблиците
            base.OnModelCreating(builder);

            // 1. Спираме каскадното изтриване на учениците при изтриване на група
            builder.Entity<Student>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // 2. Спираме каскадното изтриване на уроците при изтриване на група
            builder.Entity<LessonSession>()
                .HasOne(l => l.Group)
                .WithMany(g => g.Lessons)
                .HasForeignKey(l => l.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Настройка на връзката "много-към-много" между Ученици и Уроци за присъствията (AttendedLessons)
            builder.Entity<Student>()
                .HasMany(s => s.AttendedLessons)
                .WithMany(l => l.AttendingStudents)
                .UsingEntity(j => j.ToTable("StudentLessonAttendances"));
        }
    }
}