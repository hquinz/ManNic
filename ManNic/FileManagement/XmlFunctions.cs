using System;
using System.Windows.Data;
using System.Xml.Linq;


namespace HQ4P.Tools.ManNic.FileManagement
{
    class XmlFunctions
    {
        private string _path;

        #region Propertys

        public bool Success { get; private set; } = false;
        public string Message { get; private set; }

        #endregion


        public XmlFunctions(string filePath)
        {
            _path = filePath;
        }

        #region public methods

        public void SetFilePath(string filePath)
        {
            _path = filePath;
        }

        public XmlDataProvider GenerateDataProvider(Uri File, string xmlPathKey)
        {
            return new XmlDataProvider
            {
                Source = File,
                XPath = xmlPathKey
            };
        }

        public void GenerateFile(XmlEntryStandard rootEntry)
        {
            try
            {
                new XDocument(
                    new XElement(rootEntry.EntryPrefix
                        , new XAttribute(rootEntry.TypePrefix, rootEntry.TypeName)
                        , new XAttribute(rootEntry.NamePrefix, rootEntry.Name)
                    )
                ).Save(_path);
            }
            catch (Exception e)
            {
                SetSuccess(false, e.Message);
                return;
            }

            SetSuccess(true, "");

        }


        #endregion

        #region private methods

        private void SetSuccess(bool gotSuccess, string faultMessage)
        {
            Success = gotSuccess;
            Message = (gotSuccess ? "done": faultMessage);
        }

        #endregion
    }
}
