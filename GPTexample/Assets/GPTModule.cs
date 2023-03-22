using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using UnityEditor.PackageManager;
using System.Diagnostics;

public class GPTModule : MonoBehaviour
{
    public static string ENVPATH = "C:/Users/user/.conda/envs/NLP/python.exe";
    public static string FILEPATH = "C:/Users/user/Documents/GitHub/UnityinGPT/GPT.py";

    private Socket socket;
    private string ip = "127.0.0.1";
    private int port = 9999;
    private bool isConnected = false;
    public delegate void useOutput(string msg);
    public void ExecutePython()
    {
        try
        {
            Process psi = new Process();
            psi.StartInfo.FileName = ENVPATH;
            psi.StartInfo.Arguments = FILEPATH;
            psi.StartInfo.CreateNoWindow = true;
            psi.StartInfo.UseShellExecute = true;
            psi.Start();
            UnityEngine.Debug.Log("[알림] .py file 실행");
            ConnectToServer();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("[알림] 에러발생: " + e.Message);
        }
    }

    public void ConnectToServer()
    {
        if (isConnected) return; 
        isConnected= true;
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(ip, port, ConnectCallback,socket);
            UnityEngine.Debug.Log("서버 연결 성공");
        }
        catch(Exception e)
        {
            UnityEngine.Debug.LogFormat("서버 연결 실패! error cored : {0}",e);
        }
    }

    public void DisconnectServer()
    {
        if (!isConnected) return;

        isConnected = false;
        socket.Close();
        socket.Dispose();
    }

    public async void SendChatMessageAsync(string message,  useOutput output)
    {

        try
        {
            UnityEngine.Debug.Log("SendChatMessage");
            var data = Encoding.UTF8.GetBytes(message);
            socket.Send(BitConverter.GetBytes(data.Length));
            socket.Send(data);
            await ReceiveChatMessage(output);
        }
        catch(Exception e) { UnityEngine.Debug.LogFormat("Fail send message, {0}\n {1}", e,socket.Available); }
    }

    public async Task ReceiveChatMessage(useOutput output)
    { 
        var data = new byte[4];
        string msg = "";
        try
        { 
            await Task.Run(() =>
            {
                UnityEngine.Debug.Log("ReceiveMessage");
                socket.Receive(data, data.Length, SocketFlags.None);
                Array.Reverse(data);
                data = new byte[BitConverter.ToInt32(data, 0)];
                socket.Receive(data, data.Length, SocketFlags.None);
            });
            msg = Encoding.UTF8.GetString(data);
            msg = msg.Replace("\r\n", "");
            UnityEngine.Debug.Log(msg);
            output.Invoke(msg);
        }
        catch(Exception e) { UnityEngine.Debug.LogFormat("Fail receive message : {0}", e); }
        
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        Socket socket = (Socket)ar.AsyncState;
        socket.EndConnect(ar);
    }
}
