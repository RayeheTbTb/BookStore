using BookStore.Services.Books.Contracts;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BookStore.RestAPI.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : Controller
    {
        private readonly BookService _service;
        public BooksController(BookService service)
        {
            _service = service;
        }

        [HttpPost]
        public void Add(AddBookDto dto)
        {
            _service.Add(dto);
        }

        [HttpGet]
        public IList<GetBookDto> GetAll()
        {
            return _service.GetAll();
        }

        [HttpGet("{id}")]
        public GetBookDto Get(int id)
        {
            return _service.Get(id);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            _service.Delete(id);
        }

    }
}
