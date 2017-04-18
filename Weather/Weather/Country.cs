using System.Collections.Generic;

namespace Weather.SupportClasses
{
    public class Country
    {
        public string Name { get; set; } = string.Empty;                        //Country name
        public string Capital { get; set; } = string.Empty;                     //Capital name
        public List<string> altSpellings { get; set; } = new List<string>();    //"EXAMPLE: altSpellings": ["CO", "Republic of Colombia", "República de Colombia"]
        public int id { get; set; }                                             //Id of capital city
    }
}
