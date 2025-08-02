using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static hconsole.io.cout;

namespace hconsole.io
{
    public class exception
    {
        // List to store error messages
        public static List<string> ErrorsAre = new List<string>();

        // Add a new error message to the list
        public static void New(string err_msg)
        {
            ErrorsAre.Add(err_msg);
        }

        // Clear the error message cache
        public static void CacheClean()
        {
            ErrorsAre.Clear();
        }

        // List errors, use errlast for the last error and .err for others
        public static void ListThem()
        {
            if (ErrorsAre.Count > 0)
            {
                if (ErrorsAre.Count == 1)
                {
                     erroroutputs.errinfo($"Found an Error!");

                    for (int i = 0; i < ErrorsAre.Count - 1; i++)
                    {
                         erroroutputs.err(ErrorsAre[i], "err"); // Regular errors
                    }

                    // Use errlast for the last error
                    errlast(ErrorsAre[^1]);
                }
                else if (ErrorsAre.Count <= 20)
                {
                     erroroutputs.errinfo($"Found {ErrorsAre.Count} Errors: Listing {ErrorsAre.Count} out of {ErrorsAre.Count} Errors!");

                    for (int i = 0; i < ErrorsAre.Count - 1; i++)
                    {
                         erroroutputs.err(ErrorsAre[i], "err"); // Regular errors
                    }

                    // Use errlast for the last error
                    errlast(ErrorsAre[^1]);
                }
                else
                {
                     erroroutputs.errinfo($"Found {ErrorsAre.Count} Errors: Listing 20 out of {ErrorsAre.Count} Errors!");

                    for (int i = 0; i < 19; i++) // List first 19 as regular errors
                    {
                         erroroutputs.err(ErrorsAre[i], "err");
                    }

                    // Use errlast for the 20th error
                    errlast(ErrorsAre[19]);
                }
            }
            else
            {
                // warningoutputs.warninfo("0 Errors Found!");
            }
        }

        // List all errors and use errlast for the last error printed
        public static void ListThemAll()
        {
            if (ErrorsAre.Count == 1)
            {
                 erroroutputs.errinfo($"Found an Error!");

                for (int i = 0; i < ErrorsAre.Count - 1; i++)
                {
                     erroroutputs.err(ErrorsAre[i], "err"); // Regular errors
                }

                // Use errlast for the last error
                errlast(ErrorsAre[^1]);
            }
            else if (ErrorsAre.Count > 0)
            {
                 erroroutputs.errinfo($"Found {ErrorsAre.Count} Errors: Listing {ErrorsAre.Count} out of {ErrorsAre.Count} Errors!");

                for (int i = 0; i < ErrorsAre.Count - 1; i++) // Print all but the last error normally
                {
                     erroroutputs.err(ErrorsAre[i], "err");
                }

                // Use errlast for the last error
                errlast(ErrorsAre[^1]);
            }
            else
            {
                // warningoutputs.warninfo("0 Errors Found!");
            }
        }

        // Print the last error using errlast
        public static void errlast(string lastError)
        {
             erroroutputs.errlast(lastError, "err");
        }
    }
}
