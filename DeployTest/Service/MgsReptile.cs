using System.Linq;
using Reptile;
using XSystem.Core.Domain;
using XSystem.Core.Infrastructure;

namespace DeployTest.Service
{
    public class MgsReptile : VisionReptile
    {
        public MgsReptile(IPersistence persistence) : base(persistence)
        {
            Region = Region.Mgs;
        }


    }
}