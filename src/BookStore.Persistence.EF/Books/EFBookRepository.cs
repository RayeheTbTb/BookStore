using BookStore.Entities;
using BookStore.Services.Books.Contracts;
using BookStore.Services.Books.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Persistence.EF.Books
{
    public class EFBookRepository : BookRepository
    {
        private readonly EFDataContext _dataContext;

        public EFBookRepository(EFDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void Add(Book book)
        {
            _dataContext.Books.Add(book);
        }

        public void Delete(int id)
        {
            var book = FindById(id);

            if (book == null)
            {
                throw new BookNotFound();
            }
            _dataContext.Books.Remove(book);
        }

        public Book FindById(int id)
        {
            return _dataContext.Books.Find(id);
        }

        public GetBookDto Get(int id)
        {
            var book = _dataContext.Books.Include(_ => _.Category).Where(_ => _.Id == id).FirstOrDefault();
            return new GetBookDto
            {
                Title = book.Title,
                Author = book.Author,
                Description = book.Description,
                Pages = book.Pages,
                CategoryId = book.CategoryId,
            };
        }

        public IList<GetBookDto> GetAll()
        {
            return _dataContext.Books.Include(_ => _.Category)
                .Select(_ => new GetBookDto
                {
                    Title = _.Title,
                    Author = _.Author,
                    Description = _.Description,
                    Pages = _.Pages,
                    CategoryId = _.CategoryId
                }).ToList();
        }
    }
}
