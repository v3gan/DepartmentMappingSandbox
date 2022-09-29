using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;

namespace DepartmentMappingSandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string urlTemplate = "https://deptmap.hs.pitt.edu/deptmap/service/unit/{0}/descendants?level={1}";
            const string respCenters = "39,45";
            const string level = "department";
            var url = string.Format(urlTemplate, respCenters, level);
            List<ListItem> deptDdl;
            WebRequest req = WebRequest.Create(url);
            req.Method = "GET";
            HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                using (Stream respStream = resp.GetResponseStream())
                {
                    XElement x = XElement.Load(respStream);
                    deptDdl = x.Descendants("unit")
                        .Where(unit => unit.Attribute("retired-on") == null)
                        .Select(unit => new ListItem
                        {
                            Value = (int)unit.Element("unit-id"),
                            Text = (string)unit.Element("common-name")
                        })
                        .ToList();
                }
                foreach (var item in deptDdl)
                {
                    Console.WriteLine($"{item.Value}:{item.Text}");
                }
            }
        }
    }
    public class ListItem
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
