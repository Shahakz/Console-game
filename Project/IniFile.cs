using System;
using System.IO;
using System.Collections;

namespace Engine3D
{
  // INI file reader
  public class CIniReader
  {
    private ArrayList IniLines = new ArrayList();

    private int CurrSectionIndex;

    // Constructor
    public CIniReader(string FileName)
    {
      CurrSectionIndex = -1;

      FileInfo IniFile = new FileInfo(FileName);
      StreamReader Stream = IniFile.OpenText();

      string Line;

      // Read all lines
      while(true)
      {
        Line = Stream.ReadLine();

        if (Line == null)
          break;

        // Ignore blank lines and comments
        if ((Line != "") && !Line.StartsWith(";"))
          IniLines.Add(Line);
      }
    }

    public bool FindSection(string SectionName)
    {
      CurrSectionIndex = IniLines.IndexOf("[" + SectionName + "]");
      return (CurrSectionIndex >= 0);
    }

    public int GetIntValue(string Key, int DefaultValue)
    {
      string CurrKey,CurrValue;

      while (GetNextEntry(out CurrKey, out CurrValue))
        if (CurrKey == Key)
        {
          return Convert.ToInt32(CurrValue);
        }

      return DefaultValue;
    }

    public string GetStringValue(string Key, string DefaultValue)
    {
      string CurrKey, CurrValue;

      while (GetNextEntry(out CurrKey, out CurrValue))
        if (CurrKey == Key)
        {
          return CurrValue;
        }

      return DefaultValue;
    }

    public bool GetNextEntry(out string Key, out string Value)
    {
      CurrSectionIndex++;

      Key = "";
      Value = "";

      if (CurrSectionIndex >= IniLines.Count)
        return false;

      string NextLine = (string)IniLines[CurrSectionIndex];

      if (NextLine.StartsWith("["))
        return false;

      string[] SplittedStr = NextLine.Split('=');

      if (SplittedStr.Length != 2)
        return false;

      Key = SplittedStr[0];
      Value = SplittedStr[1];

      return true;
    }
	}
}
