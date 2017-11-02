using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Timers;
using System.Threading;

namespace draw
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
        }

        int i = 1;
        int piksel;
        Random rnd = new Random();
        int yon = 0;
        List<Mobil> mobilcemberlist = new List<Mobil>();
        Mobil mb;
        Cember mobil = new Cember();
        Pen mobilPen = new Pen(Color.Blue, 2);
        Graphics graphicsObj;
        Graphics graphicsObjMobile;
        int cap;
        List<Cember> cemberlist = new List<Cember>();
        Thread t1;
        int uzunluk;
        int yukseklik;
        List<Mobil> datagridlist = new List<Mobil>();
        int sure = 1;
        private void btnCiz_Click(object sender, EventArgs e)
        {
            #region tanımlamalar
            int i = 0;
            piksel = rnd.Next(Convert.ToInt32(txtMinPiksel.Text), Convert.ToInt32(txtMaxPiksel.Text)) / 10;
            cap = Convert.ToInt32(txtcap.Text);
            uzunluk = Convert.ToInt32(txtx.Text);
            yukseklik = Convert.ToInt32(txty.Text);
            int x = 0;
            int y = 0;
            Cember cb;
            Cember durumkontrol = new Cember();

            int cembersayisi = Convert.ToInt32(txtcembersayisi.Text);
            Font f = new Font("Arial", cap / 2);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            #endregion

            graphicsObj = this.CreateGraphics();
            Pen myPen = new Pen(System.Drawing.Color.Red, 2);
            Rectangle myRectangle = new Rectangle(x, y, uzunluk, yukseklik);

            #region ilkmobilcember
            mobil.X = uzunluk / 2;//rnd.Next(x + cap, uzunluk - cap);
            mobil.Y = yukseklik / 2;//rnd.Next(y + cap, yukseklik - cap);
            graphicsObj.DrawEllipse(mobilPen, mobil.X, mobil.Y, cap, cap);
            #endregion

            #region baz istasyonu
            Pen bazPen = new Pen(Color.Black, 2);
            int bazX = 0;
            int bazY = 0;
            switch (comboPozisyon.SelectedIndex)
            {
                case 0:
                    bazX = uzunluk - cap; bazY = 0;
                    break;
                case 1:
                    bazX = 0; bazY = 0;
                    break;
                case 2:
                    bazX = uzunluk - cap; bazY = yukseklik - cap;
                    break;
                case 3:
                    bazX = 0; bazY = yukseklik - cap;
                    break;
                case 4:
                    bazX = uzunluk / 2; bazY = yukseklik / 2;
                    break;
                default:
                    bazX = rnd.Next(x + cap, uzunluk - cap); bazY = rnd.Next(y + cap, yukseklik - cap);
                    break;
            }
            graphicsObj.DrawEllipse(bazPen, bazX, bazY, cap, cap);
            cb = new Cember();
            mb = new Mobil();
            cb.X = bazX; cb.Y = bazY; cb.no = 0; cemberlist.Add(cb);
            mb.cemberNo = 0; mb.durum = false; mb.gecirilenSure = 0; mb.girisZamani = 0; mobilcemberlist.Add(mb);
            #endregion

            #region alanın ve çemberlerin oluşturulması
            int cemberno = 1;
            graphicsObj.DrawRectangle(myPen, myRectangle);
            while (cembersayisi > i)
            {
                cb = new Cember();
                mb = new Mobil();
                int cemberx = rnd.Next(x + cap, uzunluk - cap);
                int cembery = rnd.Next(y + cap, yukseklik - cap);
                if (durumkontrol.Durum(cemberx, cembery, cemberlist, cap))
                {
                    cembersayisi--;
                    graphicsObj.DrawEllipse(myPen, cemberx, cembery, cap, cap);
                    graphicsObj.DrawString(cemberno.ToString(), f, drawBrush, cemberx, cembery);
                    cb.X = cemberx; cb.Y = cembery; cb.no = cemberno;
                    mb.cemberNo = cemberno; mb.durum = false; mb.gecirilenSure = 0; mb.girisZamani = 0;
                    mobilcemberlist.Add(mb);
                    cemberlist.Add(cb);
                    cemberno++;
                }

            }
            #endregion

            #region yonbelirleme
            yon = rnd.Next(0, 7);
            #endregion
            t1 = new Thread(new ThreadStart(MobilThread));
            t1.Start();

        }

        public void MobilThread()
        {
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "Ziyaret Edilen";
            dataGridView1.Columns[1].Name = "Giriş Zamanı ms";
            dataGridView1.Columns[2].Name = "Çıkış Zamanı ms";
            graphicsObjMobile = this.CreateGraphics();
            bool durum = true;
            cap = Convert.ToInt32(txtcap.Text);
            int maxval = uzunluk >= yukseklik ? uzunluk : yukseklik;
            #region saniye kadar ilerleme

            GridYaz(sure);
            #region randomwalk
            if (comboYontem.SelectedIndex == -1 || comboYontem.SelectedIndex == 0)
            {
                while (i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                {
                    GridYaz(sure);
                    if (i % 10 == 0)
                    {
                        yon = rnd.Next(0, 7);
                        
                        piksel = rnd.Next(0 + cap, maxval - cap) / 10;
                        if (yon == 0)
                        {
                            //mobil.Y = mobil.Y - (piksel*10+cap) < 0 ? mobil.Y : mobil.Y - piksel;
                            if (mobil.Y - (piksel * 10)-cap < 0)
                            {
                                mobil.Y = mobil.Y;
                                durum = false;
                            }
                            else { mobil.Y = mobil.Y - piksel; durum = true; }
                        }
                        else if (yon == 1)
                        {
                            if ((mobil.X + (piksel * 10) + cap) > uzunluk || (mobil.Y - (piksel * 10) - cap) < 0)
                            {
                                mobil.X = mobil.X;
                                mobil.Y = mobil.Y;
                                durum = false;
                            }
                            else
                            { mobil.X = mobil.X + piksel; mobil.Y = mobil.Y - piksel; durum = true; }

                        }

                        else if (yon == 2)
                        {
                            if (mobil.X + (piksel * 10 + cap) > uzunluk)
                            {
                                mobil.X = mobil.X;
                                durum = false;
                            }
                            else { mobil.X = mobil.X + piksel; durum = true; }
                        }
                        else if (yon == 3)
                        {
                            if ((mobil.X + (piksel * 10) + cap) > uzunluk || (mobil.Y + (piksel * 10) + cap) > yukseklik)
                            {
                                mobil.X = mobil.X;
                                mobil.Y = mobil.Y;
                                durum = false;
                            }
                            else { mobil.X = mobil.X + piksel; mobil.Y = mobil.Y + piksel; durum = true; }

                        }
                        else if (yon == 4)
                        {
                            if (mobil.Y + (piksel * 10) + cap > yukseklik)
                            {
                                mobil.Y = mobil.Y;
                                durum = false;
                            }
                            else { mobil.Y = mobil.Y + piksel; durum = true; }
                        }
                        else if (yon == 5)
                        {
                            if ((mobil.X - (piksel * 10) - cap) < 0 || (mobil.Y + (piksel * 10) + cap) > yukseklik)
                            {
                                mobil.X = mobil.X;
                                mobil.Y = mobil.Y;
                                durum = false;
                            }
                            else { mobil.X = mobil.X - piksel; mobil.Y = mobil.Y + piksel; durum = true; }

                        }
                        else if (yon == 6)
                        {
                            if (mobil.X - (piksel * 10)-cap < 0)
                            {
                                mobil.X = mobil.X;
                                durum = false;
                            }
                            else { mobil.X = mobil.X - piksel; durum = true; }
                        }
                        else if (yon == 7)
                        {
                            if ((mobil.X - (piksel * 10) -cap) < 0 || (mobil.Y - (piksel * 10) - cap) < 0)
                            {
                                mobil.X = mobil.X;
                                mobil.Y = mobil.Y;
                                durum = false;
                            }
                            else { mobil.X = mobil.X - piksel; mobil.Y = mobil.Y - piksel; durum = true; }
                        }

                    }
                    else
                    {
                        switch (yon)
                        {
                            case 0:
                                if (durum)
                                    mobil.Y = mobil.Y - piksel;
                                break;
                            case 1:
                                if (durum)
                                { mobil.X = mobil.X + piksel; mobil.Y = mobil.Y - piksel; }
                                break;
                            case 2:
                                if (durum)
                                { mobil.X = mobil.X + piksel; }
                                break;
                            case 3:
                                if (durum)
                                { mobil.X = mobil.X + piksel; mobil.Y = mobil.Y + piksel; }
                                break;
                            case 4:
                                if (durum)
                                { mobil.Y = mobil.Y + piksel; }
                                break;
                            case 5:
                                if (durum)
                                { mobil.X = mobil.X - piksel; mobil.Y = mobil.Y + piksel; }
                                break;
                            case 6:
                                if (durum)
                                { mobil.X = mobil.X - piksel < 0 ? mobil.X : mobil.X - piksel; }
                                break;
                            case 7:
                                if (durum)
                                { mobil.X = mobil.X - piksel; mobil.Y = mobil.Y - piksel; }
                                break;
                        }
                    }
                    //graphicsObjMobile.DrawEllipse(mobilPen, mobil.X, mobil.Y, cap, cap);
                    sure++;
                    i++;
                    Thread.Sleep(100);
                }
            }
            #endregion
            #region randomwaypoint
            else if (comboYontem.SelectedIndex == 1)
            {
                GridYaz(sure);
                while (i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                {
                    int newX = rnd.Next(0 + cap, uzunluk - cap);
                    int newY = rnd.Next(0 + cap, yukseklik - cap);
                    piksel = rnd.Next(Convert.ToInt32(txtMinPiksel.Text), Convert.ToInt32(txtMaxPiksel.Text)) / 10;

                    if (mobil.X < newX && mobil.Y < newY)
                        while (mobil.X < newX || mobil.Y < newY)
                        {

                            if (mobil.X < newX) mobil.X = mobil.X + piksel;
                            if (mobil.Y < newY) mobil.Y = mobil.Y + piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    else if (mobil.X < newX && mobil.Y > newY)
                        while (mobil.X < newX || mobil.Y > newY)
                        {
                            GridYaz(sure);
                            if (mobil.X < newX) mobil.X = mobil.X + piksel;
                            if (mobil.Y > newY) mobil.Y = mobil.Y - piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    else if (mobil.X == newX && mobil.Y < newY)
                        while (mobil.Y < newY)
                        {
                            GridYaz(sure);
                            mobil.Y = mobil.Y + piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    else if (mobil.X == newX && mobil.Y > newY)
                        while (mobil.Y < newY)
                        {
                            mobil.Y = mobil.Y - piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    else if (mobil.X < newX && mobil.Y == newY)
                        while (mobil.X < newX)
                        {
                            GridYaz(sure);
                            mobil.X = mobil.X + piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    else if (mobil.X > newX && mobil.Y == newY)
                        while (mobil.X > newX)
                        {
                            GridYaz(sure);
                            mobil.X = mobil.X - piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                }  
            }
            #endregion
            #region randomdirection
            else if (comboYontem.SelectedIndex == 2)
            {
                GridYaz(sure);
                while (i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                {
                    yon = rnd.Next(0, 7);
                    piksel = rnd.Next(Convert.ToInt32(txtMinPiksel.Text), Convert.ToInt32(txtMaxPiksel.Text)) / 10;
                    if (yon == 0)
                    {
                        while (mobil.Y - piksel - cap > 0 && i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                        {
                            mobil.Y = mobil.Y - piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    }
                    else if (yon == 1)
                    {
                        while ((mobil.X + piksel + cap) <= uzunluk && (mobil.Y - piksel - cap) > 0 && i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                        {
                            mobil.X = mobil.X + piksel;
                            mobil.Y = mobil.Y - piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }

                    }

                    else if (yon == 2)
                    {
                        while (mobil.X + piksel + cap <= uzunluk && i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                        {
                            mobil.X = mobil.X + piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }

                    }
                    else if (yon == 3)
                    {
                        while ((mobil.X + piksel + cap) < uzunluk && (mobil.Y + piksel + cap) < yukseklik && i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                        {
                            mobil.X = mobil.X + piksel;
                            mobil.Y = mobil.Y + piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    }
                    else if (yon == 4)
                    {
                        while (mobil.Y + piksel + cap < yukseklik && i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                        {
                            mobil.Y = mobil.Y + piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }

                    }
                    else if (yon == 5)
                    {
                        while ((mobil.X - piksel - cap) > 0 && (mobil.Y + piksel + cap) < yukseklik && i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                        {
                            mobil.X = mobil.X - piksel;
                            mobil.Y = mobil.Y + piksel;
                            sure++;
                            i++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    }
                    else if (yon == 6)
                    {
                        while ((mobil.X - piksel - cap) > 0 && i <= ((Convert.ToInt32(txtSimTime.Text) * 10)))
                        {
                            mobil.X = mobil.X - piksel;
                            i++;
                            sure++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    }
                    else if (yon == 7)
                    {
                        while ((mobil.X - piksel - cap) > 0 && (mobil.Y - piksel - cap) > 0)
                        {
                            mobil.X = mobil.X - piksel;
                            mobil.Y = mobil.Y - piksel;
                            i++;
                            sure++;
                            GridYaz(sure);
                            Thread.Sleep(100);
                        }
                    }
                }
            }
            #endregion
            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        public void GridYaz(int zaman)
        {
            graphicsObjMobile.DrawEllipse(mobilPen, mobil.X, mobil.Y, cap, cap);
            List<int> list = (from cl in cemberlist
                              where ((cl.X - cap < mobil.X && mobil.X < cl.X + cap) && (cl.Y - cap < mobil.Y && cl.Y + cap > mobil.Y))
                              select cl.no
                                  ).ToList();
            List<Mobil> mblistFalse = (from cl in mobilcemberlist
                                       where list.Contains(cl.cemberNo) && cl.durum == false
                                       select cl).ToList();
            List<Mobil> mblistTrue = (from cl in mobilcemberlist
                                      where list.Contains(cl.cemberNo) && cl.durum == true
                                      select cl).ToList();
            List<Mobil> mblistPreviousTrue = (from cl in mobilcemberlist
                                              where !list.Contains(cl.cemberNo) && cl.durum == true
                                              select cl).ToList();
            foreach (var item in mblistTrue)
            {
                item.gecirilenSure++;
            }
            foreach (var item in mblistFalse)
            {
                item.girisZamani = zaman;
                item.gecirilenSure++;
                item.durum = true;
            }
            foreach (var item in mblistPreviousTrue)
            {
                string[] row = new string[] { item.cemberNo == 0 ? "Baz İst." : item.cemberNo.ToString(), item.girisZamani.ToString(), (item.girisZamani + item.gecirilenSure).ToString() };
                dataGridView1.Rows.Add(row);
                item.girisZamani = 0;
                item.gecirilenSure = 0;
                item.durum = false;

            }

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            mobilcemberlist.Clear();
            cemberlist.Clear();
            dataGridView1.Rows.Clear();
            graphicsObj.Clear(this.BackColor);
            graphicsObjMobile.Clear(this.BackColor);
            i = 1;
            sure = 1;
        }
    }
}
