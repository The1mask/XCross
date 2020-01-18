using System.Collections;
using UnityEngine;
using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;

public class Client : MonoBehaviour
{

 
    private const int port = 8888;
    private const string host = "127.0.0.1";
    static TcpClient client;
    static NetworkStream stream;
    private string message;
    public void ConnectToServer(string userName)
    {

        Console.Write("Введите свое имя:");
        try
        {
            client = new TcpClient(host, port);
            stream = client.GetStream();
            message = String.Format("{0}", userName);
            // преобразуем сообщение в массив байтов
            byte[] data = Encoding.Unicode.GetBytes(message);
            // отправка сообщения
            stream.Write(data, 0, data.Length);
            Debug.Log("Connected");
            GetMessage();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void GetMessage()
    {
        // получаем ответ
        byte[] data = new byte[64]; // буфер для получаемых данных
        StringBuilder builder = new StringBuilder();
        int bytes = 0;
        do
        {
            bytes = stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        }
        while (stream.DataAvailable);

        message = builder.ToString();
        Debug.Log("Сервер: {0}" + message);
    }

    public void SendMessage(int userName)
    {
        string message = "hello";
        message = String.Format("{0}: {1}", userName, message);
        // преобразуем сообщение в массив байтов
        byte[] data = BitConverter.GetBytes(userName);
        // отправка сообщения
        stream.Write(data, 0, data.Length);
        GetMessage();
    }

}
