using System.Collections.Generic;
using System.Windows;

namespace TetrisWPF
{
    public partial class LeaderboardWindow : Window
    {
        public LeaderboardWindow(List<int> highScores)
        {
            InitializeComponent();
            LoadHighScores(highScores);
        }

        private void LoadHighScores(List<int> highScores)
        {
            HighScoresList.Items.Clear();
            if (highScores.Count == 0)
            {
                HighScoresList.Items.Add("Nincs elérhető pontszám!");
            }
            else
            {
                for (int i = 0; i < highScores.Count; i++)
                {
                    HighScoresList.Items.Add($"{i + 1}. {highScores[i]:N0}");
                }
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}