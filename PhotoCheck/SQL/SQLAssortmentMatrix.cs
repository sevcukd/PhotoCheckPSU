using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCheck.SQL
{
    public class SQLAssortmentMatrix
    {
        public string code { get; set; }
        public string name { get; set; }
        public string barcode_last { get; set; }
        public string articul { get; set; }
        public bool is_weight { get; set; }
        public bool isPhotoPresent { get; set; }
    }
}
