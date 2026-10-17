using LibraryManagement.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ISBN must be unique
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Isbn)
            .IsUnique();

        // Member email must be unique
        modelBuilder.Entity<Member>()
            .HasIndex(m => m.Email)
            .IsUnique();

        // Author -> Books (one-to-many)
        // Restrict: you can't delete an author who still has books
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Book <-> Category (many-to-many, EF creates the join table)
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Categories)
            .WithMany(c => c.Books);

        // Loan -> Book and Loan -> Member
        // Restrict: loan history must not disappear when a book or member is deleted
        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Book)
            .WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Loan>()
            .HasOne(l => l.Member)
            .WithMany(m => m.Loans)
            .HasForeignKey(l => l.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // ---------- Seed data ----------
        modelBuilder.Entity<Author>().HasData(
            new Author { Id = 1, Name = "George Orwell", Biography = "English novelist and essayist, known for dystopian fiction." },
            new Author { Id = 2, Name = "Jane Austen", Biography = "English novelist known for her romantic fiction and social commentary." },
            new Author { Id = 3, Name = "Isaac Asimov", Biography = "American writer and professor of biochemistry, famous for science fiction." },
            new Author { Id = 4, Name = "Agatha Christie", Biography = "English writer known for her detective novels." }
        );

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction" },
            new Category { Id = 2, Name = "Science Fiction" },
            new Category { Id = 3, Name = "Classic" },
            new Category { Id = 4, Name = "Mystery" },
            new Category { Id = 5, Name = "Romance" }
        );

        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "1984", Isbn = "9780451524935", PublishedYear = 1949, TotalCopies = 3, AuthorId = 1 },
            new Book { Id = 2, Title = "Animal Farm", Isbn = "9780451526342", PublishedYear = 1945, TotalCopies = 2, AuthorId = 1 },
            new Book { Id = 3, Title = "Pride and Prejudice", Isbn = "9780141439518", PublishedYear = 1813, TotalCopies = 2, AuthorId = 2 },
            new Book { Id = 4, Title = "Foundation", Isbn = "9780553293357", PublishedYear = 1951, TotalCopies = 4, AuthorId = 3 },
            new Book { Id = 5, Title = "I, Robot", Isbn = "9780553382563", PublishedYear = 1950, TotalCopies = 2, AuthorId = 3 },
            new Book { Id = 6, Title = "Murder on the Orient Express", Isbn = "9780062693662", PublishedYear = 1934, TotalCopies = 3, AuthorId = 4 }
        );

        // Many-to-many links go in the join table (BookCategory)
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Categories)
            .WithMany(c => c.Books)
            .UsingEntity(j => j.HasData(
                new { BooksId = 1, CategoriesId = 2 },
                new { BooksId = 1, CategoriesId = 3 },
                new { BooksId = 2, CategoriesId = 1 },
                new { BooksId = 2, CategoriesId = 3 },
                new { BooksId = 3, CategoriesId = 3 },
                new { BooksId = 3, CategoriesId = 5 },
                new { BooksId = 4, CategoriesId = 2 },
                new { BooksId = 5, CategoriesId = 2 },
                new { BooksId = 6, CategoriesId = 4 },
                new { BooksId = 6, CategoriesId = 3 }
            ));

        modelBuilder.Entity<Member>().HasData(
            new Member { Id = 1, FullName = "Ahmed Sameh", Email = "ahmed@example.com", Phone = "01000000001", JoinedOn = new DateTime(2025, 1, 1) },
            new Member { Id = 2, FullName = "Sara Ali", Email = "sara@example.com", Phone = "01000000002", JoinedOn = new DateTime(2025, 2, 15) }
        );
    }
}