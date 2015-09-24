using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using BookShopSystem.Data;
using BookShopSystem.Data.Migrations;
using BookShopSystem.Models;

namespace BookShopSystem.ConsoleClient
{
    class ConsoleClientMain
    {
        static void Main()
        {
         
            var context = new BookShopContext();

            var bookCount = context.Books.Count(); 
            Console.WriteLine(bookCount);

            var books = context.Books.Where(b => b.AuthorId == 1).ToList();

            var authorById = context.Authors.FirstOrDefault(a => a.Id == 1);

            Console.WriteLine(authorById.FirstName);

        }
    }
}
