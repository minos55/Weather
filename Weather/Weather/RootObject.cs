using System.Collections.Generic;

namespace Weather.SupportClasses
{
    public class RootObject
    {
        public int Cnt { get; set; }
        public List<City> list { get; set; } = new List<City>();
    }
}
