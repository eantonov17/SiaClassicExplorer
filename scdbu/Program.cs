//# SiaClassicExplorer
//** An Explorer for SiaClassic blockchain in C# and .Net Framework **
//* Copyright(C) 2018-2019 Eugene Antonov*
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of version 3 of the GNU General Public License
//as published by the Free Software Foundation.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.If not, see<https://www.gnu.org/licenses/>.

using Newtonsoft.Json;
using SiaClassicLib;
using System;
using System.IO;

namespace scdbu
{
    class Program
    {
        static void Main(string[] args)
        {
            const string configFileName = "config.json";
            var exe = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var configFilePath = Path.Combine(Path.GetDirectoryName(exe), configFileName);
            Config config = LoadConfig(configFilePath);
            if(string.IsNullOrWhiteSpace(config.ConnectionString) || string.IsNullOrWhiteSpace(config.UriBase))
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter configuration data into " + configFileName);
                Console.ForegroundColor = color;
                return;
            }

            var u = new Updater(config.UriBase, config.ConnectionString);

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

        private static Config LoadConfig(string configFileName)
        {
            if (configFileName.StartsWith(@"file:\"))
                configFileName = configFileName.Substring(6);

            Config config = null;
            if (File.Exists(configFileName))
                config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFileName));
            else
                File.WriteAllText(configFileName, JsonConvert.SerializeObject(config = new Config()));
            return config;
        }
    }
}
