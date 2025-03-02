using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Infrustucture
{
    public static class HelperMethods
    {
        public static Guid ToGuid(this string text) => new(text);
    }
}
