using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Net.Mime;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HQ4P.Tools.ManNic.FileManagement;


namespace HQ4P.Tools.ManNic.ViewModels
{
    public class VmTreeView : INotifyPropertyChanged
    {

        private readonly string _myRoot = AppDomain.CurrentDomain.BaseDirectory;
        private readonly XmlFileHandler _fileHandler;

        private readonly Action<string> _setState;

        #region properties

        public XmlDataProvider TreeData => _fileHandler.XmlData;

        private bool _allowDelete = false;
        private object _selctedValue;

        public bool AllowDelete
        {
            get => _allowDelete;
            set
            {
                _allowDelete = value; 
                OnPropertyChanged(nameof(AllowDelete));
            }
        }

        public object SelectedValue
        {
            get => _selctedValue;
            set
            {
                _selctedValue = value;
                NewItemSellectedNW(value);
                OnPropertyChanged(nameof(SelectedValue));
            }
        }

        #endregion

        #region Commands

        public ICommand SelectedItemChangedCmd => new RelayCommand<object>(NewItemSellectedNW);


        #endregion


        #region ctor

        public VmTreeView(Action<string> setState)
        {
            _setState = setState;
            _fileHandler = FileHandlerConstruction();

        }

        private XmlFileHandler FileHandlerConstruction()
        {
            var rootNode = new XmlEntryStandard()
            {
                EntryPrefix = Properties.Settings.Default.SettingsFileKeywordPath,
                TypePrefix = Properties.Settings.Default.SettingsFileKeywordType, TypeName = Properties.Settings.Default.SettingsFileTypeNameFolder,
                NamePrefix = Properties.Settings.Default.SettingsFileKeywordName, Name = @"Locations"
            };
            var handler= new XmlFileHandler(_myRoot
                                           , Properties.Settings.Default.SettingsFileName
                                           , Properties.Settings.Default.SettingsFileKeywordPath);
            handler.InitXmlFile(rootNode);

            _setState(handler.FileExists ? @"Locations File found" : handler.Info);
            return handler;

        }
        #endregion

        #region Command execution

        public void NewItemSellected(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var selectedValue = e.NewValue;
        }

        private void NewItemSellectedNW(object sender)
        {
            var myTreeview = (TreeView) sender;

        }

        #endregion


        #region Eventhandling

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        #endregion

    }
}