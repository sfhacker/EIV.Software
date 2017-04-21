/// <summary>
/// 
/// </summary>
namespace EIV.Demo.Data.Repository
{
    using Base;
    using Interface;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class ClientRepository : Repository<Client>, IClientRepository
    {
        private IList<Client> People = null;
        public ClientRepository()
        {
            this.Reset();
            this.PopulateClients();
        }

        public override void Add(Client entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (this.People == null)
            {
                throw new InvalidOperationException();
            }

            this.People.Add(
                new Client()
                {
                    // 'Id' should be random or calculated!
                    Id = entity.Id,
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Country = entity.Country
                }
            );
        }

        public override IList<Client> GetAll()
        {
            return this.People;
        }

        public override Client GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException();
            }

            if (this.People == null)
            {
                throw new InvalidOperationException();
            }

            return this.People.Where(x => x.Id == id).SingleOrDefault();
        }

        // This is awful, but just a test!
        public override void Update(Client entity)
        {
            Client thisClient = null;

            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            if (this.People == null)
            {
                throw new InvalidOperationException();
            }

            thisClient = this.GetById(entity.Id);
            if (thisClient != null)
            {
                thisClient.Country = entity.Country;
            }
        }

        public override void Delete(Client entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (this.People == null)
            {
                throw new InvalidOperationException();
            }

            this.People.Remove(entity);
        }

        private void Reset()
        {
            if (this.People == null)
            {
                this.People = new List<Client>();
            }
        }

        private void PopulateClients()
        {
            if (this.People == null)
            {
                return;
            }
            if (this.People.Count != 0)
            {
                return;
            }
            this.People.Add(
                new Client()
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Citizen",
                    Country = new Country { Id = 1, Name = "USA" },
                    DOB = DateTime.Now
                    
                }
            );
            this.People.Add(
                new Client()
                {
                    Id = 2,
                    FirstName = "Rosario",
                    LastName = "Santa Fe",
                    Country = new Country { Id = 2, Name = "Argentina" },
                    DOB = DateTime.Now.AddYears(-15)
                }
            );

            this.People.Add(
                new Client()
                {
                    Id = 3,
                    FirstName = "Noah",
                    LastName = "Boy",
                    Country = new Country { Id = 3, Name = "Kangarooland" },
                    DOB = DateTime.Now.AddYears(-6)
                }
            );
        }
    }
}