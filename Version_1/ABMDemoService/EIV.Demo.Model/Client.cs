/// <summary>
/// 
/// </summary>
namespace EIV.Demo.Model
{
    using Base;
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Client : EntityBase
    {
        private string firstName;
        private string lastName;
        private DateTime dateOfBirth;
        private Country country = null;

        #region Properties

        public virtual string FirstName
        {
            get
            {
                return this.firstName;
            }

            set
            {
                this.firstName = value;
            }
        }

        /// <summary>
        /// Gets or sets Last Name.
        /// </summary>
        public virtual string LastName
        {
            get
            {
                return this.lastName;
            }

            set
            {
                this.lastName = value;
            }
        }

        public virtual DateTime DOB
        {
            get
            {
                return this.dateOfBirth;
            }

            set
            {
                this.dateOfBirth = value;
            }
        }

        [ForeignKey("CountryId")]
        public virtual Country Country
        {
            get
            {
                return this.country;
            }

            set
            {
                this.country = value;
            }
        }
        #endregion Properties
    }
}