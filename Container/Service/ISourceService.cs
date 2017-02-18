using System.Threading.Tasks;
using ShipSync.Container.Entity;

namespace ShipSync.Container.Service
{
    public interface ISourceService
    {
        /// <summary>
        /// Create a new SaveSource instance from the specified KSP install.
        /// Inspects the buildID files.
        /// </summary>
        /// <param name="installPath"></param>
        /// <returns></returns>
        SaveSource Create(string installPath);

        /// <summary>
        /// Ensure that the source can be synced.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        bool Validate(SaveSource source);
    }
}
