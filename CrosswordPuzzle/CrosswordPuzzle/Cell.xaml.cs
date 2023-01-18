using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrosswordPuzzle
{

    public partial class Cell : UserControl
    {
        public Cell()
        {
            InitializeComponent();
        }
        public void CellInputChange()
        {
            if (InputCell.Visibility == Visibility.Collapsed)
            {
                InputCell.Visibility = Visibility.Visible;
                BasicCell.Visibility = Visibility.Collapsed;
            }
        }
    }
}
