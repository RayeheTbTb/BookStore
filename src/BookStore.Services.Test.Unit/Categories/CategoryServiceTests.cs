using BookStore.Entities;
using BookStore.Infrastructure.Application;
using BookStore.Infrastructure.Test;
using BookStore.Persistence.EF;
using BookStore.Persistence.EF.Categories;
using BookStore.Services.Categories;
using BookStore.Services.Categories.Contracts;
using BookStore.Services.Categories.Exceptions;
using BookStore.Test.Tools;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Services.Test.Unit.Categories
{
    public class CategoryServiceTests
    {
        private readonly EFDataContext _dataContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly CategoryService _sut;
        private readonly CategoryRepository _repository;
        
        public CategoryServiceTests()
        {
            _dataContext = new EFInMemoryDatabase().CreateDataContext<EFDataContext>();
            _unitOfWork = new EFUnitOfWork(_dataContext);
            _repository = new EFCategoryRepository(_dataContext);
            _sut = new CategoryAppService(_repository,_unitOfWork);
        }

        [Fact]
        public void Add_adds_category_properly()
        {
            AddCategoryDto dto = GenerateAddCategoryDto();

            _sut.Add(dto);

            var expected = _dataContext.Categories.FirstOrDefault();

            expected.Title.Should().Be(dto.Title);
        }

        [Fact]
        public void GetAll_returns_all_categories()
        {
            CreateCategoriesInDataBase();

            var expected = _sut.GetAll();

            expected.Should().HaveCount(3);
            expected.Should().Contain(_ => _.Title == "dummy1");
            expected.Should().Contain(_ => _.Title == "dummy2");
            expected.Should().Contain(_ => _.Title == "dummy3");
        }

        [Fact]
        public void Delete_deletes_category_properly()
        {
            var category = CreateSingleCategoryInDatabase();

            _sut.Delete(category.Id);

            _dataContext.Categories.Should().NotContain(_ => _.Id == category.Id);
        }

        [Fact]
        public void Delete_throws_CategoryNotFound_when_categoryDoesNotExist_with_given_id()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");

            Action expected = () => _sut.Delete(category.Id);

            expected.Should().ThrowExactly<CategoryNotFound>();
        }

        [Fact]
        public void Update_updates_category_properly()
        {
            var category = CreateSingleCategoryInDatabase();

            UpdateCategoryDto dto = GenerateUpdateCategoryDto();

            _sut.Update(category.Id, dto);

            var expected = _dataContext.Categories.FirstOrDefault(_ => _.Id == category.Id);
            expected.Title.Should().Be(dto.Title);
        }

        [Fact]
        public void Update_throws_CategoryNotFound_when_categoryDoesNotExist_with_given_id()
        {
            var category = CategoryFactory.CreateCategory("Dummy Category");
            UpdateCategoryDto dto = GenerateUpdateCategoryDto();

            Action expected = () => _sut.Update(category.Id, dto);

            expected.Should().ThrowExactly<CategoryNotFound>();
        }

        private Category CreateSingleCategoryInDatabase()
        {
            var category = new Category { Title = "dummy4" };
            _dataContext.Manipulate(_ => _.Categories.Add(category));

            return category;
        }

        private void CreateCategoriesInDataBase()
        {
            var categories = new List<Category>
            {
                new Category { Title = "dummy1"},
                new Category { Title = "dummy2"},
                new Category { Title = "dummy3"}
            };
            _dataContext.Manipulate(_ =>
            _.Categories.AddRange(categories));
        }


        private static AddCategoryDto GenerateAddCategoryDto()
        {
            return new AddCategoryDto
            {
                Title = "dummy"
            };
        }

        private static UpdateCategoryDto GenerateUpdateCategoryDto()
        {
            return new UpdateCategoryDto
            {
                Title = "Updated Dummy"
            };
        }
    }
}
