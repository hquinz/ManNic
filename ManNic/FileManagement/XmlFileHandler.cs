using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Xml.Linq;

namespace HQ4P.Tools.ManNic.FileManagement
{
    internal class XmlFileHandler: FileHandler
    {
        private readonly XmlDataProvider _xmlData;
        private readonly XmlFunctions _xmlFunctions;

        #region propertys

        public XmlDataProvider XmlData => _xmlData;

        #endregion

        #region ctor
        public XmlFileHandler(string filepath, string filename, string xmlPathKey) : base(filepath, filename)
        {
            _xmlFunctions = new XmlFunctions(FullPath);
            _xmlData = _xmlFunctions.GenerateDataProvider(FileUri, xmlPathKey);
        }

        #endregion

        #region public methods

        public void InitXmlFile(XmlEntryStandard rootEntry)
        {
            GenerateIfNotExists(rootEntry);
            InitialLoad();
        }


        #endregion

        private void GenerateIfNotExists(XmlEntryStandard rootEntry)
        {
            if (FileExists) return;
            if (!PathExists)
            {
                Info = @"Filepath not found";
                return;
            }

            _xmlFunctions.GenerateFile(rootEntry);
            if (!_xmlFunctions.Success) Info = _xmlFunctions.Message;
        }

        private void InitialLoad()
        {
            if (FileExists) _xmlData.InitialLoad();
        }


    }
}
