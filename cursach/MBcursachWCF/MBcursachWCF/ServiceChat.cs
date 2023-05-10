using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace MBcursachWCF
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "ServiceChat"
    // в коде и файле конфигурации.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceChat : IServiceChat
    {
        List<ServerUser> users = new List<ServerUser>();
        int nextId = 1;


        string commandText;
        string connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=VOKCMR.mdb";
        OleDbConnection conn = null;
        Exception exep = null;

        public int Connect(string name)
        {
            ServerUser user = new ServerUser()
            {
                ID = nextId,
                Name = name,
                operationContext = OperationContext.Current
            };
            nextId++;

            SendMsg(": "+user.Name + " подключился к чату",0);
            users.Add(user);
            return user.ID;
        }

        public void Disconnect(int id)
        {
            var user = users.FirstOrDefault(x => x.ID == id);
            if (user != null)
            {
                users.Remove(user);
                SendMsg(": " + user.Name + " покуинул к чат",0);
            }
        }

        public void SendMsg(string msg, int id)
        {
            foreach (var item in users)
            {
                string answer = DateTime.Now.ToShortTimeString();
                var user = users.FirstOrDefault(x => x.ID == id);
                if (user != null)
                {
                    answer+=": "+ user.Name+" :";
                }
                answer+=msg;

                item.operationContext.GetCallbackChannel<IServerChatCallback>().MsgCallback(answer);
            }
        }
        public void ConnectDB(string msg)
        {
            string txt = "";
            conn = new OleDbConnection(connectionString);
            conn.Open();
            string query = "SELECT * FROM procedures";
            OleDbCommand command = new OleDbCommand(query, conn);
            //label2.Text = command.ExecuteScalar().ToString();//1 результ
            OleDbDataReader reader = command.ExecuteReader();//много всего
            //listBox1.Items.Clear();
            while (reader.Read())
            {
                txt = reader[0].ToString() + ": " + reader[1].ToString() +" ";
            }

            reader.Close();
            //SendMsg(": " + txt + " ???", 0);
            foreach (var item in users)
            {
                item.operationContext.GetCallbackChannel<IServerChatCallback>().Show(txt);
            }
        }
    }
}
