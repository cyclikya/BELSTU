using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_LES
{
    public interface ICommon<T1, T2> : ICelebrity<T1>, ILifeevent<T2>
    {
        List<T2> GetLifeeventsByCelebrityId(int celebrityId);
        T1 GetCelebrityByLifeeventId(int lifeeventId);
    }

    public interface ICommon : ICommon<Celebrity, Lifeevent> { }
}
