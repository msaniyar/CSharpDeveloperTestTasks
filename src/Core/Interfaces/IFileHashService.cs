
namespace Core.Interfaces
{
    public interface IFileHashService
    {

        /// <summary>
        /// Calculate hex format of hash given file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        string CalculateFileHash(string filePath);
    }
}
