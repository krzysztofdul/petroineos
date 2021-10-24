using System.Collections.Generic;

namespace Petroineos.Intraday
{
    public interface IFileService
    {
        void SaveToFileAsync(string fileName, IEnumerable<(string, decimal)> rows);
        void WriteEmptySafe(string fileName);
    }
}