using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL_Projekt
{
    public partial class PL_Memo : Form
    {
        private TableLayoutPanel pl_gameTable = new TableLayoutPanel();
        private List<Label> pl_shapeLabels = new List<Label>();
        public PL_Memo()
        {
            InitializeComponent();
            //ustawienie szerokości i wysokości okna 550 px, 550px.
            this.Size = new Size(550, 550);
            pl_gameTable.BackColor = Color.CornflowerBlue;
            pl_gameTable.Dock = DockStyle.Fill;
            pl_gameTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            pl_gameTable.RowCount = 4;
            pl_gameTable.ColumnCount = 4;
            for (int pl_i = 0; pl_i < 4; pl_i++)
            {
                pl_gameTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                pl_gameTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }
            List<string> pl_allShapes = new List<string>()
            {"R", "R", "A", "A", "1", "1", "P", "P", "0", "0", "Y", "Y", "w", "w", "z", "z"};
            for (int pl_i = 0; pl_i < 4; pl_i++)
            {
                for (int pl_k = 0; pl_k < 4; pl_k++)
                {
                    Label pl_tempShape = new Label();
                    pl_tempShape.Name = pl_allShapes.Count.ToString();
                    pl_tempShape.Location = new Point(pl_k * 200, pl_i * 112);
                    pl_tempShape.BackColor = Color.CornflowerBlue;
                    pl_tempShape.AutoSize = false;
                    pl_tempShape.Dock = DockStyle.Fill;
                    pl_tempShape.TextAlign = ContentAlignment.MiddleCenter;
                    pl_tempShape.Font = new Font("Webdings", 48F, System.Drawing.FontStyle.Bold);
                    pl_tempShape.Text = pl_allShapes[new Random().Next(pl_allShapes.Count)];
                    pl_allShapes.Remove(pl_tempShape.Text);
                    pl_tempShape.Click += new EventHandler(pl_shapeClicked);
                    pl_shapeLabels.Add(pl_tempShape);
                }
            }
            Controls.Add(pl_gameTable);
            pl_gameTable.ResumeLayout(false);
            pl_gameTable.PerformLayout();
            pl_gameTable.Controls.AddRange(pl_shapeLabels.ToArray());
        }

        private void pl_shapeClicked(object pl_sender, EventArgs pl_args)
        {
            pl_shapeLabels.FindAll(pl_x => pl_x.Text == ((Label)pl_sender).Text).ForEach(pl_x => pl_x.Text = "");
        }
    }
}
