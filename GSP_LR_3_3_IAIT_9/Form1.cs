using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GSP_LR_3_3_IAIT_9
{
    public class PointInfo
    {
        public int x; // х координата границы сегмента
        public int dQ; // хранит соответствующее приращение пороговой функции с учетом веса операнда
    }

    public partial class Form1 : Form
    {
        Graphics g;
        Pen DrawPen = new Pen(Color.Black, 1);
        int TMOType = 0; // Тип тмо, по умолчанию объединение
        int MainFigureId = 0; // id главной фигуры из двух, по умолчанию главная первая
        bool IsFirstDrawn = false; // флаг, отслеживающий, нарисована ла первая фигура
        bool IsSecondDrawn = false; // флаг, отслеживающий, нарисована ли вторая фигура
        List<Point> VertexListFig1 = new List<Point>(); // Список вершин первого многоугольника
        List<Point> VertexListFig2 = new List<Point>(); // Список вершин второго многоугольника
        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics(); //инициализация графики
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Полная очистка экрана и списка вершин, после нажатия на кнопку
            g.Clear(Color.White);
            VertexListFig1.Clear();
            VertexListFig2.Clear();
            IsFirstDrawn = false;
            IsSecondDrawn = false;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Выбор цвета после изменения значения comboBox1
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    DrawPen.Color = Color.Black;
                    break;
                case 1:
                    DrawPen.Color = Color.Red;
                    break;
                case 2:
                    DrawPen.Color = Color.Green;
                    break;
                case 3:
                    DrawPen.Color = Color.Blue;
                    break;
            }
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Выбор главной фигуры, после изменения значения comboBox2
            MainFigureId = comboBox2.SelectedIndex;
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Выбор типа ТМО, после изменения значения comboBox3
            TMOType = comboBox3.SelectedIndex;
        }

        /**
         * Метод, выполняющий тмо в заисимости от переданых параметров
         */
        private void MakeFigureByTMO()
        {
            // множество значений суммы Q пороговых функций L операндов, соответствующее заданной ТМО
            List<int> SetQ = new List<int>(); 
            switch (TMOType)
            {
                case 0:
                    {
                        SetQ.Add(1);
                        SetQ.Add(3);
                        break;
                    }
                case 1:
                    {
                        SetQ.Add(3);
                        SetQ.Add(3);
                        break;
                    }
                case 2:
                    {
                        SetQ.Add(1);
                        SetQ.Add(2);
                        break;
                    }
                case 3:
                    {
                        SetQ.Add(2);
                        SetQ.Add(2);
                        break;
                    }
                case 4:
                    {
                        SetQ.Add(1);
                        SetQ.Add(1);
                        break;
                    }
                default:
                    {
                        MessageBox.Show("Выбран некорректный тип ТМО, пожалуйста исправьте введенные данные");
                        return;
                    }
            }

            // списки точек фигур А и B
            List<Point> VertexListA;
            List<Point> VertexListB;

            // Определяем фигуру А и фигуру B
            if (MainFigureId == 0)
            {
                VertexListA = VertexListFig1;
                VertexListB = VertexListFig2;
            }
            else
            {
                VertexListA = VertexListFig2;
                VertexListB = VertexListFig1;
            }

            //Ищем граничные Y значения учитывая обе фигуры
            int ymin = 0;
            int ymax = pictureBox1.Height;
            Point pmin = VertexListA[0];
            Point pmax = VertexListA[0];
            foreach (Point p in VertexListA)
            {
                if (p.Y > pmax.Y) pmax = p;
                if (p.Y < pmin.Y) pmin = p;
            }
            foreach (Point p in VertexListB)
            {
                if (p.Y > pmax.Y) pmax = p;
                if (p.Y < pmin.Y) pmin = p;
            }
            ymin = pmin.Y < ymin ? ymin : pmin.Y;
            ymax = pmax.Y > ymax ? ymax : pmax.Y;

            // списки краевых x координат для каждой фигуры
            List<int> Xra = new List<int>();
            List<int> Xla = new List<int>();
            List<int> Xrb = new List<int>();
            List<int> Xlb = new List<int>();

            // Очищаем экран перед отрисовкой
            g.Clear(Color.White);

            for (int y = ymin; y <= ymax; y++)
            {
                // очищаем списки координат
                Xra.Clear();
                Xla.Clear();
                Xlb.Clear();
                Xrb.Clear();

                // заполнение границ фигуры А
                for (int i = 0; i < VertexListA.Count; i++)
                {
                    int k = i < VertexListA.Count - 1 ? i + 1 : 0;
                    // обработка граничных случаев
                    if (((VertexListA[i].Y < y) && (VertexListA[k].Y >= y)) || ((VertexListA[i].Y >= y) && (VertexListA[k].Y < y)))
                    {
                        // формула определения x координаты пересечения двух отрезков, заданных вершинами
                        double x = VertexListA[i].X + ((VertexListA[k].X - VertexListA[i].X) * (y - VertexListA[i].Y)) / (VertexListA[k].Y - VertexListA[i].Y);
                        if (VertexListA[k].Y - VertexListA[i].Y < 0)
                        {
                            Xra.Add((int)x);
                        }
                        else
                        {
                            Xla.Add((int)x);
                        }
                    }
                }

                // заполнение границ фигуры В
                for (int i = 0; i < VertexListB.Count; i++)
                {
                    int k = i < VertexListB.Count - 1 ? i + 1 : 0;
                    // обработка граничных случаев
                    if (((VertexListB[i].Y < y) && (VertexListB[k].Y >= y)) || ((VertexListB[i].Y >= y) && (VertexListB[k].Y < y)))
                    {
                        // формула определения x координаты пересечения двух отрезков, заданных вершинами
                        double x = VertexListB[i].X + ((VertexListB[k].X - VertexListB[i].X) * (y - VertexListB[i].Y)) / (VertexListB[k].Y - VertexListB[i].Y);
                        if (VertexListB[k].Y - VertexListB[i].Y < 0)
                        {
                            Xrb.Add((int)x);
                        }
                        else
                        {
                            Xlb.Add((int)x);
                        }
                    }
                }

                List<PointInfo> M = new List<PointInfo>(); // рабочии массив

                // Заполнение рабочего массива: 
                int n = Xla.Count;

                for (int i = 0; i < n; i++)
                {
                    PointInfo Buff = new PointInfo();
                    Buff.x = Xla[i];
                    Buff.dQ = 2;
                    M.Add(Buff);
                }

                n = Xra.Count;

                for (int i = 0; i < n; i++)
                {
                    PointInfo Buff = new PointInfo();
                    Buff.x = Xra[i];
                    Buff.dQ = -2;
                    M.Add(Buff);
                }

                n = Xlb.Count;

                for (int i = 0; i < n; i++)
                {
                    PointInfo Buff = new PointInfo();
                    Buff.x = Xlb[i];
                    Buff.dQ = 1;
                    M.Add(Buff);
                }

                n = Xrb.Count;

                for (int i = 0; i < n; i++)
                {
                    PointInfo Buff = new PointInfo();
                    Buff.x = Xrb[i];
                    Buff.dQ = -1;
                    M.Add(Buff);
                }

                // сортируем M по возрастанию X координаты
                M.Sort((a, b) => a.x.CompareTo(b.x));

                int Q = 0;
                int xemin = 0;
                int xemax = pictureBox1.Width;

                // Конечные списки x границ
                List<int> Xlr = new List<int>();
                List<int> Xrr= new List<int>();

                if (M.Count == 0) continue;

                if ((M[0].x >= xemin) && (M[0].dQ < 0))
                {
                    Xlr.Add(xemin);
                    Q = -M[0].dQ;
                }
                // Выполнение ТМО
                for (int i = 0; i < M.Count; i++)
                {
                    int x = M[i].x;
                    int Qnew = Q + M[i].dQ;
                    if (
                        (Q < SetQ[0] || Q > SetQ[1]) &&
                        (Qnew >= SetQ[0] && Qnew <= SetQ[1])
                        )
                    {
                        Xlr.Add(x);
                    }
                    if (
                        (Q >= SetQ[0] && Q <= SetQ[1]) &&
                        (Qnew < SetQ[0] || Qnew > SetQ[1])
                        )
                    {
                        Xrr.Add(x);
                    }
                    Q = Qnew;
                }
                if (Q >= SetQ[0] && Q <= SetQ[1])
                {
                    Xrr.Add(xemax);
                }
                for (int id = 0; id < Xlr.Count && id < Xrr.Count; id++)
                {
                    if (Xlr[id] < Xrr[id])
                    {
                        g.DrawLine(DrawPen, new Point(Xlr[id], y), new Point(Xrr[id], y));
                    }
                }
            }
        }

        /**
         * Метод выполняющий заливку по прямым
         */
        private void FillByLine(List<Point> VertexList)
        {
            // Определяем наивысшую и наинизшую по Y координате точки
            Point pmin = VertexList[0];
            Point pmax = VertexList[0];
            int ymin = 0;
            int ymax = pictureBox1.Height;
            foreach (Point p in VertexList)
            {
                if (p.Y > pmax.Y) pmax = p;
                if (p.Y < pmin.Y) pmin = p;
            }
            ymin = pmin.Y < ymin ? ymin : pmin.Y;
            ymax = pmax.Y > ymax ? ymax : pmax.Y;

            List<int> xBoundaries = new List<int>();

            for (int y = ymin; y <= ymax; y++)
            {
                xBoundaries.Clear();
                for (int i = 0; i < VertexList.Count; i++)
                {
                    int k = i < VertexList.Count - 1 ? i + 1 : 0;
                    if (((VertexList[i].Y < y) && (VertexList[k].Y >= y)) || ((VertexList[i].Y >= y) && (VertexList[k].Y < y)))
                    {
                        // формула определения x координаты пересечения двух отрезков, заданных вершинами
                        double x = VertexList[i].X + ((VertexList[k].X - VertexList[i].X) * (y - VertexList[i].Y)) / (VertexList[k].Y - VertexList[i].Y);
                        xBoundaries.Add((int)x);
                    }
                }
                xBoundaries.Sort((a, b) => a.CompareTo(b)); // сортировка по возростанию
                for (int el = 0; el < xBoundaries.Count - 1; el += 2)
                {
                    g.DrawLine(DrawPen, new Point(xBoundaries[el], y), new Point(xBoundaries[el + 1], y));
                }
            }
        }

        /**
         * Метод реинициализирующий полотно, при изменении размера окна
         */
        private void Form1_Resize(object sender, EventArgs e)
        {
            g = pictureBox1.CreateGraphics(); //реинициализация графики
        }

        /**
         * Рисование на полотне
         * 
         * На нажатие ЛКМ - ставится точка-вершина многоугольника
         * 
         * На нажатие ПКМ №1 - попытка заливки получившейся по точкам фигуры 1
         * Если вершин < 3, заливка происходить не будет
         * 
         * На нажатие ПКМ №2 - попытка заливки получившейся по точкам фигуры 2
         * Если вершин < 3, заливка происходить не будет
         * 
         * На 3 и последющие нажатия, отрисовка ТМО с выбранными параметрами
         */
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!IsFirstDrawn)
                {
                    VertexListFig1.Add(new Point() { X = e.X, Y = e.Y });
                    g.DrawEllipse(DrawPen, e.X - 2, e.Y - 2, 5, 5);
                    int pointCount = VertexListFig1.Count - 1;
                    if (pointCount > 0)
                    {
                        g.DrawLine(DrawPen, VertexListFig1[pointCount - 1], VertexListFig1[pointCount]);
                    }
                }
                else if (!IsSecondDrawn) {
                    VertexListFig2.Add(new Point() { X = e.X, Y = e.Y });
                    g.DrawEllipse(DrawPen, e.X - 2, e.Y - 2, 5, 5);
                    int pointCount = VertexListFig2.Count - 1;
                    if (pointCount > 0)
                    {
                        g.DrawLine(DrawPen, VertexListFig2[pointCount - 1], VertexListFig2[pointCount]);
                    }
                }
                
            }
            else if (e.Button == MouseButtons.Right)
            {                
                if (!IsFirstDrawn)
                {
                    int pointCount = VertexListFig1.Count - 1;
                    if (pointCount < 2)
                    {
                        MessageBox.Show("Введено слишком мало точек для существования многоугольника, отрисовка невозможна");
                        return;
                    }
                    g.DrawLine(DrawPen, VertexListFig1[pointCount - 1], VertexListFig1[pointCount]);
                    g.DrawLine(DrawPen, VertexListFig1[pointCount], VertexListFig1[0]); // соединяем первую и последнюю точки
                    IsFirstDrawn = true;
                    FillByLine(VertexListFig1);
                } else if (!IsSecondDrawn)
                {
                    int pointCount = VertexListFig2.Count - 1;
                    if (pointCount < 2)
                    {
                        MessageBox.Show("Введено слишком мало точек для существования многоугольника, отрисовка невозможна");
                        return;
                    }
                    g.DrawLine(DrawPen, VertexListFig2[pointCount - 1], VertexListFig2[pointCount]);
                    g.DrawLine(DrawPen, VertexListFig2[pointCount], VertexListFig2[0]); // соединяем первую и последнюю точки
                    IsSecondDrawn = true;
                    FillByLine(VertexListFig2);
                }
                else if (IsFirstDrawn && IsSecondDrawn)
                {
                    // Делаем ТМО с текущими параметрами
                    MakeFigureByTMO();
                }
            }
        }
    }
}
