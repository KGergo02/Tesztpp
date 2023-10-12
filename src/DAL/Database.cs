﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Teszt__.src.Models;
using Teszt__.src.Services;

namespace Teszt__.src.DAL
{
    public class Database : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql("datasource=localhost;port=3306;username=root;password=;database=teszt++");
        }

        public User GetUserByName(string name)
        {
            try
            {
                return this.Users.Where(b => b.name == name).ToList().Count == 1 ? this.Users.Where(b => b.name == name).ToList()[0] : null;
            }
            catch(InvalidOperationException)
            {
                Message.Error("Nem sikerült kapcsolódni az adatbázishoz!");

                return null;
            }
        }
    }
}