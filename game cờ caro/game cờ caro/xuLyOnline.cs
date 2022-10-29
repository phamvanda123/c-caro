using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace game_cờ_caro
{
    public class xuLyOnline
    {
        #region tài khoản khách
        Socket Khach;
        public bool ketNoiSever()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), PORT);
            Khach = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Khach.Connect(iep);
                return true;//kết nối thành công thì return
            }
            catch
            {
                return false;//không thành công return false
        
            }
        }
        #endregion
        #region tài khoản chủ
        Socket server;
        public void taoServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), PORT);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(iep);
            server.Listen(10);
            Thread chapNhanKetNoi = new Thread(() =>
            {
               Khach = server.Accept();
            });
            chapNhanKetNoi.IsBackground = true; 
            chapNhanKetNoi.Start();
        }
        #endregion
        #region dungchung
        public string IP = "192.168.0.1";
        public int PORT = 9999;
        public const int dungLuong = 1024;
        public bool isServer = true;
        public bool gui(object data)
        {
            //chuyển đổi dữ liệu từ object về kiểu byte để gửi
            byte[] datagui =SerializeData(data);
            return guiData(Khach, datagui);
           
        }
        public object Nhan()
        {
            byte[] dataNhan = new byte[dungLuong];//khai báo mảng để nhận data
            bool isOk=NhanData(Khach, dataNhan);
            //lúc này data nhận sẽ là kiểu byte cần đổi ra kiểu object để trả về
            return DeserializeData(dataNhan);

        }
        //hàm gửi data
        private bool guiData(Socket target, byte[] data)
        {
            return target.Send(data) == 1 ? true : false;
        }
        //hàm nhận data
        //target là người gửi hoặc người nhận
        private bool NhanData(Socket target, byte[] data)
        {
            return target.Receive(data) == 1 ? true : false;
        }
        //ham bien doi tuong thanh byte
        public byte[] SerializeData(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }
        //ham ma hoa tu byte thanh doi tuong
        public object DeserializeData(byte[] theByteArray)
        {
            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;
            return bf1.Deserialize(ms);
        }
        //ham lay ra ip cua may
        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                    foreach(UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
            }
            return output;
        }
        #endregion
    }
}
