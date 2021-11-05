using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Byteology.EventSourcing.Storage
{
    public interface IDataStore : IDisposable
    {
        void Commit();
    }
}
