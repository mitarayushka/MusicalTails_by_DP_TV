using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Model.Core;
using Model.Data;

namespace MusicalTails
{
    public partial class MainWindow : Window
    {
        protected HighScore[] _highScores = Array.Empty<HighScore>();
        protected string _currentFormat = "json";
        protected readonly JsonGameSerializer _jsonSerializer;
        protected readonly XmlGameSerializer _xmlSerializer;

        public MainWindow()
        {
            InitializeComponent();
            _jsonSerializer = new JsonGameSerializer("highscores.json");
            _xmlSerializer = new XmlGameSerializer();
            InitializeHighScoresFile();
            LoadHighScores(); // загрузка рекордов
        }

        private void InitializeHighScoresFile()
        {
                // Если формат JSON и файла не существует → создаём пустой JSON - массив
            if (_currentFormat == "json" && !File.Exists(_jsonSerializer.FilePath))
            {
                File.WriteAllText(_jsonSerializer.FilePath, "[]");
            }
            // Если формат XML и файла не существует → создаём пустой XML-документ
            else if (_currentFormat == "xml" && !File.Exists(_xmlSerializer.FilePath))
            {
                var emptyData = new GameData
                {
                    HighScores = Array.Empty<HighScore>(),
                    MaxScore = 0
                };
                _xmlSerializer.Serialize(emptyData, _xmlSerializer.FilePath);
            }
        }

        private void LoadHighScores()
        {
                if (_currentFormat == "json")
                {
                    _highScores = _jsonSerializer.Deserialize(_jsonSerializer.FilePath);
                }
                else
                {
                    var gameData = _xmlSerializer.Deserialize(_xmlSerializer.FilePath);
                    _highScores = gameData.HighScores;
                }
                UpdateHighScoresList();
        }


        private void UpdateHighScoresList()
        {
            HighScoresListView.ItemsSource = _highScores
                .OrderByDescending(h => h.Score)
                .Take(10)
                .ToArray();
        }

        private void SaveHighScores(HighScore newScore)
        {
                // Создаем новый массив с добавленным рекордом
                var newHighScores = new HighScore[_highScores.Length + 1];
                Array.Copy(_highScores, newHighScores, _highScores.Length);
                newHighScores[^1] = newScore;

                // Сортируем и берем топ-10
                _highScores = newHighScores
                    .OrderByDescending(h => h.Score)
                    .Take(10)
                    .ToArray();

                if (_currentFormat == "json")
                {
                    _jsonSerializer.Serialize(_highScores, _jsonSerializer.FilePath);
                }
                else
                {
                    _xmlSerializer.Serialize(new GameData
                    {
                        HighScores = _highScores,
                        MaxScore = _highScores.Length > 0 ? _highScores[0].Score : 0
                    }, 
                    _xmlSerializer.FilePath);
                }
                UpdateHighScoresList();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (DifficultyComboBox.SelectedItem is ComboBoxItem item &&
                int.TryParse(item.Tag.ToString(), out int difficulty))
            {
                var inputDialog = new InputDialog("Введите ваше имя:");
                if (inputDialog.ShowDialog() == true)
                {
                    StartGame(inputDialog.Answer, difficulty);
                }
            }
        }

        private void StartGame(string playerName, int difficulty)
        {
            var gameLogic = new GameLogic(difficulty, difficulty);
            var gameWindow = new GameWindow(gameLogic);

            gameWindow.Closed += (s, args) =>
            {
                if (gameLogic.Score > 0 && (_highScores.Length < 10 || gameLogic.Score > _highScores.Min(h => h.Score)))
                {
                    SaveHighScores(new HighScore(
                        string.IsNullOrWhiteSpace(playerName) ? "Игрок" : playerName,
                        gameLogic.Score,
                        difficulty));
                }
                this.Show();
            };

            gameWindow.Show();
            this.Hide();
        }

        private void FormatComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FormatComboBox.SelectedItem is ComboBoxItem selectedItem &&
                selectedItem.Tag is string newFormat)
            {
                ConvertFormats(newFormat);
            }
        }

        private void ConvertFormats(string newFormat)
        {
                if (_currentFormat == newFormat) return;

                if (newFormat == "json")
                {
                    _jsonSerializer.Serialize(_highScores, _jsonSerializer.FilePath);
                }
                else
                {
                    var gameData = new GameData
                    {
                        HighScores = _highScores,
                        MaxScore = _highScores.Length > 0 ? _highScores[0].Score : 0
                    };
                    _xmlSerializer.Serialize(gameData, _xmlSerializer.FilePath);
                }

                _currentFormat = newFormat;
                LoadHighScores();
        }

        private void HighScoresListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (HighScoresListView.SelectedItem is HighScore selectedScore)
            {
                MessageBox.Show(
                    $"Игрок: {selectedScore.PlayerName}\n" +
                    $"Счёт: {selectedScore.Score}\n" +
                    $"Сложность: {selectedScore.Difficulty}",
                    "Информация о рекорде"
                );
            }
        }
    }
}