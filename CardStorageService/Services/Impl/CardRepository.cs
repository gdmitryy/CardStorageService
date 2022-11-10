using CardStorageService.Data;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CardStorageService.Services.Impl
{
    public class CardRepository : ICardRepositoryService
    {
        private readonly CardStorageServiceDbContext _context;
        private readonly ILogger<CardRepository> _logger;

        public CardRepository(CardStorageServiceDbContext context, ILogger<CardRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public string Create(Card data)
        {
            var client = _context.Clients.FirstOrDefault(client => client.ClientId == data.ClientId);

            if (client == null)
            {
                throw new Exception("404 Client not found");
            }

            _context.Cards.Add(data);
            _context.SaveChanges();

            return data.CardId.ToString();
        }

        public int Delete(string id)
        {
            throw new System.NotImplementedException();
        }

        public IList<Card> GetAll() => _context.Cards.ToList();

        public IList<Card> GetByClientId(int id) => _context.Cards.Where(card => card.ClientId == id).ToList();

        public Card GetById(string id)
        {
            throw new System.NotImplementedException();
        }

        public int Update(Card data)
        {
            throw new System.NotImplementedException();
        }
    }
}
