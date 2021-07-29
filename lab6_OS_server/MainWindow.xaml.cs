using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab6_OS_server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public NamedPipeServerStream namedPipeServerStream = new NamedPipeServerStream("pipe",
                  PipeDirection.InOut, 1);
        string value = "";
        public MainWindow()
        {
            InitializeComponent();
			this.Hide();
			//Thread.Sleep(300);
			Thread t = new Thread(delegate ()
			{
				namedPipeServerStream.WaitForConnection();


				Application.Current.Dispatcher.Invoke(() => // Выполняет заданный делегат синхронно, в потоке, с которым связан Dispatcher.
				/*он маршализирует указанный код потоку диспетчера. метод Invoke() останавливает поток до тех пор, 
				 * пока диспетчер выполняет код. 
				После того как пользователь щелкнет на кнопке и маршализируемый код завершится, Invoke() вернет управление, 
				и можно будет продолжить работу в соответствии с ответом пользователя.
				 */
				{
					while (true)
					{
						try
						{
							StreamReader sr = new StreamReader(namedPipeServerStream, Encoding.UTF8);
							value = sr.ReadLine();
							fromClient.Text = value;
							/////////////////////
							try
							{
								value = calc(value);
								if (value == "")
								value = "uncorrect symbol";
								//value = new DataTable().Compute(value, null).ToString();
								//if (value == "∞" || value == "не число" || value == "-∞")
									//value = "division zero error";
							}
							catch (System.Data.SyntaxErrorException e)
							{
								value = "Error";
							}
							//////////////////////////
							StreamWriter sw = new StreamWriter(namedPipeServerStream, Encoding.UTF8);
							sw.WriteLine(value);
							sw.Flush();
							toClient.Text = value;
						}
						catch { }
					}
				});

			});
			t.Start();
		}


	string calc(string str)
			{
				str += "\0";
			string res = "";
			StringBuilder temp = new StringBuilder(res);

			int x1 = 0, x2 = 0, x3 = 0;

			int i = 0;
			while (i < str.Length - 1)
			{
				if (str[i + 1] >= '0' && str[i + 1] <= '9')
				{
					if (str[i] == '+')
					{
						while (str[i + 1] != '+' && str[i + 1] != '-' && str[i + 1] != '*' && str[i + 1] != '/' && str[i + 1] != '\0')
						{
							i++;
							x3 = x3 * 10 + (str[i] - '0');
						}

						x1 += x2;
						x2 = x3;
						x3 = 0;
					}
					else if (str[i] == '-')
					{
						while (str[i + 1] != '+' && str[i + 1] != '-' && str[i + 1] != '*' && str[i + 1] != '/' && str[i + 1] != '\0')
						{
							i++;
							x3 = x3 * 10 + (str[i] - '0');
						}
						x1 += x2;
						x2 = -x3;
						x3 = 0;

					}
					else if (str[i] == '*')
					{
						if (i < str.Length - 1)
						{
							while (str[i + 1] != '+' && str[i + 1] != '-' && str[i + 1] != '*' && str[i + 1] != '/' && str[i + 1] != '\0')

							{
								i++;
								x3 = x3 * 10 + (str[i] - '0');
							}
							x2 *= x3;
							x3 = 0;
						}
					}
					else if (str[i] == '/')
					{
						while (str[i + 1] != '+' && str[i + 1] != '-' && str[i + 1] != '*' && str[i + 1] != '/' && str[i + 1] != '\0')
						{
							i++;
							x3 = x3 * 10 + (str[i] - '0');
						}
						if (x3 == 0)
						{
							res = "division zero error";
							return res;
						}
						x2 /= x3;
						x3 = 0;
					}
				}
				else
				{
					res = "";
					return res;
				}
				i++;
			}

			x1 += x2;

			int len = x1.ToString().Length;


			if (x1 < 0)
			{
				res = "-" + res;
				x1 *= -1;
				for (int j = 1; j < len; j++)
				{
					int t = (int)Math.Pow(10, len - j - 1);
					int a = x1 / t;
					temp.Append(a.ToString());
					x1 = x1 % t;
				}
			}

			else
			{
				for (int j = 0; j < len; j++)
				{

					int t = (int)Math.Pow(10, len - j - 1);
					int a = x1 / t;
					temp.Append(a.ToString());
					x1 = x1 % t;
				}
			}
			res = temp.ToString();

			return res;
		}

        
    }
}
