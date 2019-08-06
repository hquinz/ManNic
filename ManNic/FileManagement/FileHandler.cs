using System;
using System.IO;

namespace HQ4P.Tools.ManNic.FileManagement
{
    internal class FileHandler
    {
        #region propertys
        public string Filepath { get; private set; }
        public string Filename { get; private set; }
        public string FullPath => GenerateFullPath();
        public Uri FileUri => new Uri(GenerateFullPath());
        public string Info { get; protected set; }
        public bool PathExists => CheckFilePath();
        public bool FileExists => CheckFileExists();

        #endregion

        #region ctor
        public FileHandler(string filepath, string filename)
        {
            SetFilePath(filepath);
            Filename = filename;
            Info = @"instantiated";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// set filepath if Path is accesable
        /// </summary>
        /// <param name="path">new Filepath</param>
        /// <returns>true if found filepath</returns>
        public void SetFilePath(string path)
        {
            Filepath = path.TrimEnd(new char['\\']) + "\\";
            Info = CheckFilePath() ? @"Filepath set" : @"Filepath not found";
        }

        public void SetFilename(string name) { Filename = name; }


        #endregion

        private bool CheckFilePath() { return Directory.Exists(Filepath); }

        private bool CheckFileExists()
        {
            return File.Exists(GenerateFullPath());
        }

        private string GenerateFullPath()
        {
            var checkedFilePath = Filepath.TrimEnd(new char['\\']) + "\\";
            var checkedFileName = Filename.EndsWith(".xml") ? Filename : Filename + ".xml";
            return checkedFilePath + checkedFileName;
        }

    }
}
