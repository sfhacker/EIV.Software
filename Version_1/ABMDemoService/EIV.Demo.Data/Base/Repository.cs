/// <summary>
/// 
/// </summary>
namespace EIV.Demo.Data.Base
{
    using System.Collections.Generic;

    public abstract class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        protected Repository()
        { }

        public virtual void Add(TModel entity)
        { }

        public virtual void Delete(TModel entity)
        { }

        public virtual IList<TModel> GetAll()
        { return null; }
        
        public virtual TModel GetById(int id)
        { return null; }
        
        public virtual void Update(TModel entity)
        { }
    }
}