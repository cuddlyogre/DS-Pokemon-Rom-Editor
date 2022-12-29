using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSPRE.Editors {
  public partial class MatrixNavigator : UserControl {
    public const byte tileSize = 16;
    public int width;
    public int height;
    public List<int[]> positions;

    public event EventHandler<CellMarkedEventArgs> CellMarked;

    public MatrixNavigator() {
      InitializeComponent();
    }

    private void eventMatrixPictureBox_Click(object sender, EventArgs e) {
      Point coordinates = eventMatrixPictureBox.PointToClient(Cursor.Position);
      Point mouseTilePos = new Point(coordinates.X / tileSize, coordinates.Y / tileSize);
      MarkActiveCell(mouseTilePos.X, mouseTilePos.Y);
    }

    public void MarkActiveCell(int X, int Y) {
      const int padding = 1;
      eventMatrixPictureBox.Image = new Bitmap(padding + tileSize * width, padding + tileSize * height);

      using (Graphics g = Graphics.FromImage(eventMatrixPictureBox.Image)) {
        DrawEventMatrix(g, width, height);
        MarkUsedCells(g, positions);
        MarkChosenCell(g, X, Y);
      }

      /* Update PictureBox and current coordinates labels */
      eventMatrixPictureBox.Invalidate();

      CellMarked?.Invoke(this, new CellMarkedEventArgs() { X = X, Y = Y });
    }

    private void DrawEventMatrix(Graphics g, int width, int height) {
      g.Clear(Color.Black);

      Pen pen = Pens.White;
      for (int y = 0; y < height; y++) {
        for (int x = 0; x < width; x++) {
          drawCell(g, pen, x, y);
        }
      }
    }

    private void MarkUsedCells(Graphics g, List<int[]> positions) {
      Brush brush = Brushes.Orange;
      for (int i = 0; i < positions.Count; i++) {
        fillCell(g, brush, positions[i][0], positions[i][1]);
      }
    }

    private void MarkChosenCell(Graphics g, int x, int y) {
      Brush brush = Brushes.Lime;
      fillCell(g, brush, x, y);
    }

    private void drawCell(Graphics g, Pen pen, int x, int y) {
      const int padding = 1;
      const int cellSize = 14;
      Rectangle eventMatrixRectangle = new Rectangle(padding + tileSize * x, padding + tileSize * y, cellSize, cellSize);
      g.DrawRectangle(pen, eventMatrixRectangle);
    }

    private void fillCell(Graphics g, Brush brush, int x, int y) {
      const int padding = 2;
      const int cellSize = 13;
      Rectangle eventMatrixRectangle = new Rectangle(padding + tileSize * x, padding + tileSize * y, cellSize, cellSize);
      g.FillRectangle(brush, eventMatrixRectangle);
    }
  }
  public class CellMarkedEventArgs : EventArgs {
    public int X { get; set; }
    public int Y { get; set; }
  }
}
