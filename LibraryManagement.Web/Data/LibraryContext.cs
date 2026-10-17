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
    }
}