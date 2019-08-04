using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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
namespace BL
{
    public class ClassBL
    {
       public  DAL.SearchFileDBEntities entities = new DAL.SearchFileDBEntities();
        
        public IEnumerable<string> SearchAccessibleFiles(string root, string searchTerm)
        {
            var files = new List<string>();
            searchTerm = searchTerm.ToLower();
            root = root.ToLower();
            IEnumerable<string> listPathFile = Directory.EnumerateFiles(root).Select(e =>e.ToLower()).Where(m => m.Contains(searchTerm));
            foreach (string file in listPathFile)
            {
                int pos = file.LastIndexOf("\\");
                if (file.Substring(pos).Contains(searchTerm))
                {
                    files.Add(file);
                }
            }
            
            foreach (var subDir in Directory.EnumerateDirectories(root))
            {
                try
                {
                    files.AddRange(SearchAccessibleFiles(subDir, searchTerm));
                }
                catch (UnauthorizedAccessException ex)
                {
                    Debug.WriteLine("{0}", ex.Message);
                }
            }

            return files;
        }

        public void InsertInToDB(string fileName, string pathName, DateTime searchDateTime, string  searchPathResult)
        {
            try
            {
                entities.searchFileLogs.Add(new DAL.searchFileLog
                {
                    searchPattern = fileName,
                    searchPatternPath = pathName,
                    searchDate = searchDateTime,
                    resultPatternPath = searchPathResult
                });
                entities.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Add some logging with log4net here


                // Throw a new DbEntityValidationException with the improved exception message.
                throw new System.Data.Entity.Validation.DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

            }
        }

       

    }
}
