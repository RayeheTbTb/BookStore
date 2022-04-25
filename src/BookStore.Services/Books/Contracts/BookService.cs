using BookStore.Infrastructure.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Services.Books.Contracts
{
    public interface BookService : Service
    {
        void Add(AddBookDto dto);
        void Update(int id, UpdateBookDto dto);
        void Delete(int id);
        IList<GetBookDto> GetAll();
        GetBookDto Get(int id);
    }
}
