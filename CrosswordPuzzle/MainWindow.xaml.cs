using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using log4net;

namespace CrosswordPuzzle
{

    public partial class MainWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        

        Dictionary<string, string> UserInput_string = new Dictionary<string, string>();
        Dictionary<int, List<Cell>> CellDic = new Dictionary<int, List<Cell>>();

        List<Word> WordList = new List<Word>();
        
        int nowCnt = 0;
        int nowType = 0;
          
        public class Word
        {
            public string searchWord { get; set; }
            public string searchClue { get; set; }
            public int startCol { get; set; }
            public int startRow { get; set; }
            public string ColOrRow { get; set; }
            public int labelNum { get; set; }
        }





        public MainWindow()
        {
            InitializeComponent();
            Set_Puzzle(5, 1);
        }

        /// <summary>
        /// 선택에 따른 퍼즐 셋팅
        /// </summary>
        /// <param name="cnt">n x n n값</param>
        /// <param name="type">프리셋번호</param>
        /// 
        // --------퍼즐 셋팅 함수---------
        public void Set_Puzzle(int cnt, int type)
        {
            nowCnt = cnt;
            nowType = type;

            for (int i = 0; i < cnt; i++)
            {
                RowDefinition rowDefin = new RowDefinition();
                ColumnDefinition colDefin = new ColumnDefinition();
                input.ColumnDefinitions.Add(colDefin);
                input.RowDefinitions.Add(rowDefin);
            }

            // 판 초기화
            for (int i = 0; i < cnt; i++)
            {
                for (int j = 0; j < cnt; j++)
                {
                    Cell cell = new Cell();
                    cell.Name = $"Col{i}_Row{j}";
                    cell.GotFocus += OnGotFocusHandler;
                    cell.LostFocus += OnLostFocusHandler;
                    Grid.SetColumn(cell, i);
                    Grid.SetRow(cell, j);
                    input.Children.Add(cell);
                    input.RegisterName(cell.Name, cell);
                }
            }

            // 단어 리스트 불러오기
            using (Entities word = new Entities())
            {
                var OraLINQ_1 = (from p in word.PuzzleInfo
                                 where p.QType == cnt && p.QNum== type
                                 join w in word.WordClue on p.PuzzleID equals w.WordClueID
                                 select new
                                 {
                                     word = w.word,
                                     clue = w.clue,
                                     col = p.StartCol,
                                     row = p.StartRow,
                                     ColOrRow = p.ColOrRow
                                 }).ToList();

                for (int i = 0; i < OraLINQ_1.Count; i++)
                {
                    WordList.Add(new Word()
                    {
                        searchWord = OraLINQ_1[i].word,
                        searchClue = OraLINQ_1[i].clue,
                        startCol = (int)OraLINQ_1[i].col,
                        startRow = (int)OraLINQ_1[i].row,
                        ColOrRow = OraLINQ_1[i].ColOrRow
                    });
                }
            }

            Set_WordNum(); // 문제 번호 입력

            // 단어 리스트에 맞게 입력 텍스트 박스 표시하기. Set_TextBox()
            for (int i = 0; i < WordList.Count; i++)
            {
                Set_TextBox(WordList[i], i);
                Set_Clue(WordList[i]);
                //border(WordList[i], i);
            } 
            
            
        }

        public void Set_TextBox(Word list, int index)  // 입력 텍스트 박스 표시
        {
            List<Cell> CellList = new List<Cell>();

            if (list.ColOrRow == "r")
            {
                for (int i = 0; i < list.searchWord.Length; i++)
                {
                    var a = (Cell)this.FindName($"Col{list.startCol + i}_Row{list.startRow}");
                    CellList.Add(a);
                    a.CellInputChange();
                }
            }
            else
            {
                for (int i = 0; i < list.searchWord.Length; i++)
                {
                    var a = (Cell)this.FindName($"Col{list.startCol}_Row{list.startRow + i}");
                    CellList.Add(a);
                    a.CellInputChange();
                }
            }
            CellDic.Add(index, CellList);
        }

