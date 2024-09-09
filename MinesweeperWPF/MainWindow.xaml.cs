using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MinesweeperWPF;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private const int Rows = 10;
    private const int Columns = 10;
    private const int MineCount = 10;
    private DispatcherTimer gameTime;
    private TimeSpan elapsedTime;
    
    private Cell[,] gameGrid = new Cell[Rows, Columns];
    private Button[,] buttons = new Button[Rows, Columns];
    
    public MainWindow()
    {
        InitializeComponent();
        InitializeGameGrid();
        GenerateGridUI();
    }

    /***
     * Method that initializes the game grid
     */
    private void InitializeGameGrid()
    {
        gameGrid = new Cell[Rows, Columns];
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                gameGrid[row, column] = new Cell
                {
                    IsRevealed = false,
                    IsMine = false,
                    NeighboringMines = 0
                };
            }
        }
        PlaceMines();
        CountNeighbouringMines();
        
        gameTime = new DispatcherTimer();
        gameTime.Interval = TimeSpan.FromSeconds(1);
        gameTime.Tick += GameTimer_Tick;
        
        elapsedTime = TimeSpan.Zero;
        

    }

    /***
     * Method for starting the games timer
     */
    private void GameTimer_Tick(object sender, EventArgs e)
    {
        elapsedTime = elapsedTime.Add(TimeSpan.FromSeconds(1));
        
        LblTime.Content = elapsedTime.ToString(@"mm\:ss");
    }


    /***
     * Method for placing the mines at random places
     */
    private void PlaceMines()
    {
        
        int minesToPlace = MineCount;
        
        Random rnd = new Random();

        while (minesToPlace > 0)
        {
            int row = rnd.Next(0, Rows);
            int column = rnd.Next(0, Columns);

            if (!gameGrid[row, column].IsMine)
            {
                gameGrid[row, column].IsMine = true;
                minesToPlace--;
            }

        }
    
    }
    
   /***
    * Method for counting how many mines are adjacent to each non-mine cell
    */
    private void CountNeighbouringMines()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (gameGrid[row, col].IsMine)
                {
                    continue;
                }
                int count = 0;

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        int newRow = row + i;
                        int newColumn = col + j;

                        if (newRow >= 0 && newRow < Rows && newColumn >= 0 && newColumn < Columns &&
                            gameGrid[newRow, newColumn].IsMine)
                        {
                            count++;
                        }
                    }
                }
                gameGrid[row, col].NeighboringMines = count;
            }
        }
    }
    /***
     * Generates the grid UI, by placing buttons
     */
    private void GenerateGridUI()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                MineButton button = new MineButton
                {
                    Row = row,
                    Column = column,
                };
                button.Click += Button_Click;
                buttons[row, column] = button;
                GameGrid.Children.Add(button);

            }
        }
    }
    /***
     * Button click event
     */
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        MineButton button = sender as MineButton;
        int row = button.Row;
        int column = button.Column;

        RevealCell(row, column);
    }
    /***
    * Method for revealing the cell, that is clicked
    */
    private void RevealCell(int row, int column)
    {
        if (!gameTime.IsEnabled)
        {
            gameTime.Start();
        }

        if (row < 0 || row >= Rows || column < 0 || column >= Columns || gameGrid[row, column].IsRevealed)
        {
            return;
        }

        gameGrid[row, column].IsRevealed = true;
        buttons[row, column].IsEnabled = false;

        if (gameGrid[row, column].IsMine)
        {
            string timeSpent = elapsedTime.ToString(@"mm\:ss");
            buttons[row, column].Content = "M";
            MessageBox.Show($"Game over!\nTime spent: {timeSpent}");
            ResetGame();
            return;
        }
        
        int neighboringMines = gameGrid[row, column].NeighboringMines;
        if (neighboringMines > 0)
        {
            buttons[row, column].Content = neighboringMines.ToString();
        }
        else
        {
            buttons[row, column].Content = string.Empty;
        }

        if (neighboringMines == 0)
        {
            RevealSurroundingCells(row, column);
        }

        if (CheckForWin())
        {
            string timeSpent = elapsedTime.ToString(@"mm\:ss");
            MessageBox.Show($"You win!\nTime spent: {timeSpent}");
            ResetGame();
        }

    }
    /***
     * Method for revealing surrounding cells
     */
    private void RevealSurroundingCells(int row, int column)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {
                    continue;
                }
                int newRow = row + i;
                int newColumn = column + j;

                if (newRow >= 0 && newRow < Rows && newColumn >= 0 && newColumn < Columns)
                {
                    RevealCell(newRow, newColumn);
                }
            }
        }
    }

    /***
     * Method for resetting the game
     */
    private void ResetGame()
    {
        gameTime.Stop();
        GameGrid.Children.Clear();
        InitializeGameGrid();
        CountNeighbouringMines();
        GenerateGridUI();
    }
    /***
     * Method for checking for a win
     */
    private bool CheckForWin()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (!gameGrid[row,column].IsMine && !gameGrid[row,column].IsRevealed)
                {
                    return false;
                }
            }
        }
        return true;
    }
}