using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Log.Models
{
    public class DataSystemModel
    {
        public string ID { get; set; } = "";
        public string Title { get; set; } = "";
        public string ThumbMobile { get; set; } = "";
        public string ThumbTablet { get; set; } = "";
        public double PointRating { get; set; } = 0;
        public string TypeData { get; set; } = "";
        public string UserCreate { get; set; } = "";
    }
}
