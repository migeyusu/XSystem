
using XSystem.Core.Domain;

namespace XSystem.Core.Infrastructure
{
    public class MemoryWorkUnit:IWorkUnit
    {
        public void Commit()
        {
            //没啥事儿
        }

        public void Dispose() { }
    }
}