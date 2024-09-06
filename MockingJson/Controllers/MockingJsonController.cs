using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Xml;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MockingJson.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MockingJsonController : ControllerBase
    {
        [HttpGet("JsonGet/{str}")]
        public async Task<IActionResult> JsonGet([FromRoute] string str)
        {
            string str1 = GenerateMockData(str);
            return Ok(str1); 
        }  

        private static readonly string[] FakeNames = { "John", "Jane", "Alex", "Emily", "Chris", "Pat", "Sam", "Taylor", "Morgan", "Jordan", "Casey", "Jamie", "Riley", "Cameron", "Drew", "Harper", "Skyler", "Logan", "Bailey", "Quinn" };

        public static string GenerateMockData(string classString)
        {
            var random = new Random();
            var properties = ParseClassString(classString);
            var mockData = new Dictionary<string, object>();

            foreach (var property in properties)
            {
                object value = property.Type switch
                {
                    "int" or "long" or "double" => random.Next(1, 21),
                    "bool" => random.Next(0, 2) == 0,
                    "DateTime" => DateTime.Now.ToString("yyyy-MM-dd"),
                    "string" => property.Name.ToLower().Contains("name") ? GetRandomName(random) : "Lorem Ipsum",
                    _ => null
                };

                mockData[property.Name] = value;
            }

            return JsonConvert.SerializeObject(mockData, Newtonsoft.Json.Formatting.Indented);
        }

        private static string GetRandomName(Random random)
        {
            return FakeNames[random.Next(FakeNames.Length)];
        }

        private static List<Property> ParseClassString(string classString)
        {
            var properties = new List<Property>();
            var matches = Regex.Matches(classString, @"public\s+(\w+\??)\s+(\w+)\s*\{");

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 2)
                {
                    var type = match.Groups[1].Value.Replace("?", "");
                    var name = match.Groups[2].Value;
                    properties.Add(new Property { Type = type, Name = name });
                }
            }

            return properties;
        }

        private class Property
        {
            public string Type { get; set; }
            public string Name { get; set; }
        }
    }
}
