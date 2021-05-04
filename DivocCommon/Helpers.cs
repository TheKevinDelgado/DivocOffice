using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DivocCommon
{
    public static class Helpers
    {
        public static string CleanFilename(string dirtyName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();

            string cleanName = new string(dirtyName
              .Where(x => !invalidChars.Contains(x))
              .ToArray());

            return cleanName;        
        }
    }
}
