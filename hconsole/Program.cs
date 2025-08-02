using hconsole.cmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static hconsole.io.cout;
using static hconsole.io.exception;
using static hconsole.cmd.DesignFormat;



namespace hconsole
{
    public class Program
    {
        public static string __version__ = "4.3.za.24";
        public static string __shell__ = "c2";
        static void Main(string[] args)
        {
            Initnova(false);
        }

        public class Shell
        {
            public int Num { get; set; }
            public string Name { get; set; }
            public string Desc { get; set; }
            public string Version { get; set; }
        }

        public static List<Shell> Shells = new List<Shell>();

        public static void Initnova(bool isctrlc)
        {

            List<string> InputSet = new List<string>();
            InputSet = [$"{CommandEnv.CURRENT_USER_NAME}", "@", $"{Environment.MachineName}", ": ", $"{CommandEnv.CurrentDirDest} ", $"{get_shell_icon(__shell__)} "];
            try
            {
                while (true)
                {
                _entry_point_main:
                    try
                    {
                        // Handle CTRL+C key press to prevent quitting
                        Console.CancelKeyPress += (sender, e) =>
                        {
                            e.Cancel = false; // Prevent the app from closing
                            Console.WriteLine("\n<CTRL-C>: type `@exit` to exit!");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Initnova(true);
                        };

                        if (!isctrlc)
                        {
                            
                            TakeInput(InputSet);

                            List<string> commands = UserInput.Prepare(UserInput.Input());
                            IdentifyCommand.Identify(commands);
                            List<string> parsed_commands = IdentifyCommand.ReturnThemPlease();

                            if (parsed_commands.Count <= 0)
                                continue;

                            if (parsed_commands[0].StartsWith("!"))
                            {
                                if (parsed_commands[0].ToLower() == "!!")
                                    InputSet = [$"{CommandEnv.CurrentDirDest} ", $"{get_shell_icon(__shell__)} "];
                                else if (parsed_commands[0].ToLower() == "!")
                                    InputSet = [$"{CommandEnv.CURRENT_USER_NAME}", "@", $"{Environment.MachineName}", ": ", $"{CommandEnv.CurrentDirDest} ", $"{get_shell_icon(__shell__)} "];
                                IdentifyCommand.CacheClean();
                            }

                            PleaseCommandEnv.TheseCommands(parsed_commands);

                            hconsole.io.exception.ListThem();
                            hconsole.io.exception.CacheClean();

                            IdentifyCommand.CacheClean();
                        }
                        else
                        {
                            isctrlc = false;
                        }

                        continue;

                    }
                    catch (Exception exp) // Exception handling block
                    {
                        //hconsole.io.exception.CacheClean();
                        //hconsole.io.exception.New(exp.ToString());
                        //hconsole.io.exception.ListThem();
                        starerror("An error occured");
                    }

                    // Handle CTRL+C key press to prevent quitting
                    Console.CancelKeyPress += (sender, e) =>
                    {
                        e.Cancel = false; // Prevent the app from closing
                        //Console.WriteLine("\nTolerating CTRL+C!");
                    };
                }
            }
            catch (Exception exx)
            {
                starerror("An error occured");
            }
        }
        
    }
}
