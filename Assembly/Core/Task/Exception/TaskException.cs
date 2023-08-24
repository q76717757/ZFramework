using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZFramework
{
    internal abstract class TaskException : Exception
    {

    }

    internal class CanceledException : TaskException
    {

    }
    internal class TimeoutException : TaskException
    {

    }
}
