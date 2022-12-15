using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.PlayFabHelper
{
    public class PlayFabFileInfo
    {
        public readonly string FileName;
        public readonly string DownloadUrl;
        public readonly string UploadUrl;
        public readonly int Size;
        public readonly DateTime LastModified;

        public PlayFabFileInfo(string fileName, string dwnUrl, string upldUrl, int s, DateTime lastMod)
        {
            FileName = fileName;
            DownloadUrl = dwnUrl;
            UploadUrl = upldUrl;
            Size = s;
            LastModified = lastMod;
        }

        public override string ToString()
        {
            return $"File Name: {FileName} \n DownloadUrl: {DownloadUrl} \n " +
                $"UploadUrl: {UploadUrl} \n Size: {Size} \n  Last Modified: {LastModified}";
        }
    }
}

