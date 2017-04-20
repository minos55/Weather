using System.Collections.Generic;

namespace Nomnio.CityWeather.SupportClasses
{
    public class RootObject
    {
        public int Cnt { get; set; }
        public List<City> List { get; set; } = new List<City>();
    }
}
