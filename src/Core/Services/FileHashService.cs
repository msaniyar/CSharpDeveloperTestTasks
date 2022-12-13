using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Core.Services
{
    public class FileHashService : IFileHashService
    {


        /// <inheritdoc />
        public string CalculateFileHash(string filePath)
        {
            if(!File.Exists(filePath))
            {
                return string.Empty;
            }

            using var localSha256 = SHA256.Create();
            using FileStream fileStream = File.OpenRead(filePath);
            var fileHash = localSha256.ComputeHash(fileStream);
            return GetHex(fileHash);

        }

        private static string GetHex(byte[] byteHash)
        {
            var stringBuilder = new StringBuilder();

            for (int index = 0; index < byteHash.Length; index++)
            {
                stringBuilder.Append(byteHash[index].ToString("x2"));
            }

            return stringBuilder.ToString();
        }


    }
}
