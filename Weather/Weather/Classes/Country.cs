using Newtonsoft.Json;
using System.Collections.Generic;

namespace Nomnio.Weather
{
    public class Country
    {
        public string Name { get; set; } = string.Empty;                        //Country name
        public string CapitalCity { get; set; } = string.Empty;                 //Capital name
        public string CountryCode { get; set; } = string.Empty;                 //Country code EXAMPLE: "CO"

        public Country()
        {
        }

        public Country(string countryName, string capitalCity, string countryCode)
        {
            Name = countryName;
            CapitalCity = capitalCity;
            CountryCode = countryCode;
        }
    }
}
