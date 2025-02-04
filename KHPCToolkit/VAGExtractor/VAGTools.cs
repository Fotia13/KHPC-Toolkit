﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAGExtractor
{
    public static class VAGTools
    {
        public static List<string> ExtractVAGFiles(string inputFile, string outputFolder, int keepName = 0, bool removeSuffix = true)
        {
            var vagFiles = new List<string>();

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            var inputData = File.ReadAllBytes(inputFile);
            var inputStream = File.OpenRead(inputFile);
            var foundOffsets = SearchPatterns(inputData, "VAGp");

            for (int i = 0; i < foundOffsets.Count; i++)
            {
                int offset = foundOffsets[i];
                inputStream.Seek(offset, SeekOrigin.Begin);

                var vag = new VAG(inputStream);

                keepName = keepName > 0 && !string.IsNullOrEmpty(vag.Name) ? keepName : 0;
                var newName = i.ToString();

                if (keepName == 1)
                    newName = vag.Name;
                else if (keepName == 2)
                    newName += $"-{vag.Name}";

                vagFiles.Add(vag.Export(outputFolder, newName, removeSuffix));
            }

            inputStream.Close();

            return vagFiles;
        }

        private static List<int> SearchPatterns(byte[] data, string stringPattern)
        {
            var foundOffsets = new List<int>();

            var pattern = Encoding.ASCII.GetBytes(stringPattern);
            int patternLength = pattern.Length;
            int totalLength = data.Length;
            byte firstMatchByte = pattern[0];
            for (int i = 0; i < totalLength; i++)
            {
                if (firstMatchByte == data[i] && totalLength - i >= patternLength)
                {
                    byte[] match = new byte[patternLength];
                    Array.Copy(data, i, match, 0, patternLength);
                    if (match.SequenceEqual<byte>(pattern))
                    {
                        foundOffsets.Add(i);
                    }
                }
            }

            return foundOffsets;
        }
    }
}
