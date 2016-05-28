/*
 * Taken from http://stackoverflow.com/questions/13954630/how-to-handle-unauthorizedaccessexception-when-attempting-to-add-files-from-loca
 * the answer http://stackoverflow.com/a/13954763
 * Reason to use this class is the Directory.EnumerateFiles and Directory.Getfiles does not handle exceptions when an unaccessable file is reached i.e. system critical folders and files 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UsbTergum
{
    class FileEnumerator
    {
        

        /// <summary>
        /// enumerates through all subdirectories of a given directory 
        /// </summary>
        /// <param name="parentDirectory"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOpt"></param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateDirectories(string parentDirectory, string searchPattern, SearchOption searchOpt)
        {
            try
            {
                var directories = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories)
                {
                    directories = Directory.EnumerateDirectories(parentDirectory)
                        .SelectMany(x => EnumerateDirectories(x, searchPattern, searchOpt));
                }
                return directories.Concat(Directory.EnumerateDirectories(parentDirectory, searchPattern));
            }
            //handle UnauthorizedAccessException
            catch (UnauthorizedAccessException ex)
            {
                //return an empty string
                return Enumerable.Empty<string>();
            }
        }

        /// <summary>
        /// enumerates through all files of given directory and its sub directories
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <param name="searchOpt"></param>
        /// <returns></returns>
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOpt)
        {
            try
            {
                var dirFiles = Enumerable.Empty<string>();
                if (searchOpt == SearchOption.AllDirectories)
                {
                    dirFiles = Directory.EnumerateDirectories(path)
                                        .SelectMany(x => EnumerateFiles(x, searchPattern, searchOpt));
                }
                return dirFiles.Concat(Directory.EnumerateFiles(path, searchPattern));
            }
            //handle UnauthorizedAccessException
            catch (UnauthorizedAccessException ex)
            {
                //return an empty string
                return Enumerable.Empty<string>();
            }
        }
    }
}
