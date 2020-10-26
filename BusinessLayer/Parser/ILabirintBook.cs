using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Parser
{
    public interface ILabirintBook
    {
        int GetBookUrl();
        void Update(string lastUrl);
    }
}
