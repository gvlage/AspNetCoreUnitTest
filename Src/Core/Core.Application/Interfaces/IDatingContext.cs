using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Application.Interfaces
{
    public interface IDatingContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
