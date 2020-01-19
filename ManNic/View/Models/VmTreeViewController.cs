using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class VmTreeViewController 
    {

 
        #region properties

        #endregion


        #region Command execution

        internal void NewItemSelected(object sender)
        {
            var myTreeview = (TreeView) sender;

        }

        internal void KeyDown(ControlKeyEventArgs parameterObjects)
        {
            var myReview = (TreeView)parameterObjects.ControlObject;
            switch (parameterObjects.EventArgument.Key)
            {
                case Key.F2:
                    var test = myReview.SelectedItem;
                    break;
                case Key.Enter:
                    ;
                    break;
                     
            }

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