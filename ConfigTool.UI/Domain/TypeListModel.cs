using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luhmann.Kiosk.Tools.ConfigTool.Domain
{
    internal class TypeListModel
    {
        public string TypeName{ get; set; }
        public string FullPath { get; set; }
        public string GoodName { get { return $"{TypeName} ({FullPath})"; } }
    }
}
