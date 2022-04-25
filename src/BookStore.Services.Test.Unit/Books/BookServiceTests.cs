using BookStore.Entities;
using BookStore.Infrastructure.Application;
using BookStore.Infrastructure.Test;
using BookStore.Persistence.EF;
using BookStore.Persistence.EF.Books;
using BookStore.Persistence.EF.Categories;
using BookStore.Services.Books;
using BookStore.Services.Books.Contracts;
using BookStore.Services.Books.Exceptions;
using BookStore.Services.Categories.Contracts;
using BookStore.Test.Tools;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Services.Test.Unit.Books
{
    public class BookServiceTests
    {
        private readonly EFDataContext _dataContext;
        private readonly BookService _sut;
        private readonly BookRepository _repository;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryRepository _categoryRepository;

        public BookServiceTests()
        {
            _dataContext = new EFInMemoryDatabase().CreateDataContext<EFDataContext>();
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _repository = new EFBookRepository(_dataContext);
            _categoryRepository = new EFCategoryRepository(_dataContext);
            _sut = new BookAppService(_repository, _categoryRepository, _unitOfWork);
        }

        [Fact]
        public void Add_adds_book_properly()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            _dataContext.Manipulate(_ => _.Categories.Add(category));
            AddBookDto dto = GenerateAddBookDto(category);

            _sut.Add(dto);

            var expected = _dataContext.Books.Include(_ => _.Category).FirstOrDefault();
            expected.Title.Should().Be(dto.Title);
            expected.Author.Should().Be(dto.Author);
            expected.Description.Should().Be(dto.Description);
            expected.Pages.Should().Be(dto.Pages);
            expected.CategoryId.Should().Be(dto.CategoryId);
        }

        [Fact]
        public void Add_throws_BookCategoryDoesNotExist_when_book_given_category_id_doesNotExist()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            AddBookDto dto = GenerateAddBookDto(category);

            Action expected = () => _sut.Add(dto);

            expected.Should().ThrowExactly<BookCategoryDoesNotExist>();
        }

        [Fact]
        public void Update_updates_book_properly()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            _dataContext.Manipulate(_ => _.Categories.Add(category));
            var book = BookFactory.CreateBook("Dummy", "Dummy Author", "For Dummies", 10, category.Id);
            _dataContext.Manipulate(_ => _.Books.Add(book));
            UpdateBookDto dto = GenerateUpdateBookDto();

            _sut.Update(book.Id, dto);

            var expected = _dataContext.Books.Include(_ => _.Category).Where(_ => _.Id == book.Id).FirstOrDefault();
            expected.Title.Should().Be(dto.Title);
            expected.Author.Should().Be(dto.Author);
            expected.Description.Should().Be(dto.Description);
            expected.Pages.Should().Be(dto.Pages);
        }

        [Fact]
        public void Update_throws_BookNotFound_when_bookDoesNotExist_with_given_id()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            _dataContext.Manipulate(_ => _.Categories.Add(category));
            var book = BookFactory.CreateBook("Dummy", "Dummy Author", "For Dummies", 10, category.Id);
            UpdateBookDto dto = GenerateUpdateBookDto();

            Action expected = () => _sut.Update(book.Id, dto);

            expected.Should().ThrowExactly<BookNotFound>();
        }

        [Fact]
        public void Delete_deletes_book_properly()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            _dataContext.Manipulate(_ => _.Categories.Add(category));

            var book = BookFactory.CreateBook("Dummy", "Dummy Author", "For Dummies", 10, category.Id);
            _dataContext.Manipulate(_ => _.Books.Add(book));

            _sut.Delete(book.Id);

            _dataContext.Books.Should().NotContain(_ => _.Id == book.Id);
        }

        [Fact]
        public void Delete_throws_BookNotFound_when_bookDoesNotExist_with_given_id()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            _dataContext.Manipulate(_ => _.Categories.Add(category));
            var book = BookFactory.CreateBook("Dummy", "Dummy Author", "For Dummies", 10, category.Id);

            Action expected = () => _sut.Delete(book.Id);

            expected.Should().ThrowExactly<BookNotFound>();
        }

        [Fact]
        public void Get_returns_book_with_given_id()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            _dataContext.Manipulate(_ => _.Categories.Add(category));

            var book = BookFactory.CreateBook("Dummy", "Dummy Author", "For Dummies", 10, category.Id);
            _dataContext.Manipulate(_ => _.Books.Add(book));

            GetBookDto expected = _sut.Get(book.Id);

            expected.Title.Should().Be(book.Title);
            expected.Author.Should().Be(book.Author);
            expected.Description.Should().Be(book.Description);
            expected.Pages.Should().Be(book.Pages);

        }

        [Fact]
        public void GetAll_returns_all_book()
        {
            CreateBooksInDatabase();
            IList<GetBookDto> expected = _sut.GetAll();

            expected.Should().HaveCount(3);
            expected.Should().Contain(_ => _.Title == "Dummy1");
            expected.Should().Contain(_ => _.Title == "Dummy2");
            expected.Should().Contain(_ => _.Title == "Dummy3");

        }

        private void CreateBooksInDatabase()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            _dataContext.Manipulate(_ => _.Categories.Add(category));

            var books = new List<Book>
            {
                new Book { Title = "Dummy1", Author = "Dummy Author1",
                    Description = "For Dummies1", Pages = 10, CategoryId = category.Id},
                new Book { Title = "Dummy2", Author = "Dummy Author2", 
                    Description = "For Dummies2", Pages = 20, CategoryId = category.Id},
                new Book { Title = "Dummy3", Author = "Dummy Author3", 
                    Description = "For Dummies3", Pages = 30, CategoryId = category.Id},
            };
            _dataContext.Manipulate(_ =>
            _.Books.AddRange(books));
        }

        private UpdateBookDto GenerateUpdateBookDto()
        {
            return new UpdateBookDto
            {
                Title = "Updated Dummy",
                Author = "Updated Dummy Author",
                Description = "Updated For Dummies",
                Pages = 20,
            };
        }

        private static AddBookDto GenerateAddBookDto(Category category)
        {
            return new AddBookDto
            {
                Title = "Dummy",
                Author = "Dummy Author",
                Description = "For Dummies",
                Pages = 10,
                CategoryId = category.Id,
            };
        }
    }
}
