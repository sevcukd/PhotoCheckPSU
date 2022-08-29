using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoCheck.SQL
{
    public class SQLExpressGoods
    {
        public string Order_Button { get; set; }
        public string bar_code { get; set; }
        public string Name_Button { get; set; }
        public string code_wares { get; set; }
        public string CodeWares { get; set; }
        public string name_wares { get; set; }
        public string articl { get; set; }
        public string pathPhoto { get; set; }
        public bool isPhotoPresent { get; set; } = false;
        public bool IsWeight { get; set; }
        //public bool isPhotoPresent { get { return pathPhoto != null ? true : false; } }
    }
}
