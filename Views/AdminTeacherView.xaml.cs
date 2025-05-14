using IntellectFlow.ViewModels;
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
using System.Windows.Shapes;
using IntellectFlow.Helpers; // Подключение RelayCommand
using System.Windows.Controls;


namespace IntellectFlow.Views
{
    /// <summary>
    /// Логика взаимодействия для AdminTeacherView.xaml
    /// </summary>
    public partial class AdminTeacherView : UserControl
    {
        public AdminTeacherView()
        {
            InitializeComponent();
            DataContext = new AdminTeacherViewModel();
        }
    }
}
