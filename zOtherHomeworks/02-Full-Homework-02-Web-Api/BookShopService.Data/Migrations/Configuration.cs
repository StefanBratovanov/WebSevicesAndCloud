namespace BookShop.Data.Migrations
{
    using BookShop.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BookShop.Data.BookShopContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            this.ContextKey = "BookShop.Data.BookShopContext";
        }

        protected override void Seed(BookShop.Data.BookShopContext context)
        {
            Random random = new Random();

            // adding categories
            var categories = new List<Category>();
            if (context.Categories.Count() == 0)
            {
                using (var reader = new StreamReader("../../categories.txt"))
                {
                    var line = reader.ReadLine();
                    while (line != null)
                    {
                        var category = new Category()
                        {
                            Name = line
                        };
                        if (!categories.Contains(category))
                        {
                            categories.Add(category);
                        }
                        context.Categories.AddOrUpdate(
                            c => c.Name,
                            new Category()
                            {
                                Name = line
                            });

                        line = reader.ReadLine();
                    }
                }
            }

            // adding authors
            var authors = new List<Author>();
            if (context.Authors.Count() == 0)
            {
                using (var reader = new StreamReader("../../authors.txt"))
                {
                    var line = reader.ReadLine();
                    line = reader.ReadLine();
                    while (line != null)
                    {
                        var data = line.Split(new[] { ' ' }, 2);

                        var author = new Author()
                        {
                            FirstName = data[0],
                            LastName = data[1]
                        };
                        if (!authors.Contains(author))
                        {
                            authors.Add(author);
                        }

                        if (context.Authors.Count() == 0)
                        {
                            context.Authors.Add(
                                new Author()
                                {
                                    FirstName = data[0],
                                    LastName = data[1]
                                });
                        }

                        line = reader.ReadLine();
                    }
                }
            }

            // adding books
            var books = new List<Book>();
            if (context.Books.Count() == 0)
            {
                using (var reader = new StreamReader("../../books.txt"))
                {
                    var line = reader.ReadLine();
                    line = reader.ReadLine();
                    while (line != null)
                    {
                        var data = line.Split(new[] { ' ' }, 6);
                        var authorIndex = random.Next(0, authors.Count);
                        var author = authors[authorIndex];
                        var edition = (Edition)int.Parse(data[0]);
                        var releaseDate = DateTime.ParseExact(data[1], "d/M/yyyy", CultureInfo.InvariantCulture);
                        var copies = int.Parse(data[2]);
                        var price = decimal.Parse(data[3]);
                        //var ageRestriction = (AgeRestriction)int.Parse(data[4]);
                        var title = data[5];

                        if (context.Books.Count() == 0)
                        {
                            context.Books.Add(
                                new Book()
                                {
                                    Author = author,
                                    Edition = edition,
                                    ReleaseDate = releaseDate,
                                    Copies = copies,
                                    Price = price,
                                    Title = title
                                });
                        }

                        var book = new Book()
                        {
                            Author = author,
                            Edition = edition,
                            ReleaseDate = releaseDate,
                            Copies = copies,
                            Price = price,
                            Title = title
                        };

                        if (!books.Contains(book))
                        {
                            books.Add(book);
                        }

                        line = reader.ReadLine();

                    }
                }
            }

            var allBooks = context.Books.ToList();
            var allCategories = context.Categories.ToList();
            if (context.Books.Select(b => b.Categories).FirstOrDefault().Count == 0)
            {
                foreach (var book in allBooks)
                {
                    var categoryId = (allCategories[random.Next(1, allCategories.Count)]).Id;
                    var newCategory = context.Categories.Where(c => c.Id == categoryId).Select(c => new { Id = c.Id, Name = c.Name });

                    book.Categories.Add(allCategories[random.Next(1, allCategories.Count)]);

                }
            }

            if (context.Roles.Any())
            {
                return;
            }

            var user = context.Users.First();
            var userRoleAdmin = new IdentityRole() { Name = "Administrator"};
            var userRoleModerator = new IdentityRole() { Name = "Moderator" };

            context.Roles.Add(userRoleAdmin);
            context.Roles.Add(userRoleModerator);
            context.SaveChanges();

            userRoleAdmin.Users.Add(new IdentityUserRole() 
            {
                UserId = user.Id        
            });

            context.SaveChanges();
        }
    }
}
