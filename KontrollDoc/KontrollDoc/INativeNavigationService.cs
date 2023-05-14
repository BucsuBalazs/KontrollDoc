using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KontrollDoc
{
    public interface INativeNavigationService
    {
        Task NavigateToScanPageAsync();
    }
}
