using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nvelope
{
    /// <summary>
    /// Encapsulates an alternate environment to run some code under. For example, 
    /// an IExecutionContext could be used to run code as another user (impersonation)
    /// </summary>
    public interface IExecutionContext
    {
        void Execute(Action action);
    }
}
