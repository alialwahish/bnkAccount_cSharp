    using Microsoft.EntityFrameworkCore;

    namespace LoginReg
    {
        public class MyContext : DbContext{
            public MyContext(DbContextOptions<MyContext> options) :base(options){ }

            public DbSet<User> users {get;set;}
        }

    }