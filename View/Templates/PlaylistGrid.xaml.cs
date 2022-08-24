using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace View.Templates;

public partial class PlaylistGrid : DataGrid
{
    // ==============
    // FIELDS
    // ==============

    private int _moveRowSourceIndex;
    
    // ==============
    // INITIALIZATION
    // ==============
    
    public PlaylistGrid()
    {
        InitializeComponent();
    }
    
    // ==============
    // MOVE ROWS VIA DRAG AND DROP
    // ==============

    private void PlaylistGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        this._moveRowSourceIndex = this.FindRowIndex(e);
    }

    private void PlaylistGrid_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (this.AllowDrop)
        {
            this.MoveRowViaDragAndDrop(e);
        }
    }

    private void MoveRowViaDragAndDrop(MouseButtonEventArgs e)
    {
        int moveRowTargetIndex = this.FindRowIndex(e);
        if (this._moveRowSourceIndex != -1 && moveRowTargetIndex != -1 && moveRowTargetIndex != this._moveRowSourceIndex)
        {
            //Console.WriteLine($"Move {this._moveRowSourceIndex} to {moveRowTargetIndex}");
            
            IEnumerable? dataCollection = this.GridElem.ItemsSource;
            if (dataCollection != null)
            {
                var col = new ObservableCollection<object>(dataCollection.Cast<object>());
                col.Move(this._moveRowSourceIndex, moveRowTargetIndex);
                this.GridElem.ItemsSource = col;
            }
        }
    }

    /// <summary>
    /// Returns the row index of the given mouse button event
    /// <para/>
    /// For more details, see: https://blog.scottlogic.com/2008/12/02/wpf-datagrid-detecting-clicked-cell-and-row.html
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private int FindRowIndex(MouseButtonEventArgs e)
    {
        // Return value, is -1 if no index found
        int index = -1;
        
        // Find the parent of type DataGridRow
        DependencyObject? dep = e.OriginalSource as DependencyObject;
        while (dep != null && !(dep is DataGridRow))
        {
            dep = VisualTreeHelper.GetParent(dep);
        }

        // Get the index of the DataGridRow
        if (dep != null)
        {
            DataGridRow row = (dep as DataGridRow)!;
            DataGrid? dataGrid = ItemsControlFromItemContainer(row) as DataGrid;
            index = dataGrid?.ItemContainerGenerator.IndexFromContainer(row) ?? -1;
        }

        return index;
    }
}