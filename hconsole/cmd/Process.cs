using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.IO;



using System.Diagnostics;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;


using static hconsole.io.cout;
using static hconsole.io.outp;
using hconsole.io;


namespace hconsole.cmd
{
    #region __DEFINE_OTHERS__
    public class IdentifyCommand
    {
        public static List<string> RegisteredCommands = ["SUDO", "COFFEE"];
        public static List<string> StructuredCommands = [];
        public static void Identify(List<string> commands)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                //Console.WriteLine($"{i}: {commands[i]}");
                PleaseIdentifyThisForMe(commands[i]);
            }
        }

        public static List<string> ReturnThemPlease()
        {
            return StructuredCommands;
        }

        public static void CacheClean()
        {

            StructuredCommands.Clear();

        }

        public static void PleaseIdentifyThisForMe(string command_name)
        {
            if (string.IsNullOrEmpty(command_name)) { return; }
            else
            {
                if (RegisteredCommands.Contains(command_name.ToUpper()))
                {
                    //Console.WriteLine($"[+] Found!`{command_name.ToUpper()}` ");
                    StructuredCommands.Add("@" + command_name);
                }
                else if (!RegisteredCommands.Contains(command_name.ToUpper()))
                {
                    //Console.WriteLine($"[-] Not Found! `{command_name.ToUpper()}`");
                    StructuredCommands.Add(command_name);
                }
            }
        }
    }
    public class UserInput
    {
        public static string Input()
        {
            string input = Console.ReadLine();
            return input;
        }

        public static List<string> Prepare(string input)
        {
            try
            {
                List<string> result = [];
                if (input != null && input != string.Empty)
                {
                    result = input.Split(" ").ToList();
                }
                else
                {
                    result = [];
                }

                return result;
            }
            catch (Exception)
            {
                List<string> result = ["", ""];
                return result;
            }
        }

    }
    public class InputUtils
    {
        public static int WhatTypeIsThis(string thing)
        {
            if (thing == null) return 696969;

            // Check for path-like or special character indicators
            if (thing.Contains("/") || thing.Contains("\\"))
            {
                return ProcessLenOfThis(thing);
            }

            if (thing.Contains("%") || thing.Contains("&"))
            {
                return 2;
            }

            if (thing.Contains("root") || thing.Contains("*"))
            {
                return 1;
            }

            if (thing.Contains("#") || thing.Contains("$") || thing.Contains("|"))
            {
                return 3;
            }

            if (thing.Contains("::") || thing.Contains(":>>"))
            {
                return -1;
            }

            if (thing.StartsWith("("))
            {
                return 1;
            }

            return 0; // Default case if none of the conditions are met
        }

        public static string FurtherProcessThisPlease(string text)
        {
            return PleaseShortenThis(text);
        }

        public static int ProcessLenOfThis(string thing)
        {
            // Return 1 if the string length is 9 or greater
            return thing.Length >= 9 ? 1 : 0;
        }

        public static string PleaseShortenThis(string thing)
        {
            if (thing.Length < 350) return thing; // Return original if too short

            string shortenText = string.Concat(thing.Substring(0, 3));

            // Add ellipsis based on the length
            shortenText += new string('.', Math.Min(3, Math.Max(0, thing.Length - 7)));

            shortenText += thing.Substring(thing.Length - 4);

            return shortenText;
        }
    }
    internal class DesignFormat
    {
        public static void TakeInput(List<string> things)
        {
            if (things.Count < 2) return;

            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < things.Count; i++)
            {
                PrintStyledThing(things[i]);
            }

            // Reset to default color after processing
            Console.ResetColor();
        }

        public static string get_shell_icon(string shell)
        {

            string shell_icon = "";

            if (shell.ToLower() == "c2")
            {
                shell_icon = "#>";
            }
            else
            {
                shell_icon = "??";
            }

            return shell_icon;
        }

        private static void PrintStyledThing(string thing)
        {
            int type = InputUtils.WhatTypeIsThis(thing);

            // Determine color based on type
            switch (type)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    thing = InputUtils.FurtherProcessThisPlease(thing);
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
            }

            Console.Write(thing);
            Console.ForegroundColor = ConsoleColor.White; // Reset after printing
        }

        public static void Banner()
        {
            Console.WriteLine($@"
888888ba                                      .d88888b  .d8888P 
88    `8b                                     88.    ""' 88'     
88     88 .d8888b. dP   .dP .d8888b.          `Y88888b. 88baaa. 
88     88 88'  `88 88   d8' 88'  `88 88888888       `8b 88` `88 
88     88 88.  .88 88 .88'  88.  .88          d8'   .8P 8b. .d8 
dP     dP `88888P' 8888P'   `88888P8           Y88888P  `Y888P'

╔══════════════════════════════╗ ╔═════════════════════════════════════════════════════════════════════════╗
║ {Program.__shell__,-6} / {Program.__version__,-18}  ║ ║ Website: https://blog.hmza.vercel.app/posts/nova-project/               ║ 
║ Author: hmZa-Sfyn            ║ ║ Github: https://github.com/hmZa-Sfyn                                    ║
╚══════════════════════════════╝ ╚═════════════════════════════════════════════════════════════════════════╝                                                                                                            
╔═══════════════════════════════════════╗
║ Help: type `help` for help message    ║ 
╚═══════════════════════════════════════╝
 
");

        }

    }

    #endregion
    public class CommandEnv
    {
        public static string CURRENT_USER_NAME = $"noob{(DateTime.Now.Ticks - (DateTime.Now.Ticks - 250))}";//Novaf_Dokr.Utils.randomization.rand_user.UserName(1);
        public static string CURRENT_NODE_NAME = "127.0.0.1";


        public static string CurrentDirDest = UserHomeDir;

        public static string UserHomeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string novaEnvDir = Path.Combine(UserHomeDir, "vin_env", "vars");

        public static string EnvVarsFile = Path.Combine(novaEnvDir, "env_vars.json");
        public static string EnvPointersFile = Path.Combine(novaEnvDir, "env_pointers.json");
        public static string AliasesFile = Path.Combine(novaEnvDir, "aliases.json");

        public static List<string> DecoraterCommands = new List<string>();
        public static List<string> AlertCommands = new List<string>();

        public static Dictionary<string, string> EnvironmentVariables = new Dictionary<string, string>();
        public static Dictionary<string, string> EnvironmentPointers = new Dictionary<string, string>();
        public static Dictionary<string, string> Aliases = new Dictionary<string, string>();

        public static List<string> CommandHistory = new List<string>();

        static CommandEnv()
        {
            CurrentDirDest = Environment.CurrentDirectory;
            EnsureEnvironmentSetup();
            LoadEnvironmentVariables();
            LoadAliases();
        }
        // Ensure the directory and files exist, or create them after Listing an error
        // Ensure the directory and files exist, or create them after Listing an error
        public static void EnsureEnvironmentSetup()
        {
            try
            {
                if (!Directory.Exists(novaEnvDir))
                {
                    hconsole.io.exception.New("Error: Environment directory does not exist. Creating directory at " + novaEnvDir);
                    Directory.CreateDirectory(novaEnvDir);
                }

                if (!File.Exists(EnvVarsFile))
                {
                    hconsole.io.exception.New("Error: `/vars/env_vars.json` file not found. Creating file.");
                    File.WriteAllText(EnvVarsFile, "{}");
                }

                if (!File.Exists(AliasesFile))
                {
                    hconsole.io.exception.New("Error: `/vars/aliases.json` file not found. Creating file.");
                    File.WriteAllText(AliasesFile, "{}");
                }

                if (!File.Exists(EnvPointersFile))
                {
                    hconsole.io.exception.New("Error: `/vars/env_pointers.json` file not found. Creating file.");
                    File.WriteAllText(EnvPointersFile, "{}");
                }

                //hconsole.io.exception.ListThem();
                //hconsole.io.exception.CacheClean();
            }
            catch ( Exception ex)
            {
                hconsole.io.exception.New($"Error creating environment setup: {ex.Message}");
                //hconsole.io.exception.ListThem();
                //hconsole.io.exception.CacheClean();
            }
        }
        public static void SaveEnvironmentVariables()
        {
            File.WriteAllText(EnvVarsFile, JsonSerializer.Serialize(EnvironmentVariables, new JsonSerializerOptions { WriteIndented = true }));
        }
        public static void LoadEnvironmentVariables()
        {
            try
            {
                if (File.Exists(EnvVarsFile))
                {
                    string json = File.ReadAllText(EnvVarsFile);
                    EnvironmentVariables = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch ( Exception ex)
            {
                hconsole.io.exception.New($"Error loading environment variables: {ex.Message}");
                //hconsole.io.exception.ListThem();
                //hconsole.io.exception.CacheClean();
            }
        }
        public static void SaveEnvironmentPointers()
        {
            File.WriteAllText(EnvPointersFile, JsonSerializer.Serialize(EnvironmentPointers, new JsonSerializerOptions { WriteIndented = true }));
        }
        public static void LoadEnvironmentPointers()
        {
            try
            {
                if (File.Exists(EnvPointersFile))
                {
                    string json = File.ReadAllText(EnvPointersFile);
                    EnvironmentPointers = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch ( Exception ex)
            {
                hconsole.io.exception.New($"Error loading environment pointers: {ex.Message}");
                //hconsole.io.exception.ListThem();
                //hconsole.io.exception.CacheClean();
            }
        }
        public static void SaveAliases()
        {
            File.WriteAllText(AliasesFile, JsonSerializer.Serialize(Aliases, new JsonSerializerOptions { WriteIndented = true }));
        }
        public static void LoadAliases()
        {
            try
            {
                if (File.Exists(AliasesFile))
                {
                    string json = File.ReadAllText(AliasesFile);
                    Aliases = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
                }
            }
            catch ( Exception ex)
            {
                hconsole.io.exception.New($"Error loading aliases: {ex.Message}");
                //hconsole.io.exception.ListThem();
                //hconsole.io.exception.CacheClean();
            }
        }
        public static List<List<string>> DecoCommands(List<string> commands)
        {
            DecoraterCommands.Clear();

            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].StartsWith("@"))
                {
                    if (!commands[i].Equals("@"))
                    {
                        DecoraterCommands.Add(commands[i]);
                        commands.RemoveAt(i);
                        i--;
                    }
                    else if (commands[i].Equals("@"))
                    {
                        commands.RemoveAt(i);
                        i--;
                    }
                }
            }

            return new List<List<string>> { DecoraterCommands, commands };
        }
        public static List<string> SeperateThemCommands(List<string> commands)
        {
            List<string> SepCommands = new List<string>();
            StringBuilder currentCommand = new StringBuilder();

            foreach (var command in commands)
            {
                if (command == ";")
                {
                    SepCommands.Add(currentCommand.ToString().Trim());
                    currentCommand.Clear();
                }
                else
                {
                    if (currentCommand.Length > 0)
                    {
                        currentCommand.Append(" ");
                    }
                    currentCommand.Append(command);
                }
            }

            if (currentCommand.Length > 0)
            {
                SepCommands.Add(currentCommand.ToString().Trim());
            }

            return SepCommands;
        }
        public static void ProcessBinCommand(string[] parts)
        {
            if (parts[0] == "@bin")
            {
                if (parts.Length == 1 && parts[0] == "@bin")
                {
                    hconsole.io.exception.New($"Usage: @bin <command> - Display file contents in binary format");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                    return;
                }

                string executable = parts.Length > 1 ? parts[1] : parts[0];
                string[] args = parts.Skip(2).ToArray();

                string filePath = "";

                if (executable.StartsWith("./") || executable.StartsWith(".\\"))
                    if (executable.EndsWith(".exe"))
                        filePath = $"{Environment.CurrentDirectory}\\{executable}";
                    else
                        filePath = $"{Environment.CurrentDirectory}\\{executable}.exe";
                else
                    filePath = $"C:\\Users\\{Environment.UserName}\\vin_env\\bin\\{executable}\\{executable}.exe";

                try
                {
                    using (Process process = Process.Start(filePath, string.Join(" ", args)))
                    {
                        process.WaitForExit();
                    }
                }
                catch ( Exception ex)
                {
                    hconsole.io.exception.New($"e-s-p: Something wrong with `{filePath}`.");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                }
            }
            else
            {
                string executable = parts.Length > 1 ? parts[0] : parts[0];
                string[] args = parts.Skip(1).ToArray();

                string filePath = "";

                if (executable.StartsWith("./") || executable.StartsWith(".\\"))
                    if (executable.EndsWith(".exe"))
                        filePath = $"{Environment.CurrentDirectory}\\{executable}";
                    else
                        filePath = $"{Environment.CurrentDirectory}\\{executable}.exe";
                else
                    filePath = $"C:\\Users\\{Environment.UserName}\\vin_env\\bin\\{executable}\\{executable}.exe";

                try
                {
                    using (Process process = Process.Start(filePath, string.Join(" ", args)))
                    {
                        process.WaitForExit();
                    }
                }
                catch ( Exception ex)
                {
                    hconsole.io.exception.New($"e-s-p: Something wrong with `{filePath}`.");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                }
            }
        }
        
        public static string ReplaceEnvironmentVariables(string input)
        {
            return Regex.Replace(input, @"\$(\w+)|\$\((\w+)\)", match =>
            {
                string key = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Value;
                return EnvironmentVariables.TryGetValue(key, out string value) ? value : match.Value;
            });
        }
        //public static string ReplaceEnvironmentPointers(string input) // We dont need this!!! //
        //{
        //    return Regex.Replace(input, @":(\w+)", match =>
        //    {
        //        string key = match.Groups[1].Value;
        //        return EnvironmentPointers.TryGetValue(key, out string value) ? value : match.Value;
        //    });
        //}
        public static void CommandEnvEach(List<string> commands)
        {
            if (!commands.Any()) { return; }

            foreach (var command in commands)
            {
                if (command == "" || string.IsNullOrEmpty(command))
                    return;

                CommandHistory.Add(command);
                var parts = command.Split(' ');

                // Check if it is a get env arg value ` $(var_name) `
                //// List<string> parts = new List<string>();
                //// parts = ["@bin", "ls", "--path", "$(BinPath)"];
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = ReplaceEnvironmentVariables(parts[i]);
                    //if (parts[i].Contains(")."))
                    //{
                    //    if (parts[i].StartsWith("$("))
                    //    {
                    //        List<string> OPTS = new List<string>();

                    //        OPTS = parts[i].Split(".").ToList();

                    //        var tCmd = OPTS[1];

                    //        //foreach (var item in OPTS)
                    //        //{
                    //        //    Console.WriteLine(item);
                    //        //}

                    //        List<string> TotalOpts =
                    //        [
                    //            "toupper",  // implemented
                    //            "tolower",  // implemented

                    //            "tostring", // implemented
                    //            "toint",    // not implemented :: fooling user
                    //            "tofloat",  // not implemented :: fooling user
                    //            "todouble", // not implemented :: fooling user
                    //            "tolong",   // not implemented :: fooling user
                    //        ];

                    //        if (TotalOpts.Contains(tCmd))
                    //        {
                    //            if (tCmd.ToLower() == "toupper")
                    //            {
                    //                //Console.WriteLine($"{tCmd}: Found!");
                    //                {
                    //                    parts[i] = parts[i].Replace("$(", "");
                    //                    parts[i] = parts[i].Replace(").", "");
                    //                    parts[i] = parts[i].Replace($"{tCmd}", "");

                    //                    //Console.WriteLine($":: {parts[i]}");

                    //                    //writep(EnvironmentVariables[parts[i]]);
                    //                    try
                    //                    {
                    //                        parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //                        parts[i] = parts[i].ToUpper();
                    //                    }
                    //                    catch ( Exception exept)
                    //                    {
                    //                        //hconsole.io.exception.New(exept.ToString());
                    //                        hconsole.io.exception.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //                        //hconsole.io.exception.ListThem();
                    //                        //hconsole.io.exception.CacheClean();
                    //                        return;
                    //                    }
                    //                }
                    //            }
                    //            else if (tCmd.ToLower() == "tolower")
                    //            {
                    //                //Console.WriteLine($"{tCmd}: Found!");
                    //                {
                    //                    parts[i] = parts[i].Replace("$(", "");
                    //                    parts[i] = parts[i].Replace(").", "");
                    //                    parts[i] = parts[i].Replace($"{tCmd}", "");

                    //                    //Console.WriteLine($":: {parts[i]}");

                    //                    //writep(EnvironmentVariables[parts[i]]);
                    //                    try
                    //                    {
                    //                        parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //                        parts[i] = parts[i].ToLower();
                    //                    }
                    //                    catch ( Exception exept)
                    //                    {
                    //                        //hconsole.io.exception.New(exept.ToString());
                    //                        hconsole.io.exception.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //                        //hconsole.io.exception.ListThem();
                    //                        //hconsole.io.exception.CacheClean();
                    //                        return;
                    //                    }
                    //                }
                    //            }
                    //            else if (tCmd.ToLower().StartsWith("to"))
                    //            {
                    //                parts[i] = parts[i].Replace("$(", "");
                    //                parts[i] = parts[i].Replace(").", "");
                    //                parts[i] = parts[i].Replace($"{tCmd}", "");

                    //                //Console.WriteLine($":: {parts[i]}");

                    //                //writep(EnvironmentVariables[parts[i]]);

                    //                try
                    //                {
                    //                    parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //                }
                    //                catch ( Exception exept)
                    //                {
                    //                    //hconsole.io.exception.New(exept.ToString());
                    //                    hconsole.io.exception.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //                    //hconsole.io.exception.ListThem();
                    //                    //hconsole.io.exception.CacheClean();
                    //                    return;
                    //                }

                    //                if (tCmd.ToLower() == "tostring")
                    //                {
                    //                    parts[i] = parts[i].ToString();
                    //                }
                    //            }
                    //        }
                    //        else
                    //        {
                    //            //hconsole.io.exception.CacheClean();
                    //            hconsole.io.exception.New($"`{tCmd}` is not a real command in `{parts[i]}`");
                    //            //hconsole.io.exception.ListThem();
                    //            return;
                    //        }
                    //    }

                    //}
                    //else
                    //{
                    //    if (parts[i].StartsWith("$("))
                    //    {
                    //        parts[i] = parts[i].Replace("$(", "");
                    //        parts[i] = parts[i].Replace(")", "");

                    //        //Console.WriteLine($":: {parts[i]}");

                    //        //writep(EnvironmentVariables[parts[i]]);
                    //        try
                    //        {
                    //            parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //        }
                    //        catch ( Exception exept)
                    //        {
                    //            //hconsole.io.exception.New(exept.ToString());
                    //            hconsole.io.exception.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //            //hconsole.io.exception.ListThem();
                    //            //hconsole.io.exception.CacheClean();
                    //            return;
                    //        }
                    //    }
                    //    else if (parts[i].StartsWith("$"))
                    //    {
                    //        parts[i] = parts[i].Replace("$", "");

                    //        //Console.WriteLine($":: {parts[i]}");

                    //        //writep(EnvironmentVariables[parts[i]]);
                    //        try
                    //        {
                    //            parts[i] = parts[i].Replace(parts[i], EnvironmentVariables[parts[i]]);
                    //        }
                    //        catch ( Exception exept)
                    //        {
                    //            //hconsole.io.exception.New(exept.ToString());
                    //            hconsole.io.exception.New($"The given key '{parts[i]}' was not present in the env dictionary.");
                    //            //hconsole.io.exception.ListThem();
                    //            //hconsole.io.exception.CacheClean();
                    //            return;
                    //        }
                    //    }
                    //    else if (parts[i].StartsWith("$"))
                    //    {
                    //        // Remove the '$' to get the variable name
                    //        var vname = parts[i].Replace("$", "");

                    //        // Attempt to retrieve the variable value from the environment variables
                    //        if (EnvironmentVariables.ContainsKey(vname))
                    //        {
                    //            var basePath = EnvironmentVariables[vname]; // Get the base path

                    //            // Check if additional path segments exist
                    //            if (parts.Length > 1 && parts[i + 1].StartsWith("/"))
                    //            {
                    //                // Combine base path with the additional segments
                    //                parts[i] = Path.Combine(basePath, parts[i + 1].Substring(1)); // Remove leading '/' for concatenation
                    //            }
                    //            else
                    //            {
                    //                parts[i] = basePath; // No additional path segments, just use the base path
                    //            }
                    //        }
                    //        else
                    //        {
                    //            // Handle the case where the variable is not found
                    //            hconsole.io.exception.New($"The given key '{vname}' was not present in the env dictionary.");
                    //            //hconsole.io.exception.ListThem();
                    //            //hconsole.io.exception.CacheClean();
                    //            return;
                    //        }
                    //    }

                    //}
                }

                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].StartsWith("\\"))
                    {
                        parts[i] = parts[i].Replace("\\n", "\n");
                        parts[i] = parts[i].Replace("\\t", "\t");
                    }
                }

                string mainCommand = parts[0].ToLower();

                // Check if the command is an alias
                if (Aliases.ContainsKey(mainCommand))
                {
                    var aliasedCommand = Aliases[mainCommand];
                    parts = (aliasedCommand + " " + string.Join(" ", parts.Skip(1))).Split(' ');
                    mainCommand = parts[0].ToLower();
                }

                Console.WriteLine("");

                switch (mainCommand)
                {
                    case "@help":
                    case "help":
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("""
                            @help      // Get this help message
                            @run       // To run a command script
                            @cls       // Clear the console
                            @exit      // Exit the application
                            @let       // Manage environment variables ('set', 'get', 'list', 'free')
                            @alias     // Manage command aliases ('add', 'remove', 'list')
                            @history   // View or clear command history
                            @bin       // Display file contents in binary format
                            @cd        // Change the current directory

                            @sh        // Shell language interpreter (beta)
                            """);
                        Console.ResetColor();
                        break;

                    case "@sslib":
                        ProcessStdCommand(parts);
                        break;

                    case "@cls":
                        Console.Clear();
                        break;

                    case "@run":
                        if (parts.Length > 1)
                        {
                            if (!File.Exists(parts[1]))
                            {
                                //hconsole.io.exception.CacheClean();
                                hconsole.io.exception.New($"Error: `{parts[1]}` file not present.");
                                //hconsole.io.exception.ListThem();
                                //hconsole.io.exception.CacheClean();
                            }
                            else
                            {
                                {
                                    IdentifyCommand.CacheClean();

                                    List<string> commandsz = UserInput.Prepare(File.ReadAllText(parts[1]));

                                    IdentifyCommand.Identify(commandsz);
                                    List<string> parsed_commandsz = IdentifyCommand.ReturnThemPlease();

                                    //foreach (string command in parsed_commands) { Console.WriteLine(command); }

                                    PleaseCommandEnv.TheseCommands(parsed_commandsz);

                                    IdentifyCommand.CacheClean();
                                }
                            }
                        }
                        else
                        {
                            //hconsole.io.exception.CacheClean();
                            hconsole.io.exception.New($"Error: Usage: `@run filepath.sh`");
                            //hconsole.io.exception.ListThem();
                            //hconsole.io.exception.CacheClean();
                        }
                        break;

                    case "@shell":
                    case "@sh":
                        hconsole.cmd.sh.InterPreter.REPL();
                        break;

                    case "@cd":
                        CommandEnvCdCommand(parts);
                        break;

                    case "@exit":
                        Console.WriteLine("Exiting the application...");
                        Environment.Exit(0);
                        break;

                    case "@let":
                        CommandEnvEnvCommand(parts);
                        break;

                    case "@alias":
                        CommandEnvAliasCommand(parts);
                        break;

                    case "@history":
                        CommandEnvHistoryCommand(parts);
                        break;

                    case "@bin":
                        ProcessBinCommand(parts);
                        break;

                    default:
                        ProcessBinCommand(parts);
                        //hconsole.io.exception.New($"Command: `{command}` is not a valid internal command, type `@help` for help!");
                        ////hconsole.io.exception.ListThem();
                        ////hconsole.io.exception.CacheClean();
                        break;
                }
            }
        }
        public static void CommandEnvCdCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                hconsole.io.exception.New($"Usage: @cd <directory> - Change the current working directory.");
                //hconsole.io.exception.ListThem();
                //hconsole.io.exception.CacheClean();
                return;
            }

            string newDir = parts[1];

            try
            {
                if (newDir == "..")
                {
                    // Move up one directory level
                    newDir = Path.GetDirectoryName(CurrentDirDest);
                }
                else if (newDir == "~")
                {
                    newDir = UserHomeDir;
                }
                else if (!Path.IsPathRooted(newDir))
                {
                    newDir = Path.Combine(CurrentDirDest, newDir);
                }

                if (newDir == null)
                {
                    hconsole.io.exception.New($"Error: Cannot navigate above the root directory.");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                    return;
                }

                newDir = Path.GetFullPath(newDir);

                if (Directory.Exists(newDir))
                {
                    CurrentDirDest = newDir;
                    Environment.CurrentDirectory = newDir;  // Ensure the process working directory is also updated
                    //Console.WriteLine($"Changed directory to: {CurrentDirDest}");
                }
                else
                {
                    hconsole.io.exception.New($"Error: Directory '{newDir}' does not exist.");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                }
            }
            catch ( Exception ex)
            {
                hconsole.io.exception.New($"Error: {ex.Message}");
                //hconsole.io.exception.ListThem();
                //hconsole.io.exception.CacheClean();
            }
        }
        public static void ProcessStdCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            try
            {
                // Get the current user's name
                string currentUser = Environment.UserName;

                // Base path to the nova std directory
                string basePath = $@"C:\Users\{currentUser}\vin_env\third_party\nova\std\";

                // Extract the command after "@sslib"
                string stdCommand = parts[1];
                string[] commandParts = stdCommand.Split('.'); // Split on dot to detect category, class, etc.

                // Start building the path
                string commandPath = basePath;
                foreach (var part in commandParts)
                {
                    commandPath = Path.Combine(commandPath, part); // Keep adding to the path for each command part
                }

                // The remaining parts are the command arguments (after the first two parts)
                string[] commandArgs = parts.Skip(2).ToArray();  // All arguments after the command

                // Check if the command is already an executable (like ls.exe)
                if (!commandPath.EndsWith(".exe"))
                {
                    // Try to add .exe to the command path to see if it's an executable
                    if (File.Exists(commandPath + ".exe"))
                    {
                        commandPath += ".exe";  // Append .exe and use it
                    }
                    else if (Directory.Exists(commandPath))
                    {
                        // If it's a directory, check if there is an executable inside the directory
                        string executableFile = Directory.EnumerateFiles(commandPath, "*.exe").FirstOrDefault();
                        if (executableFile != null)
                        {
                            commandPath = executableFile;  // Use the first found executable in the directory
                        }
                        else
                        {
                            Console.WriteLine($"Error: No executable found in {commandPath}");
                            return;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: Command not found at {commandPath}");
                        return;
                    }
                }

                // Try to execute the command with the given arguments
                try
                {
                    ProcessStartInfo processInfo = new ProcessStartInfo
                    {
                        FileName = commandPath,
                        Arguments = string.Join(" ", commandArgs),  // Join the arguments into a single string
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process process = Process.Start(processInfo))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            Console.WriteLine(result);  // Output the result from the executed command
                        }
                    }
                }
                catch ( Exception ex)
                {
                    Console.WriteLine($"Error executing command: {ex.Message}");
                }
            }
            catch ( Exception ex)
            {
                Console.WriteLine($"Error processing command: {ex.Message}");
            }
        }
        public static void CommandEnvEnvCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            switch (parts[1].ToLower())
            {
                case "set":
                    if (parts.Length >= 4)
                    {
                        EnvironmentVariables[parts[2]] = string.Join(" ", parts.Skip(3));
                        SaveEnvironmentVariables();
                        //Console.WriteLine($"Environment variable '{parts[2]}' set.");
                        writep($"Environment variable '{parts[2]}' set to `{string.Join(" ", parts.Skip(3))}`.");
                    }
                    else
                        exception.New("Usage: set $name $value");
                    break;
                case "get":
                    if (parts.Length >= 3 && EnvironmentVariables.ContainsKey(parts[2]))
                    {
                        writep(EnvironmentVariables[parts[2]]);
                    }
                    else
                    {
                        hconsole.io.exception.New($"Environment variable '{parts[2]}' not found.");
                        //hconsole.io.exception.ListThem();
                        //hconsole.io.exception.CacheClean();
                    }
                    break;
                case "list":
                    if (EnvironmentVariables.Count == 0)
                    {
                        hconsole.io.exception.New("No environment variables set.");
                        //hconsole.io.exception.ListThem();
                        //hconsole.io.exception.CacheClean();
                    }
                    else
                    {
                        foreach (var kvp in EnvironmentVariables)
                        {
                            writep($" {kvp.Key} ==> {kvp.Value}");
                        }
                    }
                    break;
                case "free":
                    if (parts.Length >= 3)
                    {
                        if (EnvironmentVariables.Remove(parts[2]))
                        {
                            SaveEnvironmentVariables();
                            writep($"Environment variable '{parts[2]}' removed.");
                        }
                        else
                        {
                            hconsole.io.exception.New($"Environment variable '{parts[2]}' not found.");
                            //hconsole.io.exception.ListThem();
                            //hconsole.io.exception.CacheClean();
                        }
                    }
                    break;
                default:
                    hconsole.io.exception.New("Invalid @let command. Use 'set', 'get', 'list', or 'free'.");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                    break;
            }
        }
        public static void CommandEnvAliasCommand(string[] parts)
        {
            if (parts.Length < 2) return;

            switch (parts[1].ToLower())
            {
                case "add":
                    if (parts.Length >= 4)
                    {
                        Aliases[parts[2]] = string.Join(" ", parts.Skip(3));
                        SaveAliases();
                        writep($"Alias '{parts[2]}' added.");
                    }
                    else
                    {
                        hconsole.io.exception.New("Invalid alias command. Use '@alias add [name] [command]'.");
                        //hconsole.io.exception.ListThem();
                        //hconsole.io.exception.CacheClean();
                    }
                    break;
                case "remove":
                    if (parts.Length >= 3)
                    {
                        if (Aliases.Remove(parts[2]))
                        {
                            SaveAliases();
                            writep($"Alias '{parts[2]}' removed.");
                        }
                        else
                        {
                            hconsole.io.exception.New($"Alias '{parts[2]}' not found.");
                            //hconsole.io.exception.ListThem();
                            //hconsole.io.exception.CacheClean();
                        }
                    }
                    else
                        Console.WriteLine("Invalid alias command. Use '@alias remove [name]'.");
                    break;
                case "list":
                    if (Aliases.Count == 0)
                    {
                        hconsole.io.exception.New("No aliases defined.");
                        //hconsole.io.exception.ListThem();
                        //hconsole.io.exception.CacheClean();
                    }
                    else
                    {
                        foreach (var kvp in Aliases)
                        {
                            writep($" {kvp.Key} ==> {kvp.Value}");
                        }
                    }
                    break;
                default:
                    hconsole.io.exception.New("Invalid @alias command. Use 'add', 'remove', or 'list'.");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                    break;
            }
        }
        public static void CommandEnvHistoryCommand(string[] parts)
        {
            if (parts.Length > 1 && parts[1].ToLower() == "clear")
            {
                CommandHistory.Clear();
                writep("Command history cleared.");
            }
            else
            {
                if (CommandHistory.Count == 0)
                {
                    hconsole.io.exception.New("Command history is empty.");
                    //hconsole.io.exception.ListThem();
                    //hconsole.io.exception.CacheClean();
                }
                else
                    for (int i = 0; i < CommandHistory.Count; i++)
                        Console.WriteLine($"{i + 1}: {CommandHistory[i]}");
            }
        }
        
    }

    public class PleaseCommandEnv
    {
        public static void TheseCommands(List<string> commands)
        {
            List<string> separatedCommands = CommandEnv.SeperateThemCommands(commands);
            CommandEnv.CommandEnvEach(separatedCommands);
        }
    }
}