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
        private List<Label> pl_shapeLabels = new List<Label>();
        private Label pl_alredyShownLabel;
        private Label pl_timerLabel = new Label();
        private Button[] pl_functionButtons = new Button[3];
        private PL_FixedTableLayoutPanel pl_gameTable = new PL_FixedTableLayoutPanel();
        private Timer pl_generalTimer = new Timer();
        private int pl_generalTimerTicks = 0;
        private Timer pl_tileTimer = new Timer();

        private class PL_FixedTableLayoutPanel : TableLayoutPanel
        {
            public PL_FixedTableLayoutPanel()
            {
                this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            }
        }

        private void pl_createNewGameTable()
        {
            List<string> pl_allShapes = new List<string>()
            {"R", "R", "A", "A", "1", "1", "P", "P", "0", "0", "Y", "Y", "w", "w", "z", "z"};
            pl_shapeLabels.Clear();
            for (int pl_i = 0; pl_i < 4; pl_i++)
            {
                for (int pl_k = 0; pl_k < 4; pl_k++)
                {
                    Label pl_tempShape = new Label();
                    pl_tempShape.Name = pl_allShapes.Count.ToString();
                    pl_tempShape.BackColor = Color.CornflowerBlue;
                    pl_tempShape.ForeColor = Color.CornflowerBlue;
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
        }

        public PL_Memo()
        {
            InitializeComponent();
            //ustawienie szerokości i wysokości okna 550 px, 550px.
            Size = new Size(550, 600);
            pl_tileTimer.Interval = 5000;
            pl_tileTimer.Tick += new EventHandler(pl_hideAllTiles);
            pl_generalTimer.Interval = 1000;
            pl_generalTimer.Tick += new EventHandler(pl_generalTimerAction);
            pl_gameTable.BackColor = Color.CornflowerBlue;
            pl_gameTable.Dock = DockStyle.Fill;
            pl_gameTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            pl_gameTable.RowCount = 5;
            pl_gameTable.ColumnCount = 4;
            pl_gameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            pl_gameTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            for (int pl_i = 0; pl_i < 4; pl_i++)
            {
                pl_gameTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                pl_gameTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }
            for (int pl_i = 0; pl_i<3;pl_i++)
            {
                pl_functionButtons[pl_i] = new Button();
                pl_functionButtons[pl_i].Dock = DockStyle.Fill;
                pl_functionButtons[pl_i].Font = new Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            }
            pl_functionButtons[0].Text = "START";
            pl_functionButtons[0].Click += new EventHandler(pl_start);
            pl_functionButtons[1].Text = "RESET";
            pl_functionButtons[1].Click += new EventHandler(pl_reset);
            pl_functionButtons[2].Text = "END";
            pl_functionButtons[2].Click += new EventHandler(pl_end);
            pl_timerLabel.Dock = DockStyle.Fill;
            pl_timerLabel.TextAlign = ContentAlignment.MiddleCenter;
            pl_timerLabel.Font = new Font("Arial", 16);
            pl_gameTable.Controls.AddRange(pl_functionButtons);
            pl_gameTable.Controls.Add(pl_timerLabel);
            pl_createNewGameTable();
            pl_gameTable.Controls.AddRange(pl_functionButtons);
            pl_gameTable.Controls.Add(pl_timerLabel);
            pl_gameTable.Controls.AddRange(pl_shapeLabels.ToArray());
            pl_generalTimer.Start();
            Controls.Add(pl_gameTable);
            pl_gameTable.ResumeLayout(true);
        }

        private void pl_shapeClicked(object pl_sender, EventArgs pl_args)
        {
            if (pl_alredyShownLabel != null)
            {
                if (!pl_shapeLabels.Contains((Label)pl_sender)) return;
                Label pl_currentSelectedLabel = (Label)pl_sender;
                if (pl_alredyShownLabel.Name == pl_currentSelectedLabel.Name) return;
                pl_currentSelectedLabel.ForeColor = Color.Black;
                if (pl_alredyShownLabel.Text == pl_currentSelectedLabel.Text)
                {
                    pl_shapeLabels.RemoveAll(pl_x => pl_x.Text == pl_alredyShownLabel.Text);
                    pl_currentSelectedLabel.BackColor = Color.HotPink;
                    pl_alredyShownLabel.BackColor = Color.HotPink;
                }
                pl_shapeLabels.ForEach(pl_x => pl_x.ForeColor = Color.CornflowerBlue);
                pl_alredyShownLabel = null;
                pl_tileTimer.Stop();
                if (pl_shapeLabels.Count == 0)
                {
                    pl_generalTimer.Stop();
                    MessageBox.Show("Świetnie Ci poszło, może zagrasz znów?", "Gratulacje!");
                }
            }
            else
            {
                if (!pl_shapeLabels.Contains((Label)pl_sender)) return;
                pl_tileTimer.Start();
                ((Label)pl_sender).ForeColor = Color.Black;
                pl_alredyShownLabel = (Label)pl_sender;
            }
        }

        private void pl_start(object pl_sender, EventArgs pl_args)
        {
            if(pl_shapeLabels.Count > 0)
            {
                MessageBox.Show("Nie możesz rozpocząć już rozpoczętej gry!", "Uwaga!");
                return;
            }
            pl_gameTable.Controls.Clear();
            pl_createNewGameTable();
            pl_gameTable.Controls.AddRange(pl_functionButtons);
            pl_gameTable.Controls.Add(pl_timerLabel);
            pl_gameTable.Controls.AddRange(pl_shapeLabels.ToArray());
            pl_generalTimer.Start();
            pl_generalTimerTicks = 0;
        }

        private void pl_reset(object pl_sender, EventArgs pl_args)
        {
            if (pl_shapeLabels.Count > 0) MessageBox.Show("Może tym razem pójdzie Ci lepiej!", "Uwaga!");
            else
            {
                MessageBox.Show("Nawet nie zaczałeś, a już chcesz resetować? Najpierw zacznij grę :)", "Uwaga!");
                return;
            }
            pl_gameTable.Controls.Clear();
            pl_createNewGameTable();
            pl_gameTable.Controls.AddRange(pl_functionButtons);
            pl_gameTable.Controls.Add(pl_timerLabel);
            pl_gameTable.Controls.AddRange(pl_shapeLabels.ToArray());
            pl_generalTimer.Stop();
            pl_generalTimer.Start();
            pl_generalTimerTicks = 0;
        }

        private void pl_end(object pl_sender, EventArgs pl_args)
        {
            if (pl_shapeLabels.Count == 0)
            {
                MessageBox.Show("Nie możesz zakończyć jeszcze nierozpoczętej gry!", "Uwaga!");
                return;
            }
            pl_shapeLabels.ForEach(pl_x => pl_x.ForeColor = Color.Black);
            pl_shapeLabels.Clear();
            pl_generalTimer.Stop();
        }

        private void pl_generalTimerAction(object pl_sender, EventArgs pl_args)
        {
            pl_generalTimerTicks++;
            int pl_minutes, pl_seconds;
            pl_minutes = pl_generalTimerTicks / 60;
            pl_seconds = pl_generalTimerTicks - (60 * pl_minutes);

            pl_timerLabel.Text = pl_minutes.ToString("00") + " : " + pl_seconds.ToString("00");
        }

        private void pl_hideAllTiles(object pl_sender, EventArgs pl_args)
        {
            pl_shapeLabels.ForEach(pl_x => pl_x.ForeColor = Color.CornflowerBlue);
            pl_alredyShownLabel = null;
        }
    }
}
