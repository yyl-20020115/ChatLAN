using System;
using System.IO;
using Microsoft.Win32;

namespace ChatLAN.Objects;

[Serializable]
public class File
{
    public byte[] Data;
    public string Name;

    public void SaveFile()
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog()
        {
            FileName = Name
        };
        if (saveFileDialog.ShowDialog().HasValue)
            using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate))
                fileStream.Write(Data, 0, Data.Length);
    }
}