using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PhotoCheck.Models
{
    public class Wares
    {

        public ImageSource photo { get; set; }
        public string photoPath { get; set; }
        public string photoFullName { get; set; }
        public string kodeWares { get; set; }
        public string nameWares { get; set; }
        public string Articl { get; set; }
        public PhotoStatus savePhotoStatus { get; set; } = PhotoStatus.Miss; // 0-лишити фото; 1-невірне фото; 2-невірний код
        public bool IsWeight { get; set; }
        public string  barcode { get; set; }

        ~Wares()
        {
            //MessageBox.Show("by!");
        }

    }
}
