using System.Collections.Generic;
using System.Globalization;
using System.IO;
using BookShopSystem.Models;

namespace BookShopSystem.Data.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<BookShopContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationDataLossAllowed = true;
            this.AutomaticMigrationsEnabled = true;
            this.ContextKey = "BookShopSystem.Data.BookShopContext";
        }

        protected override void Seed(BookShopSystem.Data.BookShopContext context)
        {
            // SeedAuthors(context);
            //     SeedCategories(context);
            //     SeedBooks(context);
        }

        private static void SeedCategories(BookShopContext context)
        {
            using (var readerCategories = new StreamReader("../../categories.txt"))
            {
                Random random = new Random();

                var categoryName = readerCategories.ReadLine();

                while (categoryName != null)
                {
                    //var bookIndex = random.Next(0, books.Count);
                    if (context.Categories.Any())
                    {
                        return;
                    }

                    if (categoryName == "")
                    {
                        categoryName = readerCategories.ReadLine();
                    }

                    context.Categories.Add(new Category()
                    {

                        Name = categoryName
                    });

                    categoryName = readerCategories.ReadLine();
                }
                context.SaveChanges();
            }
        }

        private static void SeedBooks(BookShopContext context)
        {
            using (var readerBooks = new StreamReader("../../books.txt"))
            {
                Random random = new Random();

                var authors = context.Authors.ToList();

                var lineBook = readerBooks.ReadLine();
                lineBook = readerBooks.ReadLine();
                while (lineBook != null)
                {
                    var data = lineBook.Split(new[] { ' ' }, 6);

                    var authorIndex = random.Next(0, authors.Count());
                    var author = authors[authorIndex];
                    var edition = (EditionType)int.Parse(data[0]);
                    var releaseDate = DateTime.ParseExact(data[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                    var copies = int.Parse(data[2]);
                    var price = decimal.Parse(data[3]);
                    var ageRestriction = (Age)int.Parse(data[4]);
                    var title = data[5];

                    if (context.Books.Any())
                    {
                        return;
                    }

                    var numberOfCategories = 3;

                    var bookCategories = new List<Category>();

                    for (int i = 0; i < numberOfCategories; i++)
                    {
                        var categoryId = random.Next(0, context.Categories.ToList().Count + 1);
                        bookCategories.Add(context.Categories.Find(categoryId));
                    }

                    context.Books.Add(new Book()
                    {
                        Author = author,
                        Title = title,
                        EditionType = edition,
                        Price = price,
                        Copies = copies,
                        ReleaseDate = releaseDate,
                        AgeRestriction = ageRestriction,
                        Categories = bookCategories
                    });

                    lineBook = readerBooks.ReadLine();
                }
                context.SaveChanges();
            }
        }

        private static void SeedAuthors(BookShopContext context)
        {
            using (var reader = new StreamReader("../../authors.txt"))
            {
                var line = reader.ReadLine();
                line = reader.ReadLine();
                while (line != null)
                {
                    var data = line.Split(new[] { ' ' }, 2);

                    var firstName = data[0];
                    var lastName = data[1];

                    if (context.Authors.Any())
                    {
                        return;
                    }

                    context.Authors.Add(new Author()
                    {
                        FirstName = firstName,
                        LastName = lastName
                    });

                    line = reader.ReadLine();
                }
                context.SaveChanges();
            }
        }


    }
}