        public void Set_Clue(Word list)  // 단어 힌트 표시
        {
            WordClueLabel clue = new WordClueLabel();
            clue.ClueLabel.Text = $"{list.labelNum}  {list.searchClue}";

            if (list.ColOrRow == "r")
            {

                word_clue_lable.Children.Add(clue);
            }
            else
            {
                word_clue_lable2.Children.Add(clue);
            }

        }

        public void Set_WordNum() // 문제 번호
        {
            int number = 1;
            for (int i = 0; i < WordList.Count; i++)
            {
                var a = (Cell)this.FindName($"Col{WordList[i].startCol}_Row{WordList[i].startRow}");
                
                if (a.WordNumChange(number) == true)
                {
                    WordList[i].labelNum = number;
                    number++;
                }
                else
                {
                    WordList[i].labelNum = (int)a.WordNum.Content;
                }
                
            }
        }

        public void textInput(Word list, int index)  // 사용자가 textbox에 입력한 값 가져오기
        {
            string text = "";
            if (list.ColOrRow == "r")
            {
                for (int i = 0; i < list.searchWord.Length; i++)
                {
                    var a = (Cell)this.FindName($"Col{list.startCol + i}_Row{list.startRow}");
                    text += a.InputCell.Text;
                }
            }
            else
            {
                for (int i = 0; i < list.searchWord.Length; i++)
                {
                    var a = (Cell)this.FindName($"Col{list.startCol}_Row{list.startRow + i}");
                    text += a.InputCell.Text;
                }
            }
            Dic_TextInput($"{index}번단어", text);
        }

        public void Dic_TextInput(string x, string text) // 입력한 값 Dictionary에 입력
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

        public void clearAll(int iCnt)
        {
            for (int i = 0; i < iCnt; i++)
            {
                for (int j = 0; j < iCnt; j++)
                {
                    if (FindName($"Col{i}_Row{j}") != null)
                    {
                        input.UnregisterName($"Col{i}_Row{j}");
                    }
                }
            }
            WordList.Clear();
            word_clue_lable.Children.Clear();
            word_clue_lable2.Children.Clear();
            CellDic.Clear();

            input.Children.Clear();
            input.RowDefinitions.Clear();
            input.ColumnDefinitions.Clear();
        }


        //-------- Evnet ---------
        public void OnGotFocusHandler(object sender, RoutedEventArgs e)
        {
            string CellName = (sender as Cell).Name;
            int key = 0;
            for (int i = 0; i < CellDic.Count; i++)
            {
                for (int j = 0; j < CellDic[i].Count; j++)
                {
                    if (CellDic[i][j].Name == CellName)
                    {
                        key = i;
                        break;
                    }
                }
            }
            border(CellDic[key], key, "focusIn");
        }

        public void OnLostFocusHandler(object sender, RoutedEventArgs e)
        {
            var a = (Border)this.FindName("focusBorder");
            input.Children.Remove(a);
            input.UnregisterName("focusBorder");

            string CellName = (sender as Cell).Name;
            int key = 0;
            for (int i = 0; i < CellDic.Count; i++)
            {
                for (int j = 0; j < CellDic[i].Count; j++)
                {
                    if (CellDic[i][j].Name == CellName)
                    {
                        key = i;
                        break;
                    }
                }
            }

            int num = 0;
            for (int i = 0; i < CellDic[key].Count; i++)
            {
                if (CellDic[key][i].InputCell.Text != "")
                {
                    num++;
                    if (num == CellDic[key].Count)
                    {
                        if (WordList[key].ColOrRow == "r")
                        {
                            textInput(WordList[key], key);
                            background(key);
                        }
                        else
                        {
                            textInput(WordList[key], key);
                            background(key);
                        }
                    }
                }

            }
        }

