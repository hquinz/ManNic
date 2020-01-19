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
using HQ4P.Tools.ManNic.View.Tools;


namespace HQ4P.Tools.ManNic.View.Models
{
    public class VmTreeView : INotifyPropertyChanged
    {

        private readonly string _myRoot = AppDomain.CurrentDomain.BaseDirectory;
        private VmTreeViewData _data;
        private VmTreeViewController _controller;

        private readonly Action<string> _setState;

        #region properties

        public XmlDataProvider TreeData => _data.XmlData;

        private bool _allowDelete = false;

        public bool AllowDelete
        {
            get => _allowDelete;
            set
            {
                _allowDelete = value; 
                OnPropertyChanged(nameof(AllowDelete));
            }
        }

        #endregion


        #region ctor

        public VmTreeView(Action<string> setState)
        {
            _setState = setState;
            _controller = new VmTreeViewController();
            _data = new VmTreeViewData(_myRoot);
            _setState(_data.Message);
        }

        #endregion

        #region Commands

        public ICommand SelectedItemChangedCommand => new RelayCommand<object>(_controller.NewItemSelected);
        public ICommand KeyDownCommand => new RelayCommand<ControlKeyEventArgs>(_controller.KeyDown);


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