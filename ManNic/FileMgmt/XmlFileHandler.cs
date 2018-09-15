using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace HQ4P.Tools.ManNic.FileMgmt
{
    internal class XmlFileHandler
    {
        #region propertys
        public string Filepath { get; private set; }
        public string Filename { get; private set; }
        public Uri FileUri => new Uri(GenerateFullPath());
        public string Info { get; private set; }
        public bool FileExists => CheckFileExists();

        #endregion

        #region ctor
        public XmlFileHandler(string filepath, string filename)
        {
            SetFilePath(filepath);
            Filename = filename;
            Info = @"Hello World";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// setz filepath if Path is accesable
        /// </summary>
        /// <param name="path">new Filepath</param>
        /// <returns>true if found filepath</returns>
        public bool SetFilePath(string path)
        {
            Filepath = path.TrimEnd(new char['\\']) + "\\";

            if (CheckFilePath())
            {
                Info = @"Filepath set";
                return true;
            }

            Info = @"Filepath not found";
            return false;
        }

        public void SetFilename(string name) { Filename = name; }

        private bool CheckFilePath() { return Directory.Exists(Filepath); }

        public bool GenerateIfNotExists(XmlEntryStandard rootentry)
        {
            if (FileExists) return true;
            if (!CheckFilePath())
            {
                Info = @"Filepath not found";
                return false;
            }

            try
            {
                new XDocument(
                    new XElement(rootentry.EntryPrefix
                        , new XAttribute(rootentry.TypePrefix, rootentry.TypeName)
                        , new XAttribute(rootentry.NamePrefix, rootentry.Name)
                        //todo: Delete
                        , new XElement("node", new XAttribute("Type", "folder"), new XAttribute("Name", "Valued Customer"))
                    )
                ).Save(GenerateFullPath());

                return true;

            }
            catch (Exception e)
            {
                Info = e.Message;
                return false;
            }
        }


        #endregion

        private string GenerateFullPath()
        {
            var checkedFilePath = Filepath.TrimEnd(new char['\\'])+ "\\";
            var checkedFileName = Filename.EndsWith(".xml") ? Filename : Filename + ".xml";
            return checkedFilePath + checkedFileName;
        }

        private bool CheckFileExists()
        {
            return File.Exists(GenerateFullPath());
        }
    }
}
