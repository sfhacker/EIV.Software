
namespace EIV.Demo.Model
{
    using EIV.Demo.Model.Base;
    using System;

    public class City : EntityBase
    {
        private string name;
        private string postalCode;
        private Country country = null;
        private State state = null;

        private City()
        {
        }

        public City(Country country, State state)
        {
            if (country == null)
            {
                throw new ArgumentNullException();
            }

            if (state == null)
            {
                throw new ArgumentNullException();
            }

            this.country = country;
            this.state = state;
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

        public virtual string PostalCode
        {
            get
            {
                return this.postalCode;
            }

            set
            {
                this.postalCode = value;
            }
        }

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

        public virtual State State
        {
            get
            {
                return this.state;
            }

            protected set
            {
                this.state = value;
            }
        }

        #endregion
    }
}