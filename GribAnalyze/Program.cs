using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grib.Api;

namespace GribAnalyze
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "", delimiter = ":";
            try
            {
                filename = args.ElementAtOrDefault(0) ?? throw new ArgumentException("Missing filename.", "Parameter 1");
                delimiter = (args.ElementAtOrDefault(1) ?? ":")
                    .Replace("\\t", "\t");

                using (GribFile gribFile = new GribFile(filename))
                {
                    (from msg in gribFile
                     orderby msg.TypeOfLevel, msg.Level, msg.ShortName
                     group msg by new
                     {
                         msg.TypeOfLevel,
                         msg.Level,
                         msg.ShortName
                     } into g
                     let hash = (uint) String.Join(":", g.Key.TypeOfLevel, g.Key.Level, g.Key.ShortName).GetHashCode()
                     select String.Join(
                         delimiter
                         , hash
                         , g.Key.TypeOfLevel
                         , g.Key.Level
                         , g.Key.ShortName
                         , String.Join(" ", g.Select(n => n.Name).Distinct())
                         , g.Count()))
                    .ToList()
                    .ForEach(m =>
                    {
                        Console.WriteLine(m);
                    });

                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"{e.Message} {e.InnerException?.Message}");
            }
        }
    }
}
