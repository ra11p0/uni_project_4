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
        //Lista przechowująca nieznalezione przez użytkownika symbole.
        private List<Label> pl_shapeLabels = new List<Label>();
        //Zmienna przechowująca tymczasowo pierwszy odkryty symbol.
        private Label pl_alredyShownLabel;
        //Kontrolka wyświetlająca czas upływający od rozpoczęcia gry.
        private Label pl_timerLabel = new Label();
        //Tablica przechowująca przyciski funkcyjne gry.
        private Button[] pl_functionButtons = new Button[3];
        //Kontrolka definiująca ułożenie innych kontrolek w oknie programu. Obiekt TableLayoutPanel
        //został zmodyfikowany by zniwelować migotanie przy odświeżaniu zawartości.
        private PL_FixedTableLayoutPanel pl_gameTable = new PL_FixedTableLayoutPanel();
        //Obiekt Timer przy pomocy której wyświetlany jest czas od rozpoczęcia gry.
        private Timer pl_generalTimer = new Timer();
        //Zmienna typu INT przechowująca liczbę sekund ubiegłych od rozpoczęcia gry.
        private int pl_generalTimerTicks = 0;
        //Obiekt Timer przy pomocy którego odkryty symbol zostaje wygaszony po upłynięciu 5 sekund od odkrycia. 
        private Timer pl_tileTimer = new Timer();
        //Utworzenie klasy dziedziczącej po klasie TableLayoutPanel
        private class PL_FixedTableLayoutPanel : TableLayoutPanel
        {
            //Konstruktor klasy
            public PL_FixedTableLayoutPanel()
            {
                //Modyfikacje niwelujące migotanie obrazu przy odświeżaniu zawartości. Zastosowanie podwójnego bufora rysowania kontrolek.
                this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            }
        }
        //funkcja generująca nowe rozłożenie symboli na planszy gry
        private void pl_createNewGameTable()
        {
            //Deklaracja listy symboli oraz przypisanie do niej symboli możliwych do odnalezienia w grze.
            List<string> pl_allShapes = new List<string>()
            {"R", "R", "A", "A", "1", "1", "P", "P", "0", "0", "Y", "Y", "w", "w", "z", "z"};
            //Usunięcie symboli z listy symboli możliwych do odnalezienia w grze.
            pl_shapeLabels.Clear();
            //pętla generująca 16 obiektów typu Label oraz definująca właściwości tych obiektów
            for (int pl_i = 0; pl_i < 4; pl_i++)
            {
                for (int pl_k = 0; pl_k < 4; pl_k++)
                {
                    //utworzenie tymczasowego obiektu typu label
                    Label pl_tempShape = new Label();
                    //nadanie nazwy obiektowi typu label na podstawie liczby dostępnych do przypisania symboli.
                    //nazwa jest unikalna dla kazdej kontrolki label w liście, po tej nazwie dokonuje się
                    //w późniejszym etapie rozpoznania kontrolki.
                    pl_tempShape.Name = pl_allShapes.Count.ToString();
                    //ustalenie właściwości koloru tła kontrolki.
                    pl_tempShape.BackColor = Color.CornflowerBlue;
                    //ustalenie właściwości koloru tekstu kontrolki.
                    pl_tempShape.ForeColor = Color.CornflowerBlue;
                    //
                    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                    //
                    pl_tempShape.AutoSize = false;
                    //ustalenie właściwości określającej wypełnienie kontrolką dostępnej przestrzeni.
                    pl_tempShape.Dock = DockStyle.Fill;
                    //ustalenie właściwości określającej pozycję tekstu w kontrolce, ustalono na wyśrodkowanie w pionie i poziomie.
                    pl_tempShape.TextAlign = ContentAlignment.MiddleCenter;
                    //ustalenie fontu kontrolki: Fonr Webdings, rozmiar 48pt, pogrubiona
                    pl_tempShape.Font = new Font("Webdings", 48F, System.Drawing.FontStyle.Bold);
                    //przypisanie właściwości Text kontrolki losowego symbolu z listy symboli pl_allShapes.
                    pl_tempShape.Text = pl_allShapes[new Random().Next(pl_allShapes.Count)];
                    //usunięcie z listy dostępnych do przypisania symboli sombolu przypisanego do obecnie tworzonej kontrolki Label.
                    pl_allShapes.Remove(pl_tempShape.Text);
                    //definiowanie zachowania symbolu po jego naciśnięciu. po naciśnięciu zostaje wykonana funkcja pl_shapeClicked
                    pl_tempShape.Click += new EventHandler(pl_shapeClicked);
                    //dodanie tymczasowej kontrolki do listy możliwych do odnalezienia symboli.
                    pl_shapeLabels.Add(pl_tempShape);
                }
            }
        }

        public PL_Memo()
        {
            //funkcja inicjująca program
            InitializeComponent();
            //ustawienie szerokości i wysokości okna 550 px, 600px.
            Size = new Size(550, 600);
            //ustalenie interwału wywołania funkcji przez Timer. interwał ustalony na 5000ms, 5s.
            pl_tileTimer.Interval = 5000;
            //określanie funkcji wywoływanej przez Timer. Timer wywołuje funkcje pl_hideAllTiles.
            pl_tileTimer.Tick += new EventHandler(pl_hideAllTiles);
            //ustalenie interwału wywołania funkcji przez Timer. interwał ustalony na 1000ms, 1s.
            pl_generalTimer.Interval = 1000;
            //określanie funkcji wywoływanej przez Timer. Timer wywołuje funkcje pl_generalTimerAction.
            pl_generalTimer.Tick += new EventHandler(pl_generalTimerAction);
            //Ustalenie właściwości koloru tła kontrolki ustalającej ułożenie elementów w oknie programu.
            pl_gameTable.BackColor = Color.CornflowerBlue;
            //Ustalenie właściwości ustalającej wypełnienie okna programu przez kontrolke pl_gameTable.
            pl_gameTable.Dock = DockStyle.Fill;
            //ustalenie stylu obramowania komórek w obiekcie pl_gameTable.
            pl_gameTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            //ustalenie liczby wierszy w kontrolce.
            pl_gameTable.RowCount = 5;
            //ustalenie liczby kolumn w kontrolce.
            pl_gameTable.ColumnCount = 4;
            //ustalenie wysokości pierwszego wiersza na 50 px.
            pl_gameTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
            //
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //
            pl_gameTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            //pętla nadająca styl kolejnym 4-em wierszom
            for (int pl_i = 0; pl_i < 4; pl_i++)
            {
                //ustalenie wielkości wierszy na 25% dostępnej przestrzeni.
                pl_gameTable.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
                //ustalenie wielkości kolumn na 25% dostępnej przestrzeni.
                pl_gameTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            }
            //pętla inicjująca 3 przyciski funkcyjne
            for (int pl_i = 0; pl_i<3;pl_i++)
            {
                //inicjowanie nowego obiektu button
                pl_functionButtons[pl_i] = new Button();
                //nadanie właściwości obiektom by wypełniały dostępną przestrzeń
                pl_functionButtons[pl_i].Dock = DockStyle.Fill;
                //ustalenie fontu kontrolek button: Font Arial, wielkość 18pt, pogrubiona.
                pl_functionButtons[pl_i].Font = new Font("Arial", 18F, System.Drawing.FontStyle.Bold);
            }
            //ustalenie napisu na przycisku
            pl_functionButtons[0].Text = "START";
            //ustalenie zachowania przycisku po wywołaniu. po nacisnieciu zostaje wywołana funkcja pl_start.
            pl_functionButtons[0].Click += new EventHandler(pl_start);
            //ustalenie napisu na przycisku
            pl_functionButtons[1].Text = "RESET";
            //ustalenie zachowania przycisku po wywołaniu. po naciśnięciu zostaje wywołana funcja pl_reset
            pl_functionButtons[1].Click += new EventHandler(pl_reset);
            //ustalenie napisu na przycisku
            pl_functionButtons[2].Text = "END";
            //ustalenie zachowania przycisku po wywołaniu. po naciśnięciu zostaje wywołana funckja pl_end
            pl_functionButtons[2].Click += new EventHandler(pl_end);
            //nadanie właściwości kontrolce typu label by wypełniała dostępną przestrzeń
            pl_timerLabel.Dock = DockStyle.Fill;
            //ustalenie pozycji tekstu w kontrolce label. tekst jest wyśrodkowany w pionie i poziomie
            pl_timerLabel.TextAlign = ContentAlignment.MiddleCenter;
            //ustalenie fontu kontrolki: Font Arial, wielkość 16pt
            pl_timerLabel.Font = new Font("Arial", 16);
            //dodanie wszystkich uprzednio utworzonych przycisków do siatki ułożenia obiektów.
            pl_gameTable.Controls.AddRange(pl_functionButtons);
            //dodanie do siatki ułożenia obiektów kontrolki label.
            pl_gameTable.Controls.Add(pl_timerLabel);
            //wywołanie funkcji generującej ułożenie symboli na planszy gry
            pl_createNewGameTable();
            //dodanie kontrolki Label wyświetlającej czasomierz do siatki ułożenia obiektów.
            pl_gameTable.Controls.Add(pl_timerLabel);
            //dodanie wszystkich utworzonych symboli do formularza gry
            pl_gameTable.Controls.AddRange(pl_shapeLabels.ToArray());
            //zozpoczęcie działania Timera liczącego czas od początku gry.
            pl_generalTimer.Start();
            //dodanie siatki ułożenia obiektów do formularza programu. skutkuje to również wyświetleniem
            //wszystkich kontrolek dodanych do tej siatki.
            Controls.Add(pl_gameTable);
            //
            //!!!!!!!!!!!!!!!!!!!
            //
            pl_gameTable.ResumeLayout(true);
        }

        //funkcja obsługująca interakcje użytkownika z symbolami
        private void pl_shapeClicked(object pl_sender, EventArgs pl_args)
        {
            //sprawdzenie czy inny symbol jest w tym czasie wyświetlany.
            if (pl_alredyShownLabel != null)
            {
                //jeśli inny symbol jest wyświetlany:
                //identyfikacja klikniętego symbolu
                Label pl_currentSelectedLabel = (Label)pl_sender;
                //sprawdzenie, czy na liście możliwych do odnalezienia symboli znajduje się wybrany symbol
                //jeśli nie znajduje się, funkcja zostaje przerwana
                if (!pl_shapeLabels.Contains(pl_currentSelectedLabel)) return;
                //sprawdzenie, czy kliknięty symbol jest juz odkryty
                //jeśli symbol został już odkryty, funkcja zostaje przerwana
                if (pl_alredyShownLabel.Name == pl_currentSelectedLabel.Name) return;
                //ustalenie koloru klikniętego symbolu na czarny
                pl_currentSelectedLabel.ForeColor = Color.Black;
                //sprawdzenie, czy kształt klikniętego symbolu jest identyczny jak kształt uprzednio klikniętego symbolu
                if (pl_alredyShownLabel.Text == pl_currentSelectedLabel.Text)
                {
                    //jeśli kształty są takie same:
                    //usunięcie odnalezionych symboli z listy symboli moliwych do odnalezienia.
                    pl_shapeLabels.RemoveAll(pl_x => pl_x.Text == pl_alredyShownLabel.Text);
                    //ustalenie koloru tła symboli odnalezionego kształtu na różowy.
                    pl_currentSelectedLabel.BackColor = Color.HotPink;
                    pl_alredyShownLabel.BackColor = Color.HotPink;
                }
                //ustalenie koloru kazdego moliwego do odnalezienia symbolu na identyczny jak tło, co skutkuje jego niewidocznością.
                pl_shapeLabels.ForEach(pl_x => pl_x.ForeColor = Color.CornflowerBlue);
                //usunięcie symbolu przechowywanego tymczasowo
                pl_alredyShownLabel = null;
                //zatrzymanie timera odliczającego 5 sekund od wyświetlenia pierwszego symbolu
                pl_tileTimer.Stop();
                //sprawdzenie, czy gracz odkrył wszystkie możliwe go odkrycia symbole
                if (pl_shapeLabels.Count == 0)
                {
                    //jeśli gracz odkrył wszystkie symbole:
                    //zatrzymanie Timera odliczającego czas od początku gry
                    pl_generalTimer.Stop();
                    //wyświetlenie pochwały, i zachęcenie do ponownej gry
                    MessageBox.Show("Świetnie Ci poszło, może zagrasz znów?", "Gratulacje!");
                }
            }
            //jeśli żaden inny symbol nie jest obecnie wyświetlany:
            else
            {
                //sprawdzenie, czy kliknięty symbol jest na liście symboli możliwych do odnalezienia 
                //jeśli nie, funkcja jest przerywana
                if (!pl_shapeLabels.Contains((Label)pl_sender)) return;
                //rozpoczęcie działania timera odliczającego 5 sekund od wyświetlenia pierwszego symbolu
                pl_tileTimer.Start();
                //zmiana koloru klikniętego symbolu na czarny.
                ((Label)pl_sender).ForeColor = Color.Black;
                //tymczasowe przypisanie kliknietego symbolu do zmiennej pl_alredyShownLabel.
                pl_alredyShownLabel = (Label)pl_sender;
            }
        }

        //funkcja definiująca zachowanie programu po kliknięciu przycisku START
        private void pl_start(object pl_sender, EventArgs pl_args)
        {
            //sprawdzenie, czy gracz rozpoczął już grę
            if(pl_shapeLabels.Count > 0)
            {
                //jeśli gracz rozpoczął już grę:
                //wyświetlenie pouczenia, ze nie mozna rozpocząć juz rozpoczętej gry.
                MessageBox.Show("Nie możesz rozpocząć już rozpoczętej gry!", "Uwaga!");
                //przerwanie działania funkcji
                return;
            }
            //usunięcie wszystkich kontrolek z siatki ułożenia elementów w formularzu
            pl_gameTable.Controls.Clear();
            //generowanie nowego ułożenia symboli na planszy gry
            pl_createNewGameTable();
            //dodanie przycisków funkcyjnych do siatki formularza
            pl_gameTable.Controls.AddRange(pl_functionButtons);
            //dodanie kontrolki wyswietlajacej czas od rozpoczęcia gry do formularza
            pl_gameTable.Controls.Add(pl_timerLabel);
            //dodanie wszystkich symboli do planszy gry 
            pl_gameTable.Controls.AddRange(pl_shapeLabels.ToArray());
            //uruchomienie timera odliczajacego czas od rozpoczecia gry
            pl_generalTimer.Start();
            //wyzerowanie czasu od rozpoczęcia gry
            pl_generalTimerTicks = 0;
        }

        //funkcja definiująca zachowanie programu po naciśnięciu przycisku RESET
        private void pl_reset(object pl_sender, EventArgs pl_args)
        {
            //sprawdzenie, czy gracz jest w trakcje gry, jeśli tak, dodanie otuchy 
            if (pl_shapeLabels.Count > 0) MessageBox.Show("Może tym razem pójdzie Ci lepiej!", "Uwaga!");
            //jeśli gracz nie jest w trakcie gry, pouczenie, że niemożliwe jest zresetowanie
            // gry, która się nie zaczęła, przerwanie wykonywania funkcji
            else
            {
                MessageBox.Show("Nawet nie zacząłeś, a już chcesz resetować? Najpierw zacznij grę :)", "Uwaga!");
                return;
            }
            //wyczyszczenie tablicy przechowującej symbole możliwe do odnalezienia
            pl_gameTable.Controls.Clear();
            //generowanie nowego ułożenia symboli na planszy gry 
            pl_createNewGameTable();
            //dodanie przycisków funkcyjnych do siatki formularza
            pl_gameTable.Controls.AddRange(pl_functionButtons);
            //dodanie kontrolki wyświetlającej czas od rozpoczęcia gry do formularza 
            pl_gameTable.Controls.Add(pl_timerLabel);
            //dodanie wszystkich symboli do planszy gry
            pl_gameTable.Controls.AddRange(pl_shapeLabels.ToArray());
            //zywerowanie czasu od rozpoczęcia gry
            pl_generalTimerTicks = 0;
        }

        //funkcja definiująca zachowanie programu po naciśnięciu przycisku END
        private void pl_end(object pl_sender, EventArgs pl_args)
        {
            //sprawdzenie, czy gra się rozpoczęła, jeśli nie, pouczenie, że nie można 
            //zakończyć nierozpoczętej gry i przerwanie wykonywania funkcji
            if (pl_shapeLabels.Count == 0)
            {
                MessageBox.Show("Nie możesz zakończyć jeszcze nierozpoczętej gry!", "Uwaga!");
                return;
            }
            //wyświetlenie wszystkich symboli gdy gracz sie poddał i zakończył grę
            pl_shapeLabels.ForEach(pl_x => pl_x.ForeColor = Color.Black);
            //wyczyszczenie listy moliwych do odnalezienia symboli
            pl_shapeLabels.Clear();
            //zatrzymanie timera odliczającego czas od rozpoczęcia gry
            pl_generalTimer.Stop();
        }

        //funkcja definiująca zachowanie programu po wywołaniu przez timer odliczający czas gry od rozpoczęcia
        private void pl_generalTimerAction(object pl_sender, EventArgs pl_args)
        {
            //dodanie sekundy do zmiennej przechowującej sekundy od rozpoczęcia gry
            pl_generalTimerTicks++;
            //deklaracja zmiennej przechowującej minuty i sekundy od rozpoczęcia gry
            int pl_minutes, pl_seconds;
            //obliczanie ile minut upłynęło od rozpoczęcia gry
            pl_minutes = pl_generalTimerTicks / 60;
            //obliczanie ile sekund mineło od rozpoczęcia gry, po odjęciu tych, które traktuje się jako minuty
            pl_seconds = pl_generalTimerTicks - (60 * pl_minutes);
            //aktualizacja zegara czasu gry. Formatowanie liczby minut i sekund by zawsze były widoczne 4 pozycje w całym zegarze
            pl_timerLabel.Text = pl_minutes.ToString("00") + " : " + pl_seconds.ToString("00");
        }

        //funkcja definiująca zachowanie programu po wywołaniu przez timer odliczający 5 sekund od odkrycia symbolu
        private void pl_hideAllTiles(object pl_sender, EventArgs pl_args)
        {
            //ukrywa wszystkie moliwe do odnalezienia symbole poprzez ustalenie ich koloru na kolor tła 
            pl_shapeLabels.ForEach(pl_x => pl_x.ForeColor = Color.CornflowerBlue);
            //usunięcie symbolu przechowywanego tymczasowo
            pl_alredyShownLabel = null;
        }
    }
}
