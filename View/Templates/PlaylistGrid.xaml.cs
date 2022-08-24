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
    // PROPERTIES THAT CAN BE SET BY XAML
    // => See: https://stackoverflow.com/questions/25895011/how-to-add-custom-properties-to-wpf-user-control
    // ==============
    
    public bool AllowDraggingRows
    {
        get => (bool) GetValue(AllowDraggingRowsProperty);
        set => SetValue(AllowDraggingRowsProperty, value);
    }
    
    public static readonly DependencyProperty AllowDraggingRowsProperty = DependencyProperty.Register(
        "AllowDraggingRows", typeof(bool), typeof(PlaylistGrid), new PropertyMetadata(false)
    );

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
        // Start dragging
        if (this.AllowDraggingRows)
        {
            this._moveRowSourceIndex = this.FindRowIndex(e);
        }
    }
    
    private void PlaylistGrid_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        // Move the source row to the target index
        if (this.AllowDraggingRows)
        {
            this.MoveRowToTargetIndex(e);
        }
    }

    private void PlaylistGrid_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        // Reset move variable to stop moving the row
        if (this.AllowDraggingRows)
        {
            this._moveRowSourceIndex = -1;
        }
    }

    private void MoveRowToTargetIndex(MouseEventArgs e)
    {
        int moveRowTargetIndex = this.FindRowIndex(e);
        if (this._moveRowSourceIndex != -1 && moveRowTargetIndex != -1 && moveRowTargetIndex != this._moveRowSourceIndex)
        {
            //Console.WriteLine($"Move {this._moveRowSourceIndex} to {moveRowTargetIndex}");
            
            var dataCollection = this.GridElem.ItemsSource;
            if (dataCollection != null)
            {
                // Call the 'Move' method of the ObservableCollection via Reflection to avoid using the ViewModel
                // type.When using an ObservableCollection, this type would be necessary and you cannot cast to
                // ObservableCollection<object>.
                // See: https://stackoverflow.com/questions/37999337/how-to-cast-and-manipulate-observablecollection-of-unknown-type
                var method = dataCollection.GetType().GetMethod("Move", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                method?.Invoke(dataCollection, new object[] {this._moveRowSourceIndex, moveRowTargetIndex});

                // Set the new index as source index to make it possible to drag the row farther.
                // If not updating the index, the row would flicker between the original source index and the current
                // target index.
                this._moveRowSourceIndex = moveRowTargetIndex;
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
    private int FindRowIndex(MouseEventArgs e )
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