using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntellectFlow.Helpers
{
    public interface INavigationService
    {
        object CurrentView { get; }
        void NavigateTo<T>() where T : class;
    }
}
