using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System.Net;

namespace hconsole.cmd.sh
{
    public class InterPreter
    {
        private Dictionary<string, object> variables = new Dictionary<string, object>();
        private Dictionary<string, bool> varMutability = new Dictionary<string, bool>();
        private Dictionary<string, Func<List<object>, Task<object>>> functions = new Dictionary<string, Func<List<object>, Task<object>>>();
        private Dictionary<string, Dictionary<string, object>> objects = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, Func<List<object>, Task<object>>> objectMethods = new Dictionary<string, Func<List<object>, Task<object>>>();
        private List<string> exports = new List<string>();
        private Dictionary<string, Dictionary<string, object>> modules = new Dictionary<string, Dictionary<string, object>>();
        private Dictionary<string, Func<List<object>, Task<object>>> moduleFunctions = new Dictionary<string, Func<List<object>, Task<object>>>();

        public InterPreter()
        {
            InitializeBuiltInModules();
        }

        public static void REPL()
        {
            var interpreter = new InterPreter();
            Console.Clear();
            Console.WriteLine("JavaScript-like Interpreter v1.5");

            while (true)
            {
                Console.Write(">>> ");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                List<string> commandLines = new List<string> { input };

                while (!input.EndsWith(";"))
                {
                    Console.Write("... ");
                    string nextLine = Console.ReadLine();
                    commandLines.Add(nextLine);
                    input += "\n" + nextLine;
                    if (nextLine.EndsWith(";")) break;
                }

                try
                {
                    interpreter.ProcessCommand(string.Join("\n", commandLines)).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        public async Task ProcessScript(string script)
        {
            var lines = script.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                await ProcessCommand(line.Trim());
            }
        }

        private void InitializeBuiltInModules()
        {
            // std module
            var stdModule = new Dictionary<string, object>();
            modules["std"] = stdModule;

            moduleFunctions["std.print"] = async (args) =>
            {
                foreach (var arg in args)
                    Console.Write(arg + " ");
                Console.WriteLine();
                return null;
            };

            moduleFunctions["std.add"] = async (args) =>
            {
                if (args.Count != 2) throw new Exception("std.add expects 2 arguments");
                return Convert.ToDouble(args[0]) + Convert.ToDouble(args[1]);
            };

            moduleFunctions["std.sqrt"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("std.sqrt expects 1 argument");
                return Math.Sqrt(Convert.ToDouble(args[0]));
            };

            moduleFunctions["std.push"] = async (args) =>
            {
                if (args.Count < 2) throw new Exception("std.push expects at least 2 arguments");
                if (!(args[0] is List<object> list)) throw new Exception("First argument must be an array");
                list.AddRange(args.Skip(1));
                return list;
            };

            moduleFunctions["std.pop"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("std.pop expects 1 argument");
                if (!(args[0] is List<object> list)) throw new Exception("Argument must be an array");
                if (list.Count == 0) throw new Exception("Array is empty");
                var last = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return last;
            };

            moduleFunctions["std.length"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("std.length expects 1 argument");
                if (!(args[0] is List<object> list)) throw new Exception("Argument must be an array");
                return list.Count;
            };

            moduleFunctions["std.help"] = async (args) =>
            {
                Console.WriteLine(@"
Available Commands and Syntax:
  Variables:
    var x = 10;          // Mutable variable
    let x = 10;          // Immutable variable
    const x = 10;        // Constant (non-reassignable)
  Arrays:
    let arr = [1, 2, 3]; // Array literal
    std.push(arr, 4);    // Add to array
    std.pop(arr);        // Remove last element
    std.length(arr);     // Get array length
  Functions:
    x = (a, b) => { a + b }; // Arrow function
  Objects:
    object Point { x: 0, y: 0, move: (dx) => { this.x + dx } }
    Point.x              // Access field
    Point.move(5)        // Call method
  Loops:
    for (let i = 0; i < 10; i = i + 1) { print(i); }
    foreach (let x in arr) { print(x); }
    while (x < 10) { print(x); x = x + 1; }
  Import/Export:
    import { print, add } from ""std""; // Import from module
    export x;            // Export variable/function/object
  Modules:
    std: print, add, sqrt, push, pop, length, help
    http: get, post
    tcp: listen, connect, send
    env: get, set
    stdio: write, output, readLine, input
    powershell: run
");
                return null;
            };

            // http module
            var httpModule = new Dictionary<string, object>();
            modules["http"] = httpModule;
            moduleFunctions["http.get"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("http.get expects 1 argument (URL)");
                string url = args[0].ToString();
                using (var client = new HttpClient())
                {
                    try
                    {
                        return await client.GetStringAsync(url);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"HTTP GET failed: {ex.Message}");
                    }
                }
            };

            moduleFunctions["http.post"] = async (args) =>
            {
                if (args.Count != 2) throw new Exception("http.post expects 2 arguments (URL, data)");
                string url = args[0].ToString();
                string data = args[1].ToString();
                using (var client = new HttpClient())
                {
                    try
                    {
                        var content = new StringContent(data, Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(url, content);
                        return await response.Content.ReadAsStringAsync();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"HTTP POST failed: {ex.Message}");
                    }
                }
            };

            // tcp module
            var tcpModule = new Dictionary<string, object>();
            modules["tcp"] = tcpModule;
            moduleFunctions["tcp.listen"] = async (args) =>
            {
                if (args.Count != 2) throw new Exception("tcp.listen expects 2 arguments (port, callback)");
                int port = Convert.ToInt32(args[0]);
                if (!(args[1] is string callbackName) || !functions.ContainsKey(callbackName))
                    throw new Exception("Second argument must be a callback function name");

                try
                {
                    var listener = new TcpListener(IPAddress.Any, port);
                    listener.Start();
                    Console.WriteLine($"Server listening on port {port}");

                    while (true)
                    {
                        using (var client = await listener.AcceptTcpClientAsync())
                        using (var stream = client.GetStream())
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            var result = await functions[callbackName](new List<object> { message });

                            if (result != null)
                            {
                                byte[] response = Encoding.UTF8.GetBytes(result.ToString());
                                await stream.WriteAsync(response, 0, response.Length);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"TCP server failed: {ex.Message}");
                }
            };

            moduleFunctions["tcp.connect"] = async (args) =>
            {
                if (args.Count != 2) throw new Exception("tcp.connect expects 2 arguments (host, port)");
                string host = args[0].ToString();
                int port = Convert.ToInt32(args[1]);
                try
                {
                    var client = new TcpClient(host, port);
                    return "Connected";
                }
                catch (Exception ex)
                {
                    throw new Exception($"TCP connect failed: {ex.Message}");
                }
            };

            moduleFunctions["tcp.send"] = async (args) =>
            {
                if (args.Count != 3) throw new Exception("tcp.send expects 3 arguments (host, port, data)");
                string host = args[0].ToString();
                int port = Convert.ToInt32(args[1]);
                string data = args[2].ToString();
                try
                {
                    using (var client = new TcpClient(host, port))
                    using (var stream = client.GetStream())
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(data);
                        await stream.WriteAsync(bytes, 0, bytes.Length);
                        byte[] buffer = new byte[1024];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"TCP send failed: {ex.Message}");
                }
            };

            // env module
            var envModule = new Dictionary<string, object>();
            modules["env"] = envModule;
            moduleFunctions["env.get"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("env.get expects 1 argument (key)");
                string key = args[0].ToString();
                return Environment.GetEnvironmentVariable(key) ?? "null";
            };

            moduleFunctions["env.set"] = async (args) =>
            {
                if (args.Count != 2) throw new Exception("env.set expects 2 arguments (key, value)");
                string key = args[0].ToString();
                string value = args[1].ToString();
                Environment.SetEnvironmentVariable(key, value);
                return null;
            };

            // stdio module
            var stdioModule = new Dictionary<string, object>();
            modules["stdio"] = stdioModule;
            moduleFunctions["stdio.write"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("stdio.write expects 1 argument");
                Console.Write(args[0].ToString());
                return null;
            };

            moduleFunctions["stdio.output"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("stdio.output expects 1 argument");
                Console.Write(args[0].ToString());
                return null;
            };

            moduleFunctions["stdio.readLine"] = async (args) =>
            {
                return Console.ReadLine();
            };

            moduleFunctions["stdio.input"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("stdio.input expects 1 argument (prompt)");
                Console.Write(args[0].ToString());
                return Console.ReadLine();
            };

            // powershell module
            var powershellModule = new Dictionary<string, object>();
            modules["powershell"] = powershellModule;
            moduleFunctions["powershell.run"] = async (args) =>
            {
                if (args.Count != 1) throw new Exception("powershell.run expects 1 argument (command)");
                string command = args[0].ToString();
                try
                {
                    var process = new System.Diagnostics.Process
                    {
                        StartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = "powershell",
                            Arguments = $"-Command \"{command}\"",
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;
                }
                catch (Exception ex)
                {
                    throw new Exception($"PowerShell command failed: {ex.Message}");
                }
            };
        }

        private async Task ProcessCommand(string input)
        {
            input = input.Trim();
            if (input.EndsWith(";")) input = input.Substring(0, input.Length - 1);

            if (input.StartsWith("var ") || input.StartsWith("let ") || input.StartsWith("const "))
            {
                await ProcessVariableDeclaration(input);
            }
            else if (input.Contains("=>"))
            {
                await ProcessFunctionDeclaration(input);
            }
            else if (input.StartsWith("object "))
            {
                await ProcessObjectDeclaration(input);
            }
            else if (input.StartsWith("import "))
            {
                await ProcessImport(input);
            }
            else if (input.StartsWith("export "))
            {
                await ProcessExport(input);
            }
            else if (input.StartsWith("for ") || input.StartsWith("foreach ") || input.StartsWith("while "))
            {
                await ProcessLoop(input);
            }
            else
            {
                object result = await EvaluateExpression(input);
                if (result != null)
                    Console.WriteLine(result);
            }
        }

        private async Task ProcessVariableDeclaration(string input)
        {
            bool isConst = input.StartsWith("const ");
            bool isVar = input.StartsWith("var ");
            string declaration = input.StartsWith("var ") ? input.Substring(4) : input.StartsWith("let ") ? input.Substring(4) : input.Substring(6);
            var parts = declaration.Split('=').Select(p => p.Trim()).ToArray();

            if (parts.Length != 2)
                throw new Exception("Invalid variable declaration syntax");

            string varName = parts[0].Trim();
            string value = parts[1].Trim();

            if (isConst && variables.ContainsKey(varName))
                throw new Exception($"Cannot reassign constant variable '{varName}'");

            object evaluatedValue = await EvaluateExpression(value);
            variables[varName] = evaluatedValue;
            varMutability[varName] = isVar || input.StartsWith("let ");
            Console.WriteLine($"Declared {varName} = {evaluatedValue}");
        }

        private async Task ProcessFunctionDeclaration(string input)
        {
            var match = Regex.Match(input, @"(\w+)\s*=\s*\((.*?)\)\s*=>\s*{([\s\S]*?)}");
            if (!match.Success)
                throw new Exception("Invalid function declaration syntax");

            string funcName = match.Groups[1].Value;
            string[] parameters = ParseArguments(match.Groups[2].Value).ToArray();
            string body = match.Groups[3].Value.Trim();

            functions[funcName] = async (args) =>
            {
                if (args.Count != parameters.Length)
                    throw new Exception($"Function '{funcName}' expects {parameters.Length} arguments, got {args.Count}");

                var oldVars = new Dictionary<string, object>(variables);
                for (int i = 0; i < parameters.Length; i++)
                    variables[parameters[i]] = args[i];

                object result = await EvaluateExpression(body);
                variables = oldVars;
                return result;
            };

            Console.WriteLine($"Defined function {funcName}");
        }

        private async Task ProcessObjectDeclaration(string input)
        {
            var match = Regex.Match(input, @"object (\w+)\s*{([\s\S]*?)}");
            if (!match.Success)
                throw new Exception("Invalid object declaration syntax");

            string objName = match.Groups[1].Value;
            string[] fields = match.Groups[2].Value.Split(',').Select(f => f.Trim()).Where(f => !string.IsNullOrEmpty(f)).ToArray();
            var objFields = new Dictionary<string, object>();

            foreach (var field in fields)
            {
                if (field.Contains("=>"))
                {
                    var methodMatch = Regex.Match(field, @"(\w+)\s*\((.*?)\)\s*=>\s*{([\s\S]*?)}");
                    if (methodMatch.Success)
                    {
                        string methodName = $"{objName}.{methodMatch.Groups[1].Value}";
                        string[] parameters = ParseArguments(methodMatch.Groups[2].Value).ToArray();
                        string body = methodMatch.Groups[3].Value.Trim();

                        objectMethods[methodName] = async (args) =>
                        {
                            if (args.Count != parameters.Length + 1)
                                throw new Exception($"Method '{methodName}' expects {parameters.Length + 1} arguments");

                            var oldVars = new Dictionary<string, object>(variables);
                            variables["this"] = args[0];
                            for (int i = 0; i < parameters.Length; i++)
                                variables[parameters[i]] = args[i + 1];

                            object result = await EvaluateExpression(body);
                            variables = oldVars;
                            return result;
                        };
                    }
                    else
                    {
                        throw new Exception("Invalid method declaration syntax in object");
                    }
                }
                else
                {
                    var parts = field.Split(':').Select(p => p.Trim()).ToArray();
                    if (parts.Length == 2)
                        objFields[parts[0]] = await EvaluateExpression(parts[1]);
                    else
                        objFields[field] = null;
                }
            }

            objects[objName] = objFields;
            variables[objName] = objFields;
            Console.WriteLine($"Defined object {objName}");
        }

        private async Task ProcessImport(string input)
        {
            var match = Regex.Match(input, @"import \{([^}]*)\} from ['""](.*?)['""]");
            if (!match.Success)
                throw new Exception("Invalid import syntax");

            string[] importedItems = match.Groups[1].Value.Split(',').Select(i => i.Trim()).Where(i => !string.IsNullOrEmpty(i)).ToArray();
            string moduleName = match.Groups[2].Value;

            if (!modules.ContainsKey(moduleName))
                throw new Exception($"Module '{moduleName}' not found");

            foreach (var item in importedItems)
            {
                string moduleFunctionKey = $"{moduleName}.{item}";
                if (moduleFunctions.ContainsKey(moduleFunctionKey))
                {
                    functions[item] = moduleFunctions[moduleFunctionKey];
                    Console.WriteLine($"Imported function {item} from {moduleName}");
                }
                else if (modules[moduleName].ContainsKey(item))
                {
                    variables[item] = modules[moduleName][item];
                    Console.WriteLine($"Imported {item} from {moduleName}");
                }
                else
                {
                    throw new Exception($"Item '{item}' not found in module '{moduleName}'");
                }
            }
        }

        private async Task ProcessExport(string input)
        {
            string item = input.Substring(7).Trim();
            if (variables.ContainsKey(item) || functions.ContainsKey(item) || objects.ContainsKey(item))
            {
                exports.Add(item);
                Console.WriteLine($"Exported {item}");
            }
            else
            {
                throw new Exception($"Cannot export undefined item '{item}'");
            }
        }

        private async Task ProcessLoop(string input)
        {
            if (input.StartsWith("for "))
            {
                var match = Regex.Match(input, @"for \((.*?);(.*?);(.*?)\)\s*{([\s\S]*?)}");
                if (!match.Success)
                    throw new Exception("Invalid for loop syntax");

                string init = match.Groups[1].Value.Trim();
                string condition = match.Groups[2].Value.Trim();
                string update = match.Groups[3].Value.Trim();
                string body = match.Groups[4].Value.Trim();

                await ProcessVariableDeclaration(init);
                while (Convert.ToBoolean(await EvaluateExpression(condition)))
                {
                    await EvaluateExpression(body);
                    await EvaluateExpression(update);
                }
            }
            else if (input.StartsWith("foreach "))
            {
                var match = Regex.Match(input, @"foreach \((.*?)\s+in\s+(\w+)\)\s*{([\s\S]*?)}");
                if (!match.Success)
                    throw new Exception("Invalid foreach loop syntax");

                string varName = match.Groups[1].Value.Replace("let ", "").Trim();
                string arrayName = match.Groups[2].Value.Trim();
                string body = match.Groups[3].Value.Trim();

                if (!variables.ContainsKey(arrayName) || !(variables[arrayName] is List<object>))
                    throw new Exception($"Variable '{arrayName}' is not an array");

                var array = (List<object>)variables[arrayName];
                var oldVars = new Dictionary<string, object>(variables);
                foreach (var item in array)
                {
                    variables[varName] = item;
                    await EvaluateExpression(body);
                }
                variables = oldVars;
            }
            else if (input.StartsWith("while "))
            {
                var match = Regex.Match(input, @"while \((.*?)\)\s*{([\s\S]*?)}");
                if (!match.Success)
                    throw new Exception("Invalid while loop syntax");

                string condition = match.Groups[1].Value.Trim();
                string body = match.Groups[2].Value.Trim();

                while (Convert.ToBoolean(await EvaluateExpression(condition)))
                {
                    await EvaluateExpression(body);
                }
            }
        }

        private List<string> ParseArguments(string argsStr)
        {
            var args = new List<string>();
            var currentArg = new StringBuilder();
            int parenCount = 0;
            bool inQuotes = false;

            for (int i = 0; i < argsStr.Length; i++)
            {
                char c = argsStr[i];
                if (c == '"' && (i == 0 || argsStr[i - 1] != '\\'))
                {
                    inQuotes = !inQuotes;
                    currentArg.Append(c);
                }
                else if (c == '(' && !inQuotes)
                {
                    parenCount++;
                    currentArg.Append(c);
                }
                else if (c == ')' && !inQuotes)
                {
                    parenCount--;
                    currentArg.Append(c);
                }
                else if (c == ',' && parenCount == 0 && !inQuotes)
                {
                    args.Add(currentArg.ToString().Trim());
                    currentArg.Clear();
                }
                else
                {
                    currentArg.Append(c);
                }
            }

            if (currentArg.Length > 0)
                args.Add(currentArg.ToString().Trim());

            return args;
        }

        private async Task<object> EvaluateExpression(string expression)
        {
            expression = expression.Trim();
            if (string.IsNullOrEmpty(expression))
                return null;

            // Handle array literals
            var arrayMatch = Regex.Match(expression, @"\[(.*?)\]");
            if (arrayMatch.Success)
            {
                string[] elements = ParseArguments(arrayMatch.Groups[1].Value).ToArray();
                var result = new List<object>();
                foreach (var e in elements)
                    result.Add(await EvaluateExpression(e));
                return result;
            }

            // Handle function calls
            var funcMatch = Regex.Match(expression, @"(\w+)\(([\s\S]*?)\)");
            if (funcMatch.Success)
            {
                string funcName = funcMatch.Groups[1].Value;
                string argsStr = funcMatch.Groups[2].Value;
                List<string> args = ParseArguments(argsStr);
                List<object> argValues = new List<object>();
                foreach (var a in args)
                    argValues.Add(await EvaluateExpression(a));

                if (funcName == "await" && argValues.Count == 1)
                {
                    if (argValues[0] is Task<object> task)
                        return await task;
                    throw new Exception("await can only be used with async functions");
                }

                if (functions.ContainsKey(funcName))
                    return await functions[funcName](argValues);
                if (objectMethods.ContainsKey(funcName))
                    return await objectMethods[funcName](argValues);
                throw new Exception($"Unknown function or method '{funcName}'");
            }

            // Handle object field access
            var fieldMatch = Regex.Match(expression, @"(\w+)\.(\w+)");
            if (fieldMatch.Success)
            {
                string objName = fieldMatch.Groups[1].Value;
                string fieldName = fieldMatch.Groups[2].Value;
                if (objects.ContainsKey(objName) && objects[objName].ContainsKey(fieldName))
                    return objects[objName][fieldName];
                if (variables.ContainsKey(objName) && variables[objName] is Dictionary<string, object> obj && obj.ContainsKey(fieldName))
                    return obj[fieldName];
                throw new Exception($"Unknown field '{fieldName}' in object '{objName}'");
            }

            // Handle variable assignment
            var assignMatch = Regex.Match(expression, @"(\w+)\s*=\s*(.+)");
            if (assignMatch.Success)
            {
                string varName = assignMatch.Groups[1].Value;
                string value = assignMatch.Groups[2].Value;

                if (!variables.ContainsKey(varName))
                    throw new Exception($"Variable '{varName}' not declared");
                if (!varMutability[varName])
                    throw new Exception($"Cannot assign to immutable variable '{varName}'");

                object evaluatedValue = await EvaluateExpression(value);
                variables[varName] = evaluatedValue;
                return evaluatedValue;
            }

            // Handle basic literals and variables
            if (variables.ContainsKey(expression))
                return variables[expression];

            if (double.TryParse(expression, out double number))
                return number;

            if (expression.StartsWith("\"") && expression.EndsWith("\""))
                return expression.Substring(1, expression.Length - 2);

            // Handle arithmetic operations
            var arithmeticMatch = Regex.Match(expression, @"(.+?)\s*([+\-*/])\s*(.+?)");
            if (arithmeticMatch.Success)
            {
                string left = arithmeticMatch.Groups[1].Value;
                string op = arithmeticMatch.Groups[2].Value;
                string right = arithmeticMatch.Groups[3].Value;

                double leftVal = Convert.ToDouble(await EvaluateExpression(left));
                double rightVal = Convert.ToDouble(await EvaluateExpression(right));

                switch (op)
                {
                    case "+": return leftVal + rightVal;
                    case "-": return leftVal - rightVal;
                    case "*": return leftVal * rightVal;
                    case "/": return rightVal != 0 ? leftVal / rightVal : throw new Exception("Division by zero");
                    default: throw new Exception($"Unknown operator '{op}'");
                }
            }

            // Handle comparison for loops
            var comparisonMatch = Regex.Match(expression, @"(.+?)\s*(<=|>=|<|>)\s*(.+?)");
            if (comparisonMatch.Success)
            {
                string left = comparisonMatch.Groups[1].Value;
                string op = comparisonMatch.Groups[2].Value;
                string right = comparisonMatch.Groups[3].Value;

                double leftVal = Convert.ToDouble(await EvaluateExpression(left));
                double rightVal = Convert.ToDouble(await EvaluateExpression(right));

                switch (op)
                {
                    case "<": return leftVal < rightVal;
                    case ">": return leftVal > rightVal;
                    case "<=": return leftVal <= rightVal;
                    case ">=": return leftVal >= rightVal;
                    default: throw new Exception($"Unknown comparison operator '{op}'");
                }
            }

            throw new Exception($"Invalid expression: {expression}");
        }
    }
}