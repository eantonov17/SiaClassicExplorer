using SiaClassicLib;
using System;

namespace scdbu
{
    class Program
    {
        static void Main(string[] args)
        {
            var u = new Updater(Config.UriBase, Config.ConnectionString);

            //u.UpdateOutput();

            if (args.Length == 0)
                u.UpdateTopOnly(179000, true);
            else
                foreach (var arg in args)
                {
                    if (arg.StartsWith("-h="))
                    {
                        if (int.TryParse(arg.Substring(3), out int h))
                        {
                            Console.WriteLine($"/*Block at height {h}*/");
                            Console.WriteLine(u.GetBlockJson(h));
                        }
                    }
                }
        }
    }
}
