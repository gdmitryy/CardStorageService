using CardStorageService.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CardStorageService.Services.Impl
{
    public class ClientRepository : IClientRepositoryService
    {
        private readonly CardStorageServiceDbContext _context;        
        private readonly ILogger<ClientRepository> _logger;

        public ClientRepository(CardStorageServiceDbContext context, ILogger<ClientRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public int Create(Client data)
        {
            _context.Clients.Add(data);
            _context.SaveChanges();
            return data.ClientId;
        }

        public int Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public IList<Client> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public Client GetById(int id)
        {
            throw new System.NotImplementedException();
        }

        public int Update(Client data)
        {
            throw new System.NotImplementedException();
        }
    }
}
