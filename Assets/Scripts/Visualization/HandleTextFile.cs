using System.IO;
using UnityEngine;

namespace Visualization
{
    public static class HandleTextFile
    {

        static string saveDirectory = "Assets/Resources/Animations/";
        public static void WriteString(string path, string text)
        {
            //Write some text to the test.txt file
            FileStream fcreate = File.Open(path, FileMode.Create);
            StreamWriter writer= new StreamWriter(fcreate); 
            writer.WriteLine(text);
            writer.Close();
        }

        //[MenuItem("Tools/Read file")]
        public static void ReadString(string path)
        {
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            Debug.Log(reader.ReadToEnd());
            reader.Close();
        }
        public static bool FileExists(string fileName)
        {
            string path = saveDirectory + fileName + ".txt";
            return File.Exists(path);
        }
        public static string getSaveDirectory(){ return saveDirectory; }

    }
}