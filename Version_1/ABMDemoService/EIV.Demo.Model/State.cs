

namespace EIV.Demo.Model
{
    using EIV.Demo.Model.Base;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.OData.Builder;

    public class State : EntityBase
    {
        private string name;
        private Country country = null;

        //[Key, ForeignKey("Country")]
        public int CountryId { get; set; }


        // OData requires a parameterless constructor (and public)
        public State()
        {
        }

        // Proxy class ignores this constructor ????
        public State(Country country)
        {
            if (country == null)
            {
                throw new ArgumentNullException();
            }

            this.country = country;
            this.CountryId = country.Id;
        }

        #region Properties

        public virtual string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        [Association("Country", "CountryId", "")]
        //[Contained]
        public virtual Country Country
        {
            get
            {
                return this.country;
            }

            protected set
            {
                this.country = value;
            }
        }

        #endregion
    }
}