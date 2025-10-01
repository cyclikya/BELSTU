using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_LES
{
    public interface ILifeevent<T> : IDisposable
    {
        List<T> GetAllLifeevents();
        T GetLifeeventById(int id);
        bool DelLifeevent(int id);
        bool AddLifeevent(T lifeevent);
        bool UpdLifeevent(int id, T lifeevent);
    }

    public interface ILifeevent : ILifeevent<Lifeevent> { }
}
