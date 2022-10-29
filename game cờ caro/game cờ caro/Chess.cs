using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace game_cờ_caro
{
    public class Chess
    {
        public static int ChieuRong = 25;
        public static int ChieuDai = 25;
        public static int chieuDaiBanCo = 20;
        public static int chieuRongBanCo = 20;
        public static List<ImageBrush> quanCoX = new List<ImageBrush>()
        {
           new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/skin/xStyle1.jpg"))),
           new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/skin/xStyle2.png")))

        };
        public static List<ImageBrush> quanCoO = new List<ImageBrush>()
        {
           new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/skin/oStyle1.jpg"))),
           new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/skin/oStyle2.png")))


        };
        public static List<ImageBrush> backgroudBanCo = new List<ImageBrush>()
        {
            new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/backgroundBlue.png"))),
            new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/backgroundRed.png"))),
            new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/images/background1.jpg"))),
         };
        public static int IndexBackGround = 0;
        public static int IndexQuanCoX = 1;
        public static int IndexQuanCoO = 1;
        public static int setingGiay = 1000;
    }
}
