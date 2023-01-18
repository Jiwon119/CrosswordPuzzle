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

    public partial class MainWindow : Window
    {
        Dictionary<string, string> UserInput_string = new Dictionary<string, string>();
        List<Word> WordList = new List<Word>();

        public MainWindow()
        {
            InitializeComponent();
            //DataContext = this;

            // 판 초기화
            for (int i = 0; i < 5; i++)         
            {
                for (int j = 0; j < 5; j++)
                {
                    Cell cell = new Cell();
                    cell.Name = $"Col{i}_Row{j}";
                    Grid.SetColumn(cell, i);
                    Grid.SetRow(cell, j);
                    input.Children.Add(cell);
                    input.RegisterName(cell.Name, cell);
                }
            }


            // 단어 리스트 불러오기.
            using (Entities1 word = new Entities1())
            {
                var OraLINQ_1 = (from u in word.WordAndClue
                                 orderby u.id
                                 select u).ToList();
                for (int i = 0; i < OraLINQ_1.Count; i++)
                {
                    WordList.Add(new Word()
                    {
                        searchWord = OraLINQ_1[i].word,
                        searchClue = OraLINQ_1[i].clue,
                    });
                }
            }

            WordList[0].startCol = 0;
            WordList[0].startRow = 0;

            WordList[1].startCol = 0;
            WordList[1].startRow = 0;

            WordList[2].startCol = 0;
            WordList[2].startRow = 2;

            WordList[3].startCol = 2;
            WordList[3].startRow = 2;

            WordList[4].startCol = 2;
            WordList[4].startRow = 4;

            WordList[5].startCol = 4;
            WordList[5].startRow = 1;

            for (int i = 0; i < WordList.Count; i++)
            {
                textBoxInput(WordList[i].searchWord.Length, WordList[i].startCol, WordList[i].startRow, i);
                clueInput(i);
            }
        }

        
        public void textBoxInput(int wordLength, int col, int row, int index)
        {
            if((index+1) % 2 == 1)
            {
                for (int i = 0; i < wordLength; i++)
                {
                    var a = (Cell)this.FindName($"Col{col + i}_Row{row}");
                    a.CellInputChange();
                }
            }
            else
            {
                for (int i = 0; i < wordLength; i++)
                {
                    var a = (Cell)this.FindName($"Col{col}_Row{row + i}");
                    a.CellInputChange();
                }
            }
        }
        public void clueInput(int index)
        {
            WordClueLabel clue = new WordClueLabel();
            clue.Content = WordList[index].searchClue;
            if ((index + 1) % 2 != 0)
            {
                word_clue_lable.Children.Add(clue);
            }
            else
            {
                word_clue_lable2.Children.Add(clue);
            }

        }
        public void textInput(int wordLength, int col, int row, int index)
        {
            string text = "";
            if ((index + 1) % 2 == 1)
            {
                for (int i = 0; i < wordLength; i++)
                {
                    var a = (Cell)this.FindName($"Col{col + i}_Row{row}");
                    text += a.InputCell.Text;
                }
            }
            else
            {
                for (int i = 0; i < wordLength; i++)
                { 
                    var a = (Cell)this.FindName($"Col{col}_Row{row + i}");
                    text += a.InputCell.Text;
                }
            }
            UserDic_TextInput($"{index + 1}번단어", text);
        }
        private void Btn(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < WordList.Count; i++)
            {
                textInput(WordList[i].searchWord.Length, WordList[i].startCol, WordList[i].startRow, i);
            }

            string msg ="";
            for(int i = 0; i < WordList.Count; i++)
            {
                string str = (i + 1) + "번단어";
                if( UserInput_string[str] != WordList[i].searchWord)
                {
                    msg = "틀림";
                    break;
                }
                else {
                    msg = "정답";
                }
            }
            MessageBox.Show(msg);
        }

        public void UserDic_TextInput(string x, string text)
        {
            if (UserInput_string.ContainsKey(x))
            {
                UserInput_string[x] = text;
            }
            else
            {
                UserInput_string.Add(x, text);
            }
        }

        public class Word
        {
            public string searchWord { get; set; }
            public string searchClue { get; set; }
            public int startCol { get; set; }
            public int startRow { get; set; }
        }
    }
}

