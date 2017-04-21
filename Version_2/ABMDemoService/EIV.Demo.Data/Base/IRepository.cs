/// <summary>
/// 
/// </summary>
namespace EIV.Demo.Data.Base
{
    using System.Collections.Generic;

    public interface IRepository<TModel>
    {
        void Add(TModel entity);

        void Delete(TModel entity);

        IList<TModel> GetAll();
        
        TModel GetById(int id);
        
        void Update(TModel entity);
    }
}