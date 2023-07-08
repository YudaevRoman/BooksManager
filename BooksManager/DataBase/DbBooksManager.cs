using System;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BooksManager
{
    public partial class DbBooksManager : DbContext
    {
        public DbBooksManager() { }
        public virtual DbSet<Author> Author { get; set; }
        public virtual DbSet<AuthorBooks> AuthorBooks { get; set; }
        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BooksAddedByPerson> BooksAddedByPerson { get; set; }
        public virtual DbSet<BooksCategory> BooksCategory { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<ImageFile> ImageFile { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<PersonRole> PersonRole { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "data source=LEERE;" +
                    "initial catalog=BooksManager;" +
                    "integrated security=True;" +
                    "MultipleActiveResultSets=True;" +
                    "TrustServerCertificate=True;" +
                    "App=EntityFramework"
                    );
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Author>()
                .HasMany(e => e.AuthorBooks)
                .WithOne(e => e.Author)
                .HasForeignKey(e => e.AuthotBooks_Author);

            modelBuilder.Entity<Book>()
                .HasMany(e => e.AuthorBooks)
                .WithOne(e => e.Book)
                .HasForeignKey(e => e.AuthorBooks_Book);

            modelBuilder.Entity<Book>()
                .HasMany(e => e.BooksAddedByPerson)
                .WithOne(e => e.Book)
                .HasForeignKey(e => e.BooksAddedByPerson_Book);

            modelBuilder.Entity<Book>()
                .HasMany(e => e.BooksCategory)
                .WithOne(e => e.Book)
                .HasForeignKey(e => e.BooksCategory_Book);

            modelBuilder.Entity<Category>()
                .HasMany(e => e.BooksCategory)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.BooksCategory_Category);

            modelBuilder.Entity<ImageFile>()
                .HasMany(e => e.Author)
                .WithOne(e => e.ImageFile)
                .HasForeignKey(e => e.Author_Image);

            modelBuilder.Entity<ImageFile>()
                .HasMany(e => e.Book)
                .WithOne(e => e.ImageFile)
                .HasForeignKey(e => e.Book_Image);

            modelBuilder.Entity<ImageFile>()
                .HasMany(e => e.Person)
                .WithOne(e => e.ImageFile)
                .HasForeignKey(e => e.Person_Image);

            modelBuilder.Entity<Person>()
                .HasMany(e => e.BooksAddedByPerson)
                .WithOne(e => e.Person)
                .HasForeignKey(e => e.BooksAddedByPerson_Person);

            modelBuilder.Entity<PersonRole>()
                .HasMany(e => e.Person)
                .WithOne(e => e.PersonRole)
                .HasForeignKey(e => e.Person_Role);
        }
    }
}
