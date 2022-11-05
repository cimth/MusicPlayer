using System;
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

    // Initialize with -1 and false to avoid moving a row when showing the Grid first time
    private int _moveRowSourceIndex = -1;
    private bool _isMovingRow = false;
    
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
    // CUSTOM EVENTS
    // ==============

    public event EventHandler? OnRowMoved;

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
            // Fire moving event
            if (this._isMovingRow)
            {
                if (OnRowMoved != null)
                {
                    OnRowMoved(this, EventArgs.Empty);
                }
                this._isMovingRow = false;
            }
            
            // Reset source index
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
                
                // Set the target flag for moving
                this._isMovingRow = true;
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
    
    // ==============
    // FOCUS GRID WHEN SELECTION IS CHANGED
    // ==============

    private void PlaylistGrid_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // Set the focus to the Grid so that you can directly delete the next entry or else
        int selectedRowIndex = GridElem.SelectedIndex;
        if (selectedRowIndex != -1)
        {
            GridElem.Focus();
        }
    }
    
    // ==============
    // NAVIGATE WITH KEYBOARD
    // ==============

    private void PlaylistGrid_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Down:
                this.NavigateToNextItem();
                e.Handled = true;
                break;
            case Key.Up:
                this.NavigateToPreviousItem();
                e.Handled = true;
                break;
        }
    }

    private void NavigateToNextItem()
    {
        if (this.SelectedIndex < this.GridElem.Items.Count - 1)
        {
            // Select next
            this.SelectedIndex++;
        } else if (this.SelectedIndex == this.GridElem.Items.Count - 1)
        {
            // Current is the last item, thus select the first item
            this.SelectedIndex = 0;
        }
    }

    private void NavigateToPreviousItem()
    {
        if (this.SelectedIndex > 0)
        {
            // Select previous
            this.SelectedIndex--;
        } else if (this.SelectedIndex == 0)
        {
            // Current is the first item, thus select the last item
            this.SelectedIndex = this.GridElem.Items.Count - 1;
        }
    }
}