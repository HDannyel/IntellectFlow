using IntellectFlow.DataModel;
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

namespace IntellectFlow.Views
{
    /// <summary>
    /// Логика взаимодействия для StudentView.xaml
    /// </summary>
    public partial class StudentView : UserControl
    {
        public StudentView()
        {
            InitializeComponent();
        }

        private void DisciplinesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox?.SelectedItem is Discipline selectedDiscipline)
            {
                // Например: показать информацию о дисциплине
                MessageBox.Show($"Вы выбрали дисциплину: {selectedDiscipline.Name}");
            }
        }
    }
}
