using System.Threading;
using System.Threading.Tasks;

namespace TqkLibrary.WpfUi
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISaveJsonDataControl
    {
        /// <summary>
        /// 
        /// </summary>
        double DelaySaving { get; set; }
        /// <summary>
        /// 
        /// </summary>
        void ForceSave();
        /// <summary>
        /// 
        /// </summary>
        void TriggerSave();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task SaveAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        void Load();
    }
}