using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using HQ4P.Tools.ManNic.FileManagement;


namespace HQ4P.Tools.ManNic.View.Models
{
    internal class VmTreeViewData
    {

        private readonly XmlFileHandler _fileHandler;
        private string _message;

        #region properties

        internal XmlDataProvider XmlData => _fileHandler.XmlData;
        internal string Message => _message;



        #endregion

    

        #region ctor

        public VmTreeViewData(string directory)
        {
            _fileHandler = FileHandlerConstruction(directory);

        }

        private XmlFileHandler FileHandlerConstruction(string directory)
        {
            var rootNode = new XmlEntryStandard()
            {
                EntryPrefix = Properties.Settings.Default.SettingsFileKeywordPath,
                TypePrefix = Properties.Settings.Default.SettingsFileKeywordType, TypeName = Properties.Settings.Default.SettingsFileTypeNameFolder,
                NamePrefix = Properties.Settings.Default.SettingsFileKeywordName, Name = @"Locations"
            };
            var handler= new XmlFileHandler(directory
                                           , Properties.Settings.Default.SettingsFileName
                                           , Properties.Settings.Default.SettingsFileKeywordPath);
            handler.InitXmlFile(rootNode);

            _message = handler.FileExists ? @"Locations File found" : handler.Info;
            return handler;

        }
        #endregion

    }
}