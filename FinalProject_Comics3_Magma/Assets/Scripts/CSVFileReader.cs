using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class CSVFileReader
{
    public static string[][] FileMatrix { get; private set; }

    private static string[] _fileLines;
    public static void Parse(string[] lines, char separator, string valueToIgnore, bool ignoreFirstLine = false)
    {
        _fileLines = lines;

        if (ignoreFirstLine) _fileLines = _fileLines.Skip(1).ToArray();
        _fileLines = _fileLines.ToList().Select(x => x.Replace("\"", "")).ToArray();
        _fileLines = _fileLines.ToList().Select(x => x.Replace("\\", "")).ToArray();
        RemoveValue(separator, valueToIgnore);
    }

    private static string[][] RemoveValue(char separator, string valueToIgnore)
    {
        FileMatrix = new string[_fileLines.Length][];
        for (int i = 0; i < _fileLines.Length; i++)
        {
            var line = _fileLines[i].Split(separator);
            var lowerLines = line.ToList().Select(x => x.ToLower()).ToList();
            if (lowerLines.Contains(valueToIgnore.ToLower()))
            {
                var lines = lowerLines.ToList();
                lines.Remove(valueToIgnore.ToLower());
                line = lines.ToArray();
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] != "")
                        line[j] = char.ToUpper(line[j][0]) + line[j].Substring(1);
                }
                FileMatrix[i] = line;
            }
            else
                FileMatrix[i] = line;
        }
        return FileMatrix;
    }

    public static List<List<string>> GetMatrixToList()
    {
        return FileMatrix.Select(x => x.ToList()).ToList();
    }
}
