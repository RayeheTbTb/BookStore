using BookStore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Test.Tools
{
    public static class BookFactory
    {
        public static Book CreateBook(string title, string author, string description, int pages, int categoryId)
        {
            return new Book
            {
                Title = title,
                Author = author,
                Description = description,
                Pages = pages,
                CategoryId = categoryId
            };
        }
    }
}
