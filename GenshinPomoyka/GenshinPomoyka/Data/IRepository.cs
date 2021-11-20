using System;
using System.Collections.Generic;
using System.Text;

namespace GenshinPomoyka.Data
{
    public interface IRepository: IDisposable
    {
        IEnumerable<object> GetItemList(string type);
        
        object GetItem(Guid id, string type);
        
        void Create(object item);
        
        void Update(object item);
        
        void Delete(object item);
    }
}