        public void background(int index) // 정답 버튼 클릭하면 정답 여부 확인후 배경색 바꿈
        {
            for (int i = 0; i < CellDic[index].Count; i++)
            {
                if (CellDic[index][i].InputCell.Text == WordList[index].searchWord[i].ToString())
                {
                    CellDic[index][i].InputCell.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#1A47C83E");
                }
                else
                {
                    CellDic[index][i].InputCell.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#1AFF0000");

                }
            }
        }
        
        public void border(List<Cell> list, int index, string focus) // 텍스트박스에 포커스가 가면 border로 표시
        {
            Border bor = new Border();
            if (focus == "focusIn")
            {
                bor.BorderBrush = Brushes.CornflowerBlue;
                bor.BorderThickness = new Thickness(2);
                bor.Name = "focusBorder";
                Grid.SetColumn(bor, WordList[index].startCol);
                Grid.SetRow(bor, WordList[index].startRow);
                input.RegisterName(bor.Name, bor);

                if (WordList[index].ColOrRow == "r")
                {
                    Grid.SetColumnSpan(bor, list.Count);
                }
                else
                {
                    Grid.SetRowSpan(bor, list.Count);
                }
                input.Children.Add(bor);
            }
        }

        private void answerBtn_Click(object sender, RoutedEventArgs e) // 정답확인 버튼
        {
            for (int i = 0; i < WordList.Count; i++)
            {
                textInput(WordList[i], i);
                background(i);
            }

            int RightCnt = 0, WrongCnt = 0;
            for (int i = 0; i < WordList.Count; i++)
            {
                string str = (i) + "번단어";
                if (UserInput_string[str] != WordList[i].searchWord)
                {
                    WrongCnt++;
                }
                else
                {
                    RightCnt++;
                }
            }

            LogHelper.Log(RightCnt, WrongCnt);

            MessageBox.Show($"맞은 개수 : {RightCnt}  틀린 개수 : {WrongCnt}");
            log.Info($"맞은 개수 : {RightCnt}  틀린 개수 : {WrongCnt}");
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e) // 퍼즐 유형 바꾼 경우 (5x5, 7x7)
        {
            int select = (sender as ComboBox).SelectedIndex;
            if (input != null)
            {
                int cnt = 0, type = 0;
                if (select == 1)
                {
                    cnt = 7;
                    type = 1;
                }
                else if (select == 0)
                {
                    cnt = 5;
                    type = 1;
                }

                clearAll(cnt);
                Set_Puzzle(cnt, type);
            }

        }

        private void newBtn_Click(object sender, RoutedEventArgs e) // 새문제 버튼
        {
            clearAll(nowCnt);
            if(nowCnt == 5)
            {
                if (nowType == 3)
                {
                    Set_Puzzle(nowCnt, 1);
                }
                else
                {
                    Set_Puzzle(nowCnt, nowType + 1);
                }
            }
            else
            {
                if (nowType == 2)
                {
                    Set_Puzzle(nowCnt, 1);
                }
                else
                {
                    Set_Puzzle(nowCnt, nowType + 1);
                }
            }

            
        }




        //-------- Log ---------

        ///public class LogEntry
        ///{
        ///    public string DateTime { get; set; }
        ///    public int RightCount { get; set; }
        ///    public int WrongCount { get; set; }
        ///}


        public class DBLogger
        {
            string connectionString = string.Empty;
            public void Log(int RightCnt, int WrongCnt)
            {
                using (Entities log = new Entities())
                {
                    var create = log.Set<Log>();
                    create.Add(new Log
                    {
                        LogID = "",
                        RightCount = RightCnt,
                        WrongCount = WrongCnt,
                        log_date = System.DateTime.Now
                    });
                    log.SaveChanges();
                }
            }
        }

        public static class LogHelper
        {
            private static DBLogger logger = null;

            public static void Log(int RightCnt, int WrongCnt)
            {
                logger = new DBLogger();
                logger.Log(RightCnt, WrongCnt);
            }
        }
    }
}

