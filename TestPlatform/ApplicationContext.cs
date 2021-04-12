﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using TestPlatform.Entityes;

namespace TestPlatform
{
    class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } //буфер типізований моделлю
        public DbSet<Test> Tests { get; set; } //буфер типізований моделлю
        public DbSet<Question> Questions { get; set; } //буфер типізований моделлю
        public DbSet<Answear> Answears { get; set; } //буфер типізований моделлю

        public ApplicationContext (): base("DefaultConnection") { }
    }
}
