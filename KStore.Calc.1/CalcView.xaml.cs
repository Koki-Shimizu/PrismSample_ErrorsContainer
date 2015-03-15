using Microsoft.Practices.Prism.Mvvm;
using System.Windows;

namespace KStore.Calc._2
{    
    public partial class CalcView : Window
    {
        public CalcView()
        {
            InitializeComponent();
            this.DataContext = new CalcViewModel();
        }
    }
}
