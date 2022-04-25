using BookStore.Entities;
using BookStore.Infrastructure.Application;
using BookStore.Services.Books.Contracts;
using BookStore.Services.Books.Exceptions;
using BookStore.Services.Categories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Books
{
    public class BookAppService : BookService
    {
        private readonly BookRepository _bookRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly UnitOfWork _unitOfWork;

        public BookAppService(BookRepository bookRepository, CategoryRepository categoryRepository,UnitOfWork unitOfWork)
        {
            _bookRepository = bookRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public void Add(AddBookDto dto)
        {
            var category = _categoryRepository.GetById(dto.CategoryId);

            if(category == null)
            {
                throw new BookCategoryDoesNotExist();
            }

            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                Description = dto.Description,
                Pages = dto.Pages,
                CategoryId = dto.CategoryId
            };

            _bookRepository.Add(book);
            _unitOfWork.Commit();
        }

        public void Delete(int id)
        {
            _bookRepository.Delete(id);
            _unitOfWork.Commit();
        }

        public GetBookDto Get(int id)
        {
            return _bookRepository.Get(id);
        }

        public IList<GetBookDto> GetAll()
        {
            return _bookRepository.GetAll();
        }

        public void Update(int id, UpdateBookDto dto)
        {
            Book book = _bookRepository.FindById(id);

            if (book == null)
            {
                throw new BookNotFound();
            }

            book.Title = dto.Title;
            book.Description = dto.Description;
            book.Pages = dto.Pages;
            book.Author = dto.Author;

            _unitOfWork.Commit();
        }
    }
}
