using System.Windows.Controls;

namespace MinesweeperWPF;

public class MineButton : Button
{
    public int Row { get; set; }
    public int Column { get; set; }
    
}