using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
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

namespace lab6_OS_client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String value="";
        public NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(
                  ".",//Имя удаленного компьютера, к которому нужно подключиться
                      //значение "." - чтобы указать локальный компьютер.
                  "pipe",//
                  PipeDirection.InOut);//Указывает, что канал двусторонний.
        
            //Клиент устанавливает свое соединение 
        public MainWindow()
        {
            InitializeComponent();
            namedPipeClientStream.Connect();

            logs.Text += DateTime.Now + " Client connected" + "\n";
        }
   
        private void calc_Click(object sender, RoutedEventArgs e)
        {
            value = toServer.Text;
            logs.Text += DateTime.Now + " Calculate" + "\n";
            logs.Text += DateTime.Now + " Data to server> " + toServer.Text + "\n";

            Application.Current.Dispatcher.Invoke(() =>
            {

                StreamWriter sw = new StreamWriter(namedPipeClientStream, Encoding.UTF8);
                sw.WriteLine(value);
                sw.Flush();

                StreamReader sr = new StreamReader(namedPipeClientStream, Encoding.UTF8);
                value = sr.ReadLine();
                fromServer.Text = value;

                logs.Text += DateTime.Now + " Data from server> " + value + "\n";
            });
        }

        private void command(object sender, RoutedEventArgs e)
        {
            toServer.Text += ((Button)e.OriginalSource).Content;
        }

        private void delete_Click(object sender, RoutedEventArgs e)
        {
            toServer.Text = "";
            logs.Text += DateTime.Now + " Delete" + "\n";
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }




        private void save_Click(object sender, RoutedEventArgs e)
        {
            string path = logPath.Text;
            if (!File.Exists(path))
            {
                // Create a file to write to.
                StreamWriter sw = File.CreateText(path);
                {
                    //StreamWriter sw = new StreamWriter(path);
                    sw.WriteLine(logs.Text);
                    sw.Write("\n");
                    sw.Close();
                }
            }
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            string path = logPath.Text;
            //if (!File.Exists(path))
            {
                // Create a file to write to.
                //StreamReader sr = File.OpenText(path);
                {
                    StreamReader sr = new StreamReader(path);
                    logs.Text = sr.ReadToEnd();
                    sr.Close();
                }
            }
        }
    }
}
