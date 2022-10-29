using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace game_cờ_caro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //khơi tạo chế độ online
        xuLyOnline xlOnline;
        int cheDoChoi; //0 la may 1 la nguoi
        //khởi tạo 2 người chơi
        public static Player player1;
        Player player2;
        Random rnd = new Random();
        string namePlayer1 = "", namePlayer2 = "";
        int luotDanh = 0;
        //lượt đánh = 0 là của người chơi thứ nhất quân cờ o
        //mảng chồng mảng để duyệt các nút xử lý thắng thua
        List<List<Button>> maTran;
        // lưu tọa độ để làm nút trờ lại
        Stack<viTri> viTriUndo;
        //khởi tạo thời gian
        DispatcherTimer Timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            veBanCo();
            xlOnline = new xuLyOnline();
            Timer.Interval = TimeSpan.FromMilliseconds(Chess.setingGiay * 10);
            Timer.Tick += Timer_Tick;
            Setting.MouseLeftButtonDown += (o, e) =>
            {
                myGrid.Visibility = Visibility.Visible;
                myGrid1.Visibility = Visibility.Visible;
                Timer.Stop();
            };
            Reset.MouseLeftButtonDown += (o, e) => resetBanCo();
            Undo.MouseLeftButtonDown += Undo_MouseLeftButtonDown;
            myWindow.Loaded += MyWindow_Loaded;
        }
        void cheDo2NguoiChoi()
        {
            cheDoChoi = 1;
            //khởi tạo giá trị ban đầu khi chạy
            Chess.IndexQuanCoO = (int)MyComboBox.SelectedIndex;
            Chess.IndexQuanCoX = (int)MyComboBox1.SelectedIndex;
            Chess.IndexBackGround = (int)MyComboBox3.SelectedIndex;
            player1 = new Player(namePlayer1, Chess.quanCoO[Chess.IndexQuanCoO]);
            namePlayer1 = txtPlayer1.Text;
            namePlayer2 = txtPlayer2.Text;
            player2 = new Player(namePlayer2, Chess.quanCoX[Chess.IndexQuanCoX]);
            txtHienThiTenNguoiChoi.Text = player1.Name.ToString();
            ImageTenNguoiChoi.Fill = player1.Image;
            Chess.setingGiay = Convert.ToInt32(txtPlayerTime.Text);
            Timer.Interval = TimeSpan.FromMilliseconds(Chess.setingGiay);
            resetBanCo();
            myGrid.Visibility = Visibility.Hidden;
            myGrid1.Visibility = Visibility.Hidden;
            Timer.Start();
        }
        void cheDoChoiOnline()
        {

        }
        void cheDoDanhVoiMay()
        {
            Chess.IndexQuanCoO = (int)MyComboBox.SelectedIndex;
            Chess.IndexQuanCoX = (int)MyComboBox1.SelectedIndex;
            Chess.IndexBackGround = (int)MyComboBox3.SelectedIndex;
            player1 = new Player(namePlayer1, Chess.quanCoO[Chess.IndexQuanCoO]);
            namePlayer1 = txtPlayer1.Text;
            namePlayer2 = "computer";
            player2 = new Player(namePlayer2, Chess.quanCoX[Chess.IndexQuanCoX]);
            txtHienThiTenNguoiChoi.Text = player1.Name.ToString();
            ImageTenNguoiChoi.Fill = player1.Image;
            Chess.setingGiay = Convert.ToInt32(txtPlayerTime.Text+9999);
            Timer.Interval = TimeSpan.FromMilliseconds(Chess.setingGiay);
            resetBanCo();
            myGrid.Visibility = Visibility.Hidden;
            myGrid1.Visibility = Visibility.Hidden;
            Timer.Start();
        }
        #region xử lý nút tùy chọn
        private void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ChonQuanCo(MyComboBox);
            ChonQuanCo(MyComboBox1);
            ChonQuanCo(MyComboBox3);
        }
        void ChonQuanCo(ComboBox a)
        {
            List<XuLyTuyChonQuanCo> temp = new List<XuLyTuyChonQuanCo>()
            {
                new XuLyTuyChonQuanCo(){Name ="kiểu 1"},
                new XuLyTuyChonQuanCo(){Name ="kiểu 2"},
                new XuLyTuyChonQuanCo(){Name ="kiểu 3"},
                new XuLyTuyChonQuanCo(){Name ="kiểu 4"}
            };
            a.ItemsSource = temp;
            a.DisplayMemberPath = "Name";
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cheDoChoi = 1;
            cheDo2NguoiChoi();
        }
        #endregion
        #region xử lý xaml
        //hàm xử lý kéo thả của thanh controlbar
        private void DockPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();

        }
        //xử lý nút đóng
        private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
        //xử lý nút trở lại
        private void Undo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (viTriUndo.Count <= 0) return;
            viTri p = viTriUndo.Pop();
            Button btn = maTran[p.y][p.x];
            btn.Background = Chess.backgroudBanCo[Chess.IndexBackGround];
            if (luotDanh == 0)
            {
                luotDanh = 1;
                txtHienThiTenNguoiChoi.Text = player2.Name.ToString();
                ImageTenNguoiChoi.Fill = player2.Image;
                txtHienThiTenNguoiChoi.Tag = "1";
                Time.Value = 100;
            }
            else if (luotDanh == 1)
            {
                luotDanh = 0;
                txtHienThiTenNguoiChoi.Text = player1.Name.ToString();
                ImageTenNguoiChoi.Fill = player1.Image;
                txtHienThiTenNguoiChoi.Tag = "2";
                Time.Value = 100;
            }
        }
        //xử lý nút thu nhỏ
        private void Image_PreviewMouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        #endregion
        #region xử lý cobehind
        void veBanCo()
        {
            viTriUndo = new Stack<viTri>();
            maTran = new List<List<Button>>();
            Button btnOld = new Button()
            {
                Width = 0
            };
            Canvas.SetTop(btnOld, 0);
            Canvas.SetLeft(btnOld, 0);
            for (int i = 0; i < Chess.chieuDaiBanCo; i++)
            {
                maTran.Add(new List<Button>());
                for (int j = 0; j < Chess.chieuRongBanCo; j++)
                {
                    Button btn = new Button()
                    {
                        Width = Chess.ChieuRong,
                        Height = Chess.ChieuDai,
                        Background = Chess.backgroudBanCo[Chess.IndexBackGround],
                        Tag = i.ToString(),
                        Style = (Style)Resources["ButtonStyle"],
                        Content = ""
                    };
                    Canvas.SetTop(btn, Canvas.GetTop(btnOld));
                    Canvas.SetLeft(btn, Canvas.GetLeft(btnOld) + btnOld.Width);
                    btn.Click += Btn_Click;
                    myCanvas.Children.Add(btn);
                    maTran[i].Add(btn);
                    btnOld = btn;
                }
                Canvas.SetTop(btnOld, Canvas.GetTop(btnOld) + Chess.ChieuDai);
                Canvas.SetLeft(btnOld, 0);
                btnOld.Width = 0;
                btnOld.Height = 0;
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Time.Value -= 1;
            if (Time.Value == 0)
            {
                Time.Value = 100;
                danhNgauNhien();
            }
        }
        //hàm đánh 1 con cờ ngẫu nhiên
        void danhNgauNhien()
        {
            int i, j;
            do
            {
                i = rnd.Next(0, Chess.chieuRongBanCo - 1);
                j = rnd.Next(0, Chess.chieuRongBanCo - 1);
            }
            while (maTran[i][j].Background != Chess.backgroudBanCo[Chess.IndexBackGround]);
            viTriUndo.Push(LayToaDoNut(maTran[i][j]));
            if (luotDanh == 0)
            {
                maTran[i][j].Background = player1.Image;
                txtHienThiTenNguoiChoi.Text = player2.Name.ToString();
                ImageTenNguoiChoi.Fill = player2.Image;
                txtHienThiTenNguoiChoi.Tag = "1";
                luotDanh = 1;
            }
            else if (luotDanh == 1)
            {
                maTran[i][j].Background = player2.Image;
                luotDanh = 0;
                txtHienThiTenNguoiChoi.Text = player1.Name.ToString();
                ImageTenNguoiChoi.Fill = player1.Image;
                txtHienThiTenNguoiChoi.Tag = "2";
            }
            if (WinLose(maTran[i][j]))
            {
                End();
            }
        }
        //sự kiện click cho button
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            // sender là this của sự kiện đồng thời chính là button diễn ra sự kiện đó nên dòng code dưới đây dùng để lấy button thực hiện việc click
            Button btn = sender as Button;
            if (btn.Background != Chess.backgroudBanCo[Chess.IndexBackGround])
            {
                return;
            }
            if (cheDoChoi == 0)
            {
                btn.Background = player1.Image;
                Time.Value = 100;
                if (WinLose(btn))
                {
                    End();
                }
                viTri a = new viTri();
                a = TimNuocDi();
                maTran[a.x][a.y].Background = Chess.quanCoX[Chess.IndexQuanCoX];
                if (WinLose(maTran[a.x][a.y]))
                {
                    MessageBox.Show("tro choi ket thuc");
                    resetBanCo();
                    return;
                }
            }
            else
            {
                viTriUndo.Push(LayToaDoNut(btn));
                if (luotDanh == 0)
                {
                    btn.Background = player1.Image;
                    luotDanh = 1;
                    txtHienThiTenNguoiChoi.Text = player2.Name.ToString();
                    ImageTenNguoiChoi.Fill = player2.Image;
                    txtHienThiTenNguoiChoi.Tag = "1";
                    Time.Value = 100;
                }
                else if (luotDanh == 1)
                {
                    btn.Background = player2.Image;
                    luotDanh = 0;
                    txtHienThiTenNguoiChoi.Text = player1.Name.ToString();
                    ImageTenNguoiChoi.Fill = player1.Image;
                    txtHienThiTenNguoiChoi.Tag = "2";
                    Time.Value = 100;
                }
            }
            // nếu thua thì end game
            if (WinLose(btn))
            {
                End();
            }

        }
        //hàm kiểm tra thắng thua
        private bool WinLose(Button btn)
        {
            return WinCheoTren(btn) || WinChieuNgang(btn) || WinChieuDoc(btn) || WinCheoDuoi(btn);
        }
        //hàm kết thúc game
        private void End()
        {
            Timer.Stop();
            MessageBox.Show("trò chơi kết thúc");
            resetBanCo();
            Timer.Start();
        }
        private viTri LayToaDoNut(Button btn)
        {
            //convert dùng để ép kiểu về kiểu dữ liệu int lấy ra vị trí hàng dọc của mảng
            int hangNgang = Convert.ToInt32(btn.Tag);
            //indexof ******
            int hangDoc = maTran[hangNgang].IndexOf(btn);
            viTri p = new viTri(hangDoc, hangNgang);
            return p;
        }
        private bool WinChieuNgang(Button btn)
        {
            int DemBenTrai = 0;
            int DemBenPhai = 0;
            viTri p = LayToaDoNut(btn);
            for (int i = p.x; i >= 0; i--)
            {
                if (maTran[p.y][i].Background == btn.Background)
                {
                    DemBenTrai++;
                }
                else
                {
                    break;
                }
            }
            for (int i = p.x + 1; i < Chess.chieuRongBanCo; i++)
            {
                if (maTran[p.y][i].Background == btn.Background)
                {
                    DemBenPhai++;
                }
                else
                {
                    break;
                }
            }
            return DemBenPhai + DemBenTrai == 5;
        }
        private bool WinChieuDoc(Button btn)
        {
            int DemBenTren = 0;
            int DemBenDuoi = 0;
            viTri p = LayToaDoNut(btn);
            for (int i = p.y; i >= 0; i--)
            {
                if (maTran[i][p.x].Background == btn.Background)
                {
                    DemBenTren++;
                }
                else
                {
                    break;
                }
            }
            for (int i = p.y + 1; i < Chess.chieuRongBanCo; i++)
            {
                if (maTran[i][p.x].Background == btn.Background)
                {
                    DemBenDuoi++;
                }
                else
                {
                    break;
                }
            }
            return DemBenTren + DemBenDuoi == 5;

        }
        private bool WinCheoTren(Button btn)
        {
            int DemBenTren = 0;
            int DemBenDuoi = 0;
            viTri p = LayToaDoNut(btn);
            for (int i = 0; i <= p.x; i++)
            {
                if (p.x - i < 0 || p.y - i < 0) break;
                if (maTran[p.y - i][p.x - i].Background == btn.Background)
                {
                    DemBenTren++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= Chess.chieuRongBanCo - p.x; i++)
            {
                if (p.y + i >= Chess.chieuDaiBanCo || p.x + i >= Chess.chieuRongBanCo) break;
                if (maTran[p.y + i][p.x + i].Background == btn.Background)
                {
                    DemBenDuoi++;
                }
                else
                {
                    break;
                }
            }
            return DemBenTren + DemBenDuoi == 5;
        }
        private bool WinCheoDuoi(Button btn)
        {
            int DemBenTren = 0;
            int DemBenDuoi = 0;
            viTri p = LayToaDoNut(btn);
            for (int i = 0; i <= p.x; i++)
            {
                if (p.x + i > Chess.chieuRongBanCo || p.y - i < 0) break;
                if (maTran[p.y - i][p.x + i].Background == btn.Background)
                {
                    DemBenTren++;
                }
                else
                {
                    break;
                }
            }
            for (int i = 1; i <= Chess.chieuRongBanCo - p.x; i++)
            {
                if (p.y + i >= Chess.chieuDaiBanCo || p.x - i < 0) break;
                if (maTran[p.y + i][p.x - i].Background == btn.Background)
                {
                    DemBenDuoi++;
                }
                else
                {
                    break;
                }
            }
            return DemBenTren + DemBenDuoi == 5;
        }
        void resetBanCo()
        {
            if (luotDanh == 0)
            {
                txtHienThiTenNguoiChoi.Text = player1.Name.ToString();
                ImageTenNguoiChoi.Fill = player1.Image;
            }
            myCanvas.Children.Clear();
            Time.Value = 100;
            veBanCo();
            if (cheDoChoi == 0) maTran[10][10].Background = Chess.quanCoX[Chess.IndexQuanCoX];

        }
        #endregion
        // xử lý đánh với máy
        public long[] tanCong = new long[7]
        {
            0,9,81,729,6561,59049,531441
        };
        public long[] phongThu = new long[7]
        {
            0,4,32,256,2048,16384,131072
        };
        //nước đánh đầu tiên
        //hàm tìm nước đi
        #region temp ai
        viTri TimNuocDi()
        {
            viTri p = new viTri();
            long Score = 0;
            for (int i = 0; i < Chess.chieuRongBanCo; i++)
            {
                for (int j = 0; j < Chess.chieuDaiBanCo; j++)
                {
                    long p_TanCong = 0;
                    long P_PhongThu = 0;
                    if (maTran[i][j].Background == Chess.backgroudBanCo[Chess.IndexBackGround])
                    {
                        p_TanCong += attack_Doc(i, j);
                        p_TanCong += attack_Ngang(i, j);
                        p_TanCong += attack_CheoLeft(i, j);
                        p_TanCong += attack_CheoRight(i, j);
                        P_PhongThu += defend_Doc(i, j);
                        P_PhongThu += defend_Ngang(i, j);
                        P_PhongThu += defend_CheoLeft(i, j);
                        P_PhongThu += defend_CheoRight(i, j);
                        if (P_PhongThu<= p_TanCong)
                        {
                            if (Score <= p_TanCong)
                            {
                                Score = p_TanCong;
                                p = new viTri(i, j);
                            }
                        }
                        else
                        {
                            if (Score <= P_PhongThu)
                            {
                                Score = P_PhongThu;
                                p = new viTri(i, j);
                            }
                        }
                    }
                }
            }
            return p;
        }
        #region Chiến thuật Tấn công
        private long attack_Doc(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Row + count < Chess.chieuRongBanCo; count++)
            {
                if (maTran[curr_Row + count][ curr_Column].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row + count][ curr_Column].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else    // gặp những ô trống thì thoát không xét
                    break;
            }

            for (int count = 1; count < 6 && curr_Row - count >= 0; count++)
            {
                if (maTran[curr_Row - count][ curr_Column].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row - count][ curr_Column].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanDich == 2)
                return 0;

            Sum += tanCong[QuanTa];
            Sum -= tanCong[QuanDich];
            PointTemp += Sum;

            return PointTemp;
        }

        private long attack_Ngang(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Column + count < Chess.chieuDaiBanCo; count++)
            {
                if (maTran[curr_Row][ curr_Column + count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row][ curr_Column + count].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else    // gặp những ô trống thì thoát không xét
                    break;
            }


            for (int count = 1; count < 6 && curr_Column - count >= 0; count++)
            {
                if (maTran[curr_Row][ curr_Column - count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row][ curr_Column - count].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanDich == 2)
                return 0;

            Sum += tanCong[QuanTa];
            Sum -= tanCong[QuanDich];
            PointTemp += Sum;

            return PointTemp;
        }

        private long attack_CheoLeft(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Column + count < Chess.chieuDaiBanCo && curr_Row - count >= 0; count++)
            {
                if (maTran[curr_Row - count][ curr_Column + count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row - count][ curr_Column + count].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else    // gặp những ô trống thì thoát không xét
                    break;
            }


            for (int count = 1; count < 6 && curr_Column - count >= 0 && curr_Row + count < Chess.chieuRongBanCo; count++)
            {
                if (maTran[curr_Row + count][ curr_Column - count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row + count][ curr_Column - count].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh 
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanDich == 2)
                return 0;

            Sum += tanCong[QuanTa];
            Sum -= tanCong[QuanDich];
            PointTemp += Sum;

            return PointTemp;
        }

        private long attack_CheoRight(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Column + count < Chess.chieuDaiBanCo && curr_Row + count < Chess.chieuRongBanCo; count++)
            {
                if (maTran[curr_Row + count][ curr_Column + count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row + count][ curr_Column + count].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else    // gặp những ô trống thì thoát không xét
                    break;
            }


            for (int count = 1; count < 6 && curr_Column - count >= 0 && curr_Row - count >= 0; count++)
            {
                if (maTran[curr_Row - count][ curr_Column - count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                    QuanTa++;
                else
                    if (maTran[curr_Row - count][ curr_Column - count].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh 
                {
                    QuanDich++;
                    PointTemp -= tanCong[1];
                    break;
                }
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanDich == 2)
                return 0;

            Sum += tanCong[QuanTa];
            Sum -= tanCong[QuanDich];
            PointTemp += Sum;

            return PointTemp;
        }
        #endregion
        #region Chiến thuật Phòng ngự
        private long defend_Doc(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Row + count < Chess.chieuRongBanCo; count++)
            {
                if (maTran[curr_Row + count][curr_Column].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;
                }
                else
                    if (maTran[curr_Row + count][curr_Column].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                    QuanDich++;
                else    // gặp những ô trống thì thoát không xét
                    break;
            }


            for (int count = 1; count < 6 && curr_Row - count >= 0; count++)
            {
                if (maTran[curr_Row - count][ curr_Column].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;
                }
                else
                    if (maTran[curr_Row - count][ curr_Column].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh
                    QuanDich++;
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanTa == 2)
                return 0;

            Sum += phongThu[QuanDich];
            if (QuanDich > 0)
                Sum -= tanCong[QuanTa] * 2;
            PointTemp += Sum;

            return PointTemp;
        }

        private long defend_Ngang(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Column + count < Chess.chieuDaiBanCo; count++)
            {
                if (maTran[curr_Row][ curr_Column + count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;
                }
                else
                    if (maTran[curr_Row][ curr_Column + count].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                    QuanDich++;
                else    // gặp những ô trống thì thoát không xét
                    break;
            }


            for (int count = 1; count < 6 && curr_Column - count >= 0; count++)
            {
                if (maTran[curr_Row][ curr_Column - count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;
                }
                else
                    if (maTran[curr_Row][ curr_Column - count].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh
                    QuanDich++;
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanTa == 2)
                return 0;

            Sum += phongThu[QuanDich];
            if (QuanDich > 0)
                Sum -= tanCong[QuanTa] * 2;
            PointTemp += Sum;


            return PointTemp;
        }

        // Chéo ngược
        private long defend_CheoLeft(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Column + count < Chess.chieuDaiBanCo && curr_Row - count >= 0; count++)
            {
                if (maTran[curr_Row - count][curr_Column + count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;
                }
                else
                    if (maTran[curr_Row - count][ curr_Column + count].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                    QuanDich++;
                else    // gặp những ô trống thì thoát không xét
                    break;
            }


            for (int count = 1; count < 6 && curr_Column - count >= 0 && curr_Row + count < Chess.chieuRongBanCo; count++)
            {
                if (maTran[curr_Row + count][ curr_Column - count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;
                }
                else
                    if (maTran[curr_Row + count][ curr_Column - count].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh 
                    QuanDich++;
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanTa == 2)
                return 0;

            Sum += phongThu[QuanDich];
            if (QuanDich > 0)
                Sum -= tanCong[QuanTa] * 2;
            PointTemp += Sum;

            return PointTemp;
        }

        // chéo xuôi
        private long defend_CheoRight(int curr_Row, int curr_Column)
        {
            long Sum = 0;
            long PointTemp = 0;
            int QuanTa = 0;
            int QuanDich = 0;
            for (int count = 1; count < 6 && curr_Column + count < Chess.chieuDaiBanCo && curr_Row + count < Chess.chieuRongBanCo; count++)
            {
                if (maTran[curr_Row + count][ curr_Column + count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;

                }
                else
                    if (maTran[curr_Row + count][ curr_Column + count].Background == Chess.quanCoX[Chess.IndexQuanCoX])       // quân ta đánh
                    QuanDich++;
                else    // gặp những ô trống thì thoát không xét
                    break;
            }


            for (int count = 1; count < 6 && curr_Column - count >= 0 && curr_Row - count >= 0; count++)
            {
                if (maTran[curr_Row - count][ curr_Column - count].Background == Chess.quanCoO[Chess.IndexQuanCoO])        // quân của máy đánh ra
                {
                    QuanTa++;
                    break;
                }
                else
                    if (maTran[curr_Row - count][ curr_Column - count].Background == Chess.quanCoX[Chess.IndexQuanCoX])    // quân ta đánh 
                    QuanDich++;
                else            // gặp những ô trống thì thoát không xét
                    break;
            }

            // bị chặn 2 đầu
            if (QuanTa == 2)
                return 0;

            Sum += phongThu[QuanDich];
            if (QuanDich > 0)
                Sum -= tanCong[QuanTa] * 2;
            PointTemp += Sum;

            return PointTemp;
        }
        #endregion
        #endregion
        public class viTri
        {
            public int x;
            public int y;
            public viTri(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public viTri()
            {

            }

        }
        public class Player
        {
            private string name;
            public string Name { get => name; set => name = value; }
            private ImageBrush image;
            public ImageBrush Image { get => image; set => image = value; }
            public Player(string name, ImageBrush image)
            {
                Name = name;
                Image = image;
            }
            public Player()
            {
                Name = "nguoi choi";
                Image = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/loading.gif")));
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            xlOnline.IP =
            xlOnline.GetLocalIPv4(NetworkInterfaceType.Wireless80211);
            if (!xlOnline.ketNoiSever())
            {
                xlOnline.taoServer();
                MessageBox.Show("đã tạo sever");
                Thread listenThread = new Thread(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(500);
                        try
                        {
                            listen();
                            break;
                        }
                        catch
                        {

                        }
                    }
                });
                listenThread.IsBackground = true;
                listenThread.Start();
            }
            else 
            {
                Thread listenThread = new Thread(() =>
                {
                    listen();
                });
                listenThread.IsBackground = true;
                listenThread.Start();
                //sau nay xoa
                xlOnline.gui("thông tin từ máy");
            }
        }
        void listen()
        {
            string data = (string)xlOnline.Nhan();
            MessageBox.Show(data +"pham van da");
            MessageBox.Show(data);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            cheDoChoi = 0;
            cheDoDanhVoiMay();
        }

        public class XuLyTuyChonQuanCo
        {
            public string Name { get; set; }

        }
    }
}
