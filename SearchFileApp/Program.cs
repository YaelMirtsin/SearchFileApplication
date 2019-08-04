using BL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchFileApp
{
    class Program
    {
        /**
 1. MainMenu - The function prints menu.
 2. GetUserRequest - The function gets a user request and performs search according this request.
 3. FileNameRequested - The function returns file name from the user.
 4. PathRequested - The function returns path from the user if the user chose option 2. If the user chose option 1 the path is C:\\ by default.
 5. SearchAccessibleFiles - The function receives 2 parameters: filename and directory - path (where to search this file) and 
    returns list of paths if there 1 or more and 0 if the file name not found;
 6. InsertInToDB - The function inserts the result to the database (SearchFileDB) with a table name searchFileLog and columns:
    ID, seachPattern, seachPatternPath, searchDate, resultPatternPath.
    * */
        static void Main(string[] args)
        {
            ClassBL bl = new ClassBL();
            MainMenu();

            void MainMenu()
            {
                Console.WriteLine("___________________The Program Search file name ______________________________\n");
                Console.WriteLine("1. Enter file name to search.");
                Console.WriteLine("2. Enter file name to search. + parent directory to search in.");
                Console.WriteLine("3. Exit.");

                Boolean isExit = GetUserRequest(Console.ReadLine());
                if (isExit)
                {
                    Environment.Exit(0);
                }
            }


            Boolean GetUserRequest(string selectedNum)
            {
                DateTime searchDateTime = DateTime.Now;
                string fileName = "";
                string pathName = "C:\\";
                IEnumerable<string> listFilePath;

                switch (selectedNum)
                {
                    case "1":
                        fileName = FileNameRequested();
                        if (ErrorInputPatternName(fileName)) break;

                        Console.WriteLine("Start searching ... {0}", searchDateTime);
                        listFilePath = bl.SearchAccessibleFiles(pathName, fileName);
                        if (listFilePath.Count() == 0)
                        {
                            bl.InsertInToDB(fileName, pathName, searchDateTime, "File Not Found");
                            Console.WriteLine("\nFile name " + fileName + " Not Found. Please try again.");
                        }
                        else
                        {
                            foreach (string path in listFilePath)
                            {
                                bl.InsertInToDB(fileName, pathName, searchDateTime, path);
                                Console.WriteLine("{0}", path);
                            }
                        }
                        break;

                    case "2":
                        fileName = FileNameRequested();
                        if (ErrorInputPatternName(fileName)) break;

                        pathName = PathRequested();
                        if (ErrorInputPatternName(pathName)) break;

                        Console.WriteLine("Start searching ... {0}", searchDateTime);
                        listFilePath = bl.SearchAccessibleFiles(pathName, fileName);
                        if (listFilePath.Count() == 0)
                        {
                            bl.InsertInToDB(fileName, pathName, searchDateTime, "File Not Found");
                            Console.WriteLine("\nFile name " + fileName + " Not Found. Please try again.");
                        }
                        else
                        {
                            foreach (string path in listFilePath)
                            {
                                bl.InsertInToDB(fileName, pathName, searchDateTime, path);
                                Console.WriteLine("{0}", path);
                            }
                        }
                        break;
                    case "3": return true;
                    default:
                        Console.WriteLine("You have to choose 1,2 or 3.");
                        break;
                }
                MainMenu();
                return false;
            }

            string FileNameRequested()
            {
                Console.Write("Enter file name to search: ");
                return Console.ReadLine();
            }

            string PathRequested()
            {
                Console.Write("Enter root directory to search in: ");
                string userPath = Console.ReadLine();
                bool isDirectoryExist = Directory.Exists(userPath);
                return (isDirectoryExist) ? userPath : null;

            }

            bool ErrorInputPatternName(string pattern)
            {
                if (string.IsNullOrEmpty(pattern))
                {
                    Console.WriteLine("\n  Error in file name or path. Try again.");
                }
                return string.IsNullOrEmpty(pattern);
            }
        }
    }
}
