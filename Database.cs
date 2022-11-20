﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Data.SqlTypes;

namespace TRPO_3
{
    public enum TableType
    {
        Client,
        Attorney,
        Case,
        Archive
    }

    public class AttorneyContext : DbContext
    {
        public DbSet<Human> Human { get; set; }
        public DbSet<Attorney> Attorney { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<Case> Case { get; set; }

        public string DbPath { get; }


        public AttorneyContext()
        {
            var folder = Environment.SpecialFolder.MyDocuments;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "database.db");
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}")
            .LogTo(s => System.Diagnostics.Debug.WriteLine(s));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Human>()
                .Property(b => b.HumanId)
                .IsRequired();

            modelBuilder.Entity<Human>()
                .Ignore(x => x.FullName);

            modelBuilder.Entity<Attorney>()
                .Property(b => b.AttorneyId)
                .IsRequired();

            modelBuilder.Entity<Client>()
                .Property(b => b.ClientId)
                .IsRequired();

            modelBuilder.Entity<Case>()
                .Property(b => b.CaseId)
                .IsRequired();
        }
    }

    public class Human
    {
        public int HumanId;
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }

        public virtual string FullName => $"{Name} {Surname} {Patronymic}";
    }

    public class Attorney
    {
        public int AttorneyId;

        public Attorney()
        {

        }

        public Attorney(Human human)
        {
            Human = human;
            HumanId = human.HumanId;
        }

        public int HumanId;
        public virtual Human Human { get; set; }
    }

    public class Client
    {
        public int ClientId;
        public Client()
        {
        }

        public Client(Human human)
        {
            Human = human;
            HumanId = human.HumanId;
        }

        public int HumanId;
        public virtual Human Human { get; set; }
    }

    public class Case
    {
        public int CaseId;
        public int AttorneyId;
        public int ClientId;
        public virtual Client Client { get; set; }

        public Case()
        {

        }

        public Case(Attorney attorney, Client client, int pay, string startDate, string endDate, string comment = "")
        {
            Client = client;
            ClientId = client.ClientId;
            AttorneyId = attorney.AttorneyId;
            Attorney = attorney;
            Pay = new SqlMoney(pay);
            StartDate = DateTime.Parse(startDate);
            EndDate = DateTime.Parse(endDate);
            Comment = comment;
        }

        public virtual Attorney Attorney { get; set; }
        public DateTime StartDate;
        public DateTime EndDate;
        public SqlMoney Pay;
        public string Comment;
        public bool Archived = false;
    }
}
