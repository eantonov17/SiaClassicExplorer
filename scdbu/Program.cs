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
