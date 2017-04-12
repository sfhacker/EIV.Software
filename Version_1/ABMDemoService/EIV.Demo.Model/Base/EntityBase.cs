/// <summary>
/// 
/// </summary>
namespace EIV.Demo.Model.Base
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class EntityBase
    {
        protected int id;
        protected bool isDeleted;

        #region Properties

        [Key]
        public virtual int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
            }
        }

        [NotMapped]
        public virtual bool IsDeleted
        {
            get { return this.isDeleted; }
            protected set { this.isDeleted = value; }
        }

        #endregion
    }
}