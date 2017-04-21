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

    public sealed class CountryRepository : Repository<Country>, ICountryRepository
    {
        private IList<Country> Countries = null;
        public CountryRepository()
        {
            this.Reset();
            this.PopulateCountries();
        }

        public override void Add(Country entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (this.Countries == null)
            {
                throw new InvalidOperationException();
            }

            this.Countries.Add(
                new Country()
                {
                    // 'Id' should be random or calculated!
                    Id = entity.Id,
                    Name = entity.Name,
                    DDI = entity.DDI
                }
            );
        }

        public override void Update(Country entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (this.Countries == null)
            {
                throw new InvalidOperationException();
            }

            var thisItem = this.Countries.Where<Country>(x => x.Id.Equals(entity.Id)).SingleOrDefault();
            if (thisItem != null)
            {
                thisItem.Name = entity.Name;
                thisItem.DDI = entity.DDI;
            }
        }

        public override void Delete(Country entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (this.Countries == null)
            {
                throw new InvalidOperationException();
            }

            var thisItem = this.Countries.Where<Country>(x => x.Id.Equals(entity.Id)).SingleOrDefault();
            if (thisItem != null)
            {
                this.Countries.Remove(thisItem);
            }
        }

        public override IList<Country> GetAll()
        {
            return this.Countries;
        }

        public override Country GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException();
            }

            if (this.Countries == null)
            {
                throw new InvalidOperationException();
            }

            return this.Countries.Where(x => x.Id == id).SingleOrDefault();
        }

        private void Reset()
        {
            if (this.Countries == null)
            {
                this.Countries = new List<Country>();
            }
        }

        private void PopulateCountries()
        {
            if (this.Countries == null)
            {
                return;
            }
            if (this.Countries.Count != 0)
            {
                return;
            }

            this.Countries.Add(
                new Country()
                {
                    Id = 1,
                    Name = "USA"
                }
            );
            this.Countries.Add(
                new Country()
                {
                    Id = 2,
                    Name = "Argentina"
                }
            );

            this.Countries.Add(
                new Country()
                {
                    Id = 3,
                    Name = "Kangarooland"
                }
            );

            State newState = new State(this.Countries[0]) {  Id = 1, Name = "Washington DC" };
            this.Countries[0].States.Add(newState);

            State anotherState = new State(this.Countries[1]) { Id = 2, Name = "Santa Fe" };
            this.Countries[1].States.Add(anotherState);
        }
    }
}