using System.Windows;
using System.IO;

namespace TetrisWPF
{
    public partial class OptionsWindow : Window
    {
        public OptionsWindow()
        {
            InitializeComponent();
        }

        public void ClearHighScores()
        {
            string filePath = "toplista.txt";
            if (File.Exists(filePath))
            {
                File.WriteAllText(filePath, string.Empty);
            }
        }

        private void OptionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Biztosan törlöd a toplistát?", "Törlés", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClearHighScores();
                MessageBox.Show("Toplista törölve!", "Törlés", MessageBoxButton.OK);
            }
        }
    }
}