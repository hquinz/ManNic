using System.Windows.Controls;

namespace HQ4P.Tools.ManNic.View.Tools
{
    public class TreeViewCondition
    {
        private readonly TreeView _treeView;

        public TreeViewCondition(TreeView treeView)
        {
            _treeView = treeView;
        }




        private bool IsRootNode()
        {
            //todo Check if top level node
            //_treeView.SelectedItem
            return false;
        }

        public void SetTexBoxReadOnly(TextBox item, bool state)
        {
            item.IsReadOnly = state;
        }
    }

}