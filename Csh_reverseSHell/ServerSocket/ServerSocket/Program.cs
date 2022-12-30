using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ServerSocket
{
    internal class Program
    {
        public string SendCommand(Socket client_socket)
        {
            string command;
            //ask for new command to be send
            Console.Write("Enter command: ");
            command = Console.ReadLine();
            //send the command
            client_socket.Send(Encoding.ASCII.GetBytes(command));
            return command;
        }
        static void Main(string[] args)
        {
            string command_to_client;
            Program prog = new Program(); // only need this to access the method SondCommand
            //start new IP endpoint for all network interfaces
            IPEndPoint ipe = new IPEndPoint(IPAddress.Any, 4444);
            //create a socket socket stream, protocol Tcp
            Socket server_socket = new Socket(IPAddress.Any.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //bind socket and IP endpoint
            server_socket.Bind(ipe);
            server_socket.Listen(5); // Listen to 5 connections

            //on connection, accept
            Socket client_socket = server_socket.Accept();
            //write message of connection
            Console.WriteLine("Connection from: {0}", client_socket.RemoteEndPoint);
            // call SendCommand that will take socket and return string command entered by the user
            command_to_client = prog.SendCommand(client_socket); ;

            // define command "drop" to close connection
            while (command_to_client != "drop")
            {

                //creating byte[] to be send
                byte[] server_message_b = new byte[2048];

                //clear array
                Array.Clear(server_message_b, 0, server_message_b.Length);

                //receive response from the client 
                client_socket.Receive(server_message_b);

                // write the response
                Console.WriteLine(Encoding.ASCII.GetString(server_message_b).TrimEnd('\0'));

                //return string entered by the user
                command_to_client = prog.SendCommand(client_socket);
            }

            // close server socket and client socket if the command was 'drop'
            client_socket.Close();
            server_socket.Close();
        }
    }
}
