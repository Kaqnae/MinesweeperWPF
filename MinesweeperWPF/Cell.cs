namespace MinesweeperWPF;

public class Cell
{
    public bool IsMine { get; set; }
    public bool IsRevealed { get; set; }
    public int NeighboringMines { get; set; }
}