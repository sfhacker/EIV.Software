
namespace EIV.Demo.Data.Repository
{
    using EIV.Demo.Data.Base;
    using Interface;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class StateRepository : Repository<State>, IStateRepository
    {
        private IList<State> states = null;
        public StateRepository()
        {
            this.Reset();
            this.PopulateStates();
        }

        public override void Add(State entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (entity.CountryId < 1)
            {
                throw new ArgumentException("Invalid Id", "CountryId");
            }

            if (this.states == null)
            {
                throw new InvalidOperationException();
            }

            /*
            State newState = new State(entity.Country)
            {
                // 'Id' should be random or calculated!
                Id = entity.Id,
                Name = entity.Name
            };*/

            State newState = new State()
            {
                Id = entity.Id,
                Name = entity.Name,
                CountryId = entity.CountryId
            };
            this.states.Add(newState);
        }

        public override IList<State> GetAll()
        {
            return this.states;
        }

        public override State GetById(int id)
        {
            if (id < 1)
            {
                throw new ArgumentNullException();
            }

            if (this.states == null)
            {
                throw new InvalidOperationException();
            }

            return this.states.Where(x => x.Id == id).SingleOrDefault();
        }

        // This is awful, but just a test!
        public override void Update(State entity)
        {
            State thisState = null;

            if (entity == null)
            {
                throw new ArgumentNullException();
            }
            if (this.states == null)
            {
                throw new InvalidOperationException();
            }

            thisState = this.GetById(entity.Id);
            if (thisState != null)
            {
                // thisState.Country = entity.Country;
            }
        }

        public override void Delete(State entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            if (this.states == null)
            {
                throw new InvalidOperationException();
            }

            this.states.Remove(entity);
        }

        private void Reset()
        {
            if (this.states == null)
            {
                this.states = new List<State>();
            }
        }

        private void PopulateStates()
        {
            if (this.states == null)
            {
                return;
            }
            if (this.states.Count != 0)
            {
                return;
            }

            Country thisCountry = new Country { Id = 1, Name = "USA" };

            this.states.Add(
                new State(thisCountry)
                {
                    Id = 1,
                    Name = "New York",
                    CountryId = thisCountry.Id

                }
            );
        }
    }
}