using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MBcursachClient.ServiceChat;

namespace MBcursachClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                if(tbUserName.Text == "Имя сотрудника") 
                {
                    MessageBox.Show("Введите имя");
                }
                else
                {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(tbUserName.Text);
                tbUserName.IsEnabled = false;
                btnName.Content = "Отключиться";
                isConnected = true;
                }
            }
        }
        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                tbUserName.IsEnabled = true;
                btnName.Content = "Подключиться";
                isConnected = false;
            }
        }

        public void Show(string Content)
        {
            lbtable.Items.Add(Content);
            lbtable.ScrollIntoView(lbChat.Items[lbChat.Items.Count - 1]);
        }

        private void btnName_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }
        }

        public void MsgCallback(string msg)
        {
            lbChat.Items.Add(msg);
            lbChat.ScrollIntoView(lbChat.Items[lbChat.Items.Count-1]);
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DisconnectUser();
        }

        private void lbMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(lbMessage.Text, ID);
                    lbMessage.Text = String.Empty;
                }
            }
        }

        private void tbUserName_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //string commandText;
        //string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=VOKCMR.mdb";
        //OleDbConnection conn = null;
        //Exception exep = null;
        private void btnShow_Click(object sender, RoutedEventArgs e)
        {
            string txt = "";
            if (client != null)
            {
                client.ConnectDB(txt);
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (client != null)
            {
                client.SendMsg(lbMessage.Text, ID);
                lbMessage.Text = String.Empty;
            }
        }
    }
}
